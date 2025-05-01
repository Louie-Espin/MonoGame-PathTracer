using Microsoft.Xna.Framework;
using NumVector3 = System.Numerics.Vector3;
using NumVector4 = System.Numerics.Vector4;
using System;

namespace PathTracer.Core.Source.Model; 
public class Sphere {

	private NumVector3 position;
	private float radius;
	private Material material;

	/* Constructs a Sphere */
	public Sphere(NumVector3 position, float radius, NumVector4 diffuseColor, NumVector4 specularColor, float specular, float gloss) {
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
	public Sphere(NumVector3 position, float radius, NumVector4 diffuseColor, float specular, float gloss) {
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
	public Sphere(NumVector3 position, float radius, NumVector3 lightColor, float lightIntensity) {
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

	public NumVector3 Position {
		get => position;
		set { position = value; }
	}

	public float Radius => radius;

	public NumVector4 DiffuseColor {
		get { return material.color; }
		set { material.color = value; }
	}

	public NumVector3 LightColor {
		get => material.lightColor;
		set { material.lightColor = value; }
	}

	public NumVector4 SpecularColor {
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
