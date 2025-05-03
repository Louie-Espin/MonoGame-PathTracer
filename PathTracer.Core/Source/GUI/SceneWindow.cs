using Microsoft.Xna.Framework;
using ImGuiNET;
using NumVector4 = System.Numerics.Vector4;
using NumVector3 = System.Numerics.Vector3;
using NumVector2 = System.Numerics.Vector2;
using PathTracer.Core.Source.Model;

namespace PathTracer.Core.Source.GUI; 
public class SceneWindow {

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

	public void DrawGUI(string title, ModelList models) {
		ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		if (!ImGui.CollapsingHeader(title))
			return;
		
		/* Draw nested items */
		DrawSelectSphereButtons(models.Spheres);
		DrawSphereSettings(ref models.Spheres);
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
		ImGui.SliderFloat("Smooth##SpecIntensity", ref _specular, v_min: 0.0f, v_max: 1.0f);
		ImGui.SliderFloat("Gloss", ref _gloss, v_min: 0.0f, v_max: 1.0f);
		ImGui.Separator();
		ImGui.SeparatorText("Emission Settings");
		ImGui.ColorEdit3("Color##LiteColor", ref _colorLight);
		ImGui.DragFloat("Intensity##LiteIntensity", ref _light, v_speed: 0.1f, v_min: 0.0f, v_max: 100.0f);
		ImGui.SeparatorText("Transform Settings");
		ImGui.Text("Position");
		ImGui.Text("Radius");
		ImGui.EndGroup();
	}
}
