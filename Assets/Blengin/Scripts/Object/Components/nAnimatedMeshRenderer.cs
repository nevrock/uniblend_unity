using UnityEngine;
using System.Collections.Generic;

namespace Ngin {
    [ExecuteAlways]
    public class nAnimatedMeshRenderer : nComponent {
        [ReadOnly]
        public SkinnedMeshRenderer skinnedMeshRenderer;
        [ReadOnly]
        public MeshFilter meshFilter;
        public MeshData meshData;

        protected override void AddClasses() {
            skinnedMeshRenderer = ComponentCheck<SkinnedMeshRenderer>(true);
            meshFilter = ComponentCheck<MeshFilter>(true);
        }
        protected override void StoreData(Lexicon data) {
            meshData = new MeshData();
            meshData.LoadFromLexicon(data);
        }

        protected override void Launch() {
            nAnimator armature = FindArmature();
            List<string> bones = meshData.bones;
            Debug.Log("Animated mesh has bones: " + bones.Count);
            Transform[] boneTransforms = new Transform[bones.Count];
            Matrix4x4[] bindPoses = new Matrix4x4[bones.Count];

            for (int i = 0; i < bones.Count; i++) {
                boneTransforms[i] = FindBone(bones[i], armature);
                bindPoses[i] = boneTransforms[i].worldToLocalMatrix * transform.localToWorldMatrix;
            }

            Mesh mesh = meshData.GetMesh();
            mesh.bindposes = bindPoses;
            meshFilter.sharedMesh = mesh;

            skinnedMeshRenderer.bones = boneTransforms;

            mesh.RecalculateBounds();
            skinnedMeshRenderer.localBounds = mesh.bounds;

            if (meshData.quality == "4Bones") {
                skinnedMeshRenderer.quality = SkinQuality.Bone4;
            } else if (meshData.quality == "2Bones") {
                skinnedMeshRenderer.quality = SkinQuality.Bone2;
            } else if (meshData.quality == "1Bone") {
                skinnedMeshRenderer.quality = SkinQuality.Bone1;
            }

            Material[] materials = new Material[meshData.materials.Count];
            for (int i = 0; i < meshData.materials.Count; i++) {
                materials[i] = meshData.GetMaterial(i);
            }
            skinnedMeshRenderer.sharedMaterials = materials;

            skinnedMeshRenderer.rootBone = armature.transform;

            skinnedMeshRenderer.sharedMesh = mesh;
        }

        nAnimator FindArmature() {
            Transform armature = transform.parent.Find("Armature");
            if (armature == null) {
                Debug.LogError("Armature not found.");
            }
            nAnimator animator = armature.GetComponent<nAnimator>();
            if (animator == null) {
                Debug.LogError("Armature not found.");
            }
            return animator;
        }
        Transform FindBone(string name, nAnimator armature) {
            if (armature == null) {
                return null;
            }
            Transform bone = armature.FindBone(name);
            if (bone == null) {
                Debug.LogError("Bone not found: " + name);
            }
            return bone;
        }
    }
}