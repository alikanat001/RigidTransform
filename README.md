# RigidTransform
Finding Transformation matrix between two different coordinate systems using Unity and Accord.NET

This script is created via following instructions from http://nghiaho.com/?page_id=671 as a test for finding Transformation matrix between Unity coordinate system and Laser scanner coordinate system. This method requires usage of 3x3 matrices and since Unity does not offer 3x3 matrices Accord.NET library is used. To be able to use the script please install Accord.NET using https://github.com/accord-net/framework/wiki/Unity

After installing Accord.Net in your empty project create 3 ref objects (blue objects in the image), 3 transform objects (red objects in the image) and a test object (yellow object in the image). Drag them to regarding public gameobject fields. Then you can Transform yellow object using Transformation Matrix calculated from RigidTransform. Initial state of the scene can be seen in the image below:

![alt text](https://github.com/alikanat001/RigidTransform/blob/master/InitialState.png)

Transformed state of the scene can be seen in the image below:

![alt text](https://github.com/alikanat001/RigidTransform/blob/master/TransformedState.png)
