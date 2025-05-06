using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.ImGuiNet;
using ImGuiNET;
using NumVector4 = System.Numerics.Vector4;
using NumVector2 = System.Numerics.Vector2;

namespace PathTracer.Core.Source.GUI;

public class MainWindow {

	const ImGuiWindowFlags flags = ImGuiWindowFlags.None;
	static ImGuiRenderer GuiRenderer;

	/* GUI WINDOW DATA */
	bool _toggleGUI = true;
	private readonly (float width, float height, int x, int y) _window;
	
	public MainWindow(Game game, GraphicsDevice graphics) {
		/* Initialize Imgui Renderer */
		GuiRenderer = new ImGuiRenderer(game);
		_window = (
			width: (graphics.Viewport.Width  * .24f),
			height: (graphics.Viewport.Height * .95f),
			x: graphics.Viewport.Bounds.Top + 10,
			y: graphics.Viewport.Bounds.Left + 10
		);

		SetTheme();
	}

	public void onLoad() {
		GuiRenderer.RebuildFontAtlas();
	}

	public static void SetTheme() {

		var style = ImGui.GetStyle();

		/* COLOR PALETTE */
		NumVector4 _paletteBG   = new NumVector4(.098f, 0.18f, 0.137f, 1);
		NumVector4 _paletteMAIN = new NumVector4(0.21f, 0.38f, 0.29f, 1);
		NumVector4 _paletteACC  = new NumVector4(0.43f, 0.44f, 0.725f, 1);
		NumVector4 _paletteWARN = new NumVector4(0.76f, 0.57f, 0.4f, 1);
		NumVector4 _paletteWHITE = new NumVector4(1.0f, 1.0f, 1.0f, 1);

		/* Setting Colors */
		ImGui.PushStyleColor(ImGuiCol.WindowBg, _paletteBG);
		ImGui.PushStyleColor(ImGuiCol.Header, _paletteMAIN);
		ImGui.PushStyleColor(ImGuiCol.TitleBg, _paletteMAIN);
		ImGui.PushStyleColor(ImGuiCol.TitleBgActive, _paletteMAIN);
		ImGui.PushStyleColor(ImGuiCol.Button, _paletteACC);
		ImGui.PushStyleColor(ImGuiCol.SliderGrab, _paletteACC);
		ImGui.PushStyleColor(ImGuiCol.FrameBg, _paletteMAIN);
		ImGui.PushStyleColor(ImGuiCol.CheckMark, _paletteWHITE);
		/* Setting Variables */
		style.WindowRounding = 5.3f;
	}

	public void BeginDraw(GameTime gameTime) {
		if (!_toggleGUI) return;

		GuiRenderer.BeginLayout(gameTime);
		ImGui.SetNextWindowSize(new (_window.width, _window.height), ImGuiCond.Once);
		ImGui.SetNextWindowPos(new (_window.x, _window.y), ImGuiCond.Once);
		ImGui.SetNextItemOpen(true, ImGuiCond.Once);
		ImGui.Begin("Path Tracer Settings", flags);		
	}

	public void EndDraw() {
		ImGui.End();
		GuiRenderer.EndLayout();
	}
}
