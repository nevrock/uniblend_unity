using UnityEngine;
namespace Ngin {
    public class nGameObject : MonoBehaviour {
        public void Link(nObject obj) {
            Object = obj;
        }

        void Awake() {
            if (Application.isPlaying) {
                if (Object != null) {
                    Object.Refresh();
                }
            }
        }
        void Update() {
            if (Application.isPlaying && Object != null) {
                Object.Update();
            }
        }

        public nObject Object;
    }
}