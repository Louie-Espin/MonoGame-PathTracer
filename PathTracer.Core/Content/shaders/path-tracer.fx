#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

/* ################################################################################################################################
 * 
 * SHADER DATA STRUCTURES
 * 
 * ################################################################################################################################
 */

struct FS_Input {
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoord : TEXCOORD0;
};

struct Ray {
    float3 Origin;
    float3 Direction;
};

struct Material {
    float4 Color;
    float3 lightColor;
    float  lightIntensity;
    float4 specularColor;
    float  specularIntensity; // AKA smoothness
    float  gloss; // probability of hit to be specular vs diffuse
};

struct Sphere {
    float3 Position;
    float radius;
    Material material;
};

struct Hit {
    bool hit;
    float distance;
    float3 Point;
    float3 Normal;
    Material material;
};

/* ################################################################################################################################
 * 
 * HELPER FUNCTIONS
 * 
 * ################################################################################################################################
 */

/* Ray Constructor */
Ray _ray(float3 origin, float3 target) {
    Ray r;
    r.Origin = origin;
    r.Direction = normalize(target - origin);
    return r;
}

/* Returns P(t) = A + tb */
float3 RayAt(Ray R, float t) {
    return R.Origin + (t * R.Direction);
}

/* Sphere Constructor */
Sphere newSphere(float3 position, float radius, Material material) {
    Sphere s;
    s.Position = position;
    s.radius = radius;
    s.material = material;
    return s;
}

Material newMaterial(float4 color, float3 lightColor, float lightIntensity, float4 specularColor, float specularIntensity, float gloss) {
    Material m;
    m.Color = color;
    m.lightColor = lightColor;
    m.lightIntensity = lightIntensity;
    m.specularColor = specularColor;
    m.specularIntensity = specularIntensity;
    m.gloss = gloss;
    return m;
}

/* [FROM CG BOOK] expands a range-compressed vector from [0,1] to [-1,1]*/
float3 expand(float3 v) { // TODO: could this be 'inout' ?
    return (v - 0.5) * 2;
}

/* Calculates Local Position for a given Point in the Viewport */
float3 ViewportPointLocal(float2 uv, float focalLength, float2 viewportSize) {
    // UV values of the current point transformed into coordinates [WARNING: ASSUMES CAMERA FACING Z-AXIS]
    float3 local = expand(float3(uv, 1));
    
    return float3(-(local.x * viewportSize.x), -(local.y * viewportSize.y), focalLength);
}

/* ################################################################################################################################
 * RNG FUNCTIONS - FIXME: research into normal distribution rng & attribute missing sources
 * ################################################################################################################################
 */

float getPCG(inout uint state) {
    state = state * 747796405 + 2891336453;
    uint result = ((state >> ((state >> 28) + 4)) ^ state) * 277803737;
    result = (result >> 22) ^ result;
    return result;
}

float rand(inout uint state) {
    return getPCG(state) / 4294967295.0; // 2^32 - 1
}

float rand_MC(inout uint state) {
    float theta = 2 * 3.1415926 * rand(state);
    float rho = sqrt(-2 * log(rand(state)));
    return rho * cos(theta);
}

float3 Random(inout uint state) {
    float3 dir = float3(rand_MC(state), rand_MC(state), rand_MC(state));
    return normalize(dir);
}

/* ################################################################################################################################
 * 
 * ################################################################################################################################
 */

/* IntersectSphere(Ray R, Sphere S):
 * Performs hit calculation based on given Ray & Sphere using quadratic formula.
 * Returns the closest Hit, if any.
 * READ: Computer Graphics from Scratch (Chapter 2: Basic Raytracing, page 25)
 */
Hit IntersectSphere(Ray R, Sphere S) {
    Hit hitInfo = (Hit) 0;
    
    /* CO - offset between the Ray's Origin & Sphere's Center */
    float3 CO = R.Origin - S.Position;
    
    /* Calculating quadratic equation variables */
    float a = dot(R.Direction, R.Direction);
    float b = 2 * dot(CO, R.Direction);
    float c = dot(CO, CO) - pow(S.radius, 2);
    
    float discriminant = b * b - 4 * a * c;
    
    /* The Ray Missed if there is no solution (discriminant < 0) */
    if (discriminant >= 0) {
        /* Calculate distance to the nearest Intersection between Ray and Sphere */
        float dist = (-b - sqrt(discriminant)) / (2 * a);
        
        /* Ignore the Hit if it occurs behind the camera */
        if (dist < 0) return hitInfo;
        
        hitInfo.hit = true;
        hitInfo.distance = dist;
        hitInfo.Point = RayAt(R, dist);
        hitInfo.Normal = normalize(hitInfo.Point - S.Position);
    }
    
    return hitInfo;
}

/* ################################################################################################################################
 * 
 * SHADER PROPERTIES
 * 
 * ################################################################################################################################
 */
#define NUM_SPHERES 5

float4x4 _worldMat;
float4x4 _viewMat;
float4x4 _projectionMat;

float3 _cameraPosition; // Camera Position in World-Space Coordinates 
float4x4 _cameraTransform; // Camera Transformation Matrix
float _focalLength; // d: Length between Camera & Viewport
float2 _viewportSize;

