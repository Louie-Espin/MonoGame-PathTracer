using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using static System.Math;

using PathTracer.Core.Source.Camera;
using PathTracer.Core.Source.Control;
using PathTracer.Core.Source.Model;
using PathTracer.Core.Source.RenderPass;
using PathTracer.Core.Source.GUI;

namespace PathTracer.Core {

	enum Technique { PATH_TRACE, RASTERIZE };
	public class Game_PathTracer : Game {

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private RenderTarget2D _target;
		private RenderTarget2D _unlitTarget;

		private Texture2D _renderCurr;
		private Texture2D _renderPrev;
		private Color[] _textureData;

		private long _totalFrames = 0;
		private long _accumFrames = 0;

		private Effect _ptEffect;
		private Effect _accumEffect;
		private SpriteFont _font;
		private Model _sphere;
		private PathTraceCamera _camera;

		// private PathTracePass _pathTrace; // FIXME

		private ControlHandler _controlHandler = new ControlHandler();
		private MouseDrag _cameraRotate = new(new(0.5f, 0.7f), MouseButtonType.RIGHT, value: new(0, 0));
		private MouseDrag _cameraXY = new(new(0.05f, 0.07f), MouseButtonType.RIGHT, value: new(0, 0));
		private MouseDrag _cameraZ = new(new(0.05f, 0.07f), MouseButtonType.WHEEL, value: new(0, -10.0f));// TODO: MIN-MAX
		private KeyToggle<Action<ArcballCamera>> _cameraReset = new(Keys.S, (ArcballCamera c) => c.Reset());
		private KeyToggle<Action<MouseControl<Vector2>>> _transformReset = new(Keys.S, (MouseControl<Vector2> t) => t.Value = Vector2.Zero);

		/* PATH TRACE DATA */
		ModelList _spheres = new ModelList();

		/* GUI COMPONENTS */
		private MainWindow _GUI;
		private SceneWindow _sceneEdit;
		private ProfilerWindow _profiler;
		private SettingsWindow _settings;

		public Game_PathTracer() {
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			this.IsMouseVisible = true;
			this.IsFixedTimeStep = false;

			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
		}

		protected override void Initialize() {
			// TODO: Add your initialization logic here
			_graphics.PreferredBackBufferWidth = 1280;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();

			/* Initialize GUI Windows */
			_GUI = new MainWindow(this, GraphicsDevice);
			_sceneEdit = new (_spheres.Size);
			_settings = new ();
			_profiler = new ();

			/* Main Camera */
			_camera = new PathTraceCamera(
				position: new(_cameraXY.Value, _cameraZ.Y),
				lookAt: Vector3.Zero,
				perspective: new Projection(60, GraphicsDevice.Viewport.AspectRatio, (1, 100))
			);

			/* Render Target to Draw to */
			_target = new RenderTarget2D(
				graphicsDevice: GraphicsDevice,
				width: GraphicsDevice.PresentationParameters.BackBufferWidth,
				height: GraphicsDevice.PresentationParameters.BackBufferHeight,
				mipMap: false,
				preferredFormat: GraphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24Stencil8,
				preferredMultiSampleCount: GraphicsDevice.PresentationParameters.MultiSampleCount,
				usage: RenderTargetUsage.PreserveContents
			);

			/* Render Target to Draw (Rasterization) */
			_unlitTarget = new RenderTarget2D(
				graphicsDevice: GraphicsDevice,
				width: GraphicsDevice.PresentationParameters.BackBufferWidth,
				height: GraphicsDevice.PresentationParameters.BackBufferHeight,
				mipMap: false,
				preferredFormat: GraphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24Stencil8,
				preferredMultiSampleCount: GraphicsDevice.PresentationParameters.MultiSampleCount,
				usage: RenderTargetUsage.DiscardContents
			);

			/* Instantiate textures that will hold the previous & current renders */
			_renderCurr = new Texture2D(
				GraphicsDevice,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight
			);

			_renderPrev = new Texture2D(
				GraphicsDevice,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight
			);

			/* Instantiate array that will be used to copy render data between textures */
			_textureData = new Color[_target.Width * _target.Height];

			base.Initialize();
		}

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_GUI.onLoad();
			_font = Content.Load<SpriteFont>("fonts/font");
			_sphere = Content.Load<Model>("models/sphere/sphere");
			_ptEffect = Content.Load<Effect>("shaders/path-tracer");
			_accumEffect = Content.Load<Effect>("shaders/denoise");

			// FIXME: RenderPass objects should load their own effects probably
			// _pathTrace = new(ref _target, GraphicsDevice, ref _spriteBatch, _ptEffect, ref _camera);

			/* Set _ptEffect with a list of default sphere object data */
			foreach (SphereParam e in Enum.GetValues(typeof(SphereParam))) {
				_spheres.SetData(ref _ptEffect, e);
			}
		}

