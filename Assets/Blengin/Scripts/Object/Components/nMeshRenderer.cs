using UnityEngine;

namespace Ngin {
    public class nMeshRenderer : nComponent {
        [ReadOnly]
        public MeshRenderer meshRenderer;
        [ReadOnly]
        public MeshFilter meshFilter;
        public MeshData meshData;

        protected override void AddClasses() {
            meshRenderer = ComponentCheck<MeshRenderer>(true);
            meshFilter = ComponentCheck<MeshFilter>(true);
        }
        protected override void StoreData(Lexicon data) {
            meshData = new MeshData();
            meshData.LoadFromLexicon(data);
        }

        protected override void Launch() {
            Mesh mesh = meshData.GetMesh();
            //meshFilter.sharedMesh = mesh;

            Material[] materials = new Material[meshData.materials.Count];
            for (int i = 0; i < meshData.materials.Count; i++) {
                materials[i] = meshData.GetMaterial(i);
            }
            meshRenderer.sharedMaterials = materials;
        }


        // data
        
    }
}