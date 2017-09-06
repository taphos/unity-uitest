using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class UITestExample : UITest
{
    MockNetworkClient mockNetworkClient;

    [SetUp]
    public void Init()
    {
        mockNetworkClient = new MockNetworkClient();
        // Replace the real networkClient with mock object, it will be injected later into FirstScreen component
        DependencyInjector.ReplaceComponent<NetworkClient>(mockNetworkClient);
    }

    [UnityTest]
    public IEnumerator SecondScreenCanBeOpenedFromTheFirstOne()
    {        
        yield return LoadScene("TestableGameScene");
        
        // Wait until object with given component appears in the scene
        yield return WaitFor(new ObjectAppeared<FirstScreen>());

        // Wait until button with given name appears and simulate press event
        yield return Press("Button-OpenSecondScreen");

        yield return WaitFor(new ObjectAppeared<SecondScreen>());

        // Wait until Text component with given name appears and assert its value
        yield return AssertLabel("SecondScreen/Text", "Second screen");

        yield return Press("Button-Close");

        // Wait until object with given component disappears from the scene
        yield return WaitFor(new ObjectDisappeared<SecondScreen>());
    }

    [UnityTest]
    public IEnumerator SuccessfulNetworkResponseIsDisplayedOnTheFirstScreen()
    {
        yield return LoadScene("TestableGameScene");
        
        yield return WaitFor(new ObjectAppeared<FirstScreen>());

        // Predefine the mocked server response
        mockNetworkClient.mockResponse = "Success!";

        yield return Press("Button-NetworkRequest");

        // Check the response displayed on UI
        yield return AssertLabel("FirstScreen/Text-Response", "Success!");

        // Assert the requested server parameter
        Assert.AreEqual(mockNetworkClient.mockRequest, "i_need_data");
    }

    [UnityTest]
    public IEnumerator FailingBoolCondition()
    {
        yield return LoadScene("TestableGameScene");
        
        yield return WaitFor(new ObjectAppeared("FirstScreen"));
        var s = Object.FindObjectOfType<FirstScreen>();

        // Wait until FirstScene component is disabled, this line will fail by timeout
        // BoolCondition can be used to wait until any condition is satisfied
        yield return WaitFor(new BoolCondition(() => !s.enabled));
    }
}

 