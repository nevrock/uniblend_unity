namespace Ngin {
    using UnityEngine;
    using System.Collections.Generic;
    [System.Serializable]
    public class IObject {
        public string _name;
        public GameObject _gameObject;
        public List<string> _childrenNames;
        public string _parentName;

        protected Lexicon _data;
        protected Lexicon _env;
        protected List<nComponent> _components;

        public string GetName() {
            return _name;
        }
        public void Setup() {}
        public void Build() {}
        public void Refresh() {}
        public void Update() {}
    }
}