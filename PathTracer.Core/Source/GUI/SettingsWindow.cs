using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using NumVector4 = System.Numerics.Vector4;
using NumVector3 = System.Numerics.Vector3;
using PathTracer.Core.Source.Model;

namespace PathTracer.Core.Source.GUI;

public class SettingsWindow {

	public static ImGuiRenderer GuiRenderer;

	/* GUI DATA */
	bool _toggleGUI = true;

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

	private int _textureId;
	private IntPtr? _TextureId;
	private Dictionary<IntPtr, Texture2D> _loadedTextures = new();

	public SettingsWindow(Game game, int numSpheres) {
		/* Imgui Renderer */
		GuiRenderer = new ImGuiRenderer(game);
		_numSpheres = numSpheres;
	}

	public void onLoad() {
		GuiRenderer.RebuildFontAtlas();
	}

	public void Draw(GameTime gameTime, ref ModelList models) {
		if (!_toggleGUI) return;

        ImGuiWindowFlags flags = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse;

        GuiRenderer.BeginLayout(gameTime);
		ImGui.Begin("Path Tracer Settings", ref _toggleGUI, flags);

		if (ImGui.BeginMenuBar()) {
			if (ImGui.BeginMenu("File")) {
				if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
				if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }
				if (ImGui.MenuItem("Close", "Ctrl+W")) { _toggleGUI = false; }
				ImGui.EndMenu();
			}
			ImGui.EndMenuBar();
		}

		/* Draw Sphere List Settings */
		ImGui.BeginGroup();
		for (int i = 0; i < models.Spheres.Length; i++) {
			DrawSphereSettings(ref models.Spheres[i]);
		}
		ImGui.EndGroup();

		/* Draw FPS */
        ImGui.Text($"FPS: {Math.Round(1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000), 1)}");

        ImGui.End();
		GuiRenderer.EndLayout();
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

	public void DrawSphereSettings(ref Sphere _sphere) {
        ImGui.BeginGroup();

        ImGui.Text("Diffuse Settings");
        ImGui.ColorEdit4("Color", ref _colorDiffuse);
        ImGui.Separator();
        ImGui.Text("Specular Settings");
        ImGui.ColorEdit4("Color", ref _colorSpecular);
        ImGui.DragFloat("Intensity", ref _specular, v_speed: 0.001f, v_min: 0.0f, v_max: 1.0f);
        ImGui.Separator();
		ImGui.Text("Gloss Settings");
		ImGui.DragFloat("Gloss", ref _gloss, v_speed: 0.001f, v_min: 0.0f, v_max: 1.0f);
		ImGui.Separator();
        ImGui.Text("Light Settings");
        ImGui.ColorEdit3("Color", ref _colorLight);
        ImGui.DragFloat("Intensity", ref _light, v_speed: 0.5f, v_min: 0.0f, v_max: 100.0f);
        ImGui.Separator();

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
