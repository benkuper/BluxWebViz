#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0429, 0162 // Unreachable expression code detected (because of Noise3D.isSupported on mobile)

namespace VLB
{
    [CustomEditor(typeof(VolumetricLightBeam))]
    [CanEditMultipleObjects]
    public class VolumetricLightBeamEditor : EditorCommon
    {
        SerializedProperty trackChangesDuringPlaytime;
        SerializedProperty colorFromLight, colorMode, color, colorGradient;
        SerializedProperty alphaInside, alphaOutside, blendingMode;
        SerializedProperty fresnelPow, glareFrontal, glareBehind;
        SerializedProperty spotAngleFromLight, spotAngle;
        SerializedProperty coneRadiusStart, geomMeshType, geomCustomSides, geomCustomSegments, geomCap;
        SerializedProperty fadeEndFromLight, fadeStart, fadeEnd;
        SerializedProperty attenuationEquation, attenuationCustomBlending;
        SerializedProperty depthBlendDistance, cameraClippingDistance;
        SerializedProperty noiseEnabled, noiseIntensity, noiseScaleUseGlobal, noiseScaleLocal, noiseVelocityUseGlobal, noiseVelocityLocal;
        SerializedProperty sortingLayerID, sortingOrder;

        List<VolumetricLightBeam> m_Entities;
        string[] m_SortingLayerNames;
        static bool ms_ShowTips = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Entities = new List<VolumetricLightBeam>();
            foreach (var ent in targets)
            {
                if (ent is VolumetricLightBeam)
                    m_Entities.Add(ent as VolumetricLightBeam);
            }
            Debug.Assert(m_Entities.Count > 0);

            colorFromLight = FindProperty((VolumetricLightBeam x) => x.colorFromLight);
            color = FindProperty((VolumetricLightBeam x) => x.color);
            colorGradient = FindProperty((VolumetricLightBeam x) => x.colorGradient);
            colorMode = FindProperty((VolumetricLightBeam x) => x.colorMode);

            alphaInside = FindProperty((VolumetricLightBeam x) => x.alphaInside);
            alphaOutside = FindProperty((VolumetricLightBeam x) => x.alphaOutside);
            blendingMode = FindProperty((VolumetricLightBeam x) => x.blendingMode);

            fresnelPow = FindProperty((VolumetricLightBeam x) => x.fresnelPow);

            glareFrontal = FindProperty((VolumetricLightBeam x) => x.glareFrontal);
            glareBehind = FindProperty((VolumetricLightBeam x) => x.glareBehind);

            spotAngleFromLight = FindProperty((VolumetricLightBeam x) => x.spotAngleFromLight);
            spotAngle = FindProperty((VolumetricLightBeam x) => x.spotAngle);

            coneRadiusStart = FindProperty((VolumetricLightBeam x) => x.coneRadiusStart);

            geomMeshType = FindProperty((VolumetricLightBeam x) => x.geomMeshType);
            geomCustomSides = FindProperty((VolumetricLightBeam x) => x.geomCustomSides);
            geomCustomSegments = FindProperty((VolumetricLightBeam x) => x.geomCustomSegments);
            geomCap = FindProperty((VolumetricLightBeam x) => x.geomCap);

            fadeEndFromLight = FindProperty((VolumetricLightBeam x) => x.fadeEndFromLight);
            fadeStart = FindProperty((VolumetricLightBeam x) => x.fadeStart);
            fadeEnd = FindProperty((VolumetricLightBeam x) => x.fadeEnd);

            attenuationEquation = FindProperty((VolumetricLightBeam x) => x.attenuationEquation);
            attenuationCustomBlending = FindProperty((VolumetricLightBeam x) => x.attenuationCustomBlending);

            depthBlendDistance = FindProperty((VolumetricLightBeam x) => x.depthBlendDistance);
            cameraClippingDistance = FindProperty((VolumetricLightBeam x) => x.cameraClippingDistance);

            // NOISE
            noiseEnabled = FindProperty((VolumetricLightBeam x) => x.noiseEnabled);
            noiseIntensity = FindProperty((VolumetricLightBeam x) => x.noiseIntensity);
            noiseScaleUseGlobal = FindProperty((VolumetricLightBeam x) => x.noiseScaleUseGlobal);
            noiseScaleLocal = FindProperty((VolumetricLightBeam x) => x.noiseScaleLocal);
            noiseVelocityUseGlobal = FindProperty((VolumetricLightBeam x) => x.noiseVelocityUseGlobal);
            noiseVelocityLocal = FindProperty((VolumetricLightBeam x) => x.noiseVelocityLocal);

