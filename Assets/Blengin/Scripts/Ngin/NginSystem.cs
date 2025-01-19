namespace Ngin {
    using UnityEngine;
    public class NginSystem : MonoBehaviour {
        public string startObject = "scene/start";
        void Awake() {
            _origin = nObject.Spawn(startObject);
        }
        void Start() {
            _origin.Debug();
        }

        nObject _origin;
    }
}