using Microsoft.Xna.Framework;

namespace PathTracer.Core.Source.Model; 
public record struct Material {
	public Vector4 color;
	public Vector3 lightColor;
	public Vector4 specularColor;
	public float lightIntensity;
	public float specularIntensity; // AKA smoothness
	public float gloss; // probability of hit to be specular vs diffuse
}