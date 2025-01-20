namespace Ngin {
    using UnityEngine;
    public class NginSystem : MonoBehaviour {
        public string startObject = "Start";
        void Awake() {
            _origin = nObject.Spawn(startObject);
        }
        void Start() {
            _origin.LogConsole();
        }

        nObject _origin;
    }
}