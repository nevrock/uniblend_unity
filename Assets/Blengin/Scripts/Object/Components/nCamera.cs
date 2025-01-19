using UnityEngine;

namespace Ngin {
    public class nCamera : MonoBehaviour {
        public Camera camera;
        void Awake() {
            camera = GetComponent<Camera>();
            if (camera == null) {
                Debug.LogError("Camera component not found.");
            }
        }
    }
}