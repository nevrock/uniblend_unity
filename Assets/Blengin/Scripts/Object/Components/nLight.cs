using UnityEngine;

namespace Ngin {
    public class nLight : MonoBehaviour {
        public Light light;
        void Awake() {
            light = GetComponent<Light>();
            if (light == null) {
                Debug.LogError("Light component not found.");
            }
        }
    }
}
