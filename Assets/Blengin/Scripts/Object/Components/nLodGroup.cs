using UnityEngine;
using System.Collections.Generic;

namespace Ngin {
    [ExecuteAlways]
    public class nLodGroup : nComponent {
        public LODGroup lodGroup;
        public List<float> lodScreenPercentages; // Screen relative transition heights for each LOD level

        protected override void AddClasses() {
            lodGroup = ComponentCheck<LODGroup>(true);
        }

        protected override void StoreData(Lexicon data) {
            lodScreenPercentages = data.Get<List<float>>("lodScreenPercentages", new List<float>());
        }

        protected override void Launch() {
            LOD[] lods = new LOD[lodScreenPercentages.Count + 1]; // +1 for the empty mesh LOD
            for (int i = 0; i < lodScreenPercentages.Count; i++) {
                List<Renderer> renderers = new List<Renderer>();
                Transform lodTransform = transform.Find($"LOD{i}");
                if (lodTransform != null) {
                    foreach (Transform child in lodTransform) {
                        Renderer renderer = child.GetComponent<Renderer>();
                        if (renderer != null) {
                            renderers.Add(renderer);
                        }
                    }
                }
                lods[i] = new LOD(lodScreenPercentages[i], renderers.ToArray());
            }
            // Add an empty mesh as the last LOD level
            lods[lodScreenPercentages.Count] = new LOD(0.01f, new Renderer[0]);

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
    }
}
