// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility.
// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_UTILS_INCLUDED_
#define _VLB_SHADER_UTILS_INCLUDED_

#include "ShaderMaths.cginc"

#if VLB_DEPTH_BLEND
#ifndef UNITY_DECLARE_DEPTH_TEXTURE // handle Unity pre 5.6.0
#define UNITY_DECLARE_DEPTH_TEXTURE(tex) sampler2D_float tex
#endif
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

inline float SampleSceneZ_Eye(float4 uv)
{
    return LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, uv));
}

inline float SampleSceneZ_01(float4 uv)
{
    return Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, uv));
}


inline float4 DepthFade_VS_ComputeProjPos(float4 vertex_in, float4 vertex_out)
{
    float4 projPos = ComputeScreenPos(vertex_out);
    projPos.z = -UnityObjectToViewPos(vertex_in).z; // = COMPUTE_EYEDEPTH
    return projPos;
}

inline float DepthFade_PS_BlendDistance(float4 projPos, float distance)
{
    float sceneZ = max(0, SampleSceneZ_Eye(UNITY_PROJ_COORD(projPos)) - _ProjectionParams.g);
    float partZ = max(0, projPos.z - _ProjectionParams.g);
    return saturate((sceneZ - partZ) / distance);
}

inline float DepthFade_PS_BlendDistance(float4 projPos, float3 geomPosObjectSpace, float distance)
{
    float sceneZ = max(0, SampleSceneZ_Eye(UNITY_PROJ_COORD(projPos)) /*- _ProjectionParams.g*/);
    float partZ = max(0, -UnityObjectToViewPos(geomPosObjectSpace).z /*- _ProjectionParams.g*/);
    return saturate((sceneZ - partZ) / distance);
}

inline float DepthFade_PS_BlendDistance(float sceneZ, float3 geomPosObjectSpace, float distance)
{
    float partZ = max(0, -UnityObjectToViewPos(geomPosObjectSpace).z /*- _ProjectionParams.g*/);
    return saturate((sceneZ - partZ) / distance);
}
#endif


#if VLB_NOISE_3D
uniform sampler3D _VLB_NoiseTex3D;
uniform float4 _VLB_NoiseGlobal;

float3 Noise3D_GetUVW(float3 wpos)
{
    float4 noiseLocal = VLB_GET_PROP(_NoiseLocal);
    float3 noiseParam = VLB_GET_PROP(_NoiseParam);
    float3 velocity = lerp(noiseLocal.xyz, _VLB_NoiseGlobal.xyz, noiseParam.y);
    float scale = lerp(noiseLocal.w, _VLB_NoiseGlobal.w, noiseParam.z);
	//return frac(wpos.xyz * scale + (_Time.y * velocity)); // frac doesn't give good results on VS
	return (wpos.xyz * scale + (_Time.y * velocity));
}

float Noise3D_GetFactorFromUVW(float3 uvw)
{
    float3 noiseParam = VLB_GET_PROP(_NoiseParam);
    float intensity = noiseParam.x;
	float noise = tex3D(_VLB_NoiseTex3D, uvw).a;
    return lerp(1, noise, intensity);
}

float Noise3D_GetFactorFromWorldPos(float3 wpos)
{
    return Noise3D_GetFactorFromUVW(Noise3D_GetUVW(wpos));
}
#endif // VLB_NOISE_3D


inline float ComputeAttenuation(float pixDistZ, float fadeStart, float fadeEnd, float lerpLinearQuad)
{
    // Attenuation
    float distFromSourceNormalized = invLerpClamped(fadeStart, fadeEnd, pixDistZ);

    // Almost simple linear attenuation between Fade Start and Fade End: Use smoothstep for a better fall to zero rendering
    float attLinear = smoothstep(0, 1, 1 - distFromSourceNormalized);

    // Unity's custom quadratic attenuation https://forum.unity.com/threads/light-attentuation-equation.16006/
    float attQuad = 1.0 / (1.0 + 25.0 * distFromSourceNormalized * distFromSourceNormalized);

    const float kAttQuadStartToFallToZero = 0.8;
    attQuad *= saturate(smoothstep(1.0, kAttQuadStartToFallToZero, distFromSourceNormalized)); // Near the light's range (fade end) we fade to 0 (because quadratic formula never falls to 0)

    return lerp(attLinear, attQuad, lerpLinearQuad);
}



#if VLB_COLOR_GRADIENT_MATRIX_HIGH || VLB_COLOR_GRADIENT_MATRIX_LOW
#if VLB_COLOR_GRADIENT_MATRIX_HIGH
#define FLOAT_PACKING_PRECISION 64
#else
#define FLOAT_PACKING_PRECISION 8
#endif
inline half4 UnpackToColor(float packedFloat)
{
    half4 color;

    color.a = packedFloat % FLOAT_PACKING_PRECISION;
    packedFloat = floor(packedFloat / FLOAT_PACKING_PRECISION);

    color.b = packedFloat % FLOAT_PACKING_PRECISION;
    packedFloat = floor(packedFloat / FLOAT_PACKING_PRECISION);

    color.g = packedFloat % FLOAT_PACKING_PRECISION;
    packedFloat = floor(packedFloat / FLOAT_PACKING_PRECISION);

    color.r = packedFloat;

    return color / (FLOAT_PACKING_PRECISION - 1);
}

inline float GetAtMatrixIndex(float4x4 mat, uint idx) { return mat[idx % 4][floor(idx / 4)]; }

inline half4 DecodeGradient(float t, float4x4 colorMatrix)
{
#define kColorGradientMatrixSize 16
    float sampleIndexFloat = t * (kColorGradientMatrixSize - 1);
    float ratioPerSample = sampleIndexFloat - (int)sampleIndexFloat;
    uint sampleIndexInt = min((uint)sampleIndexFloat, kColorGradientMatrixSize - 2);
    half4 colorA = UnpackToColor(GetAtMatrixIndex(colorMatrix, sampleIndexInt + 0));
    half4 colorB = UnpackToColor(GetAtMatrixIndex(colorMatrix, sampleIndexInt + 1));
    return lerp(colorA, colorB, ratioPerSample);
}
#elif VLB_COLOR_GRADIENT_ARRAY
inline half4 DecodeGradient(float t, float4 colorArray[kColorGradientArraySize])
{
    uint arraySize = kColorGradientArraySize;
    float sampleIndexFloat = t * (arraySize - 1);
    float ratioPerSample = sampleIndexFloat - (int)sampleIndexFloat;
    uint sampleIndexInt = min((uint)sampleIndexFloat, arraySize - 2);
    half4 colorA = colorArray[sampleIndexInt + 0];
    half4 colorB = colorArray[sampleIndexInt + 1];
    return lerp(colorA, colorB, ratioPerSample);
}
#endif // VLB_COLOR_GRADIENT_*

#endif // _VLB_SHADER_UTILS_INCLUDED_
