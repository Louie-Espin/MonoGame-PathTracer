using Microsoft.Xna.Framework.Graphics;

namespace PathTracer.Core.Source.RenderPass;

public abstract class RenderPass {

    /* GENERALIZED RENDER PASS GETTERS */
    protected RenderTarget2D _target;
    protected GraphicsDevice _graphics;
    protected SpriteBatch _spriteBatch;

    protected Texture2D _texture;

    /* RENDER PASS CONSTRUCTOR */
    public RenderPass(RenderTarget2D target, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) {
        _target = target;
        _graphics = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    /* GETTERS & SETTERS */
    public Texture2D Texture => _texture;
    public GraphicsDevice Graphics => _graphics;
    public RenderTarget2D Target => _target;

    /* Pass: Implements a Render Pass */
    public abstract void Pass(Texture2D srcTexture);
}
