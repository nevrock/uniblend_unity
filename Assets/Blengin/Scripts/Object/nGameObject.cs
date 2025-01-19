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

        public nObject Object;
    }
}