		protected override void Update(GameTime gameTime) {
			_controlHandler.MouseCurrent(Mouse.GetState());
			_controlHandler.KeyboardCurrent(Keyboard.GetState());

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			/* Poll Camera Transformation */
			_controlHandler.PollMouseDrag(_cameraRotate, (Vector2 v) => _camera.Rotate(new(v, 0)));
			// _controlHandler.PollMouseDrag(_cameraXY,     (Vector2 v) => _camera.Translate(new(v, _camera.Offset.M43)));
			_controlHandler.PollMouseDrag(_cameraZ, (Vector2 v) => _camera.Translate(new(_camera.Offset.M41, _camera.Offset.M42, v.Y)));

			_controlHandler.MousePrevious(Mouse.GetState());
			_controlHandler.KeyboardPrevious(Keyboard.GetState());
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {

			Debug.WriteLine($"FRAMETIME: {gameTime.ElapsedGameTime.TotalMilliseconds}");
			Debug.WriteLine($"frame: {_totalFrames}");

			/* Store the previous render in _RenderPrev */
			_renderCurr.GetData(_textureData);
			_renderPrev.SetData(_textureData);

			/* First (Unlit) Pass, saved to renderUnlit */
			_unlitTarget = (RenderTarget2D)UnlitPass(_unlitTarget);

			/* Path Trace Pass */
			_renderCurr = PathTracePass(_target);

			/* Temporal Accumulation Pass */
			_renderCurr = AccumulationPass(_target);

			/* Finally, draw to screen */
			_spriteBatch.Begin(
				SpriteSortMode.Deferred, BlendState.AlphaBlend,
				rasterizerState: RasterizerState.CullNone, effect: null
			);
			/* TODO: Replace with DrawMain */
			_spriteBatch.Draw(
				(_settings.View) ? _target : _unlitTarget,
				new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
				Color.White
			);
			/* TODO: Replace with DrawUI */
			_spriteBatch.Draw(
				(_settings.View) ? _unlitTarget : _target,
				new Rectangle(
					x: GraphicsDevice.Viewport.Bounds.Right - (GraphicsDevice.Viewport.Width / 4) - 25,
					y: GraphicsDevice.Viewport.Bounds.Bottom - (GraphicsDevice.Viewport.Height / 4) - 25,
					width: GraphicsDevice.Viewport.Width / 4,
					height: GraphicsDevice.Viewport.Height / 4
				),
				Color.White
			);
			_debug(true);
			_spriteBatch.End();

			/* Store the current render in _RenderCurr */
			GraphicsDevice.GetBackBufferData(_textureData);
			_renderCurr.SetData(_textureData);

			/* Update total # of frames */
			_totalFrames++;

			base.Draw(gameTime);

			/* Draw GUI & All Sub Windows */
			_GUI.BeginDraw(gameTime);
			_sceneEdit.DrawGUI("Scene Settings", _spheres);
			_settings.DrawGUI("Render Settings");
			_profiler.DrawGUI("Profiling", gameTime);
			_GUI.EndDraw();
		}

		private Texture2D UnlitPass(RenderTarget2D target) {
			// Set the Render Target & Clear
			GraphicsDevice.SetRenderTarget(target);
			GraphicsDevice.Clear(new Color(0, 0, 0));

			GraphicsDevice.BlendState = BlendState.Opaque; // Needed for DrawString() function
			GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

			// Draw the scene
			DrawScene();

			// Drop the render target
			GraphicsDevice.SetRenderTarget(null);

			return target;
		}

		private Texture2D PathTracePass(RenderTarget2D target) {
			// Set the Render Target & Clear
			GraphicsDevice.SetRenderTarget(target);
			GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Blue, 1.0f, 0);

			// Set Effect Parameters
			_ptEffect.Parameters["_cameraPosition"].SetValue(_camera.Position);
			_ptEffect.Parameters["_cameraTransform"].SetValue(
				Matrix.Multiply(_camera.Offset, Matrix.CreateFromQuaternion(_camera.Rotation))
			);
			_ptEffect.Parameters["_viewportSize"].SetValue(_camera.ViewportDimensions);
			_ptEffect.Parameters["_focalLength"].SetValue(_camera.Clip.Near);
			_ptEffect.Parameters["_frame"].SetValue((int)_totalFrames);
			_ptEffect.Parameters["_screenX"].SetValue(GraphicsDevice.Viewport.Width);
			_ptEffect.Parameters["_screenY"].SetValue(GraphicsDevice.Viewport.Height);
			_ptEffect.Parameters["SPP"].SetValue(_settings.Samples);
			_ptEffect.Parameters["BOUNCES"].SetValue(_settings.Bounces);

			// Draw Sprite Batch with Path Tracing Effect
			_spriteBatch.Begin(
				SpriteSortMode.Deferred, BlendState.Opaque,
				rasterizerState: RasterizerState.CullNone, effect: _ptEffect
			);
			_spriteBatch.Draw(
				_renderCurr,
				new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
				Color.White
			);
			_spriteBatch.End();

			// Drop the render target
			GraphicsDevice.SetRenderTarget(null);

			return target;
		}

