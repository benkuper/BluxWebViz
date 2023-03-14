using UnityEngine;
using UnityEngine.UI;

namespace VLB_Samples
{
    [RequireComponent(typeof(Text))]
    public class FeaturesNotSupportedMessage : MonoBehaviour
    {
        void Start()
        {
            var textUI = GetComponent<Text>();
            Debug.Assert(textUI);
            textUI.text = VLB.Noise3D.isSupported ? "" : VLB.Noise3D.isNotSupportedString;
        }
    }
}
