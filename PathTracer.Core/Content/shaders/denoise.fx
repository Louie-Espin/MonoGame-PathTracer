/* ################################################################################################################################
 * denoise.fx
 * Accumulates the average color of a fragment based on the current & previous renders of the scene
 * 
 * Credit: Sebastian Lague - "Coding Adventures: Ray Tracing"
 * ################################################################################################################################
 */

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

/* ################################################################################################################################
 * SHADER DATA STRUCTURES
 * ################################################################################################################################
 */

struct FS_Input {
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoord : TEXCOORD0;
};

/* ################################################################################################################################
 * SHADER PROPERTIES
 * ################################################################################################################################
 */

/* Number of frames; used to calculate the average */
int _frame;

/* Current & Previous Renders that will be averaged */
sampler SamplerCurr : register(s0) {
    Texture = <CurrRender>;
};

Texture2D PrevRender;
sampler SamplerPrev : register(s1) {
    Texture = <PrevRender>;
};

/* ################################################################################################################################
 * SHADER FUNCTIONS
 * ################################################################################################################################
 */

float4 FS_Denoise(FS_Input input) : COLOR {
    float4 prevColor = tex2D(SamplerPrev, input.TextureCoord);
    float4 currColor = tex2D(SamplerCurr, input.TextureCoord);
	
    float weight = 1.0 / (_frame + 1);
    float weightInv = 1 - weight;
	
    return saturate(prevColor * weightInv + currColor * weight);
}

/* FS_Default: does not apply denoise */
float4 FS_Default(FS_Input input) : COLOR {
    float4 currColor = tex2D(SamplerCurr, input.TextureCoord);
    return currColor;
}

/* ################################################################################################################################
 * SHADER TECHNIQUES
 * ################################################################################################################################
 */

technique AccumEnable {
	pass P0 { PixelShader = compile PS_SHADERMODEL FS_Denoise(); }
};

technique AccumDisable {
    pass P0 { PixelShader = compile PS_SHADERMODEL FS_Default(); }
};