            trackChangesDuringPlaytime = serializedObject.FindProperty("_TrackChangesDuringPlaytime");

            // 2D
            sortingLayerID = serializedObject.FindProperty("_SortingLayerID");
            sortingOrder = serializedObject.FindProperty("_SortingOrder");
            m_SortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();
        }

        static void PropertyThickness(SerializedProperty sp)
        {
            sp.FloatSlider(
                EditorStrings.SideThickness,
                0, 1,
                (value) => Mathf.Clamp01(1 - (value / Consts.FresnelPowMaxValue)),    // conversion value to slider
                (value) => (1 - value) * Consts.FresnelPowMaxValue                    // conversion slider to value
                );
        }


        class FromLightComponentScope : System.IDisposable
        {
            SerializedProperty m_Property;
            bool m_HasLightSpot = false;

            void Enable()
            {
                if (m_HasLightSpot)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(m_Property.boolValue);
                }
            }

            void Disable()
            {
                if (m_HasLightSpot)
                {
                    EditorGUI.EndDisabledGroup();
                    m_Property.ToggleFromLight();
                    EditorGUILayout.EndHorizontal();
                }
            }

            public FromLightComponentScope(SerializedProperty prop, bool hasLightSpot)
            {
                m_Property = prop;
                m_HasLightSpot = hasLightSpot;
                Enable();
            }

            public void Dispose() { Disable(); }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Debug.Assert(m_Entities.Count > 0);

            bool hasLightSpot = false;
            var light = m_Entities[0].GetComponent<Light>();
            if (light)
            {
                hasLightSpot = light.type == LightType.Spot;
                if (!hasLightSpot)
                {
                    EditorGUILayout.HelpBox(EditorStrings.HelpNoSpotlight, MessageType.Warning);
                }
            }

            Header("Basic");
            {
                // Color
                using (new FromLightComponentScope(colorFromLight, hasLightSpot))
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(colorMode, EditorStrings.ColorMode);

                    if (colorMode.enumValueIndex == (int)ColorMode.Gradient)
                        EditorGUILayout.PropertyField(colorGradient, EditorStrings.ColorGradient);
                    else
                        EditorGUILayout.PropertyField(color, EditorStrings.ColorFlat);
                }

                EditorGUILayout.PropertyField(alphaInside, EditorStrings.AlphaInside);
                EditorGUILayout.PropertyField(alphaOutside, EditorStrings.AlphaOutside);
                EditorGUILayout.PropertyField(blendingMode, EditorStrings.BlendingMode);

                EditorGUILayout.Separator();

                // Spot Angle
                using (new FromLightComponentScope(spotAngleFromLight, hasLightSpot))
                {
                    EditorGUILayout.PropertyField(spotAngle, EditorStrings.SpotAngle);
                }

                PropertyThickness(fresnelPow);

                EditorGUILayout.Separator();

                EditorGUILayout.PropertyField(glareFrontal, EditorStrings.GlareFrontal);
                EditorGUILayout.PropertyField(glareBehind, EditorStrings.GlareBehind);

                EditorGUILayout.Separator();

