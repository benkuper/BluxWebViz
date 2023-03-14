#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VLB
{
    public static class EditorExtensions
    {
        public static GameObject NewBeam()
        {
            return new GameObject("Volumetric Light Beam", typeof(VolumetricLightBeam));
        }

        public static GameObject NewBeamAndDust()
        {
            return new GameObject("Volumetric Light Beam + Dust", typeof(VolumetricLightBeam), typeof(VolumetricDustParticles));
        }

        public static GameObject NewSpotLightAndBeam()
        {
            var light = Utils.NewWithComponent<Light>("Spotlight and Beam");
            light.type = LightType.Spot;
            var gao = light.gameObject;
            gao.AddComponent<VolumetricLightBeam>();
            return gao;
        }

        static void OnNewGameObjectCreated(GameObject gao)
        {
            if (Selection.activeGameObject)
                gao.transform.SetParent(Selection.activeGameObject.transform);

            Selection.activeGameObject = gao;
        }

        [MenuItem("GameObject/Light/Volumetric Beam", false, 10)]
        public static void Menu_CreateNewBeam()
        {
            OnNewGameObjectCreated(NewBeam());
        }

        [MenuItem("GameObject/Light/Volumetric Beam and Spotlight", false, 11)]
        public static void Menu_CreateSpotLightAndBeam()
        {
            OnNewGameObjectCreated(NewSpotLightAndBeam());
        }

        [MenuItem("CONTEXT/Light/Attach a Volumetric Beam")]
        public static void Menu_AttachBeam(MenuCommand menuCommand)
        {
            var light = menuCommand.context as Light;
            if (light)
                light.gameObject.AddComponent<VolumetricLightBeam>();
        }

        [MenuItem("CONTEXT/VolumetricLightBeam/Documentation")]
        public static void Menu_Beam_Doc(MenuCommand menuCommand) { Application.OpenURL(Consts.HelpUrlBeam); }

        [MenuItem("CONTEXT/VolumetricDustParticles/Documentation")]
        public static void Menu_DustParticles_Doc(MenuCommand menuCommand) { Application.OpenURL(Consts.HelpUrlDustParticles); }

        [MenuItem("CONTEXT/DynamicOcclusion/Documentation")]
        public static void Menu_DynamicOcclusion_Doc(MenuCommand menuCommand) { Application.OpenURL(Consts.HelpUrlDynamicOcclusion); }

        [MenuItem("CONTEXT/TriggerZone/Documentation")]
        public static void Menu_TriggerZone_Doc(MenuCommand menuCommand) { Application.OpenURL(Consts.HelpUrlTriggerZone); }

        [MenuItem("CONTEXT/Config/Documentation")]
        public static void Menu_Config_Doc(MenuCommand menuCommand) { Application.OpenURL(Consts.HelpUrlConfig); }

        [MenuItem("CONTEXT/VolumetricLightBeam/Open Global Config")]
        [MenuItem("CONTEXT/VolumetricDustParticles/Open Global Config")]
        [MenuItem("CONTEXT/DynamicOcclusion/Open Global Config")]
        [MenuItem("CONTEXT/TriggerZone/Open Global Config")]
        public static void Menu_Beam_Config(MenuCommand menuCommand) { Config.EditorSelectInstance(); }

        [MenuItem("CONTEXT/VolumetricLightBeam/Add Dust Particles")]
        public static void Menu_AddDustParticles(MenuCommand menuCommand)
        {
            var vlb = menuCommand.context as VolumetricLightBeam;
            if (vlb)
                vlb.gameObject.AddComponent<VolumetricDustParticles>();
        }

        [MenuItem("CONTEXT/VolumetricLightBeam/Add Dynamic Occlusion")]
        public static void Menu_AddDynamicOcclusion(MenuCommand menuCommand)
        {
            var vlb = menuCommand.context as VolumetricLightBeam;
            if (vlb)
                vlb.gameObject.AddComponent<DynamicOcclusion>();
        }

        [MenuItem("CONTEXT/VolumetricLightBeam/Add Trigger Zone")]
        public static void Menu_AddTriggerZone(MenuCommand menuCommand)
        {
            var vlb = menuCommand.context as VolumetricLightBeam;
            if (vlb)
                vlb.gameObject.AddComponent<TriggerZone>();
        }

        public static void HorizontalLineSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        /// <summary>
        /// Add a EditorGUILayout.ToggleLeft which properly handles multi-object editing
        /// </summary>
        public static void ToggleLeft(this SerializedProperty prop, GUIContent label, params GUILayoutOption[] options)
        {
            ToggleLeft(prop, label, prop.boolValue, options);
        }

        public static void ToggleLeft(this SerializedProperty prop, GUIContent label, bool forcedValue, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
            var newValue = EditorGUILayout.ToggleLeft(label, forcedValue, options);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.boolValue = newValue;
        }

        /// <summary>
        /// Create a EditorGUILayout.Slider which properly handles multi-object editing
        /// We apply the 'convIn' conversion to the SerializedProperty value before exposing it as a Slider.
        /// We apply the 'convOut' conversion to the Slider value to get the SerializedProperty back.
        /// </summary>
        /// <param name="prop">The value the slider shows.</param>
        /// <param name="label">Label in front of the slider.</param>
        /// <param name="leftValue">The value at the left end of the slider.</param>
        /// <param name="rightValue">The value at the right end of the slider.</param>
        /// <param name="convIn">Conversion applied on the SerializedProperty to get the Slider value</param>
        /// <param name="convOut">Conversion applied on the Slider value to get the SerializedProperty</param>
        public static void FloatSlider(
            this SerializedProperty prop,
            GUIContent label,
            float leftValue, float rightValue,
            System.Func<float, float> convIn,
            System.Func<float, float> convOut,
            params GUILayoutOption[] options)
        {
            var floatValue = convIn(prop.floatValue);
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
                {
                    floatValue = EditorGUILayout.Slider(label, floatValue, leftValue, rightValue, options);
                }
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = convOut(floatValue);
        }

        public static void FloatSlider(
            this SerializedProperty prop,
            GUIContent label,
            float leftValue, float rightValue,
            params GUILayoutOption[] options)
        {
            var floatValue = prop.floatValue;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
                {
                    floatValue = EditorGUILayout.Slider(label, floatValue, leftValue, rightValue, options);
                }
                EditorGUI.showMixedValue = false;
            }
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = floatValue;
        }

        public static void ToggleFromLight(this SerializedProperty prop)
        {
            ToggleLeft(
                prop,
                new GUIContent("From Spot", "Get the value from the Light Spot"),
                GUILayout.MaxWidth(80.0f));
        }

        public static void ToggleUseGlobalNoise(this SerializedProperty prop)
        {
            ToggleLeft(
                prop,
                new GUIContent("Global", "Get the value from the Global 3D Noise"),
                GUILayout.MaxWidth(55.0f));
        }
    }
}
#endif