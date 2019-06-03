Note: this library is currently unsupported, and has not been updated in a while. There are things I've learned in the mean time to make it much more efficient, but I do not currently have time to update MeshX library. Stay tuned and I might update at some point.

# MeshX
Mesh extensions for unity

MeshX is a library for creating procedural meshes as simple as possible, for example:

```csharp
gameObject.InitMesh(MeshX.Cube());
```

This line will create a unit cube, create a MeshFilter, and apply the cube to it, create a MeshRenderer and apply a default unity material to it, all in just one short line.

Also,

```csharp
gameObject.InitChildMesh(MeshX.Cube());
```


will do the same thing, but to a new, child GameObject.

To start using MeshX, you just need to add:
```using MeshXtensions;```

It also includes methods to create Cubes, Spheres, Cylinders, etc.

Some additional features:
- It includes a fractional UV solver that will snap to the closest tile in cubic meshes
