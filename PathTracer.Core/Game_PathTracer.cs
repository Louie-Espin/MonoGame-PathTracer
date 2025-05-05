using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using PathTracer.Core.Source.Camera;
using PathTracer.Core.Source.Control;
using PathTracer.Core.Source.Model;
using PathTracer.Core.Source.GUI;

namespace PathTracer.Core {

	enum Technique { PATH_TRACE, RASTERIZE };
	public class Game_PathTracer : Game {

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private (int Width, int Height, float AspectRatio) _screen;

		private RenderTarget2D _pathTarget;
		private RenderTarget2D _rasterTarget;
		private Texture2D _renderPrev;
		private Color[]   _textureData;

		private Effect _effect;
		private SpriteFont _font;
		private Model _sphere;
		private PathTraceCamera _camera;

		/* CONTROL HANDLER: Polls for mouse movement and updates camera transform accordingly */
		private ControlHandler _controlHandler = new();
		private MouseDrag _cameraRot = new(new(0.5f, 0.7f),   MouseButtonType.RIGHT, value: new(0, 0));
		private MouseDrag _cameraXY  = new(new(0.05f, 0.07f), MouseButtonType.RIGHT, value: new(0, 0));
		private MouseDrag _cameraZ   = new(new(0.05f, 0.07f), MouseButtonType.WHEEL, value: new(0, -5)); // TODO: BOUNDS

		/* SCENE DATA */
		ModelList _spheres = new();

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
			/* Request preferred resolution & keep track of screen information */
			_graphics.PreferredBackBufferWidth  = 1280;
			_graphics.PreferredBackBufferHeight = 720;
			_graphics.ApplyChanges();

			_screen = (
				GraphicsDevice.Viewport.Width,
				GraphicsDevice.Viewport.Height,
				GraphicsDevice.Viewport.AspectRatio
			);

			/* Initialize GUI Windows */
			_GUI = new (this, GraphicsDevice);
			_sceneEdit = new ();
			_settings  = new ();
			_profiler  = new ();

			/* Main Camera */
			_camera = new PathTraceCamera(
				position: new(_cameraXY.Value, _cameraZ.Y),
				lookAt: Vector3.Zero,
				perspective: new Projection(60, _screen.AspectRatio, (0.1f, 100))
			);

			/* Render Target to Draw (Path Tracing) */
			_pathTarget = new RenderTarget2D(
				graphicsDevice: GraphicsDevice,
				width:  _screen.Width,
				height: _screen.Height,
				mipMap: false,
				preferredFormat: GraphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24Stencil8,
				preferredMultiSampleCount: GraphicsDevice.PresentationParameters.MultiSampleCount,
				usage: RenderTargetUsage.PreserveContents
			);

			/* Render Target to Draw (Rasterization) */
			_rasterTarget = new RenderTarget2D(
				graphicsDevice: GraphicsDevice,
				width:  _screen.Width,
				height: _screen.Height,
				mipMap: false,
				preferredFormat: GraphicsDevice.PresentationParameters.BackBufferFormat,
				preferredDepthFormat: DepthFormat.Depth24Stencil8,
				preferredMultiSampleCount: GraphicsDevice.PresentationParameters.MultiSampleCount,
				usage: RenderTargetUsage.DiscardContents
			);

			/* Instantiate texture that will hold the previous renders */
			_renderPrev = new Texture2D(GraphicsDevice, _screen.Width, _screen.Height);

			/* Instantiate array that will be used to copy render data between GPU & CPU */
			_textureData = new Color[_pathTarget.Width * _pathTarget.Height];

			base.Initialize();
		}

		protected override void LoadContent() {
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_GUI.onLoad();
			_font = Content.Load<SpriteFont>("fonts/font");
			_sphere = Content.Load<Model>("models/sphere/sphere");
			_effect = Content.Load<Effect>("shaders/path-tracer");
			_spheres.onLoad(_effect);
		}

