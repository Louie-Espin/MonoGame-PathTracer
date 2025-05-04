using Microsoft.Xna.Framework;
using NumVector3 = System.Numerics.Vector3;
using NumVector4 = System.Numerics.Vector4;
using System;

namespace PathTracer.Core.Source.Model; 
public class Sphere {
	private string _id;
	private NumVector3 position;
	private float radius;
	private Material material;

	/* Constructs a Sphere */
	public Sphere(NumVector3 position, float radius, NumVector4 diffuseColor, NumVector4 specularColor, float specular, float gloss, string id = "Sphere") {
		Position = position;
		this.radius = radius;
		material = new Material();
		DiffuseColor = diffuseColor;
		SpecularColor = specularColor;
		Specular = specular;
		Gloss = gloss;

		Light = 0;
		LightColor = new(0, 0, 0); // No light

		_id = id;
	}

	/* Constructs a Sphere, assumes SpecularColor is White */
	public Sphere(NumVector3 position, float radius, NumVector4 diffuseColor, float specular, float gloss, string id = "Sphere") {
		Position = position;
		this.radius = radius;
		material = new Material();
		DiffuseColor = diffuseColor;
		Specular = specular;
		Gloss = gloss;

		Light = 0;
		LightColor    = new(0,0,0);   // No light
		SpecularColor = new(1,1,1,1); // Default: white specular light

		_id = id;
	}

	/* Constructs light-emitting Sphere */
	public Sphere(NumVector3 position, float radius, NumVector3 lightColor, float lightIntensity, string id = "Sphere") {
		Position = position;
		this.radius = radius;
		material = new Material();
		Light = lightIntensity;
		LightColor = lightColor;

		DiffuseColor  = new(0,0,0,1);
		SpecularColor = new(0,0,0,1);
		Specular = 0;
		Gloss = 0;

		_id = id;
	}

	public ref NumVector3 Position => ref position;
	public ref float Radius => ref radius;
	public ref NumVector4 DiffuseColor => ref material.color;
	public ref NumVector3 LightColor => ref material.lightColor;
	public ref NumVector4 SpecularColor => ref material.specularColor;
	public ref float Light => ref material.lightIntensity;
    public ref float Specular => ref material.specularIntensity;
	public ref float Gloss => ref material.gloss;
	public string Id => _id;
}