int SPP = 200; // (Samples-Per-Pixel) # of rays traced per pixel
int BOUNCES = 100; // Maximum # of bounces to calculate on a ray 
int _frame; // Index of current frame; used to randomize
int _screenX, _screenY; // Width & Heigh of Screen in Pixels

/* ARRAY OF SPHERES */
float3 SPHERE_POS[NUM_SPHERES];
float  SPHERE_RADIUS[NUM_SPHERES];
float4 SPHERE_DIFF_COL[NUM_SPHERES];
float4 SPHERE_SPEC_COL[NUM_SPHERES];
float3 SPHERE_LITE_COL[NUM_SPHERES];
float  SPHERE_SPEC[NUM_SPHERES];
float  SPHERE_GLOSS[NUM_SPHERES];
float  SPHERE_LITE[NUM_SPHERES];
/* **************** */

/* Texture that will be targeted by SpriteBatch before drawing */
sampler TextureSampler = sampler_state {
    Texture = <ViewportTexture>;
};

/* ################################################################################################################################
 * 
 * SHADER FUNCTIONS
 * 
 * ################################################################################################################################
 */

/* CalculateSpheres(R) */
Hit CalculateSpheres(Ray R) {  
    /* We haven't hit anything, so our closest hit is inifinitely far away */
    Hit closestHit = (Hit) 0;
    closestHit.distance = 1000000000;
    
    for (int i = 0; i < NUM_SPHERES; i++) {
        Material mat = newMaterial(SPHERE_DIFF_COL[i], SPHERE_LITE_COL[i], SPHERE_LITE[i], SPHERE_SPEC_COL[i], SPHERE_SPEC[i], SPHERE_GLOSS[i]);
        Sphere sphere = newSphere(SPHERE_POS[i], SPHERE_RADIUS[i], mat);

        Hit hit = IntersectSphere(R, sphere);
        
        if (hit.hit && hit.distance < closestHit.distance) {
            closestHit = hit;
            closestHit.material = sphere.material;
        }
    }
    
    return closestHit;
}

/* Trace Function */
float3 Trace(Ray r, inout uint rng) {
    /* Keeping track of light and color */
    float3 incomingLight = float3(0, 0, 0);
    float3 rayColor = float3(1, 1, 1);
    
    for (int i = 0; i <= BOUNCES; i++) {
        
        Hit hit = CalculateSpheres(r);
        Material mat = hit.material;
        
        if (!hit.hit) {
            incomingLight += float3(0.0, 0.0, 0.0) * rayColor;
            break;
        }
        
        /* Use diffuse or specular based on material's gloss. (high gloss = more likely to be reflected) */
        bool isReflected = mat.gloss >= rand(rng);
        
        /* Calculate light from traced ray hit */
        float3 lightEmmitted = mat.lightColor * mat.lightIntensity;
        
        /* Add to hit info to our values */
        incomingLight += lightEmmitted * rayColor; // if ray hits light source
        rayColor *= lerp(mat.Color, mat.specularColor, isReflected).xyz; // FIXME: does not keep track of alpha channel
        
        /* Randomize the direction of the next bounce. */
        float3 dirDiffuse  = normalize(hit.Normal + Random(rng));
        float3 dirSpecular = reflect(r.Direction, hit.Normal);
        
        r.Origin = hit.Point;
        r.Direction = normalize(lerp(dirDiffuse, dirSpecular, hit.material.specularIntensity * isReflected));
    }

    return incomingLight;
}

float4 FS_Main(FS_Input input) : COLOR0 {
	
    float4 color = tex2D(TextureSampler, input.TextureCoord);
	
    /* pixel coordinates as a percetange of the screen, from 0 to 1 on each axis */
    float2 uv = input.TextureCoord.xy;
	
    /* Calculate the Position for a given Point in the Viewport */
    float3 localPoint = ViewportPointLocal(uv, _focalLength, _viewportSize);
    float3 Point = mul(float4(localPoint, 1), _cameraTransform).xyz;
    
    /* Cast Ray from camera position towards Viewport Point */
    Ray r = _ray(_cameraPosition, Point);
    
    /* Create RNG Seed */
    uint2 numPixels = uint2(_screenX, _screenY); // FIXME: HARD-CODED VALUE
    uint2 pixelCoord = uv * numPixels;
    uint pixelIdx = pixelCoord.y * numPixels.x + pixelCoord.x;
    uint rng = pixelIdx + _frame * 719393;
    
    
    // Hit hit = CalculateSpheres(r);
    // return hit.hit ? hit.material.Color : float4(r.Direction, color.w);
    
    // Trace Pixel Color
    float3 totalPixelColor = float3(0, 0, 0);
    for (int rayIdx = 0; rayIdx < SPP; rayIdx++) {
        totalPixelColor += Trace(r, rng);
    }
    float3 avgColor = totalPixelColor / SPP;
    
    return float4(avgColor, 1);

}

float4 FS_Rasterize(FS_Input input) : COLOR0 {
    float4 color = tex2D(TextureSampler, input.TextureCoord);
    return color;
}

/* ################################################################################################################################
 * 
 * SHADER TECHNIQUES
 * 
 * ################################################################################################################################
 */

technique PathTrace {
	pass P0 { PixelShader = compile PS_SHADERMODEL FS_Main(); }
};

technique Rasterize {
    pass P0 { PixelShader = compile PS_SHADERMODEL FS_Rasterize(); }
}