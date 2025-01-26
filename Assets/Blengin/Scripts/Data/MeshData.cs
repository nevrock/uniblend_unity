namespace Ngin {
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    public class MeshData : nData {

        public string name;
        public List<string> materials;  
        public List<string> bones;

        public string quality;


        public MeshData() {
            name = "";
            materials = new List<string>();
        }

        public override void LoadFromLexicon(Lexicon lexicon) {
            name = lexicon.Get<string>("mesh", "");
            materials = lexicon.Get<List<string>>("materials", new List<string>());
            bones = lexicon.Get<List<string>>("bones", new List<string>());
            quality = lexicon.Get<string>("quality", "1Bone");
        }

        public MeshData(string name, List<string> materials = null) {
            this.name = name;
            materials = new List<string>();
            if (materials != null) {
                this.materials = materials;
            }
        }
        public Material GetMaterial(int index) {
            return Assets.GetMaterial(materials[index]);
        }
        public Mesh GetMesh() {
            return Assets.GetMesh(name);
        }
    }
}