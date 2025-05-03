using ImGuiNET;
using Microsoft.Xna.Framework;
using System;

using NumVector4 = System.Numerics.Vector4;
using NumVector2 = System.Numerics.Vector2;

namespace PathTracer.Core.Source.GUI; 
public class ProfilerWindow {

	readonly NumVector4 okay = new NumVector4(1.0f, 1.0f, 1.0f, 1);
	readonly NumVector4 warn = new NumVector4(0.76f, 0.57f, 0.4f, 1);

	private long _totalFrames = 0;

	const int FPS_HISTORY_LEN = 200;
	private float[] _fps = new float[FPS_HISTORY_LEN];
	private float[] _ftime = new float[FPS_HISTORY_LEN];
	private int _fpsOffset = 0;

	public long TotalFrames => _totalFrames;

	private int SetOffset(int currOffset, int arrSize) => (currOffset + 1) % arrSize;

	public void DrawGUI(string title, GameTime gameTime) {

		/* Update total # of frames */
		_totalFrames++; // [MUST BE DONE HERE ELSE DOES NOT UPDATE WHEN GUI IS COLLAPSED]

		ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		if (!ImGui.CollapsingHeader(title))
			return;

		/* Update FPS & FrameTime History */
		_fps[_fpsOffset]   = (float) (1 / (gameTime.ElapsedGameTime.TotalMilliseconds / 1000));
		_ftime[_fpsOffset] = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

		/* Draw FPS Info */
		ImGui.SeparatorText("Frames-Per-Second");
		ImGui.PlotLines("##plotFPS",
			values: ref _fps[0],
			_fps.Length,
			_fpsOffset,
			overlay_text: null,
			scale_min: 0,
			scale_max: 60
		);
		ImGui.SameLine();
		ImGui.TextColored((_fps[_fpsOffset] > 30) ? okay : warn, $"{Math.Round(_fps[_fpsOffset], 1)}");

		/* Draw FrameTime Info */
		ImGui.SeparatorText("Frame Time");
		ImGui.PlotLines("##plotFrameTime",
			values: ref _ftime[0],
			_ftime.Length,
			_fpsOffset,
			overlay_text: null,
			scale_min: 0,
			scale_max: 100
		);
		ImGui.SameLine();
		ImGui.TextColored((_ftime[_fpsOffset] < 30) ? okay: warn, $"{Math.Round(_ftime[_fpsOffset], 0)} (ms)");
		
		/* Move to next offset */
		_fpsOffset = SetOffset(_fpsOffset, _fps.Length);
	}
}
