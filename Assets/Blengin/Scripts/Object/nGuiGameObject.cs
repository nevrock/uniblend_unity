using UnityEngine;
namespace Ngin {
    public class nGuiGameObject : MonoBehaviour {
        
        public void Link(nGuiObject obj) {
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

        public nGuiObject Object;
    }
}