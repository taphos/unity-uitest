# Unity UI Test Automation Framework

# Features

* Allows to write automated tests that drive the game in a way similar to how a user would
* Integrated with Unity UI solution, can easily be integrated with custom solutions like NGUI or EZGUI
* Tests can be executed in Editor or Unity Player (tested Android, iOS and Standalone)
* Test reports in Editor GUI, Console and XML files (JUnit format)
* Includes lightweight dependency injection framework for object mocking


# Running

* To run tests in Editor open scene Assets/UITest/Example/TestRunner.unit and click Play
* Click TestRunner GameObject to see the test report in realtime
* Use filter field to run tests partially
* To run on device just set Tests scene as a first one before building


# Implementing tests

* To add a new test create a new class anywhere in the project extending UITest
* Use UITest, UISetUp and UITearDown attributes same way as you would in Unit tests
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


Have fun testing ;)

Filipp Keks

