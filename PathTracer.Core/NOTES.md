## NOTES
- Do not use any cameras other than arcball, they are not done.

## TODO
1. Temporal Filtering does not work
	1. Omg... you are not saving the temporal filter result, you are only saving the output texture of your path trace pass
	2. Draw the temporal filter pass to a render target then display on-screen & save to prev target

2. Unlit Pass is no longer shown when toggling between techniques
	1. Issue likely caused by clearing the buffers once you draw to the render target

3. Write Triangle data with a buffer

4. Find a cleaner way to draw all passes
	1. A generic "Draw Pass function might be a good idea

5. Fix the translation issue.
	1. Notice the difference between Path-Traced & Rasterized techs when translating with RMB
	2. Could have something to do with '_cameraPosition' or '_cameraTransform'
	3. To keep parity, the camera transformations done in the GPU should match exactly 
	   those of the Arcball/PathTrace Camera

6. Allow the camera to be initially positioned in other directions
	1. Changing the current camera implementation's initial position causes issues
	2. This might have something to do with 'ViewportPointLocal' it currently only expects to face the z-axis
	3. A fix is to calculate the facing direction.




## NOT URGENT
1. Decouple more properties from the AbstractCamera class, like the transformation matrix. These need to be calculated properly

## LINKS
https://gabrielgambetta.com/computer-graphics-from-scratch/05-extending-the-raytracer.html#arbitrary-camera-positioning
https://github.com/SebLague/Ray-Tracing/blob/Episode01/Assets/Scripts/RayTracingManager.cs
