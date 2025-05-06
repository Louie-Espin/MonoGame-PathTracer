# README.md

## Hello!
The program has been set up to boot with low path trace settings in order to prevent slowdown.
I've also added some profiling so users can track performance as they change the render quality and scene.
Please feel free to play around with them!

## What to Run
Run `PathTracer.WindowsDX` to run all code from `PathTracer.Core` in a Windows platform
(`PathTracer.DesktopGL` is a WIP and currently not supported)

## Controls
**Left Mouse Button:** Use the GUI
**Right Mouse Button:** Press to rotate an arcball camera around the scene.
**Middle Mouse Wheel:** Press to translate an arcball camera along the Z-axis.

## GUI
**Scene Material Editor:** change material properties of any object in the scene
**Render Settings:** toggle between rendering techniques and change the parameters of the ray tracing kernel
**Profiler:** check out the program's performance as render settings change or you update the scene
**Click the arrow in the top-left corner to hide the UI.**

# DEV NOTES:

## TODO
1. Write Triangle data with a buffer
2. Implement material refraction settings
3. Implement more cameras besides arcball
4. Find a cleaner way to draw all passes (A generic DrawPass class might be a good idea)

## BONUS / NOT URGENT
1. Fix the translation issue when using cameras other than arcball.
	1. Notice the difference between Path-Traced & Rasterized techs when translating with RMB
	2. Could have something to do with '_cameraPosition' or '_cameraTransform'
	3. To keep parity, the camera transformations done in the GPU should match exactly 
	   those of the PathTrace Camera
2. Allow the camera to be initially positioned in other directions
	1. Changing the current camera implementation's initial position causes issues
	2. This might have something to do with 'ViewportPointLocal' it currently only expects to face the z-axis
	3. A fix is to calculate the facing direction.
3. Decouple more properties from the AbstractCamera class, like the transformation matrix. These need to be calculated properly

## LINKS

### General Resources
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