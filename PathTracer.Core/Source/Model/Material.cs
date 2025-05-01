using NumVector3 = System.Numerics.Vector3;
using NumVector4 = System.Numerics.Vector4;

namespace PathTracer.Core.Source.Model; 
public record struct Material {
	public NumVector4 color;
	public NumVector3 lightColor;
	public NumVector4 specularColor;
	public float lightIntensity;
	public float specularIntensity; // AKA smoothness
	public float gloss; // probability of hit to be specular vs diffuse
}