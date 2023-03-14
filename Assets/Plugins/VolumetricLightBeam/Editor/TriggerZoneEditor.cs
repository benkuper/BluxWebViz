#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VLB
{
    [CustomEditor(typeof(TriggerZone))]
    [CanEditMultipleObjects]
    public class TriggerZoneEditor : EditorCommon
    {
        SerializedProperty setIsTrigger, rangeMultiplier;
        List<VolumetricLightBeam> m_Beams;
        static bool ms_ShowTips = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            setIsTrigger = FindProperty((TriggerZone x) => x.setIsTrigger);
            rangeMultiplier = FindProperty((TriggerZone x) => x.rangeMultiplier);

            m_Beams = new List<VolumetricLightBeam>();
            foreach (var trigger in targets)
            {
                if (trigger is TriggerZone)
                {
                    var beam = (trigger as TriggerZone).GetComponent<VolumetricLightBeam>();
                    if(beam)
                        m_Beams.Add(beam);
                }
            }
            Debug.Assert(m_Beams.Count > 0);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(setIsTrigger, new GUIContent("Set Is Trigger", "Define if the Collider will be created as a convex trigger (not physical, most common behavior) or as a regular collider (physical)."));
            EditorGUILayout.PropertyField(rangeMultiplier, new GUIContent("Range Multiplier", "Change the length of the Collider.\nFor example, set 2.0 to make the Collider 2x longer than the beam."));

            ms_ShowTips = EditorGUILayout.Foldout(ms_ShowTips, "Infos");
            if (ms_ShowTips)
            {
                EditorGUILayout.HelpBox("Generate a Collider with the same shape than the beam", MessageType.Info);

                foreach (var beam in m_Beams)
                {
                    if (beam.trackChangesDuringPlaytime)
                    {
                        EditorGUILayout.HelpBox("The TriggerZone collider cannot be changed in realtime.\nIf you animate a property which change the shape of the beam, the collider shape won't fit anymore.", MessageType.Warning);
                        break;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
