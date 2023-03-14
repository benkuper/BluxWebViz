using UnityEngine;

namespace VLB
{
    public static class Consts
    {
        // HELP
        const string HelpUrlBase = "http://saladgamer.com/vlb-doc/";
        public const string HelpUrlBeam = HelpUrlBase + "comp-lightbeam/";
        public const string HelpUrlDustParticles = HelpUrlBase + "comp-dustparticles/";
        public const string HelpUrlDynamicOcclusion = HelpUrlBase + "comp-dynocclusion/";
        public const string HelpUrlTriggerZone = HelpUrlBase + "comp-triggerzone/";
        public const string HelpUrlConfig = HelpUrlBase + "config/";

        // INTERNAL
        public static readonly bool ProceduralObjectsVisibleInEditor = true;
        public static HideFlags ProceduralObjectsHideFlags { get { return ProceduralObjectsVisibleInEditor ? (HideFlags.NotEditable | HideFlags.DontSave) : (HideFlags.HideAndDontSave); } }

        // BEAM
        public static readonly Color FlatColor = Color.white;
        public const ColorMode ColorModeDefault = ColorMode.Flat;

        public const float Alpha = 1f;
        public const float SpotAngleDefault = 35f;
        public const float SpotAngleMin = 0.1f;
        public const float SpotAngleMax = 179.9f;
        public const float ConeRadiusStart = 0.1f;
        public const MeshType GeomMeshType = MeshType.Shared;
        public const int GeomSidesDefault = 18;
        public const int GeomSidesMin = 3;
        public const int GeomSidesMax = 256;
        public const int GeomSegmentsDefault = 5;
        public const int GeomSegmentsMin = 0;
        public const int GeomSegmentsMax = 64;
        public const bool GeomCap = false;

        public const AttenuationEquation AttenuationEquationDefault = AttenuationEquation.Quadratic;
        public const float AttenuationCustomBlending = 0.5f;
        public const float FadeStart = 0f;
        public const float FadeEnd = 3f;
        public const float FadeMinThreshold = 0.01f;

        public const float DepthBlendDistance = 2f;
        public const float CameraClippingDistance = 0.5f;

        public const float FresnelPowMaxValue = 10f;
        public const float FresnelPow = 8f;

        public const float GlareFrontal = 0.5f;
        public const float GlareBehind = 0.5f;

        public const float NoiseIntensityMin = 0.0f;
        public const float NoiseIntensityMax = 1.0f;
        public const float NoiseIntensityDefault = 0.5f;

        public const float NoiseScaleMin = 0.01f;
        public const float NoiseScaleMax = 2f;
        public const float NoiseScaleDefault = 0.5f;

        public static readonly Vector3 NoiseVelocityDefault = new Vector3(0.07f, 0.18f, 0.05f);

        public const BlendingMode BlendingModeDefault = BlendingMode.Additive;

        // DYNAMIC OCCLUSION
        public const OccluderDimensions DynOcclusionDimensionsDefault = OccluderDimensions.Occluders3D;
        public static readonly LayerMask DynOcclusionLayerMaskDefault = -1;
        public const float DynOcclusionMinOccluderAreaDefault = 0.0f;
        public const int DynOcclusionWaitFrameCountDefault = 3;
        public const float DynOcclusionMinSurfaceRatioDefault = 0.5f;
        public const float DynOcclusionMinSurfaceRatioMin = 50f;
        public const float DynOcclusionMinSurfaceRatioMax = 100f;
        public const float DynOcclusionMaxSurfaceDotDefault = 0.25f; // around 75 degrees
        public const float DynOcclusionMaxSurfaceAngleMin = 45f;
        public const float DynOcclusionMaxSurfaceAngleMax = 90f;
        public const PlaneAlignment DynOcclusionPlaneAlignmentDefault = PlaneAlignment.Surface;
        public const float DynOcclusionPlaneOffsetDefault = 0.1f;

        // CONFIG
        public const bool ConfigGeometryOverrideLayerDefault = true;
        public const int ConfigGeometryLayerIDDefault = 1;
        public const string ConfigGeometryTagDefault = "Untagged";
        public const RenderQueue ConfigGeometryRenderQueueDefault = RenderQueue.Transparent;
        public const RenderingMode ConfigGeometryRenderingModeDefault = RenderingMode.MultiPass;
        public const int ConfigNoise3DSizeDefault = 64;
        public const int ConfigSharedMeshSides = 24;
        public const int ConfigSharedMeshSegments = 5;
    }
}