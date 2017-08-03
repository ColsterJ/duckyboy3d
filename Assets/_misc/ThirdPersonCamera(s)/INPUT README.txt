How to Rotate the BasicCameraController has been a question that's come up a few times before.

There are 2 options on the EasyUnityInput object that change when input occurs.
1) Enable Rotation Only When Mouse Pressed
This is also affected by the "Mouse Input Button Name"
2) Enable Rotation Only When Cursor Locked
This is affected by the Cursor object's  “Lock Button Input Name” and “Hold To Release Input Name” or “Hold To Release Lock”

The option is programatically creating your own way to provide input to the Input component.
Both the InputValues object on the InputComponent and the InputComponent itself have multiple methods for manipulating the values of the input.
Accessing either of these directly is completely okay and designed to work.

If you do that, then you can write you’re own way of providing input to the basic camera and it’ll consume what ever is on it’s InputComponent to perform rotations and zooming.

The EasyUnityInputComponent just does some basic grabbing of information using Unity’s default input system and passes it for you, thus why I call it “Easy”. If you have you’re own input system, you’ll have to pass it yourself.

If you want to use an input system different than Unity's default that I have programmed into the EasyUnityInputComponent, you will have to write your own way of passing InputValues into the InputComponent on the BasicCameraController.