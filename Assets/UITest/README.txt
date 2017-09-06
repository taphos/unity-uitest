# Unity UI Test Automation Framework

This version of framework is supported by ***Unity 2017*** and later.

Comparing to the previous version it is integrated with Unity PlayMode Test Runner instead of custom runner implementation.
Note that running tests in platform players (for example Standalone, Android, or iOS) from command line is not currently supported by Unity. https://docs.unity3d.com/Manual/testing-editortestsrunner.html

If you would like to execute tests from command line or write tests for older Unity versions check out previous version of the framework https://github.com/taphos/unity-uitest/tree/1.0


# Features

* Allows to write automated tests that drive the game in a way similar to how a user would
* Integrated with Unity UI solution, can easily be integrated with custom solutions like NGUI or EZGUI
* Integrated with Unity Test Runner
* Tests can be executed in Unity Player or Editor (PlayMode)
* Includes lightweight dependency injection framework for object mocking


# Running

* In Editor Open Window->Test Runner
* Switch to PlayMode tab  
* Click Run


# Implementing tests

* To add a new test create a new class anywhere in the project extending UITest
* Use UnityTest, SetUp and TearDown attributes same way as you would in Unit tests
* Checkout example in Assets/UITest/Examples/UITestExample.cs


# API

API is designed to be readable as a natural language so it can be understood by non technical people too. All API calls are designed to wait until its function could be executed with a certain timeout.

* `Press(<GameObjectName>)` - Simulates a button press. If an object with a given name is not found in the scene, it waits for it to appear.
* `LoadScene(<SceneName>)` - Load new scene and wait until scene is fully loaded.
* `AssertLabel(<GameObjectName>, <Text>)` - Asserts text value, waits until value is changed.
* `WaitFor(<Condition>)` - Generic method to wait until given condition is satisfied.
* `WaitFor(new LabelTextAppeared(<GameObjectName>, <Text>))` - Wait for label with given text to appear
* `WaitFor(new SceneLoaded(<SceneName>))` - Wait until scene is fully loaded
* `WaitFor(new ObjectAppeared(<GameObjectName>))` - Wait for object with given name to appear
* `WaitFor(new ObjectAppeared<ObjectType>())` - Wait for object with component of given type to appear
* `WaitFor(new ObjectDisappeared(<GameObjectName>))` - Wait for object with given name to disappear
* `WaitFor(new ObjectDisappeared<ObjectType>())` - Wait for object with component of given type to disappear
* `WaitFor(new BoolCondition(<BoolFunction>))` - Generic condition is satisfied when a given bool expression becomes true


Check out my blog post for in depth description http://blog.filippkeks.com/2016/11/21/why-game-developers-are-afraid-of-test-automation.html
Have fun testing ;)

Filipp Keks
