using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReportingTestsRunner))]
public class ReportingTestsRunnerInspector : Editor
{
	ReportingTestsRunner TestsRunner { get { return (ReportingTestsRunner)serializedObject.targetObject; } }

    static string filter;
	static Dictionary<Type, List<string>> testsDictionary = new Dictionary<Type, List<string>>();
	static readonly List<string> testsPassed = new List<string>();
	static readonly List<string> testsFailed = new List<string>();
	static string lastPassedFailedUpdateTestName;

	static bool stylesCreated;
	static GUIStyle styleHeader;
	static GUIStyle styleNormal;
	static GUIStyle styleSkipped;
	static GUIStyle stylePassed;
	static GUIStyle styleFailed;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		string newFilter = TestsRunner.testfilter;
        if (filter != newFilter)
			UpdateTestLists();
		filter = newFilter;

		if (!stylesCreated)
		{
			styleHeader = new GUIStyle();
			styleHeader.normal.textColor = new Color(.8f, .8f, .8f);
			styleHeader.fontStyle = FontStyle.Bold;
			styleNormal = new GUIStyle();
			styleNormal.normal.textColor = Color.white;
			styleSkipped = new GUIStyle();
			styleSkipped.normal.textColor = Color.gray;
			stylePassed = new GUIStyle();
			stylePassed.normal.textColor = Color.green;
			styleFailed = new GUIStyle();
			styleFailed.normal.textColor = Color.red;

			UpdateTestLists();

			stylesCreated = true;
		}

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.Separator();
		if (Application.isPlaying)
			DrawTestsRuntime();
		else
			DrawTestsEditor();

		EditorGUILayout.EndVertical();
	}

	void DrawTestsRuntime()
	{
		foreach (var testPair in testsDictionary)
		{
			bool headerPrinted = false;
			for (int i = 0; i < testPair.Value.Count; i++)
			{
				var testName = testPair.Value[i];
				if (!headerPrinted)
				{
					GUILayout.Label("Test Fixture: " + testPair.Key, styleHeader);
					headerPrinted = true;
				}
				if (TestsRunner.CurrentFullTestName != lastPassedFailedUpdateTestName)
					UpdatePassedFailed();
				bool current = !string.IsNullOrEmpty(TestsRunner.CurrentFullTestName) && TestsRunner.CurrentFullTestName == testName;
				if (current)
					GUILayout.Label("...." + testName, styleNormal);
				else if (testsPassed.Contains(testName))
					GUILayout.Label("  + " + testName, stylePassed);
				else if (testsFailed.Contains(testName))
					GUILayout.Label("  - " + testName, styleFailed);
				else
					GUILayout.Label("  . " + testName, styleSkipped);
			}
		}

		EditorGUILayout.Separator();
        GUILayout.Label("RESULT: " + TestsRunner.testsFailedCount + " failed of " + TestsRunner.testsCount);
	}

	void DrawTestsEditor()
	{
		foreach (var testPair in testsDictionary)
		{
			if (testPair.Value.Count > 0) GUILayout.Label("Test Fixture: " + testPair.Key, styleHeader);
			for (int i = 0; i < testPair.Value.Count; i++)
			{
				var testName = testPair.Value[i];
				GUILayout.Label(("  + ") + testName, styleNormal);
			}
		}
	}

	void UpdateTestLists()
	{
		testsDictionary = GetTestsDictionary();
		testsPassed.Clear();
		testsFailed.Clear();
	}        

	Dictionary<Type, List<string>> GetTestsDictionary()
	{
		var result = new Dictionary<Type, List<string>>();

        var fixtures = TestsRunner.TestFixtures;
		for (int i = 0; i < fixtures.Count; i++)
		{
			Type testClassType = fixtures[i];
			if (!result.ContainsKey(testClassType))
				result.Add(testClassType, new List<string>());
			var classTests = TestsRunner.GetTestMethods(testClassType);
			for (int j = 0; j < classTests.Length; j++)
			{
				var test = classTests[j];
				string testName = testClassType + "." + test.Name;
                bool match = string.IsNullOrEmpty(TestsRunner.testfilter) || testName.ToLower().Contains(TestsRunner.testfilter.ToLower());
                if (match) result[testClassType].Add(testName);
			}
		}
		return result;
	}

	void UpdatePassedFailed()
	{
		lastPassedFailedUpdateTestName = TestsRunner.CurrentFullTestName;
		testsPassed.Clear();
		testsFailed.Clear();

        if (TestsRunner.testReports != null)
		{
			foreach (var e in TestsRunner.testReports)
			{
                foreach (var test in e.Value)
				{
					if (test.Failed) testsFailed.Add(test.name);
					else testsPassed.Add(test.name);
				}
			}
		}
	}
}