		private Texture2D AccumulationPass(RenderTarget2D target) {
			// Set the Render Target & Clear
			GraphicsDevice.SetRenderTarget(target);

			// Set Effect Parameters
			if (_settings.Accumulation) {
				_accumEffect.CurrentTechnique = _accumEffect.Techniques["AccumEnable"];
				_accumEffect.Parameters["_frame"].SetValue((int)_accumFrames);
				_accumEffect.Parameters["PrevRender"].SetValue(_renderPrev);

				/* Update number of frames only when accumulating */
				_accumFrames++;
			}
			else {
				_accumEffect.CurrentTechnique = _accumEffect.Techniques["AccumDisable"];
				_accumFrames = 0;
			}

			// Draw Sprite Batch with Temporal Accumulation Effect
			_spriteBatch.Begin(
				SpriteSortMode.Deferred, BlendState.AlphaBlend,
				rasterizerState: RasterizerState.CullNone, effect: _accumEffect
			);
			_spriteBatch.Draw(
				_target,
				new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
				Color.White
			);
			_spriteBatch.End();

			/* Drop the render target from device and set it on _RenderCurr */
			GraphicsDevice.SetRenderTarget(null);

			return target;
		}

		private void DrawScene() {
			// DrawModel(model, world, view, projection);
			DrawSphere(position: new(0, 0, 60), color: new(1, 1, 1, 1), scale: 40);
			DrawSphere(position: new(4, -0.5f, 0), color: new(1, 0, 0, 1), scale: 2);
			DrawSphere(position: new(0, 0, 0), color: new(0.1f, 1.0f, 0.1f, 1), scale: 2);
			DrawSphere(position: new(-4, -0.5f, 0), color: new(0, 0, 1.0f, 1), scale: 2);
			DrawSphere(position: new(0, -32, 0), color: new(1, 0, 1, 1), scale: 30);
		}

		private void DrawSphere(Vector3 position, Vector4 color, float scale = 1.0f) {
			foreach (ModelMesh mesh in _sphere.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					BasicEffect effect = (BasicEffect)part.Effect;

					effect.EnableDefaultLighting();
					effect.PreferPerPixelLighting = true; ;
					effect.DiffuseColor = new Vector3(color.X, color.Y, color.Z);
					effect.World = mesh.ParentBone.Transform * (Matrix.CreateScale(scale) * Matrix.CreateTranslation(position));
					effect.View = _camera.View;
					effect.Projection = Matrix.CreatePerspectiveFieldOfView(
						fieldOfView: MathHelper.ToRadians(98.3f), aspectRatio: 16 / 9.02f, 1, 100
					);
				}
				mesh.Draw();
			}
		}

		private void _debug(bool toggle = false) {
			if (!toggle) return;
			Vector3 camDir = Vector3.Normalize(_camera.Target - _camera.Position);
			string _valueStr =
				"\n" +
				$"Rotation (LMB): [{Round(_cameraRotate.X, 2)}, {Round(_cameraRotate.Y, 2)}]" + "\n" +
				$"TranslationXY (RMB): [{Round(_cameraXY.X, 2)}, {Round(_cameraXY.Y, 2)}]" + "\n" +
				$"ZOOM (SCROLL): [{_cameraZ.Y.ToString("F1")}]"
				+ "\n" + "\n" +
				$"Camera = [{Round(_camera.Position.X, 2)}, {Round(_camera.Position.Y, 2)}, {Round(_camera.Position.Z, 2)}]" + "\n" +
				$"Rotated = [{Round(_camera.Rotation.X, 2)}, {Round(_camera.Rotation.Y, 2)}, {Round(_camera.Rotation.Z, 2)}]" + "\n" +
				$"Offset = [{Round(_camera.Offset.M41, 2)}, {Round(_camera.Offset.M42, 2)}, {Round(_camera.Offset.M43, 2)}]" + "\n" +
				$"Initial = [{Round(_camera.InitialPosition.X, 2)}, {Round(_camera.InitialPosition.Y, 2)}, {Round(_camera.InitialPosition.Z, 2)}]"
				+ "\n" + "\n";

			_spriteBatch.DrawString(
				_font, toggle ? _valueStr : "",
				new Vector2(GraphicsDevice.Viewport.Bounds.Right - _font.MeasureString(_valueStr).X, GraphicsDevice.Viewport.Bounds.Top),
				Color.White
			);
		}
	}
}