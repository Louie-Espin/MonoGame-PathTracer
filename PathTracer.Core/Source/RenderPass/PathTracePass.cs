using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathTracer.Core.Source.Camera;

namespace PathTracer.Core.Source.RenderPass;

public class PathTracePass: RenderPass {
    enum Technique { PathTrace, Rasterize };

    private Technique _technique = Technique.PathTrace;
    private int _totalFrames = 0;

    private Effect _effect;
    private PathTraceCamera _cameraRef;
    private (int Width, int Height) _window;

    public PathTracePass(ref RenderTarget2D target, GraphicsDevice graphicsDevice, ref SpriteBatch spriteBatch, Effect shader, ref PathTraceCamera camera)
        : base(target, graphicsDevice, spriteBatch) {
        _effect = shader; // FIXME: MAKE SURE EFFECT IS LOADED BEFORE INITIALIZING THIS OBJECT!
        _cameraRef = camera; // FIXME: are you sure camera must be a ref?

        _window = (Graphics.Viewport.Width, Graphics.Viewport.Height);
        InitParameters();
    }

    /* Sets Parameters that will not change at each Draw call */
    private void InitParameters() {
        /* _screenX & _screenY will only change on window size change */
        _effect.Parameters["_screenX"].SetValue(_window.Width);
        _effect.Parameters["_screenY"].SetValue(_window.Height);

        /* camera settings */
        _effect.Parameters["_viewportSize"].SetValue(_cameraRef.ViewportDimensions);
        _effect.Parameters["_focalLength"].SetValue(_cameraRef.Clip.Near);
    }

    public override void Pass(Texture2D srcTexture) {
        // Set the Render Target & Clear
        Graphics.SetRenderTarget(Target);
        Graphics.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

        // Set Effect Parameters
        _effect.CurrentTechnique = _effect.Techniques[(int)_technique];
        _effect.Parameters["_frame"].SetValue((int)_totalFrames);
        _effect.Parameters["_cameraPosition"].SetValue(_cameraRef.Position);
        _effect.Parameters["_cameraTransform"].SetValue(
            Matrix.Multiply(_cameraRef.Offset, Matrix.CreateFromQuaternion(_cameraRef.Rotation))
        );
        
        // Draw srcTexture with Path Tracing Effect
        _spriteBatch.Begin(
            SpriteSortMode.Immediate, BlendState.AlphaBlend,
            SamplerState.LinearWrap, DepthStencilState.Default,
            RasterizerState.CullNone, effect: _effect
        );
        _spriteBatch.Draw(
            srcTexture,
            new Rectangle(0, 0, _window.Width, _window.Height),
            Color.White
        );
        _spriteBatch.End();

        // Drop the render target from the graphics device
        Graphics.SetRenderTarget(null);

        // Set the result of the draw call to _texture
        _texture = _target;
        _totalFrames++;
    }

    /* Callback: sets the Effect params _screenX & _screenY to updated values when viewport changes size */
    public void OnWindowSizeChange() {
        // _window = (Graphics.Viewport.Width, Graphics.Viewport.Height);
        // _effect.Parameters["_screenX"].SetValue(Graphics.Viewport.Width);
        // _effect.Parameters["_screenY"].SetValue(Graphics.Viewport.Height);
        throw new NotImplementedException();
    }
}
