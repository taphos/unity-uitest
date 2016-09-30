using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using System.IO;

public class ReportingTestsRunner : TestsRunner
{
    public const string TestReportPath = "test-report";

    const string Delimeter = "--------------------------------------------------------------------------------";

    public string testfilter;

    [NonSerialized]
    public Dictionary<Type, List<TestReport>> testReports;

    public int testsFailedCount 
    {
        get {return testReports.Keys.Sum(f => testReports[f].Count(t => t.Failed)); }
    }

    public int testsCount
    {
        get {return testReports.Keys.Sum(f => testReports[f].Count()); }
    }

    [NonSerialized] public string CurrentFullTestName;

    DateTime timeStarted;

    public class TestReport
    {
        public string name;
        public string failedMessage;
        public string failedStackTrace;
        public float duration;

        public bool Failed 
        {
            get {return failedMessage != null;}    
        }
    }        

    void Awake()
    {
        DontDestroyOnLoad(this);
        Application.runInBackground = true;
        this.Inject();

        timeStarted = DateTime.Now;

        if (Directory.Exists(TestReportPath))
            Directory.Delete(TestReportPath, true);
        Directory.CreateDirectory(TestReportPath);
    }

    protected override IEnumerator RunUIFixtures()
    {
        testReports = new Dictionary<Type, List<TestReport>>();
        yield return StartCoroutine(base.RunUIFixtures());
        ReportText();
    }

    public override IEnumerator RunUIFixture(Type fixtureType, MethodInfo[] methods)
    {
        Debug.LogFormat("{0}\nTEST FIXTURE START: {1}", Delimeter, fixtureType);
        var timeStarted = DateTime.Now;

        yield return StartCoroutine(base.RunUIFixture(fixtureType, methods));

        var timeEnded = DateTime.Now;
        Debug.LogFormat("TEST FIXTURE END: {0} ({1}s)\n{2}",
            fixtureType,
            (timeEnded - timeStarted).TotalSeconds.ToString("0.000"),
            Delimeter);            
        ReportXml(fixtureType, timeStarted, timeEnded);
    }

    public void ReportXml(Type fixtureType, DateTime timeStarted, DateTime timeEnded)
    {
        if (testReports[fixtureType].Count == 0) return;
        
        using (var writer = File.CreateText(TestReportPath + "/TEST-" + fixtureType + ".xml")) {            
            int startedTests = testReports[fixtureType].Count();
            int failedTests = testReports[fixtureType].Count(t => t.Failed);

            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            writer.WriteLine(string.Format("<testsuite name=\"{0}\" tests=\"{1}\" skipped=\"0\" failures=\"{2}\" errors=\"0\" timestamp=\"{3}\" time=\"{4}\">",
                fixtureType,
                startedTests,
                failedTests,
                timeStarted.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                (timeEnded - timeStarted).TotalSeconds.ToString("0.000")));
            foreach (var r in testReports[fixtureType])
                ReportXml(fixtureType, r, writer);
            writer.WriteLine("</testsuite>");
        }
    }

    public void ReportXml(Type fixtureType, TestReport report, StreamWriter writer)
    {
        writer.WriteLine(string.Format("<testcase name=\"{0}\" classname=\"{1}\" time=\"{2}\">",
            report.name.Replace(fixtureType + ".", ""),
            fixtureType,
            report.duration.ToString("0.000")));
        if (report.Failed)
        {
            writer.Write(string.Format("<failure message=\"{0}\"><![CDATA[", XmlEscapeFailedMessage(report.failedMessage)));
            writer.Write(report.failedStackTrace);
            writer.WriteLine("]]></failure>");
        }
        writer.Write("<system-out><![CDATA[");
        using (var output = File.OpenText(GetReportLogFilePath(report.name)))
            Copy(output, writer);
        writer.WriteLine("]]></system-out>");
        writer.WriteLine("</testcase>");
    }

    static long Copy(StreamReader source, StreamWriter target)
    {
        long total = 0;
        var buffer = new char[0x1000];
        int n;
        while ((n = source.Read(buffer, 0, buffer.Length)) != 0)
        {
            target.Write(buffer, 0, n);
            total += n;
        }
        return total;
    }

