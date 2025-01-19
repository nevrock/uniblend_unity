using UnityEngine;
namespace Ngin {
    public class nComponent : MonoBehaviour {
        public void Link(nObject obj, Lexicon data) {
            _nObject = obj;
            _data = data;
        }
        public nObject NginObject {
            get { return _nObject; }
        }
        nObject _nObject;
        Lexicon _data;
    }
}