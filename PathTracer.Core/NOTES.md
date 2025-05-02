## NOTES
- Do not use any cameras other than arcball, they are not done.

## TODO
1. Fix Temporal Accumulation & Add Accumulation Profiling
   1. Ensure you are saving the temporal accumulation result
   2. Draw the temporal filter pass to a render target then display on-screen & save to prev target
   3. Render a profiler for accumulation (0 - 1 value between prev & curr contribution)
2. Include an FPS profiler
3. Write Triangle data with a buffer
4. Find a cleaner way to draw all passes
	1. A generic "Draw Pass function might be a good idea

## BONUS / NOT URGENT
1. Fix the translation issue.
	1. Notice the difference between Path-Traced & Rasterized techs when translating with RMB
	2. Could have something to do with '_cameraPosition' or '_cameraTransform'
	3. To keep parity, the camera transformations done in the GPU should match exactly 
	   those of the Arcball/PathTrace Camera

2. Allow the camera to be initially positioned in other directions
	1. Changing the current camera implementation's initial position causes issues
	2. This might have something to do with 'ViewportPointLocal' it currently only expects to face the z-axis
	3. A fix is to calculate the facing direction.
3. Decouple more properties from the AbstractCamera class, like the transformation matrix. These need to be calculated properly

## LINKS
CG Graphics From Scratch
https://gabrielgambetta.com/computer-graphics-from-scratch/05-extending-the-raytracer.html#arbitrary-camera-positioning

Coding Adventures Sebastian Lague
https://github.com/SebLague/Ray-Tracing/blob/Episode01/Assets/Scripts/RayTracingManager.cs

Fix Your Time Step!
https://gafferongames.com/post/fix_your_timestep/

### ImGUI Resources

ImGUI Manual
https://pthom.github.io/imgui_manual_online/manual/imgui_manual.html

ImGUI FAQ
https://github.com/ocornut/imgui/blob/master/docs/FAQ.md#q-why-is-the-wrong-widget-reacting-when-i-click-on-one

ImGUI: Rendering your game scene into a texture
https://github.com/ocornut/imgui/wiki/Image-Loading-and-Displaying-Examples#rendering-your-game-scene-into-a-texture

ImGUI: rendering to texture .NET example
https://github.com/ImGuiNET/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram.XNA/ImGuiRenderer.cs

Extra - Cool Ray Tracer that also uses ImGUI
https://github.com/gallickgunner/Yune