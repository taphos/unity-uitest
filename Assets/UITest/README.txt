UI test automation framework

Features

* Allows to write automated tests that drive the game in a way similar to how a user would
* Integrated with Unity UI solution, can easily be integrated with custom solutions like NGUI or EZGUI
* Tests can be executed in Editor or Unity Player (tested Android, iOS and Standalone)
* Test reports in Editor GUI, Console and XML files (JUnit format)
* Includes lightweight dependency injection framework for object mocking


Running

* To run tests in Editor open scene Assets/UITest/Tests.unit and click Play
* Click TestRunner GameObject to see the test report in realtime
* Use filter field to run tests partially
* To run on device just set Tests scene as a first one before building


Implementing tests

* To add a new test create a new class anywhere in the project extending UITest
* Use UITest, UISetUp and UITearDown attributes same way as you would in Unit tests
* Checkout example in Assets/UITest/Examples/UITestExample.cs


Have fun testing ;)

Filipp Keks

