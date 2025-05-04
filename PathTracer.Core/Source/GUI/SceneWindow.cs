using Microsoft.Xna.Framework.Graphics;
using NumVector2 = System.Numerics.Vector2;
using ImGuiNET;

using PathTracer.Core.Source.Model;

namespace PathTracer.Core.Source.GUI; 
public class SceneWindow {

	ImGuiChildFlags flags = ImGuiChildFlags.ResizeY | ImGuiChildFlags.AlwaysUseWindowPadding;

	public void DrawGUI(string title, ModelList models, Effect effect) {
		NumVector2 size = new(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y / 2);

		if (!ImGui.BeginChild($"{title}##sceneChild", size, flags))
			return;
		
		/* Draw scene item settings */
		for (int i = 0; i < models.Spheres.Length; i++) {
			DrawSphereSettings(i, models);
			ImGui.Dummy(new(0,5));
		}

		ImGui.EndChild();
	}

	public void DrawSphereSettings(int idx, ModelList models) {
		ref Sphere _sphere = ref models.Spheres[idx];
		// ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		if (!ImGui.CollapsingHeader($"{_sphere.Id}##sphere[{idx}]"))
			return;

		/* Material Settings */
		ImGui.SeparatorText("Material Settings");
		if (ImGui.ColorEdit4($"Diffuse##DiffColor[{idx}]", ref _sphere.DiffuseColor))
			models.SetData(SphereParam.SPHERE_DIFF_COL);
		if (ImGui.ColorEdit4($"Specular##SpecColor[{idx}]", ref _sphere.SpecularColor))
			models.SetData(SphereParam.SPHERE_SPEC_COL);
		if (ImGui.SliderFloat($"Smooth##Spec[{idx}]", ref _sphere.Specular, v_min: 0.0f, v_max: 1.0f))
			models.SetData(SphereParam.SPHERE_SPEC);
		if (ImGui.SliderFloat($"Gloss##Gloss[{idx}]", ref _sphere.Gloss, v_min: 0.0f, v_max: 1.0f))
			models.SetData(SphereParam.SPHERE_GLOSS);

		ImGui.Dummy(new(0, 2));

		/* Emission Settings */
		ImGui.SeparatorText("Emission Settings");
		if (ImGui.ColorEdit3($"Color##LiteColor[{idx}]", ref _sphere.LightColor))
			models.SetData(SphereParam.SPHERE_LITE_COL);
		if (ImGui.DragFloat($"Intensity##Lite[{idx}]", ref _sphere.Light, v_speed: 0.1f, v_min: 0.0f, v_max: 100.0f))
			models.SetData(SphereParam.SPHERE_LITE);

		ImGui.Dummy(new(0, 2));

		/* Transform Settings [TODO] */
	}
}
