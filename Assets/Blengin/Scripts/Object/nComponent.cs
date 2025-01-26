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
    }
}