namespace Ngin {
    using UnityEngine;
    using System.Collections.Generic;

    public class nPrefab {
        public static GameObject Spawn(string name, Transform parent = null) {
            GameObject obj = Resources.Load<GameObject>("Object/" + name);
            return Object.Instantiate(obj, parent);
        }
    }
}