                trackChangesDuringPlaytime.ToggleLeft(EditorStrings.TrackChanges);
                DrawAnimatorWarning();
            }

            Header("Attenuation");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(attenuationEquation, EditorStrings.AttenuationEquation);
                    if (attenuationEquation.enumValueIndex == (int)AttenuationEquation.Blend)
                        EditorGUILayout.PropertyField(attenuationCustomBlending, EditorStrings.AttenuationCustomBlending);
                }
                EditorGUILayout.EndHorizontal();

                // Fade End
                using (new FromLightComponentScope(fadeEndFromLight, hasLightSpot))
                {
                    EditorGUILayout.PropertyField(fadeEnd, EditorStrings.FadeEnd);
                }

                if (fadeEnd.hasMultipleDifferentValues)
                    EditorGUILayout.PropertyField(fadeStart, EditorStrings.FadeStart);
                else
                    fadeStart.FloatSlider(EditorStrings.FadeStart, 0f, fadeEnd.floatValue - Consts.FadeMinThreshold);
            }

            Header("3D Noise");
            {
                EditorGUILayout.PropertyField(noiseEnabled, EditorStrings.NoiseEnabled);

                if (noiseEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(noiseIntensity, EditorStrings.NoiseIntensity);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledGroupScope(noiseScaleUseGlobal.boolValue))
                        {
                            EditorGUILayout.PropertyField(noiseScaleLocal, EditorStrings.NoiseScale);
                        }
                        noiseScaleUseGlobal.ToggleUseGlobalNoise();
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledGroupScope(noiseVelocityUseGlobal.boolValue))
                        {
                            EditorGUILayout.PropertyField(noiseVelocityLocal, EditorStrings.NoiseVelocity);
                        }
                        noiseVelocityUseGlobal.ToggleUseGlobalNoise();
                    }

                    ButtonOpenConfig();

                    if (Noise3D.isSupported && !Noise3D.isProperlyLoaded)
                        EditorGUILayout.HelpBox(EditorStrings.HelpNoiseLoadingFailed, MessageType.Error);

                    if (!Noise3D.isSupported)
                        EditorGUILayout.HelpBox(Noise3D.isNotSupportedString, MessageType.Info);
                }
            }

            Header("Soft Intersections Blending Distances");
            {
                EditorGUILayout.PropertyField(cameraClippingDistance, EditorStrings.CameraClippingDistance);
                EditorGUILayout.PropertyField(depthBlendDistance, EditorStrings.DepthBlendDistance);
            }

            Header("Cone Geometry");
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(coneRadiusStart, EditorStrings.ConeRadiusStart);
                    EditorGUI.BeginChangeCheck();
                    {
                        geomCap.ToggleLeft(EditorStrings.GeomCap, GUILayout.MaxWidth(40.0f));
                    }
                    if (EditorGUI.EndChangeCheck()) { SetMeshesDirty(); }
                }

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(geomMeshType, EditorStrings.GeomMeshType);
                }
                if (EditorGUI.EndChangeCheck()) { SetMeshesDirty(); }

                if (geomMeshType.intValue == (int)MeshType.Custom)
                {
                    EditorGUI.indentLevel++;

                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(geomCustomSides, EditorStrings.GeomSides);
                        EditorGUILayout.PropertyField(geomCustomSegments, EditorStrings.GeomSegments);
                    }
                    if (EditorGUI.EndChangeCheck()) { SetMeshesDirty(); }

                    EditorGUI.indentLevel--;
                }

                if (m_Entities.Count == 1)
                {
                    EditorGUILayout.HelpBox(m_Entities[0].meshStats, MessageType.Info);
                }
            }

            Header("2D");
            {
                DrawSortingLayer();
                DrawSortingOrder();
            }

            EditorGUILayout.Separator();
            EditorExtensions.HorizontalLineSeparator();
            EditorGUILayout.Separator();

            DrawCustomActionButtons();
            DrawAdditionalFeatures();
            DrawInfos();

            serializedObject.ApplyModifiedProperties();
        }

        void SetMeshesDirty()
        {
            foreach (var entity in m_Entities) entity._EditorSetMeshDirty();
        }

        void DrawSortingLayer()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = sortingLayerID.hasMultipleDifferentValues;
            int layerIndex = System.Array.IndexOf(m_SortingLayerNames, SortingLayer.IDToName(sortingLayerID.intValue));
            layerIndex = EditorGUILayout.Popup(EditorStrings.SortingLayer, layerIndex, m_SortingLayerNames);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(m_Entities.ToArray(), "Edit Sorting Layer");
                sortingLayerID.intValue = SortingLayer.NameToID(m_SortingLayerNames[layerIndex]);
                foreach (var entity in m_Entities) { entity.sortingLayerID = sortingLayerID.intValue; } // call setters
            }
        }

        void DrawSortingOrder()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sortingOrder, EditorStrings.SortingOrder);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(m_Entities.ToArray(), "Edit Sorting Order");
                foreach (var entity in m_Entities) { entity.sortingOrder = sortingOrder.intValue; } // call setters
            }
        }

        bool HasAtLeastOneBeamWith(System.Func<VolumetricLightBeam, bool> lambda)
        {
            foreach (var entity in m_Entities)
            {
                if (lambda(entity))
                {
                    return true;
                }
            }
            return false;
        }

        void DrawAnimatorWarning()
        {
            var showAnimatorWarning = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<Animator>() != null && beam.trackChangesDuringPlaytime == false; });

            if (showAnimatorWarning)
                EditorGUILayout.HelpBox(EditorStrings.HelpAnimatorWarning, MessageType.Warning);
        }

        void DrawCustomActionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(EditorStrings.ButtonResetProperties, EditorStyles.miniButtonLeft))
                {
                    UnityEditor.Undo.RecordObjects(m_Entities.ToArray(), "Reset Light Beam Properties");
                    foreach (var entity in m_Entities) { entity.Reset(); entity.GenerateGeometry(); }
                }

                if (geomMeshType.intValue == (int)MeshType.Custom)
                {
                    if (GUILayout.Button(EditorStrings.ButtonGenerateGeometry, EditorStyles.miniButtonRight))
                    {
                        foreach (var entity in m_Entities) entity.GenerateGeometry();
                    }
                }
            }
        }

        void DrawAdditionalFeatures()
        {
#if UNITY_5_5_OR_NEWER
            bool showButtonDust         = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<VolumetricDustParticles>() == null; });
