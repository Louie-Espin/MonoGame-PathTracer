using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathTracer.Core.Source.Camera;

public readonly record struct Projection(float fieldOfView, float aspectRatio, (float near, float far) clip) {
	public (float Near, float Far) Clip => clip;
	public float AspectRatio => aspectRatio;
	public float FieldOfView => MathHelper.ToRadians(fieldOfView);
};

public abstract class AbstractCamera {

	/* GENERALIZED CAMERA PROPERTIES */
	private readonly Vector3 _initialPosition;
	private Texture2D _sprite;
	private readonly Projection _perspective; // Used to calculate the Camera's Projection Matrix

	/* Protected Camera Transform Properties */
	protected Matrix _offset;
	protected Quaternion _rotation;

	/* CAMERA CONSTRUCTOR */
	public AbstractCamera(Vector3 position, Projection perspective) {
		_initialPosition = position;
		_perspective = perspective;

		_offset = Matrix.CreateTranslation(position);
		_rotation = Quaternion.Identity;
	}

	/* GENERALIZED CAMERA GETTERS */
	public static Vector3 WorldUp => Vector3.UnitY;
	public Vector3 InitialPosition => _initialPosition;
	public float FieldOfView => _perspective.FieldOfView;
	public float AspectRatio => _perspective.AspectRatio;
	public (float Near, float Far) Clip => _perspective.Clip;

	/* CAMERA TRANSFORMATION GETTERS */
	public Quaternion Rotation => _rotation;
	public Matrix Offset => _offset;
	public Vector3 Position => Vector3.Transform(Vector3.Transform(Vector3.Zero, Offset), Rotation);

	/* Returns the Camera's Projection Matrix.
	 * Note: All Camera's currently return Perspective Projection.
	 * TODO: Set to an abstract method when no longer perspective-only
	 */
	public virtual Matrix Projection => (
		Matrix.CreatePerspectiveFieldOfView(
			FieldOfView, AspectRatio, Clip.Near, Clip.Far
		)
	);

	/* Virtual - Sets the Camera's Offset Matrix. */
	public virtual void Translate(Vector3 translateBy) {
		_offset = Matrix.CreateTranslation(translateBy);
	}
	/* Virtual - Sets the Camera's Rotation Matrix. */
	public virtual void Rotate(Vector3 rotateBy) {

		// Calculate the change in Euler angles (assumes intial quaternion rotation == identity)
		float deltaYaw = MathHelper.ToRadians(rotateBy.X);
		float deltaPitch = MathHelper.ToRadians(rotateBy.Y);
		float deltaRoll = MathHelper.ToRadians(rotateBy.Z);

		_rotation = Quaternion.CreateFromYawPitchRoll(deltaYaw, deltaPitch, deltaRoll);
	}

	/* Abstract - Returns the Camera's View Matrix. */
	public abstract Matrix View { get; }
	/* Abstract - Reset's the camera to Initial values. */
	public abstract void Reset();
}
