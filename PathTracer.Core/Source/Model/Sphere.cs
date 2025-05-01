using Microsoft.Xna.Framework;
using System;

namespace PathTracer.Core.Source.Model; 
public class Sphere {

	private Vector3 position;
	private float radius;
	private Material material;

	/* Constructs a Sphere */
	public Sphere(Vector3 position, float radius, Vector4 diffuseColor, Vector4 specularColor, float specular, float gloss) {
		Position = position;
		this.radius = radius;
		material = new Material();
		DiffuseColor = diffuseColor;
		SpecularColor = specularColor;
		Specular = specular;
		Gloss = gloss;

		Light = 0;
		LightColor = new(0, 0, 0); // No light
	}

	/* Constructs a Sphere, assumes SpecularColor is White */
	public Sphere(Vector3 position, float radius, Vector4 diffuseColor, float specular, float gloss) {
		Position = position;
		this.radius = radius;
		material = new Material();
		DiffuseColor = diffuseColor;
		Specular = specular;
		Gloss = gloss;

		Light = 0;
		LightColor    = new(0,0,0);   // No light
		SpecularColor = new(1,1,1,1); // Default: white specular light
	}

	/* Constructs light-emitting Sphere */
	public Sphere(Vector3 position, float radius, Vector3 lightColor, float lightIntensity) {
		Position = position;
		this.radius = radius;
		material = new Material();
		Light = lightIntensity;
		LightColor = lightColor;

		DiffuseColor  = new(0,0,0,1);
		SpecularColor = new(0,0,0,1);
		Specular = 0;
		Gloss = 0;
	}

	public Vector3 Position {
		get => position;
		set { position = value; }
	}

	public float Radius => radius;

	public Vector4 DiffuseColor {
		get { return material.color; }
		set { material.color = value; }
	}

	public Vector3 LightColor {
		get => material.lightColor;
		set { material.lightColor = value; }
	}

	public Vector4 SpecularColor {
		get => material.specularColor;
		set { material.specularColor = value; }
	}

	public float Light {
		get => material.lightIntensity;
		set { material.lightIntensity = value; }
	}

	public float Specular {  
		get => material.specularIntensity;
		set { material.specularIntensity = Math.Clamp(value, 0, 1); }
	}

	public float Gloss {
		get =>  material.gloss;
		set { material.gloss = Math.Clamp(value, 0, 1); }
	}
}
