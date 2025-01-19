using UnityEngine;

namespace Ngin {
    public class nLightPoint : MonoBehaviour {
        public Light light;
        void Awake() {
            light = GetComponent<Light>();
            if (light == null) {
                Debug.LogError("Light component not found.");
            }
        }
    }
}
