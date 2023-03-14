#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VLB
{
    [CustomEditor(typeof(VolumetricDustParticles))]
    [CanEditMultipleObjects]
    public class VolumetricDustParticlesEditor : EditorCommon
    {
        SerializedProperty alpha, size, direction, speed, density, spawnMaxDistance, cullingEnabled, cullingMaxDistance;
        static bool ms_ShowTips = true;

        static bool AreParticlesInfosUpdated() { return VolumetricDustParticles.isFeatureSupported && Application.isPlaying; }
        public override bool RequiresConstantRepaint() { return AreParticlesInfosUpdated(); }

        protected override void OnEnable()
        {
            base.OnEnable();

            alpha = FindProperty((VolumetricDustParticles x) => x.alpha);
            size = FindProperty((VolumetricDustParticles x) => x.size);
            direction = FindProperty((VolumetricDustParticles x) => x.direction);
            speed = FindProperty((VolumetricDustParticles x) => x.speed);
            density = FindProperty((VolumetricDustParticles x) => x.density);
            spawnMaxDistance = FindProperty((VolumetricDustParticles x) => x.spawnMaxDistance);
            cullingEnabled = FindProperty((VolumetricDustParticles x) => x.cullingEnabled);
            cullingMaxDistance = FindProperty((VolumetricDustParticles x) => x.cullingMaxDistance);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var particles = target as VolumetricDustParticles;

            if (!VolumetricDustParticles.isFeatureSupported)
            {
                EditorGUILayout.HelpBox("Volumetric Dust Particles feature is only supported in Unity 5.5 or above", MessageType.Warning);
            }
            else if (particles.gameObject.activeSelf && particles.enabled && !particles.particlesAreInstantiated)
            {
                EditorGUILayout.HelpBox("Fail to instantiate the Particles. Please check your Config.", MessageType.Error);
                ButtonOpenConfig();
            }

            Header("Rendering");
            EditorGUILayout.PropertyField(alpha, new GUIContent("Alpha", "Max alpha of the particles"));
            EditorGUILayout.PropertyField(size, new GUIContent("Size", "Max size of the particles"));

            Header("Direction & Velocity");
            EditorGUILayout.PropertyField(direction, new GUIContent("Direction", "Direction of the particles\nCone: particles follows the cone/beam direction\nRandom: random direction"));
            EditorGUILayout.PropertyField(speed, new GUIContent("Speed", "Movement speed of the particles"));

            Header("Culling");
            EditorGUILayout.PropertyField(cullingEnabled, new GUIContent("Enabled", "Enable particles culling based on the distance to the Main Camera.\nWe highly recommend to enable this feature to keep good runtime performances."));
            if(cullingEnabled.boolValue)
                EditorGUILayout.PropertyField(cullingMaxDistance, new GUIContent("Max Distance", "The particles will not be rendered if they are further than this distance to the Main Camera"));

            Header("Spawning");
            EditorGUILayout.PropertyField(density, new GUIContent("Density", "Control how many particles are spawned. The higher the density, the more particles are spawned, the higher the performance cost is"));
            EditorGUILayout.PropertyField(spawnMaxDistance, new GUIContent("Max Distance", "The maximum distance (from the light source) where the particles are spawned.\nThe lower it is, the more the particles are gathered near the light source."));

            if (VolumetricDustParticles.isFeatureSupported)
            {
                var infos = "Particles count:\nCurrent: ";
                if (AreParticlesInfosUpdated()) infos += particles.particlesCurrentCount;
                else infos += "(playtime only)";
                if (particles.isCulled)
                    infos += string.Format(" (culled by '{0}')", particles.mainCamera.name);
                infos += string.Format("\nMax: {0}", particles.particlesMaxCount);
                EditorGUILayout.HelpBox(infos, MessageType.Info);
            }

            ms_ShowTips = EditorGUILayout.Foldout(ms_ShowTips, "Infos");
            if (ms_ShowTips)
            {
                EditorGUILayout.HelpBox("We do not recommend to use this feature if you plan to move or change properties of the beam during playtime.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
