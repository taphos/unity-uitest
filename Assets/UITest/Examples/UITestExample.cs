using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class UITestExample : UITest
{
    MockNetworkClient mockNetworkClient;

    [UISetUp]
    public IEnumerable Init()
    {
        mockNetworkClient = new MockNetworkClient();
        // Replace the real networkClient with mock object, it will be injected later into FirstScreen component
        DependencyInjector.ReplaceComponent<NetworkClient>(mockNetworkClient);

#if UNITY_EDITOR
        yield return LoadSceneByPath("Assets/UITest/Examples/TestableGameScene.unity");
#elif
        yield return LoadScene("TestableGameScene");
#endif
    }

    [UITest]
    public IEnumerable SecondScreenCanBeOpenedFromTheFirstOne()
    {
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

    [UITest]
    public IEnumerable SuccessfulNetworkResponseIsDisplayedOnTheFirstScreen()
    {
        yield return WaitFor(new ObjectAppeared<FirstScreen>());

        // Predefine the mocked server response
        mockNetworkClient.mockResponse = "Success!";

        yield return Press("Button-NetworkRequest");

        // Check the response displayed on UI
        yield return AssertLabel("FirstScreen/Text-Response", "Success!");

        // Assert the requested server parameter
        Assert.AreEqual(mockNetworkClient.mockRequest, "i_need_data");
    }

    [UITest]
    public IEnumerable FailingBoolCondition()
    {
        yield return WaitFor(new ObjectAppeared("FirstScreen"));
        var s = FindObjectOfType<FirstScreen>();

        // Wait until FirstScene component is disabled, this line will fail by timeout
        // BoolCondition can be used to wait until any condition is satisfied
        yield return WaitFor(new BoolCondition(() => !s.enabled));
    }
}

