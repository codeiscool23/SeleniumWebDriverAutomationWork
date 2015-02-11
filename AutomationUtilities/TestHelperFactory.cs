using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkareaAutomation.PageObjects;

namespace WorkareaAutomation.Helpers
{
    /// <summary>
    /// A factory to create helper classes.
    /// </summary>
    public static class TestHelperFactory
    {
        private class BrowserHelper : IBrowserHelper
        {
            private const int arbitraryNegativeWaitTime = -10;
            private int _defaultJavascriptWaitTime = arbitraryNegativeWaitTime;
            private int defaultJavascriptWaitTime
            {
                get
                {
                    if (_defaultJavascriptWaitTime == arbitraryNegativeWaitTime)
                    {
                        _defaultJavascriptWaitTime = Convert.ToInt32(ConfigurationManager.AppSettings["javascriptWaitTimeInSeconds"]);
                    }
                    return _defaultJavascriptWaitTime;
                }
            }
            public IBrowserHelper WaitForJavascriptExecution(int? seconds = null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(seconds ?? defaultJavascriptWaitTime));
                return this;
            }
        }

        private class FileSiteHelper : ISiteHelper
        {
            public ISiteHelper CopyTemplateToSite(string fileName, string sourcePath, string targetPath)
            {
                System.IO.Directory.CreateDirectory(targetPath);
                System.IO.FileInfo file = new System.IO.FileInfo(sourcePath);
                System.IO.FileInfo destFile = new System.IO.FileInfo(System.IO.Path.Combine(targetPath, fileName));

                //copy
                if (destFile.Exists == false)
                {
                    System.IO.File.Copy(System.IO.Path.Combine(sourcePath, fileName), System.IO.Path.Combine(targetPath, fileName));                                
                }
                // overwrite file only if its newer
                else
                {
                    if (file.LastWriteTime > destFile.LastWriteTime)
                    {
                        System.IO.File.Copy(System.IO.Path.Combine(sourcePath, fileName), System.IO.Path.Combine(targetPath, fileName), true);
                    }
                }                       
                return this;

                //System.IO.Directory.CreateDirectory(targetPath);
                //// To copy files to another location and overwrite the destination files if it already exists.
                
                //System.IO.File.Copy(System.IO.Path.Combine(sourcePath, fileName), System.IO.Path.Combine(targetPath, fileName), true);

                //return this;
            }

            public ISiteHelper RemoveTemplateFromSite(string fileName, string targetPath)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(targetPath, fileName)))
                {
                    // Use a try block to catch IOExceptions, to
                    // handle the case of the file already being
                    // opened by another process.
                    try
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(targetPath, fileName));
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);

                    }
                }
                return this;
            }
        }

        private class SQLHelper : ISQLHelper
        {
            public ISQLHelper BackupSiteDatabase(string database, string connString)
            {
                string message = string.Empty;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = System.Data.CommandType.Text;

                        conn.Open();

                        cmd.CommandText = "USE MASTER";
                        cmd.ExecuteNonQuery();


                        cmd.CommandText = string.Format("BACKUP DATABASE {0} TO DISK = 'TestAutomationBackup.bak' WITH INIT", database);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                        message = string.Format("Database {0} successfully backedup", database);
                    }
                }
                catch (Exception ex)
                {
                    message = string.Format("Problem backing up database {0}. Error: {1}",database,ex.Message);

                }
                finally
                {
                    sw.Stop();
                    Console.WriteLine(message);
                    Console.WriteLine(string.Format("Duration: {0}",sw.Elapsed.Seconds.ToString()));
                }
              

                return this;
                
            }


            public ISQLHelper RestoreSiteDatabase(string database, string connString)
            {
                string message = string.Empty;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                try
                {

                    using (SqlConnection conn = new SqlConnection(connString))

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = System.Data.CommandType.Text;

                        conn.Open();

                        cmd.CommandText = "USE MASTER";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("Alter Database {0} SET SINGLE_USER With ROLLBACK IMMEDIATE ", database);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = string.Format("restore database {0} from disk='TestAutomationBackup.bak' WITH  FILE = 1,  NOUNLOAD ,  STATS = 10,  RECOVERY , REPLACE ", database);
                        cmd.ExecuteNonQuery();

                        conn.Close();

                        message = string.Format("Database {0} successfully restored", database);
                    }
                }
                catch (Exception ex)
                {
                    message = string.Format("Problem restoring database {0}. Error: {1}", database, ex.Message);
                }
                finally
                {
                    sw.Stop();
                    Console.WriteLine(message);
                    Console.WriteLine(string.Format("Duration: {0}",sw.Elapsed.Seconds.ToString()));
                }



                return this;


            }

            public double GetDBVersion(string database, string connString)
            {
                string message = string.Empty;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                object dbVersion = 0;
                double dbNum = 0;
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = System.Data.CommandType.Text;

                        conn.Open();

                        cmd.CommandText = string.Format("USE  {0} ", database);
                        cmd.ExecuteNonQuery();


                        cmd.CommandText = " select * from version_history";
                        dbVersion = cmd.ExecuteScalar();

                        conn.Close();
                        message = string.Format("Database {0} version acquired", database);
                        var dbVerString= dbVersion.ToString();
                        dbNum = Convert.ToDouble(dbVerString.Substring(0, 3));


                    }
                }
                catch (Exception ex)
                {
                    message = string.Format("Problem getting database {0} version. Error: {1}", database, ex.Message);

                }
                finally
                {
                    sw.Stop();
                    Console.WriteLine(message);
                    Console.WriteLine(string.Format("Duration: {0}", sw.Elapsed.Seconds.ToString()));
                }


                return dbNum;

            }
        }

        /// <summary>
        /// Create an instance of <see cref="IBrowserHelper"/>
        /// </summary>
        /// <returns>A implementation of <see cref="IBrowserHelper"/></returns>
        public static IBrowserHelper CreateBrowserHelper()
        {
            return new BrowserHelper();
        }

        /// <summary>
        /// Create an instance of <see cref="ISiteHelper"/>
        /// </summary>
        /// <returns>A implementation of <see cref="ISiteHelper"/></returns>
        public static ISiteHelper CreateSiteHelper()
        {
            return new FileSiteHelper();
        }
        /// <summary>
        /// Create an instance of <see cref="ISQLHelper"/>
        /// </summary>
        /// <returns>A implementation of <see cref="ISQLHelper"/></returns>
        public static ISQLHelper CreateSQLHelper()
        {
            return new SQLHelper();
        }
    }


}
