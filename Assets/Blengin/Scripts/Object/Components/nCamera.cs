using UnityEngine;

namespace Ngin {
    public class nCamera : nComponent {
        public Camera camera;
        void Awake() {
            camera = GetComponent<Camera>();
            if (camera == null) {
                Debug.LogError("Camera component not found.");
            }
        }
    }
}