# AutoSideScrollCamera
Utility for creating camera paths for 2.5d games (3d objects in a 2d playspace) in Unity Engine.

## License
This code is freely available to you via the [WTFPL License](https://en.wikipedia.org/wiki/WTFPL)

***

## How To Use

### Linear Path
* On (or above) your camera object, place a **AutoSideScrollLinear** component.
* Place a series of gameobjects at the points along which you'd like to move.
* Add the objects in order to the **path** variable on the **AutoSideScrollLinear** component.
* To customize properties such as speed and camera FOV at given points, add an **AutoSideScrollNode** component to them and specify the desired values.
* To make the same customizations across the entire path, change the values on the **AutoSideScrollLinear** component.

### Curved Path
* On (or above) your camera object, place a **AutoSideScrollBezier** component.
* Create a path using the **BezierCurve** component ([available on the Asset Store for free](https://www.assetstore.unity3d.com/en/#!/content/11278))
* Add the path to the **AutoSideScrollBezier** component's **curve** field.
* To customize properties such as speed and camera FOV at given points, add an **AutoSideScrollNode** component to them and specify the desired values.
* To make the same customizations across the entire path, change the values on the **AutoSideScrollBezier** component.

***

## Classes
### AutoSideScrollPath (MonoBehavior Class)
Used to define a path that an object can travel along at specified world speed. Abstract class.  Child class must override AdvancePath and GetPosition according to the type of path.
* **skyboxSpeed** Set to nonzero value to cause the skybox to rotate at a speed relative to movement of the camera along the x axis.
* **playerCollideEdges** Set to true to automatically create 2D collision boxes at the edges of the screen which follow the camera.
* **defaultNodeInfo** Default values to use at all points along the path where no info has been explicitly specified.  See **AutoSideScrollNodeInfo**
* **fovChangeSpeed** The maximum rate (in degrees per second) at which the camera's FOV may change to match the values along the path.

***

### AutoSideScrollLinear ###
Linear version of **AutoSideScrollPath**.  Will move directly from one node to the next at the rates specified by **AutoSideScrollNodeInfo**
* **path** The list of nodes from which the path is generated.  These nodes may or may not each contain **AutoSideScrollNodeInfo** to override the default values specified by the path.

### AutoSideScrollBezier ###
Bezier spline version of **AutoSideScrollPath**.  Will move in a curved line from one node to the next at the rates specified by **AutoSideScrollNodeInfo**.  Depends on **BezierSpline** implementation [available on the Asset Store for free](https://www.assetstore.unity3d.com/en/#!/content/11278).
* **curve** Bezier curve path along which the camera will move.
