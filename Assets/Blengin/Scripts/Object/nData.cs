using UnityEngine;
using System.Reflection;

namespace Ngin {
    public struct nData : IData {
        
        public nData(Lexicon lexicon) {
            _data = lexicon;
        }
        public void LoadFromLexicon(Lexicon lexicon) {
            SetFieldsFromLexicon(lexicon);
        }

        private Lexicon _data;
        void SetFieldsFromLexicon(Lexicon lexicon) {
            foreach (var kvp in lexicon.Objects) {
                FieldInfo field = this.GetType().GetField(kvp.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && field.FieldType == kvp.Value.GetType()) {
                    field.SetValue(this, kvp.Value);
                }
            }
        }
    }
}
