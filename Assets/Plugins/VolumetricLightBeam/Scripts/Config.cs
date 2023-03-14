using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VLB
{
    [HelpURL(Consts.HelpUrlConfig)]
    public class Config : ScriptableObject
    {
        /// <summary>
        /// Override the layer on which the procedural geometry is created or not
        /// </summary>
        public bool geometryOverrideLayer = Consts.ConfigGeometryOverrideLayerDefault;

        /// <summary>
        /// The layer the procedural geometry gameObject is in (only if geometryOverrideLayer is enabled)
        /// </summary>
        public int geometryLayerID = Consts.ConfigGeometryLayerIDDefault;

        /// <summary>
        /// The tag applied on the procedural geometry gameObject
        /// </summary>
        public string geometryTag = Consts.ConfigGeometryTagDefault;

        /// <summary>
        /// Determine in which order beams are rendered compared to other objects.
        /// This way for example transparent objects are rendered after opaque objects, and so on.
        /// </summary>
        public int geometryRenderQueue = (int)Consts.ConfigGeometryRenderQueueDefault;

        [System.Obsolete("Use 'renderingMode' instead")]
        public bool forceSinglePass = false;

        /// <summary>
        /// MultiPass: Use the 2 pass shader. Will generate 2 drawcalls per beam.
        /// SinglePass: Use the 1 pass shader. Will generate 1 drawcall per beam. Mandatory when using Render Pipeline such as HDRP and LWRP.
        /// GPUInstancing: Dynamically batch multiple beams to combine and reduce draw calls (Feature only supported in Unity 5.6 or above). More info: https://docs.unity3d.com/Manual/GPUInstancing.html
        /// </summary>
        public RenderingMode renderingMode = Consts.ConfigGeometryRenderingModeDefault;

        /// <summary>
        /// Actual Rendering Mode used on the current platform
        /// </summary>
        public RenderingMode actualRenderingMode { get { return (renderingMode == RenderingMode.GPUInstancing && !GpuInstancing.isSupported) ? RenderingMode.SinglePass : renderingMode; } }

        /// <summary>
        /// Depending on the actual Rendering Mode used, returns true if the single pass shader will be used, false otherwise.
        /// </summary>
        public bool useSinglePassShader { get { return actualRenderingMode != RenderingMode.MultiPass; } }

        /// <summary>
        /// Main shaders applied to the cone beam geometry
        /// </summary>
        [SerializeField, HighlightNull] Shader beamShader1Pass = null;
   
        [FormerlySerializedAs("BeamShader"), FormerlySerializedAs("beamShader")]
        [SerializeField, HighlightNull] Shader beamShader2Pass = null;

        public Shader beamShader { get { return useSinglePassShader ? beamShader1Pass : beamShader2Pass; } }

        /// <summary>
        /// Number of Sides of the shared cone mesh
        /// </summary>
        public int sharedMeshSides = Consts.ConfigSharedMeshSides;

        /// <summary>
        /// Number of Segments of the shared cone mesh
        /// </summary>
        public int sharedMeshSegments = Consts.ConfigSharedMeshSegments;

        /// <summary>
        /// Global 3D Noise texture scaling: higher scale make the noise more visible, but potentially less realistic.
        /// </summary>
        [Range(Consts.NoiseScaleMin, Consts.NoiseScaleMax)]
        public float globalNoiseScale = Consts.NoiseScaleDefault;

        /// <summary>
        /// Global World Space direction and speed of the noise scrolling, simulating the fog/smoke movement
        /// </summary>
        public Vector3 globalNoiseVelocity = Consts.NoiseVelocityDefault;

        /// <summary>3D Noise param sent to the shader</summary>
        public Vector4 globalNoiseParam { get { return new Vector4(globalNoiseVelocity.x, globalNoiseVelocity.y, globalNoiseVelocity.z, globalNoiseScale); } }

        /// <summary>
        /// Binary file holding the 3D Noise texture data (a 3D array). Must be exactly Size * Size * Size bytes long.
        /// </summary>
        [HighlightNull]
        public TextAsset noise3DData = null;

        /// <summary>
        /// Size (of one dimension) of the 3D Noise data. Must be power of 2. So if the binary file holds a 32x32x32 texture, this value must be 32.
        /// </summary>
        public int noise3DSize = Consts.ConfigNoise3DSizeDefault;

        /// <summary>
        /// ParticleSystem prefab instantiated for the Volumetric Dust Particles feature (Unity 5.5 or above)
        /// </summary>
        [HighlightNull]
        public ParticleSystem dustParticlesPrefab = null;

        // INTERNAL
#pragma warning disable 0414
        [SerializeField] int pluginVersion = -1;
#pragma warning restore 0414

        public void Reset()
        {
            geometryOverrideLayer = Consts.ConfigGeometryOverrideLayerDefault;
            geometryLayerID = Consts.ConfigGeometryLayerIDDefault;
            geometryTag = Consts.ConfigGeometryTagDefault;
            geometryRenderQueue = (int)Consts.ConfigGeometryRenderQueueDefault;

            beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
            beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");

            sharedMeshSides = Consts.ConfigSharedMeshSides;
            sharedMeshSegments = Consts.ConfigSharedMeshSegments;

            globalNoiseScale = Consts.NoiseScaleDefault;
            globalNoiseVelocity = Consts.NoiseVelocityDefault;

            noise3DData = Resources.Load("Noise3D_64x64x64") as TextAsset;
            noise3DSize = Consts.ConfigNoise3DSizeDefault;

            dustParticlesPrefab = Resources.Load("DustParticles", typeof(ParticleSystem)) as ParticleSystem;

            renderingMode = Consts.ConfigGeometryRenderingModeDefault;

#if UNITY_EDITOR
            GlobalMesh.Destroy();
            VolumetricLightBeam._EditorSetAllMeshesDirty();
#endif
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            noise3DSize = Mathf.Max(2, Mathf.NextPowerOfTwo(noise3DSize));

            sharedMeshSides = Mathf.Clamp(sharedMeshSides, Consts.GeomSidesMin, Consts.GeomSidesMax);
            sharedMeshSegments = Mathf.Clamp(sharedMeshSegments, Consts.GeomSegmentsMin, Consts.GeomSegmentsMax);
        }
#endif
        public ParticleSystem NewVolumetricDustParticles()
        {
            if (!dustParticlesPrefab)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("Failed to instantiate VolumetricDustParticles prefab.");
                }
                return null;
            }

            var instance = Instantiate(dustParticlesPrefab);
#if UNITY_5_4_OR_NEWER
            instance.useAutoRandomSeed = false;
#endif
            instance.name = "Dust Particles";
            instance.gameObject.hideFlags = Consts.ProceduralObjectsHideFlags;
            instance.gameObject.SetActive(true);
            return instance;
        }

#if UNITY_EDITOR
        public static void EditorSelectInstance()
        {
            Selection.activeObject = Config.Instance;
            if(Selection.activeObject == null)
                Debug.LogErrorFormat("Cannot find any Config resource");
        }
#endif

        void OnEnable()
        {
            HandleBackwardCompatibility(pluginVersion, Version.Current);
            pluginVersion = Version.Current;
        }

        void HandleBackwardCompatibility(int serializedVersion, int newVersion)
        {
            if (serializedVersion == newVersion) return;    // same version: nothing to do

            if (serializedVersion < 1600)
            {
#pragma warning disable 0618
                renderingMode = forceSinglePass ? RenderingMode.SinglePass : RenderingMode.MultiPass;
#pragma warning restore 0618
            }

            Utils.MarkObjectDirty(this);
        }

        // Singleton management
        static Config m_Instance = null;
        public static Config Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var found = Resources.LoadAll<Config>("Config");
                    Debug.Assert(found.Length != 0, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", typeof(Config)));
                    m_Instance = found[0];
                }
                return m_Instance;
            }
        }
    }
}
