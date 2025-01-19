using UnityEngine;

namespace Ngin {
    [ExecuteAlways]
    public class nSkinnedMeshRenderer : MonoBehaviour {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        void Start() {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

            // find armature
            Transform armature = transform.parent.Find("Armature");

            // Setup bones
            Transform[] bones = new Transform[1];
            bones[0] = armature.Find("Cube");
            skinnedMeshRenderer.bones = bones;
        }
    }
}