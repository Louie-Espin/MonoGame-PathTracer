using ImGuiNET;

namespace PathTracer.Core.Source.GUI;

public enum Accumulate { OFF = 0, CONSTANT = 1, WEIGHTED = 2 } 

public class SettingsWindow {

	private bool _view = true;
	private int _accumulation = (int) Accumulate.CONSTANT;
	private int _SPP = 20;
	private int _BOUNCE = 5;
	private long _accumFrames = 0;

	const string _helpText =  "Off:      Disable temporal accumulation." + "\n"
							+ "Constant: Camera control at expense of quality." + "\n"
							+ "Weighted: High quality, poor camera control.";

	public bool View => _view;
	public Accumulate Accumulation => (Accumulate) _accumulation;
	public int Samples => _SPP;
	public int Bounces => _BOUNCE;
	public long AccumulatedFrames => _accumFrames;

	public void DrawGUI(string title) {

		/* If Accumulation is enabled, Update number of accumulated frames */
		_accumFrames = (Accumulation != Accumulate.OFF) ? (_accumFrames + 1) : 0;

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
		ImGui.SeparatorText("Temporal Accumulation");
		ImGui.RadioButton("Off",      ref _accumulation, 0); ImGui.SameLine();
		ImGui.RadioButton("Constant", ref _accumulation, 1); ImGui.SameLine();
		if (ImGui.RadioButton("Weighted", ref _accumulation, 2))
			_accumFrames = 0; // ensure weighted accumulation begins fresh
		
		Components.HelpMarker(_helpText, true);

		ImGui.Dummy(new(0, 5));
	}

}
