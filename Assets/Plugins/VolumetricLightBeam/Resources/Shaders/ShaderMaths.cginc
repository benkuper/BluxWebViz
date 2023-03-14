// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility.
// UNITY_SHADER_NO_UPGRADE

#ifndef _VLB_SHADER_MATHS_INCLUDED_
#define _VLB_SHADER_MATHS_INCLUDED_

#include "UnityCG.cginc"

#if UNITY_VERSION < 540
#define matWorldToObject _World2Object
#define matObjectToWorld _Object2World
inline float4 UnityObjectToClipPos(in float3 pos) { return mul(UNITY_MATRIX_MVP, float4(pos, 1.0)); }
inline float3 UnityObjectToViewPos(in float3 pos) { return mul(UNITY_MATRIX_MV, float4(pos, 1.0)).xyz; }
#else
#define matWorldToObject unity_WorldToObject
#define matObjectToWorld unity_ObjectToWorld
#endif

inline float3 UnityWorldToObjectPos(in float3 pos) { return mul(matWorldToObject, float4(pos, 1.0)).xyz; }
inline float3 UnityObjectToWorldPos(in float3 pos) { return mul(matObjectToWorld, float4(pos, 1.0)).xyz; }

inline float dot2(float2 v) { return dot(v,v); }
inline float lerpClamped(float a, float b, float t) { return lerp(a, b, saturate(t)); }
inline float invLerp(float a, float b, float t) { return (t - a) / (b - a); }
inline float invLerpClamped(float a, float b, float t) { return saturate(invLerp(a, b, t)); }
inline float fromABtoCD_Clamped(float valueAB, float A, float B, float C, float D)  { return lerpClamped(C, D, invLerpClamped(A, B, valueAB)); }

// Get signed distance of pos from the plane (normal ; d).
// Normal should be normalized.
// If we want to disable this feature, we could set normal and d to 0 (no discard in this case).
inline float DistanceToPlane(float3 pos, float3 normal, float d) { return dot(normal, pos) + d; }


inline float parabola( float x, float k )
{
    return pow( 4.0*x*(1.0-x), k );
}

// https://en.wikipedia.org/wiki/Smoothstep
inline float smootherstep(float edge0, float edge1, float x)
{
    // Scale, and clamp x to 0..1 range
    x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    // Evaluate polynomial
    return x*x*x*(x*(x * 6 - 15) + 10);
}

#endif
