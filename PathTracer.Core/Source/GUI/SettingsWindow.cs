using ImGuiNET;

namespace PathTracer.Core.Source.GUI;

public class SettingsWindow {

	private bool _view = true;
	private bool _accumulation = false;
	private int _SPP = 50;
	private int _BOUNCE = 3;
	private long _accumFrames = 0;

	public bool View => _view;
	public bool Accumulation => _accumulation;
	public int Samples => _SPP;
	public int Bounces => _BOUNCE;
	public long AccumulatedFrames => _accumFrames;

	public void DrawGUI(string title) {

		/* If Accumulation is enabled, Update number of accumulated frames */
		_accumFrames = Accumulation ? (_accumFrames + 1) : 0;

		ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		if (!ImGui.CollapsingHeader(title))
			return;

		/* Toggle between Path Tracing & Rasterizer as the main view */
		ImGui.SeparatorText("Toggle View");
		if (ImGui.Button($"{(View ? "Path Tracer" : " Raterizer ")}##viewToggle")) {
			_view = !_view;
		}

		ImGui.Dummy(new(0, 1));

		/* Control the Path Tracer's Samples/Pixel & Maximum # of Bounces */
		ImGui.SeparatorText("Path Trace Settings");
		ImGui.InputInt("Samples/Pixel", ref _SPP,   step: 1, step_fast: 5);
		ImGui.InputInt("Bounce Limit", ref _BOUNCE, step: 1);

		/* Toggle Temporal Accumulation */
		ImGui.Checkbox("Temporal Accumulation", ref _accumulation);

		ImGui.Dummy(new(0, 5));
	}

}
