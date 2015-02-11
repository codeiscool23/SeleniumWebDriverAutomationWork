using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AutomationUtilities
{
    public class TestManager
    {
        private static ITestManagementService _tms;
        private static ITestManagementTeamProject _project;
        private static ITestPlan _testPlan;
        private static IList<ITestPoint> _testPoints;
        private static ITestRun _testRun;
        private static ITestCaseResult _testCaseResult;
        static private Dictionary<string,int> automatedCases = new Dictionary<string, int>();
        
        static private List<AutomatedTest> automatedTests = new List<AutomatedTest>(); 

        private static TfsTeamProjectCollection _tfs;

        /// <summary>
        /// Creates a test run based off the supplied TFS Address and Test Plan.  Required in order
        /// to report to Test Manager the results of a given test.
        /// </summary>
        /// <param name="tfsAddress"></param>
        /// <param name="testPlan"></param>
        public static void CreateTfsTestRun(string tfsAddress, int testPlan)
        {
            _tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri("http://tfs:8080"));
            _tfs.Authenticate();            
            
            string project = ConfigurationManager.AppSettings["Project"];
            _tms = _tfs.GetService<ITestManagementService>();
            
            
            _project = _tms.GetTeamProject(project);

            _testPlan = _project.TestPlans.Find(testPlan);
            
         
            _testPoints = _testPlan.QueryTestPoints("Select * FROM TestPoint");
            _testRun = CreateTestRun(_testPlan, _testPoints, ConfigurationManager.AppSettings["TestPlanName"]);

            foreach (var testPoint in _testPoints)
            {
                if (testPoint.IsTestCaseAutomated)
                {
                    try
                    {
                        ITestCase t = testPoint.TestCaseWorkItem;

                        AutomatedTest t2 = new AutomatedTest
                        {
                            testMethod = t.Implementation.DisplayText,
                            testCaseId = t.Id,
                            configurationName = testPoint.ConfigurationName

                        };

                        automatedTests.Add(t2);
                        automatedCases.Add(t.Implementation.DisplayText, t.Id);
                        
                    }
                    catch (Exception ex)
                    {
                        
                    }

                }
            }
            
        }


        /// <summary>
        /// Sends a pass result to TFS Test Manager based on the run created in CreateTfsTestRun.
        /// </summary>
        /// <param name="testCaseId"></param>
        public static void PassTestCase(int testCaseId)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == testCaseId);
            _testCaseResult.Outcome = TestOutcome.Passed;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
        }


        /// <summary>
        /// Sends a pass result to TFS Test Manager based on the run created in CreateTfsTestRun.
        /// </summary>
        /// <param name="testMethod"></param>
        public static void PassTestCase(string testMethod)
        {           
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == getTestCaseId(testMethod));
            _testCaseResult.Outcome = TestOutcome.Passed;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
           
        }

        /// <summary>
        /// Sends a pass result to TFS Test Manager based on the run created in CreateTfsTestRun.
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="configurationName"></param>
        public static void PassTestCase(string testMethod, string configurationName)
        {
			List<AutomatedTest> tests = getTestCases(testMethod);
            AutomatedTest targetTest = tests.First(x => x.configurationName == configurationName);
            List<ITestCaseResult> tempResult = _testRun.QueryResults().Where(x => x.TestCaseId == targetTest.testCaseId).ToList();
            _testCaseResult = tempResult.First(x => x.TestConfigurationName == configurationName);             
            _testCaseResult.Outcome = TestOutcome.Passed;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
           
        }

        
        /// <summary>
        /// Sends a fail result to TFS Test Manager based on the run created in CreateTfsTestRun.
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="ex"></param>
        public static void FailTestCase(string testMethod, Exception ex)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == getTestCaseId(testMethod));
            _testCaseResult.Outcome = TestOutcome.Failed;
            _testCaseResult.Comment = ex.Message;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();

        }

        /// <summary>
        /// Sends a fail result to TFS Test Manager based on the run created in CreateTfsTestRun
        /// </summary>
        /// <param name="testCaseId"></param>
        /// <param name="ex"></param>
        public static void FailTestCase(int testCaseId, Exception ex)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == testCaseId);
            _testCaseResult.Outcome = TestOutcome.Failed;
            _testCaseResult.Comment = ex.ToString();
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
        }


        /// <summary>
        /// Sends a fail result to TFS Test Manager based on the run created in CreateTfsTestRun
        /// </summary>
        /// <param name="testCaseId"></param>
        /// <param name="message"></param>
        public static void FailTestCase(int testCaseId, string message)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == testCaseId);
            _testCaseResult.Outcome = TestOutcome.Failed;
            _testCaseResult.Comment = message;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
        }

        /// <summary>
        /// Sends a fail result to TFS Test Manager based on the run created in CreateTfsTestRun
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="message"></param>
        public static void FailTestCase(string testMethod, string message)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == getTestCaseId(testMethod));
            _testCaseResult.Outcome = TestOutcome.Failed;
            _testCaseResult.Comment = message;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
        }


        /// <summary>
        /// Sends a fail result to TFS Test Manager based on the run created in CreateTfsTestRun.
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="ex"></param>
        /// <param name="screenShotDirectory"></param>
        public static void FailTestCase(string testMethod, Exception ex, string screenShotDirectory)
        {
            string screenShotTitle = testMethod + ".png";

            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == getTestCaseId(testMethod));
            _testCaseResult.Outcome = TestOutcome.Failed;
            _testCaseResult.Comment = ex.Message;
            _testCaseResult.Attachments = Path.Combine(screenShotDirectory, screenShotTitle);
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();

        }

        
        /// <summary>
        /// Sends a not executed result to TFS Test Manager based on the run created in CreateTfsTestRun
        /// </summary>
        /// <param name="testCaseId"></param>
        /// <param name="message"></param>
        public static void TestCaseNotExecuted(int testCaseId, string message)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == testCaseId);
            _testCaseResult.Outcome = TestOutcome.NotExecuted;
            _testCaseResult.Comment = message;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
        }

        /// <summary>
        /// Sends a not executed result to TFS Test Manager based on the run created in CreateTfsTestRun
        /// </summary>
        /// <param name="testMethod"></param>
        /// <param name="message"></param>
        public static void TestCaseNotExecuted(string testMethod, string message)
        {
            _testCaseResult = _testRun.QueryResults().First(x => x.GetTestCase().Id == getTestCaseId(testMethod));
            _testCaseResult.Outcome = TestOutcome.NotExecuted;
            _testCaseResult.Comment = message;
            _testCaseResult.State = TestResultState.Completed;
            _testCaseResult.Save();
        }

        private static ITestRun CreateTestRun(ITestPlan plan, IList<ITestPoint> points, string testTitle)
        {
            ITestRun run = plan.CreateTestRun(false);
            run.Title = String.Format("{0} {1}", testTitle, DateTime.Now);
            foreach (ITestPoint tp in points)
            {
                run.AddTestPoint(tp, null);
            }
            run.BuildNumber = "Build # Placeholder";

            run.Save();
            return run;
        }
        

        private static int getTestCaseId(string partialMethodText)
        {
            int testCaseId = automatedCases.First(x => x.Key.Contains(partialMethodText)).Value;
            return testCaseId;
        }

        private static List<int> getTestCaseIds(string partialMethodText)
        {
            List<AutomatedTest> tests = automatedTests.Where(x => x.testMethod.Contains(partialMethodText)).ToList();
            List<int> testCaseIds = new List<int>();
            tests.ForEach(x => testCaseIds.Add(x.testCaseId));
            return testCaseIds;
        }

        private static List<AutomatedTest> getTestCases(string partialMethodText)
        {
            List<AutomatedTest> tests = automatedTests.Where(x => x.testMethod.Contains(partialMethodText)).ToList();
            return tests;
        }

        public static void logTestResult(TestContext testContext)
        {
            if (testContext.CurrentTestOutcome.ToString() == "Passed")
                PassTestCase(testContext.TestName);
            FailTestCase(testContext.TestName,testContext.CurrentTestOutcome.ToString());            
        }
    }

    internal class AutomatedTest
    {
        public int testCaseId { get; set; }
        public string configurationName { get; set; }
        public string testMethod { get; set; }
    }
}
