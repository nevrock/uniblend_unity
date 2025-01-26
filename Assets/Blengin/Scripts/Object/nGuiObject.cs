using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ngin {
    [System.Serializable]
    public class nGuiObject : IObject {

        public RectTransform _rectTransform;
        public RectData _rectData;
        public nGuiGameObject _nGameObject;

        private nGuiObject _parent;
        private List<nGuiObject> _children;

        public nGuiObject(string lexiconName) {
            _data = Lexicon.FromResourcesLexicon(lexiconName);
            _name = _data.Get<string>("name");
            _children = new List<nGuiObject>();
            _childrenNames = new List<string>();
        }
        public nGuiObject(string name, Lexicon data, nGuiObject parent = null) {
            _name = name;
            _data = data;
            _children = new List<nGuiObject>();
            _childrenNames = new List<string>();
            if (parent != null) {
                _parent = parent;
                _parentName = parent._name;
            }
        }
        // editor asset loading methods:
        public void Setup() {
            _rectData = new RectData();
            _rectData.LoadFromLexicon(_data.Get<Lexicon>("rect", new Lexicon()));

            SetupChildren(_data.Get<Lexicon>("children", new Lexicon()));
        }
        public void Build() {
            BuildGameObjects();
            SetupComponents();
            foreach (var child in _children) {
                child.Build();
            }
            _rectData.Link(_rectTransform);

            _rectData.Apply();
        }
        public void EditorRefresh() {
            foreach (var component in _components) {
                component.EditorRefresh();
            }
            foreach (var child in _children) {
                child.EditorRefresh();
            }
        }
        public void Update() {
            if (_rectData != null)
                _rectData.Apply();
        }

        // runtime refresh to get the nGuiObject references back
        public void Refresh() {
            Log.Console("Refreshing: " + _name);
            Log.Console("Children count: " + _childrenNames.Count);

            _nGameObject = _gameObject.GetComponent<nGuiGameObject>();
            _children = new List<nGuiObject>();
            _components = new List<nComponent>();

            if (_rectTransform.parent != null) {
                nGuiGameObject parentNGameObject = _rectTransform.parent.GetComponent<nGuiGameObject>();
                _parent = parentNGameObject.Object as nGuiObject;
            }

            foreach (string childName in _childrenNames) {
                RectTransform childTransform = _rectTransform.Find(childName) as RectTransform;
                if (childTransform != null) {
                    nGuiObject child = childTransform.GetComponent<nGuiGameObject>().Object as nGuiObject;
                    if (child != null)
                        _children.Add(child);
                }
            }

            foreach (var component in _gameObject.GetComponents<nComponent>()) {
                _components.Add(component);
            }
        }

        public GameObject GameObject {
            get {
            return _gameObject;
            }
        }
        public List<GameObject> GetAllGameObjects(List<GameObject> currentList = null) {
            if (currentList == null)
                currentList = new List<GameObject>();
            currentList.Add(_gameObject);
            foreach (var child in _children) {
                child.GetAllGameObjects(currentList);
            }
            return currentList;
        }
        public nGuiGameObject NginGameObject {
            get {
            return _nGameObject;
            }
        }
        public RectTransform RectTransform {
            get {
            return _rectTransform;
            }
        }
        public RectData RectData {
            get {
                return _rectData;
            }
        }

        public Vector2 Position {
            get {
                return _rectData.Position;
            }
            set {
                _rectData.Position = value;
            }
        }
        public Vector2 Size {
            get {
                return _rectData.Size;
            }
            set {
                _rectData.Size = value;
            }
        }
        public Vector2 AnchorMin {
            get {
                return _rectData.AnchorMin;
            }
            set {
                _rectData.AnchorMin = value;
            }
        }
        public Vector2 AnchorMax {
            get {
                return _rectData.AnchorMax;
            }
            set {
                _rectData.AnchorMax = value;
            }
        }
        public Vector2 Pivot {
            get {
                return _rectData.Pivot;
            }
            set {
                _rectData.Pivot = value;
            }
        }
        
        public void LogConsole() {
            Log.Console("Object: " + _name + ", has children: " + _children.Count.ToString());
            foreach (var child in _children) {
                child.LogConsole();
            }
        }

        public Lexicon Env {
            get {
                return _env;
            }
        }
        public T GetEnv<T>(string name, T defaultVal = default(T)) {
            return _env.Get<T>(name, defaultVal);
        }
        public void SetEnv<T>(string name, T value) {
            _env.Set(name, value);
        }

        public T GetComponent<T>(bool forceAdd = false) where T : nComponent {
            foreach (nComponent comp in this._components)
            {
                if (comp is T val)
                {
                    return val;
                }
            }
            if (forceAdd)
            {
                return AddComponent<T>();
            }
            return default(T);
        }
        public List<T> GetComponents<T>() where T : nComponent {
            List<T> bagOut = new List<T>();

            foreach (nComponent comp in this._components)
            {
                if (comp is T val)
                {
                    bagOut.Add(val);
                }
            }

            return bagOut;
        }
        public List<T> GetChildComponents<T>(bool isLimitedToFirstLevel = false) where T : nComponent {
            List<T> bagOut = GetComponents<T>();
            foreach (nGuiObject n in this._children)
            {
                if (n != null)
                {
                    if (isLimitedToFirstLevel)
                    {
                        bagOut.AddRange(n.GetComponents<T>());
                    } else
                    {
                        bagOut.AddRange(n.GetChildComponents<T>());
                    }
                }
            }
            return bagOut;
        }
        public T AddComponent<T>(Lexicon vars = null) where T : nComponent {
            Type classType = typeof(T);

            object o = null;

            Component component = _gameObject.GetComponent(classType);
            if (component == null)
                component = _gameObject.AddComponent(classType);

            o = component as object;

            nComponent a = o as nComponent;

            if (vars == null)
                vars = new Lexicon();
                
            a.Link(this, vars);

            this._components.Add(a);

            return (T)o;
        }
        public void AddComponent(string componentTypeName, Lexicon vars = null) {
            Debug.Log("Adding component: " + componentTypeName);
            
            Type classType = Utility.GetNginType(componentTypeName);

            object o = null;

            Component component = _gameObject.GetComponent(classType);
            if (component == null)
                component = _gameObject.AddComponent(classType);

            o = component as object;

            nComponent a = o as nComponent;

            if (vars == null)
                vars = new Lexicon();
                
            a.Link(this, vars);

            this._components.Add(a);
        }

        public static IObject Spawn(string name, 
                                    RectTransform parent = null,
                                    bool isZeroOut = true) {
            // attempt to spawn from prefab
            GameObject prefab = Resources.Load<GameObject>("Object/" + name);
            if (prefab != null) {
                GameObject go = GameObject.Instantiate(prefab);
                IObject nObj = go.GetComponent<nGuiGameObject>().Object as IObject;
                return nObj;
            } return null;
        } 
        public string Name {
            get {
                return _name;
            }
        }
        public string GetName() {
            return _name;
        }

        void BuildGameObjects() {
            _gameObject = new GameObject(_name);
            _rectTransform = _gameObject.AddComponent<RectTransform>();
            _rectData.Link(_rectTransform);
            if (_parent != null) {
                _gameObject.transform.SetParent(_parent.RectTransform);
                _gameObject.transform.localPosition = Vector3.zero;
            }
            _nGameObject = _gameObject.AddComponent<nGuiGameObject>();
            _nGameObject.Link(this);
        }
        void SetupComponents() {
            _components = new List<nComponent>();
            var components = _data.Get<Lexicon>("components", null);
            if (components == null)
                return;
            foreach (var component in components.Objects) {
                var componentData = component.Value as Lexicon;
                var componentName = component.Key;
                string type = componentData.Get<string>("type");
                AddComponent(type, componentData);
            }
        }
        void SetupChildren(Lexicon children) {
            _children = new List<nGuiObject>();
            foreach (var child in children.Objects) {
                var childData = child.Value as Lexicon;
                var childName = child.Key;

                nGuiObject nChild = new nGuiObject(childName, childData, this);
                nChild.Setup();

                _children.Add(nChild);

                _childrenNames.Add(childName);
            }
        }
    }
}
