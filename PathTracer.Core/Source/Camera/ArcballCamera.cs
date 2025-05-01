using System;
using Microsoft.Xna.Framework;

namespace PathTracer.Core.Source.Camera;

public class ArcballCamera : AbstractCamera {

	/* Arcball Camera Specific Properties */
	private Vector3 _target;

	public ArcballCamera(Vector3 position, Projection perspective) : base(position, perspective) {
		_target = Vector3.Zero; // defaults to target = origin

		/* Set Camera Transform */
		Rotate(new Vector3(0, 0, 0)); // results in identity quaternion
		Translate(InitialPosition);
	}

	public ArcballCamera(Vector3 position, Vector3 lookAt, Projection perspective) : this(position, perspective) {
		_target = lookAt;
	}

	/* Arcball Camera Specific Getters */
	public Vector3 Target => _target;

	/* Returns the Arcball Camera's View Matrix.
	 * The View Matrix is the inverse of the camera’s transformation matrix in world-space.
	 * Read More: https://www.3dgep.com/understanding-the-view-matrix/#arcball-orbit-camera
	 */
	public override Matrix View => (
		Matrix.CreateLookAt(
			Position,
			Target,
			Vector3.Transform(WorldUp, Rotation) // prevents camera from flipping when perpendicular to Unit.Y
		)
	);

	/* Arcball Camera Specific Methods */
	public override void Translate(Vector3 translateBy) {
		_offset = Matrix.CreateTranslation(translateBy);
	}
	public override void Rotate(Vector3 rotateBy) {

		// Calculate the change in Euler angles (assumes intial quaternion rotation == identity)
		float deltaYaw = MathHelper.ToRadians(rotateBy.X);
		float deltaPitch = MathHelper.ToRadians(rotateBy.Y);
		float deltaRoll = MathHelper.ToRadians(rotateBy.Z);

		_rotation = Quaternion.CreateFromYawPitchRoll(deltaYaw, deltaPitch, deltaRoll);
	}
	/* Reset the Camera & Target to Initial Position  */
	public override void Reset() {
		Rotate(new Vector3(0, 0, 0)); // results in identity quaternion
		Translate(InitialPosition);
	}
}
