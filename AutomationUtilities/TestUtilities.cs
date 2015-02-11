using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using WorkareaAutomation.PageObjects;

namespace WorkareaAutomation
{
    public static class TestUtilities
    {
  




        public static void CopyTemplate(string fileName, string sourcePath, string targetPath)
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
        }

        public static void RemoveTemplate(string fileName, string targetPath)
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

        }

        public static void BackupSiteDatabase(string database, string connString)
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
                message = string.Format("Problem backing up database {0}. Error: {1}", database, ex.Message);

            }
            finally
            {
                sw.Stop();
                Console.WriteLine(message);
                Console.WriteLine(string.Format("Duration: {0}", sw.Elapsed.Seconds.ToString()));
            }




        }
        
        public static void RestoreSiteDatabase(string database, string connString)
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
                Console.WriteLine(string.Format("Duration: {0}", sw.Elapsed.Seconds.ToString()));
            }

        }


        public static void LaunchTestSetupPage(string testSetupPage)
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["siteUrl"]);
            LoginPage loginPage = new LoginPage(driver);
            loginPage.EnterUserName(ConfigurationManager.AppSettings["adminusername"])
                .EnterUserPassWord(ConfigurationManager.AppSettings["adminpassword"])
                .Login()
                .OpenWorkarea();
            driver.Navigate().GoToUrl(string.Format("{0}/{1}",ConfigurationManager.AppSettings["siteUrl"], testSetupPage));
            driver.Close();
            driver.Quit();


        }



    }
}
