using UnityEngine;

namespace Ngin {
    public class nAnimator : nComponent {
        public Animation animator;
        void Awake() {
            animator = GetComponent<Animation>();
            if (animator == null) {
                Debug.LogError("Camera component not found.");
            }
        }

        public Transform FindBone(string name) {
            return FindBoneRecursive(transform, name);
        }
        private Transform FindBoneRecursive(Transform parent, string name) {
            if (parent.name == name) {
                return parent;
            }
            foreach (Transform child in parent) {
                Transform result = FindBoneRecursive(child, name);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }
    }
}