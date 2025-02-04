using UnityEngine;
namespace Ngin {
    public class nComponent : MonoBehaviour {
        public void Link(IObject obj, Lexicon data) {
            Object = obj;
            StoreData(data);
        }
        public IObject Object;

        void Awake() {
            AddClasses();
            Setup();
        }
        void Start() {
            Launch();
        }
        void Update() {
            Tick();
        }
        void FixedUpdate() {
            TickPhysics();
        }

        public virtual void EditorRefresh() {
            AddClasses();
            Setup();
            Launch();
        }

        protected virtual void StoreData(Lexicon data) {}
        protected virtual void AddClasses() {}
        protected virtual void Setup() {}
        protected virtual void Launch() {}
        protected virtual void Tick() {}
        protected virtual void TickPhysics() {}

        public T ComponentCheck<T>(bool forceAdd = true) where T : Component
        {
            T val = this.gameObject.GetComponent<T>();
            if (val == null && forceAdd)
                return this.gameObject.AddComponent<T>();

            return val;
        }
        protected Transform FindChild(string name, Transform parent = null) {
            if (parent == null) {
                parent = this.transform;
            }
            if (parent.name == name) {
                return parent;
            }
            foreach (Transform child in parent) {
                Transform result = FindChild(name, child);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }
        protected nObject FindChildObject(string name, nObject parent = null) {
            nObject obj = this.Object as nObject;
            if (obj == null) {
                return null;
            }
            return obj.FindChild(name, parent);
        }
    }
}