		protected override void Update(GameTime gameTime) {
			_controlHandler.MouseCurrent(Mouse.GetState());
			_controlHandler.KeyboardCurrent(Keyboard.GetState());

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			/* Poll Camera Transformation */
			_controlHandler.PollMouseDrag(_cameraRot, (Vector2 v) => _camera.Rotate(new(v, 0)));
			// _controlHandler.PollMouseDrag(_cameraXY, (Vector2 v) => _camera.Translate(new(v, _camera.Offset.M43)));
			_controlHandler.PollMouseDrag(_cameraZ, (Vector2 v) => _camera.Translate(new(_camera.Offset.M41, _camera.Offset.M42, v.Y)));

			_controlHandler.MousePrevious(Mouse.GetState());
			_controlHandler.KeyboardPrevious(Keyboard.GetState());
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {

			/* Store previous render's data in _RenderPrev [WARNING: expensive operation] */
			_renderPrev.SetData(_textureData);

			/* Rasterized Pass */
			RasterPass(_rasterTarget);

			/* Path Trace Pass */
			PathTracePass(_pathTarget);

			/* Retrieve the current render data from GPU [WARNING: expensive operation] */
			_pathTarget.GetData(_textureData);

			/* Draw both renders to screen */
			_spriteBatch.Begin(
				SpriteSortMode.Deferred, BlendState.AlphaBlend,
				rasterizerState: RasterizerState.CullNone, effect: null
			);
			_spriteBatch.Draw(
				(_settings.View) ? _pathTarget : _rasterTarget,
				new Rectangle(0, 0, _screen.Width, _screen.Height),
				Color.White
			);
			_spriteBatch.Draw(
				(_settings.View) ? _rasterTarget : _pathTarget,
				new Rectangle(
					x: GraphicsDevice.Viewport.Bounds.Right  - (_screen.Width / 4) - 25,
					y: GraphicsDevice.Viewport.Bounds.Bottom - (_screen.Height / 4) - 25,
					width:  _screen.Width / 4,
					height: _screen.Height / 4
				),
				Color.White
			);
			_spriteBatch.End();

			base.Draw(gameTime);

			/* Draw GUI & All Sub Windows */
			_GUI.BeginDraw(gameTime);
			_sceneEdit.DrawGUI("Scene Settings", _spheres, _effect);
			_settings.DrawGUI("Render Settings");
			_profiler.DrawGUI("Profiling", gameTime);
			_GUI.EndDraw();
		}

		private void RasterPass(RenderTarget2D target) {
			// Set the Render Target & Clear
			GraphicsDevice.SetRenderTarget(target);
			GraphicsDevice.Clear(new Color(0, 0, 0));

			GraphicsDevice.BlendState = BlendState.Opaque; // Needed for DrawString() function
			GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

			/* Draw the scene */
			foreach (Sphere s in _spheres.Spheres) {
				DrawSphere(s);
			}

			/* Drop the render target */
			GraphicsDevice.SetRenderTarget(null);
		}

		private void PathTracePass(RenderTarget2D target) {
			// Set the Render Target & Clear
			GraphicsDevice.SetRenderTarget(target);
			GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Blue, 1.0f, 0);

			// Set Effect Parameters
			_effect.Parameters["_cameraPosition"].SetValue(_camera.Position);
			_effect.Parameters["_cameraTransform"].SetValue(
				Matrix.Multiply(_camera.Offset, Matrix.CreateFromQuaternion(_camera.Rotation))
			);
			_effect.Parameters["_viewportSize"].SetValue(_camera.ViewportDimensions);
			_effect.Parameters["_focalLength"].SetValue(_camera.Clip.Near);
			_effect.Parameters["_frame"].SetValue((int)_profiler.TotalFrames);
			_effect.Parameters["_screenX"].SetValue(GraphicsDevice.Viewport.Width);
			_effect.Parameters["_screenY"].SetValue(GraphicsDevice.Viewport.Height);
			_effect.Parameters["SPP"].SetValue(_settings.Samples);
			_effect.Parameters["BOUNCES"].SetValue(_settings.Bounces);
			_effect.Parameters["_accumulated"].SetValue((int) _settings.AccumulatedFrames);
			_effect.Parameters["_accumWeighted"].SetValue(_settings.Accumulation == Accumulate.WEIGHTED);

			// Draw Sprite Batch with Path Tracing Effect
			_spriteBatch.Begin(
				SpriteSortMode.Deferred, BlendState.Opaque,
				rasterizerState: RasterizerState.CullNone, effect: _effect
			);
			_spriteBatch.Draw(
				_renderPrev,
				new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
				Color.White
			);
			_spriteBatch.End();

			// Drop the render target
			GraphicsDevice.SetRenderTarget(null);
		}

		private void DrawSphere(Sphere s) {
			Vector3 color = new(s.DiffuseColor.X, s.DiffuseColor.Y, s.DiffuseColor.Z);
			Vector3 specular = new(s.SpecularColor.X, s.SpecularColor.Y, s.SpecularColor.Z);
			Matrix transform = (Matrix.CreateScale(s.Radius) * Matrix.CreateTranslation(s.Position));
			foreach (ModelMesh mesh in _sphere.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					BasicEffect effect = (BasicEffect)part.Effect;
					effect.EnableDefaultLighting();
					effect.PreferPerPixelLighting = true;
					effect.DiffuseColor  = color;
					effect.EmissiveColor = s.LightColor;
					effect.World = mesh.ParentBone.Transform * transform;
					effect.View = _camera.View;
					effect.Projection = Matrix.CreatePerspectiveFieldOfView(
						fieldOfView: MathHelper.ToRadians(98.3f), aspectRatio: 16 / 9.02f, 0.1f, 100
					);
				}
				mesh.Draw();
			}
		}
	}
}