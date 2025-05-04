using Microsoft.Xna.Framework;
using ImGuiNET;
using NumVector4 = System.Numerics.Vector4;
using NumVector3 = System.Numerics.Vector3;
using NumVector2 = System.Numerics.Vector2;
using PathTracer.Core.Source.Model;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace PathTracer.Core.Source.GUI; 
public class SceneWindow {

	ImGuiChildFlags flags = ImGuiChildFlags.ResizeY;
	ImGuiWindowFlags _flags2 = ImGuiWindowFlags.NoBackground;

	/* SPHERE SETTINGS */
	int _currentSphere = 0;
	readonly int _numSpheres;

	NumVector3 _position = new(0, 0, 0);
	NumVector4 _colorDiffuse = Color.Black.ToVector4().ToNumerics();
	NumVector4 _colorSpecular = Color.Black.ToVector4().ToNumerics();
	NumVector3 _colorLight = Color.Black.ToVector3().ToNumerics();
	float _radius = 0.0f;
	float _specular = 0.0f;
	float _light = 0.0f;
	float _gloss = 0.0f;

	public SceneWindow(int numSpheres) {
		_numSpheres = numSpheres;
	}

	public void DrawGUI(string title, ModelList models, Effect effect) {
		NumVector2 size = new(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y / 2);

		if (!ImGui.BeginChild($"{title}##sceneChild", size, flags))
			return;
		
		/* Draw scene item settings */
		for (int i = 0; i < models.Spheres.Length; i++) {
			DrawSphereSettings(ref models.Spheres[i], i);
			ImGui.Dummy(new(0,5));
		}

		ImGui.EndChild();

		/* Set _ptEffect with a list of default sphere object data FIXME DONT DO THIS!!!! */
		foreach (SphereParam e in Enum.GetValues(typeof(SphereParam))) {
			models.SetData(ref effect, e);
		}
	}

	public void SetCurrentSphere(int index, Sphere sphere) {
		_currentSphere = index;
		_position = sphere.Position;
		_radius = sphere.Radius;
		_colorDiffuse = sphere.DiffuseColor;
		_colorSpecular = sphere.SpecularColor;
		_colorLight = sphere.LightColor;
		_specular = sphere.Specular;
		_gloss = sphere.Gloss;
		_light = sphere.Light;
	}

	public void DrawSphereSettings(ref Sphere _sphere, int idx) {
		
		ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		if (!ImGui.CollapsingHeader($"sphere[{idx}]"))
			return;

		/* Material Settings */
		ImGui.SeparatorText("Material Settings");
		ImGui.ColorEdit4($"Diffuse##DiffColor[{idx}]", ref _sphere.DiffuseColor);
		ImGui.ColorEdit4($"Specular##SpecColor[{idx}]", ref _sphere.SpecularColor);
		ImGui.SliderFloat($"Smooth##SpecIntensity[{idx}]", ref _sphere.Specular, v_min: 0.0f, v_max: 1.0f);
		ImGui.SliderFloat($"Gloss##Gloss[{idx}]", ref _sphere.Gloss, v_min: 0.0f, v_max: 1.0f);

		ImGui.Dummy(new(0, 2));

		/* Emission Settings */
		ImGui.SeparatorText("Emission Settings");
		ImGui.ColorEdit3($"Color##LiteColor[{idx}]", ref _sphere.LightColor);
		ImGui.DragFloat($"Intensity##LiteIntensity[{idx}]", ref _sphere.Light, v_speed: 0.1f, v_min: 0.0f, v_max: 100.0f);

		ImGui.Dummy(new(0, 2));

		/* Transform Settings */
		ImGui.SeparatorText("Transform Settings");
		ImGui.Text("Position");
		ImGui.Text("Radius");

		ImGui.Dummy(new(0, 2));
	}
}
