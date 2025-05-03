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

	private bool _view = true;
	private bool _accumulation = false;
	private int _SPP = 50;
	private int _BOUNCE = 2;

	public bool View => _view;
	public bool Accumulation => _accumulation;
	public int Samples => _SPP;
	public int Bounces => _BOUNCE;

	public void DrawGUI(string title) {
		ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		if (!ImGui.CollapsingHeader(title))
			return;

		ImGui.SeparatorText("Toggle View");
		if (ImGui.Button($"{(View ? "Path Tracer" : " Raterizer ")}##viewToggle")) {
			_view = !_view;
		}

		ImGui.SeparatorText("Temporal Accumulation");
		if (ImGui.Button($"{(Accumulation ? "Enabled  " : "Disabled ")}##accumToggle")) {
			_accumulation = !_accumulation;
		}

		ImGui.SeparatorText("Path Trace Settings");
		ImGui.DragInt("Samples Per Pixel", ref _SPP,    v_speed: 1,    v_min: 1, v_max: 500);
		ImGui.DragInt("Max. Bounce Limit", ref _BOUNCE, v_speed: 0.5f, v_min: 1, v_max: 500);

		ImGui.Text(title);
	}

}
