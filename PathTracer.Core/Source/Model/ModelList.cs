using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PathTracer.Core.Source.Model;
public enum SphereParam {
	SPHERE_POS,
	SPHERE_RADIUS,
	SPHERE_DIFF_COL,
	SPHERE_SPEC_COL,
	SPHERE_LITE_COL,
	SPHERE_SPEC, 
	SPHERE_GLOSS, 
	SPHERE_LITE 
}
public class ModelList {

	private Effect _effect;
	const int NUM_SPHERES = 10;
	Sphere[] _spheres = [
		new( position: new(0, 14.5f, 0), radius: 10f,  lightColor:   new(1, 1, 1),    lightIntensity: 1.0f, "Light" ),
		new( position: new(0, 0, 0),     radius: 1.5f, diffuseColor: new(1, 1, 1, 1), specular: 1.0f, gloss: 1.0f ),
		new( position: new(3, 3, 0),     radius: 1.5f, diffuseColor: new(1, 0, 0, 1), specular: 1.0f, gloss: 0.5f ),
		new( position: new(-3, -3, 0),   radius: 1.5f, diffuseColor: new(0, 1, 0, 1), specular: 1.0f, gloss: 0.5f ),
		new( position: new(0, 0, 205),   radius: 200,  diffuseColor: new(1, 1, 1, 1), specular: 0.0f, gloss: 0.0f, "Wall Z"  ),
		new( position: new(0, 0, -205),  radius: 200,  diffuseColor: new(1, 1, 1, 1), specular: 0.0f, gloss: 0.0f, "Wall -Z" ),
		new( position: new(0, 205, 0),   radius: 200,  diffuseColor: new(1, 1, 1, 1), specular: 0.0f, gloss: 0.0f, "Wall Y"  ),
		new( position: new(0, -205, 0),  radius: 200,  diffuseColor: new(1, 1, 1, 1), specular: 0.0f, gloss: 0.0f, "Wall -Y" ),
		new( position: new(205, 0, 0),   radius: 200,  diffuseColor: new(1, 1, 1, 1), specular: 1.0f, gloss: 0.8f, "Wall X"  ),
		new( position: new(-205, 0, 0),  radius: 200,  diffuseColor: new(1, 1, 1, 1), specular: 1.0f, gloss: 0.8f, "Wall -X" ),
	];

	/* Set the Path Tracer Effect with a list of default sphere object data */
	public void onLoad(Effect pathTracerEffect) {
		_effect = pathTracerEffect;
		
		/* Monogame lacks 'StructuredBuffer' data types that would allow us to send sphere data 
		 * to the GPU in a sequential order. Instead, we work around this limitation by sending 
		 * sphere data through multiple arrays, each containing the values of one specific sphere parameter.
		 */
		foreach (SphereParam e in Enum.GetValues(typeof(SphereParam))) {
			SetData(e);
		}
	}

	/* Shortcuts for accessing effect parameters */
	EffectParameter _positionParam;
	EffectParameter _radiusParam;
	EffectParameter _diffColorParam;
	EffectParameter _specColorParam;
	EffectParameter _liteColorParam;
	EffectParameter _liteParam;
	EffectParameter _specParam;
	EffectParameter _glossParam;

	public void setParams(ref Effect ptEffect) {
		/* Setting up Effects */
		_positionParam  = ptEffect.Parameters["SPHERE_POS"];
		_radiusParam    = ptEffect.Parameters["SPHERE_RADIUS"];
		_diffColorParam = ptEffect.Parameters["SPHERE_DIFF_COL"];
		_specColorParam = ptEffect.Parameters["SPHERE_SPEC_COL"];
		_liteColorParam = ptEffect.Parameters["SPHERE_LITE_COL"];
		_specParam      = ptEffect.Parameters["SPHERE_SPEC"];
		_glossParam     = ptEffect.Parameters["SPHERE_GLOSS"];
		_liteParam      = ptEffect.Parameters["SPHERE_LITE"];
	}

	public ref Sphere[] Spheres => ref _spheres;

	/* FIXME: Not DRY. Find a way to get generic Sphere properties */
	public void SetData(SphereParam param) {

		switch (param) {
			case SphereParam.SPHERE_POS: {
				Vector3[] _arr = new Vector3[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Position;
				}
				_effect.Parameters["SPHERE_POS"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_RADIUS: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Radius;
				}
				_effect.Parameters["SPHERE_RADIUS"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_DIFF_COL: {
				Vector4[] _arr = new Vector4[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].DiffuseColor;
				}
				_effect.Parameters["SPHERE_DIFF_COL"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_SPEC_COL: {
				Vector4[] _arr = new Vector4[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].SpecularColor;
				}
				_effect.Parameters["SPHERE_SPEC_COL"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_LITE_COL: {
				Vector3[] _arr = new Vector3[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].LightColor;
				}
				_effect.Parameters["SPHERE_LITE_COL"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_SPEC: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Specular;
				}
				_effect.Parameters["SPHERE_SPEC"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_LITE: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Light;
				}
				_effect.Parameters["SPHERE_LITE"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_GLOSS: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Gloss;
				}
				_effect.Parameters["SPHERE_GLOSS"].SetValue(_arr);
				return;
			}
			default:
				throw new Exception("EXCEPTION: INVALID SPHERE PARAMETER.");
		}
	}
}
