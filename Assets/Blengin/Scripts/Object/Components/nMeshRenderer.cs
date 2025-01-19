using UnityEngine;

namespace Ngin {
    public class nMeshRenderer : MonoBehaviour {
        public MeshRenderer meshRenderer;
        void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) {
                Debug.LogError("MeshRenderer component not found.");
            }
        }
    }
}