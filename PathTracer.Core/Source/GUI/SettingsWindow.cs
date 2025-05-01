using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Numerics.Vector4;
using MonoGame.ImGuiNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace PathTracer.Core.Source.GUI;

public class SettingsWindow {

	public static ImGuiRenderer GuiRenderer;

	/* GUI DATA */
	bool _toggleGUI = true;
	System.Numerics.Vector4 _colorDiffuse = Color.Red.ToVector4().ToNumerics();
	System.Numerics.Vector4 _colorSpecular = Color.White.ToVector4().ToNumerics();
	System.Numerics.Vector4 _colorLight = Color.White.ToVector4().ToNumerics();

	(float min, float max) _specular = (0.0f, 1.0f);

	private int _textureId;
	private IntPtr? _TextureId;
	private Dictionary<IntPtr, Texture2D> _loadedTextures = new();

	public SettingsWindow(Game game) {
		/* Imgui Renderer */
		GuiRenderer = new ImGuiRenderer(game);
		// _colorDiffuse = Color.White.ToVector4().ToNumerics();
	}

	public void onLoad() {
		GuiRenderer.RebuildFontAtlas();
	}

	public void Draw(GameTime gameTime) {
		if (!_toggleGUI) return;

		GuiRenderer.BeginLayout(gameTime);
		ImGuiWindowFlags flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse;

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

		ImGui.Text("Diffuse Settings");
		ImGui.ColorEdit4("Color", ref _colorDiffuse);

		ImGui.Separator();

		ImGui.Text("Specular Settings");
		ImGui.ColorEdit4("Color", ref _colorSpecular);
		ImGui.DragFloatRange2("Intensity", ref _specular.min, ref _specular.max);

		ImGui.Separator();

		ImGui.Text("Light Settings");
		ImGui.ColorEdit4("Color", ref _colorLight);
		ImGui.DragFloatRange2("Intensity", ref _specular.min, ref _specular.max);

		ImGui.Separator();

		ImGui.Text($"FPS: {Math.Round(1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000), 1)}");

		ImGui.End();

		GuiRenderer.EndLayout();
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
