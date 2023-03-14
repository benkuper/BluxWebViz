#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VLB
{
    public static class EditorStrings
    {
        public static readonly GUIContent SideThickness = new GUIContent(
            "Side Thickness",
            "Thickness of the beam when looking at it from the side.\n1 = the beam is fully visible (no difference between the center and the edges), but produces hard edges.\nLower values produce softer transition at beam edges.");

        public static readonly GUIContent ColorMode = new GUIContent("Color", "Apply a flat/plain/single color, or a gradient.");
        public static readonly GUIContent ColorGradient = new GUIContent("", "Use the gradient editor to set color and alpha variations along the beam.");
        public static readonly GUIContent ColorFlat = new GUIContent("", "Use the color picker to set a plain RGBA color (takes account of the alpha value).");

        public static readonly GUIContent AlphaInside  = new GUIContent("Alpha (inside)",  "Modulate the beam inside opacity. Is multiplied to Color's alpha.");
        public static readonly GUIContent AlphaOutside = new GUIContent("Alpha (outside)", "Modulate the beam outside opacity. Is multiplied to Color's alpha.");
        public static readonly GUIContent BlendingMode = new GUIContent("Blending Mode", "Additive: highly recommended blending mode\nSoftAdditive: softer additive\nTraditional Transparency: support dark/black colors");

        public static readonly GUIContent SpotAngle = new GUIContent("Spot Angle", "Define the angle (in degrees) at the base of the beam's cone");

        public static readonly GUIContent GlareFrontal = new GUIContent("Glare (frontal)", "Boost intensity factor when looking at the beam from the inside directly at the source.");
        public static readonly GUIContent GlareBehind  = new GUIContent("Glare (from behind)", "Boost intensity factor when looking at the beam from behind.");

        public static readonly GUIContent TrackChanges = new GUIContent(
            " Track changes during Playtime",
            "Check this box to be able to modify properties during Playtime via Script, Animator and/or Timeline.\nEnabling this feature is at very minor performance cost. So keep it disabled if you don't plan to modify this light beam during playtime.");

        public static readonly GUIContent AttenuationEquation = new GUIContent("Equation", "Attenuation equation used to compute fading between 'Fade Start Distance' and 'Range Distance'.\n- Linear: Simple linear attenuation\n- Quadratic: Quadratic attenuation, which usually gives more realistic results\n- Blend: Custom blending mix between linear (0.0) and quadratic attenuation (1.0)");
        public static readonly GUIContent AttenuationCustomBlending = new GUIContent("", "Blending value between Linear (0.0) and Quadratic (1.0) attenuation equations.");

        public static readonly GUIContent FadeStart = new GUIContent("Fade Start Distance", "Distance from the light source (in units) the beam intensity will start to fall off.");
        public static readonly GUIContent FadeEnd = new GUIContent("Range Distance", "Distance from the light source (in units) the beam is entirely faded out");

        public static readonly GUIContent NoiseEnabled = new GUIContent("Enabled", "Enable 3D Noise effect");
        public static readonly GUIContent NoiseIntensity = new GUIContent("Intensity", "Higher intensity means the noise contribution is stronger and more visible");
        public static readonly GUIContent NoiseScale = new GUIContent("Scale", "3D Noise texture scaling: higher scale make the noise more visible, but potentially less realistic");
        public static readonly GUIContent NoiseVelocity = new GUIContent("Velocity", "World Space direction and speed of the noise scrolling, simulating the fog/smoke movement");

        public static readonly GUIContent CameraClippingDistance = new GUIContent("Camera", "Distance from the camera the beam will fade.\n0 = hard intersection\nHigher values produce soft intersection when the camera is near the cone triangles.");
        public static readonly GUIContent DepthBlendDistance = new GUIContent("Opaque geometry", "Distance from the world geometry the beam will fade.\n0 = hard intersection\nHigher values produce soft intersection when the beam intersects other opaque geometry.");

        public static readonly GUIContent ConeRadiusStart = new GUIContent("Truncated Radius", "Radius (in units) at the beam's source (the top of the cone).\n0 will generate a perfect cone geometry.\nHigher values will generate truncated cones.");

        public static readonly GUIContent GeomMeshType = new GUIContent("Mesh Type", "");
        public static readonly GUIContent GeomCap = new GUIContent("Cap", "Show Cap Geometry (only visible from inside)");
        public static readonly GUIContent GeomSides = new GUIContent("Sides", "Number of Sides of the cone.\nHigher values make the beam looks more 'round', but require more memory and graphic performance.\nA recommended value for a decent quality while keeping the poly count low is 18.");
        public static readonly GUIContent GeomSegments = new GUIContent("Segments", "Number of Segments of the cone.\nHigher values give better looking results but require more performance. We recommend at least 3 segments, specially regarding Attenuation and Gradient, otherwise the approximation could become inaccurate.\nThe longer the beam, the more segments we recommend to set.\nA recommended value is 4.");

        public const string SortingLayer = "Sorting Layer";
        public static readonly GUIContent SortingOrder = new GUIContent("Order in Layer", "The overlay priority within its layer. Lower numbers are rendered first and subsequent numbers overlay those below.");

        // BUTTONS
        public static readonly GUIContent ButtonResetProperties = new GUIContent("Default values", "Reset properties to their default values.");
        public static readonly GUIContent ButtonGenerateGeometry = new GUIContent("Regenerate geometry", "Force to re-create the Beam Geometry GameObject.");
        public static readonly GUIContent ButtonAddDustParticles = new GUIContent("+ Dust Particles", "Add a 'VolumetricDustParticles' component.");
        public static readonly GUIContent ButtonAddDynamicOcclusion = new GUIContent("+ Dynamic Occlusion", "Add a 'DynamicOcclusion' component.");
        public static readonly GUIContent ButtonAddTriggerZone = new GUIContent("+ Trigger Zone", "Add a 'TriggerZone' component.");
        public static readonly GUIContent ButtonOpenGlobalConfig = new GUIContent("Open Global Config");

        // HELP BOXES
        public const string HelpNoSpotlight = "To bind properties from the Light and the Beam together, this component must be attached to a Light of type 'Spot'";
        public const string HelpNoiseLoadingFailed = "Fail to load 3D noise texture. Please check your Config.";
        public const string HelpAnimatorWarning = "If you want to animate your light beam in real-time, you should enable the 'trackChangesDuringPlaytime' property.";
        public const string HelpTrackChangesEnabled = "This beam will keep track of the changes of its own properties and the spotlight attached to it (if any) during playtime. You can modify every properties except 'geomSides'.";
        public const string HelpDepthTextureMode = "To support 'Soft Intersection with Opaque Geometry', your camera must use 'DepthTextureMode.Depth'.";
        public const string HelpDepthMobile = "On mobile platforms, the depth buffer precision can be pretty low. Try to keep a small depth range on your cameras: the difference between the near and far clip planes should stay as low as possible.";

        // DYNAMIC OCCLUSION
        public static readonly GUIContent DynOcclusionMinSurfaceRatio = new GUIContent("Min Occluded %", "Approximated percentage of the beam to collide with the surface in order to be considered as occluder.");
        public static readonly GUIContent DynOcclusionMaxSurfaceDot = new GUIContent("Max Angle", "Max angle (in degrees) between the beam and the surface in order to be considered as occluder.");

        // CONFIGS
        public static readonly GUIContent ConfigGeometryOverrideLayer = new GUIContent("Override Layer", "The layer the GameObjects holding the procedural cone meshes are created on");
        public static readonly GUIContent ConfigGeometryTag = new GUIContent("Tag", "The tag applied on the procedural geometry GameObjects");
        public static readonly GUIContent ConfigGeometryRenderQueue = new GUIContent("Render Queue", "Determine in which order beams are rendered compared to other objects.\nThis way for example transparent objects are rendered after opaque objects, and so on.");
        public static readonly GUIContent ConfigGeometryRenderingMode = new GUIContent("Rendering Mode",
@"- Multi-Pass: Use the 2 pass shader. Will generate 2 drawcalls per beam (Not compatible with custom Render Pipeline such as HDRP and LWRP).
- Single-Pass: Use the 1 pass shader. Will generate 1 drawcall per beam.
- GPU Instancing: Dynamically batch multiple beams to combine and reduce draw calls.");
        public const string ConfigGeometryGpuInstancingNotSupported = "GPU Instancing Rendering Mode is only supported on Unity 5.6 or above!\nSingle Pass will be used.";

        public static readonly GUIContent ConfigBeamShader1Pass = new GUIContent("Shader (single-pass)", "Main shader (1 pass version) applied to the cone beam geometry");
        public static readonly GUIContent ConfigBeamShader2Pass = new GUIContent("Shader (multi-pass)", "Main shader (multi-pass version) applied to the cone beam geometry");
        public static readonly GUIContent ConfigSharedMeshSides = new GUIContent("Sides", "Number of Sides of the cone.\nHigher values make the beam looks more 'round', but require more memory and graphic performance.\nA recommended value for a decent quality while keeping the poly count low is 18.");
        public static readonly GUIContent ConfigSharedMeshSegments = new GUIContent("Segments", "Number of Segments of the cone.\nHigher values give better looking results but require more performance. We recommend at least 3 segments, specially regarding Attenuation and Gradient, otherwise the approximation could become inaccurate.\nThe longer the beam, the more segments we recommend to set.\nA recommended value is 4.");
        public static readonly GUIContent ConfigGlobalNoiseScale = new GUIContent("Scale", "Global 3D Noise texture scaling: higher scale make the noise more visible, but potentially less realistic");
        public static readonly GUIContent ConfigGlobalNoiseVelocity = new GUIContent("Velocity", "Global World Space direction and speed of the noise scrolling, simulating the fog/smoke movement");
        public static readonly GUIContent ConfigNoise3DData = new GUIContent("3D Noise Data binary file", "Binary file holding the 3D Noise texture data (a 3D array). Must be exactly Size * Size * Size bytes long.");
        public static readonly GUIContent ConfigNoise3DSize = new GUIContent("3D Noise Data dimension", "Size (of one dimension) of the 3D Noise data. Must be power of 2. So if the binary file holds a 32x32x32 texture, this value must be 32.");
        public static readonly GUIContent ConfigDustParticlesPrefab = new GUIContent("Dust Particles Prefab", "ParticleSystem prefab instantiated for the Volumetric Dust Particles feature (Unity 5.5 or above)");
        public static readonly GUIContent ConfigOpenDocumentation = new GUIContent("Documentation", "Open the online documentation."); 
        public static readonly GUIContent ConfigResetToDefaultButton = new GUIContent("Default values", "Reset properties to their default values.");
    }
}
#endif
