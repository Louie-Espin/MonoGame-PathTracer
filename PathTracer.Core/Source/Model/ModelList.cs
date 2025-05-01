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
	const int NUM_SPHERES = 5;
	int _selected = 0;
	Sphere[] _spheres = [
		new( position: new(0, 0, 60),  radius: 40, lightColor:   new(1, 1, 1),    lightIntensity: 5.0f ),
		new( position: new(0, -32, 0), radius: 30, diffuseColor: new(1, 0, 1, 1), specular: 0.4f, gloss: 0.6f ),
		new( position: new(4, 0, 0),   radius: 2,  diffuseColor: new(1, 0, 0, 1), specular: 1.0f, gloss: 1.0f ),
		new( position: new(0, 0, 0),   radius: 2,  diffuseColor: new(0, 1, 0, 1), specular: 1.0f, gloss: 0.7f ),
		new( position: new(-4, 0, 0),  radius: 2,  diffuseColor: new(0, 0, 1, 1), specular: 0.5f, gloss: 0.5f ),
	];

	/* Shortcuts for accessing effect parameters */
	EffectParameter _positionParam;
	EffectParameter _radiusParam;
	EffectParameter _diffColorParam;
	EffectParameter _specColorParam;
	EffectParameter _liteColorParam;
	EffectParameter _liteParam;
	EffectParameter _specParam;
	EffectParameter _glossParam;

	public int Selected {
		get => _selected;
		set { _selected = (_selected + 1) % 5; }
	}

	public int Size => NUM_SPHERES;

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
	public void SetData(ref Effect ptEffect, SphereParam param) {

		switch (param) {
			case SphereParam.SPHERE_POS: {
				Vector3[] _arr = new Vector3[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Position;
				}
				ptEffect.Parameters["SPHERE_POS"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_RADIUS: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Radius;
				}
				ptEffect.Parameters["SPHERE_RADIUS"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_DIFF_COL: {
				Vector4[] _arr = new Vector4[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].DiffuseColor;
				}
				ptEffect.Parameters["SPHERE_DIFF_COL"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_SPEC_COL: {
				Vector4[] _arr = new Vector4[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].SpecularColor;
				}
				ptEffect.Parameters["SPHERE_SPEC_COL"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_LITE_COL: {
				Vector3[] _arr = new Vector3[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].LightColor;
				}
				ptEffect.Parameters["SPHERE_LITE_COL"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_SPEC: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Specular;
				}
				ptEffect.Parameters["SPHERE_SPEC"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_LITE: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Light;
				}
				ptEffect.Parameters["SPHERE_LITE"].SetValue(_arr);
				return;
			}
			case SphereParam.SPHERE_GLOSS: {
				float[] _arr = new float[NUM_SPHERES];
				for (int i = 0; i < NUM_SPHERES; i++) {
					_arr[i] = _spheres[i].Gloss;
				}
				ptEffect.Parameters["SPHERE_GLOSS"].SetValue(_arr);
				return;
			}
			default:
				throw new Exception("EXCEPTION: INVALID SPHERE PARAMETER.");
		}
	}
}