#else
            bool showButtonDust = false;
#endif
            bool showButtonOcclusion    = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<DynamicOcclusion>() == null; });
            bool showButtonTriggerZone  = HasAtLeastOneBeamWith((VolumetricLightBeam beam) => { return beam.GetComponent<TriggerZone>() == null; });

            if (showButtonDust || showButtonOcclusion || showButtonTriggerZone)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (showButtonDust && GUILayout.Button(EditorStrings.ButtonAddDustParticles, EditorStyles.miniButton))
                    {
                        Undo.RecordObjects(m_Entities.ToArray(), "Add Floating Dust Particles");
                        foreach (var entity in m_Entities) { entity.gameObject.AddComponent<VolumetricDustParticles>(); }
                    }

                    if (showButtonOcclusion && GUILayout.Button(EditorStrings.ButtonAddDynamicOcclusion, EditorStyles.miniButton))
                    {
                        Undo.RecordObjects(m_Entities.ToArray(), "Add Dynamic Occlusion");
                        foreach (var entity in m_Entities) { entity.gameObject.AddComponent<DynamicOcclusion>(); }
                    }

                    if (showButtonTriggerZone && GUILayout.Button(EditorStrings.ButtonAddTriggerZone, EditorStyles.miniButton))
                    {
                        Undo.RecordObjects(m_Entities.ToArray(), "Add Trigger Zone");
                        foreach (var entity in m_Entities) { entity.gameObject.AddComponent<TriggerZone>(); }
                    }
                }
            }
        }

        void DrawInfos()
        {
            var tips = GetInfoTips();
            var gpuInstancingReport = GetGpuInstancingReport();

            if (tips.Count > 0 || !string.IsNullOrEmpty(gpuInstancingReport))
            {
#if UNITY_5_5_OR_NEWER
                ms_ShowTips = EditorGUILayout.Foldout(ms_ShowTips, "Infos", toggleOnLabelClick: true);
#else
            ms_ShowTips = EditorGUILayout.Foldout(ms_ShowTips, "Infos");
#endif
                if (ms_ShowTips)
                {
                    foreach (var tip in tips)
                        EditorGUILayout.HelpBox(tip, MessageType.Info);

                    if (!string.IsNullOrEmpty(gpuInstancingReport))
                        EditorGUILayout.HelpBox(gpuInstancingReport, MessageType.Warning);
                }
            }
        }

        List<string> GetInfoTips()
        {
            var tips = new List<string>();
            if (m_Entities.Count == 1)
            {
                if (depthBlendDistance.floatValue > 0f || !Noise3D.isSupported || trackChangesDuringPlaytime.boolValue)
                {
                    if (depthBlendDistance.floatValue > 0f)
                    {
                        tips.Add(EditorStrings.HelpDepthTextureMode);
#if UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID
                        tips.Add(EditorStrings.HelpDepthMobile);
#endif
                    }

                    if (trackChangesDuringPlaytime.boolValue)
                        tips.Add(EditorStrings.HelpTrackChangesEnabled);
                }
            }
            return tips;
        }

        string GetGpuInstancingReport()
        {
            if (m_Entities.Count > 1 && GpuInstancing.isSupported)
            {
                string reasons = "";
                for (int i = 1; i < m_Entities.Count; ++i)
                {
                    if (!GpuInstancing.CanBeBatched(m_Entities[0], m_Entities[i], ref reasons))
                    {
                        return "Selected beams can't be batched together:\n" + reasons;
                    }
                }
            }
            return null;
        }
    }
}
#endif
