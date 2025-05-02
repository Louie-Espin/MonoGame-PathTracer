using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using NumVector4 = System.Numerics.Vector4;
using NumVector3 = System.Numerics.Vector3;
using NumVector2 = System.Numerics.Vector2;
using PathTracer.Core.Source.Model;

namespace PathTracer.Core.Source.GUI;

public class SettingsWindow {

	public static ImGuiRenderer GuiRenderer;

	/* GUI WINDOW DATA */
	bool _toggleGUI = true;
	private readonly NumVector2 _windowSize;

	/* SPHERE SETTINGS */
	int _currentSphere = 0;
	readonly int _numSpheres;
    
    NumVector3 _position      = new (0,0,0);
	NumVector4 _colorDiffuse  = Color.Black.ToVector4().ToNumerics();
	NumVector4 _colorSpecular = Color.Black.ToVector4().ToNumerics();
	NumVector3 _colorLight    = Color.Black.ToVector3().ToNumerics();
    float _radius   = 0.0f;
    float _specular = 0.0f;
	float _light    = 0.0f;
	float _gloss    = 0.0f;
	
	/* PROFILER SETTINGS */
	private static float _currFps = 0;
	private float[] _fps = new float[200];
	private int _fpsOffset = 0;
	private int setOffset(int currOffset, int arrSize) => (currOffset + 1) % arrSize;

	private int _textureId;
	private IntPtr? _TextureId;
	private Dictionary<IntPtr, Texture2D> _loadedTextures = new();

	public SettingsWindow(Game game, GraphicsDevice graphics, int numSpheres) {
		/* Imgui Renderer */
		GuiRenderer = new ImGuiRenderer(game);
		_numSpheres = numSpheres;
		_windowSize = new NumVector2(
			x: (graphics.Viewport.Width  * .25f),
			y: (graphics.Viewport.Height * .75f)
		);
	}

	public void onLoad() {
		GuiRenderer.RebuildFontAtlas();
	}

	public void Draw(GameTime gameTime, ref ModelList models) {
		if (!_toggleGUI) return;

        ImGuiWindowFlags flags = ImGuiWindowFlags.NoMove;

        GuiRenderer.BeginLayout(gameTime);
        ImGui.SetNextWindowSize(_windowSize);
		ImGui.Begin("Path Tracer Settings", ref _toggleGUI, flags);
		
		/*
		  if (ImGui.BeginMenuBar()) {
		   	if (ImGui.BeginMenu("File")) {
		   		if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff * / }
		   		if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff * / }
		   		if (ImGui.MenuItem("Close", "Ctrl+W")) { _toggleGUI = false; }
		   		ImGui.EndMenu();
		   	}
		   	ImGui.EndMenuBar();
		   }
		 */

		/* Draw Sphere List Sub-Window */
		DrawSphereSettings("Object List", models);
		
		/* Draw Information Sub-Window */
		DrawInformationWindow("Profiling", gameTime);
		
        ImGui.End();
		GuiRenderer.EndLayout();
	}

	public void DrawSphereSettings(string title, ModelList models) {
		ImGuiChildFlags flags = ImGuiChildFlags.Border | ImGuiChildFlags.FrameStyle;
		ImGui.BeginChild(title, new NumVector2(0, _windowSize.Y / 2), flags);
		
		/* Draw nested items */
		DrawSelectSphereButtons(models.Spheres);
		DrawSphereSettings(ref models.Spheres);
		
		ImGui.EndChild();
	}

	public void DrawInformationWindow(string title, GameTime gameTime) {
		ImGuiChildFlags flags = ImGuiChildFlags.Border | ImGuiChildFlags.FrameStyle | ImGuiChildFlags.AlwaysAutoResize;
		ImGui.BeginChild(title, new NumVector2(0, 0), flags);
		
		/* Update & Draw FPS */
		_currFps = (float) (1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
		_fps[_fpsOffset] = _currFps;
		_fpsOffset = setOffset(_fpsOffset, _fps.Length);
		ImGui.SeparatorText("Frames-Per-Second");
		ImGui.PlotLines($"{Math.Round(_currFps, 1)}##plotFPS",
			values: ref _fps[0],
			_fps.Length,
			_fpsOffset,
			null,
			scale_min: 0,
			scale_max: 60
		);
		
		ImGui.EndChild();
	}

	public void SetCurrentSphere(int index, Sphere sphere) {
		_currentSphere = index;
		_position      = sphere.Position;
		_radius        = sphere.Radius;
		_colorDiffuse  = sphere.DiffuseColor;
		_colorSpecular = sphere.SpecularColor;
		_colorLight	   = sphere.LightColor;
		_specular      = sphere.Specular;
		_gloss         = sphere.Gloss;
		_light         = sphere.Light;
	}

	public void DrawSelectSphereButtons(Sphere[] spheres) {
		ImGui.SeparatorText("Select Sphere");
		ImGui.NewLine();
		for (int i = 0; i < spheres.Length; i++) {
			ImGui.SameLine();
			if (ImGui.Button($"{i}"))
				SetCurrentSphere(i, spheres[i]);
		}
	}

	public void DrawSphereSettings(ref Sphere[] _sphere) {
        ImGui.BeginGroup();
        ImGui.SeparatorText("Material Settings");
        ImGui.ColorEdit4("Diffuse##DiffColor", ref _colorDiffuse);
        ImGui.ColorEdit4("Specular##SpecColor", ref _colorSpecular);
        ImGui.DragFloat("Smooth##SpecIntensity", ref _specular, v_speed: 0.001f, v_min: 0.0f, v_max: 1.0f);
		ImGui.DragFloat("Gloss", ref _gloss, v_speed: 0.001f, v_min: 0.0f, v_max: 1.0f);
		ImGui.Separator();
        ImGui.SeparatorText("Emission Settings");
        ImGui.ColorEdit3("Color##LiteColor", ref _colorLight);
        ImGui.SliderFloat("Intensity##LiteIntensity", ref _light, v_min: 0.0f, v_max: 100.0f);
        ImGui.SeparatorText("Transform Settings");
        ImGui.Text("Position");
        ImGui.Text("Radius");
        ImGui.EndGroup();
    }

	// https://github.com/ImGuiNET/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram.XNA/ImGuiRenderer.cs
	// Creates a pointer to a texture, which can be passed through ImGui calls.
	// That pointer is then used by ImGui to let us know what texture to draw
	public virtual IntPtr BindTexture(Texture2D texture) {
		var id = new IntPtr(_textureId++);

		_loadedTextures.Add(id, texture);

		return id;
	}

	// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
	public virtual void UnbindTexture(IntPtr textureId) {
		_loadedTextures.Remove(textureId);
	}
}
