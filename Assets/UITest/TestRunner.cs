using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class TestsRunner : MonoBehaviour
{
    List<Type> testFixtures;
    public List<Type> TestFixtures {
        get {
            if (testFixtures == null) {
                testFixtures = new List<Type>();
                foreach (Type type in Assembly.GetAssembly(typeof(UITest)).GetTypes()
                         .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(UITest))))
                    testFixtures.Add(type);
            }
            return testFixtures;
        }
    }

    IEnumerator Start()
    {		
		yield return StartCoroutine(RunUIFixtures());
        Application.Quit();
    }

    protected virtual IEnumerator RunUIFixtures()
    {
        foreach (var type in TestFixtures) {
            var methods = GetTestMethods(type);
            if (methods.Length != 0)
                yield return StartCoroutine(RunUIFixture(type, methods));
        }
    }

    public virtual IEnumerator RunUIFixture(Type fixtureType, MethodInfo[] methods)
    {
        foreach (var method in methods)
            yield return StartCoroutine(RunUITest(fixtureType, method));
    }
        
    public MethodInfo[] GetTestMethods(Type fixtureType)
    {
        return fixtureType.GetMethods()
            .Where(m => Attribute.IsDefined(m, typeof(UITestAttribute)) && ShouldRunTest(fixtureType + "." + m.Name))
            .ToArray();
    }

    protected virtual bool ShouldRunTest(string name)
    {
        return true;
    }

    protected virtual IEnumerator RunUITest(Type fixtureType, MethodInfo testMethod)
    {
        var go = new GameObject(fixtureType.ToString());
        var component = (UITest)go.AddComponent(fixtureType);

        Application.LogCallback logReceived = (condition, stackTrace, type) => {
				if ((type == LogType.Error || type == LogType.Exception) && !UnityInternalError(condition))
                Destroy(component.gameObject);
        };
        Application.logMessageReceived += logReceived;

        StartCoroutine(component.RunTest(testMethod));
        while (component != null)
            yield return new WaitForSeconds(0.5f);

        Application.logMessageReceived -= logReceived;
    }

	protected static bool UnityInternalError(string condition)
	{
		return condition.StartsWith("The profiler has run out of samples") ||
				condition.StartsWith("Multiple plugins with the same name") ||
				condition.StartsWith("String too long for TextMeshGenerator");
	}
}
