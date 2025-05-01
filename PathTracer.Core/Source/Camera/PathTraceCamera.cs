using Microsoft.Xna.Framework;
using System;

namespace PathTracer.Core.Source.Camera;
public struct Viewport {
	private float _aspectRatio; // ideal ratio, not actual
	private Vector3[,] _grid;
	(int x, int y) _pointCount;
	float _focalLength;
	float _width, _height;

	/**
	 * <param name="fieldOfView">Assumes the parameter is given in Radians.</param>
	 */
	public Viewport(float fieldOfView, float aspectRatio, float cameraNear, (int x, int y) pointCount) {
		_focalLength = cameraNear; // Distance from the camera

		// Calculate the dimensions of the viewport
		_height = _focalLength * MathF.Tan(fieldOfView / 2) * 2;
		_width = _height * aspectRatio;

		// Initialize the Points Grid
		_pointCount = pointCount;
		_grid = new Vector3[_pointCount.x, _pointCount.y];
	}

	// Delta between Points within the Viewport grid
	public readonly (float x, float y) PointDelta => (_width / _pointCount.x, _height / _pointCount.y);

	public Vector3[,] Points => _grid;
	public float Width => _width;
	public float Height => _height;
	public float FocalLength => _focalLength;

}
public class PathTraceCamera : ArcballCamera {

	// RAYTRACE SPECIFIC PROPERTIES
	Viewport _view;

	public PathTraceCamera(Vector3 position, Vector3 lookAt, Projection perspective) : base(position, lookAt, perspective) {
		_view = new Viewport(
			perspective.FieldOfView,
			perspective.AspectRatio,
			perspective.Clip.Near,
			(160, 90)
		);
	}

	public Vector3[,] ViewportGrid => _view.Points;
	public (float x, float y) PointDelta => _view.PointDelta;
	public Viewport Viewport => _view;
	public Vector2 ViewportDimensions => new(_view.Width, _view.Height);
}