    string GetReportLogFilePath(string testName)
    {
        return TestReportPath + "/TEST-" + testName + ".out";
    }
        
    void ReportText()
    {
        Debug.Log(Delimeter + "\n");
        Debug.Log("TESTS DURATION: " + FormatTime(DateTime.Now, timeStarted) + "\n");
        Debug.Log("TESTS DURATION TOTALSECONDS: " + (DateTime.Now - timeStarted).TotalSeconds);
        Debug.Log("\n" + Delimeter);
        Debug.Log("ALL UI TESTS FINISHED");

        var output = new StringBuilder();
        if (testsFailedCount == 0 && testsCount > 0)
            output.Append(Delimeter + "\nALL UI TESTS PASSED\n");
        float percentage = ((testsCount - testsFailedCount) / (float)testsCount) * 100f;
        string results = "TEST RESULTS: " + percentage.ToString("0.000") + "% = " + (testsCount - testsFailedCount) + "/" + testsCount;
        output.Append(Delimeter + "\n" + results + "\n");
        foreach (var type in testReports.Keys)
            ReportText(type, testReports[type], output);
        output.Append(Delimeter + "\n" + results + "\n");

        Debug.Log(output.ToString());
    }

    public void ReportText(Type fixtureType, List<TestReport> reports, StringBuilder sb)
    {
        sb.AppendLine("\tTest Fixture : " + fixtureType);
        foreach (var r in reports)
            sb.AppendLine("\t\t" + (r.Failed ? "FAILED" : "OK    ") + " : " + r.name + " (" + r.duration.ToString("0.000") + "s)");
    }

    protected override bool ShouldRunTest(string name)
    {
        return testfilter == null ||               
               name.ToLower().Contains(testfilter.ToLower());
    }

    public static string FormatTime(DateTime endTime, DateTime startTime)
    {
        var finalTime = endTime - startTime;
        string hour = finalTime.Hours.ToString("00");
        string minute = finalTime.Minutes.ToString("00");
        string seconds = finalTime.Seconds.ToString("00");
        return hour + ":" + minute + ":" + seconds;
    }

    protected override IEnumerator RunUITest(Type fixtureType, MethodInfo method)
    {        
        var timeStarted = DateTime.Now;

        CurrentFullTestName = fixtureType + "." + method.Name;
        Debug.LogFormat("{0}\nTEST STARTED: {1}\n", Delimeter, CurrentFullTestName);
        if (!testReports.ContainsKey(fixtureType)) testReports[fixtureType] = new List<TestReport>();
        var report = new TestReport();
        report.name = CurrentFullTestName;
        testReports[fixtureType].Add(report);
        var output = File.CreateText(GetReportLogFilePath(report.name));

        Application.LogCallback logReceived = (condition, stackTrace, type) => {
            if ((type == LogType.Error || type == LogType.Exception) && !UnityInternalError(condition)) {
                report.failedMessage = condition;
                report.failedStackTrace = stackTrace;
            }
            output.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + type.ToString().PadRight(10) + ": " + condition);
            if (type != LogType.Log) output.WriteLine(stackTrace);
        };
        Application.logMessageReceived += logReceived;

        yield return StartCoroutine(base.RunUITest(fixtureType, method));

        Application.logMessageReceived -= logReceived;
        output.Close();
        report.duration = (float)(DateTime.Now - timeStarted).TotalSeconds;
        Debug.LogFormat("TEST {0}: {1} ({2}s)\n",
            (report.Failed ? "FAILED" : "PASSED"),
            report.name,
            report.duration.ToString("0.000"));
        CurrentFullTestName = null;
    }        

    public static string XmlEscapeFailedMessage(string str)
    {
        if (string.IsNullOrEmpty(str))
            return "";
        var idx = str.IndexOf("\n", StringComparison.Ordinal);
        if (idx != -1)
            str = str.Substring(0, idx);
        return str.Replace("<", "&lt;").Replace(">", "&gt;");
    }
}
