using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ngin {
    [System.Serializable]
    public class nObject : IObject {

        public Transform _transform;
        public TransformData _transformData;
        public nWorldGameObject _nGameObject;

        private nObject _parent;
        private List<nObject> _children;


        public nObject(string lexiconName) {
            _data = Lexicon.FromResourcesLexicon(lexiconName);
            _name = _data.Get<string>("name");
            _children = new List<nObject>();
            _childrenNames = new List<string>();
        }
        public nObject(string name, Lexicon data, nObject parent = null) {
            _name = name;
            _data = data;
            _children = new List<nObject>();
            _childrenNames = new List<string>();
            if (parent != null) {
                _parent = parent;
                _parentName = parent._name;
            }
        }
        // editor asset loading methods:
        public void Setup() {
            _transformData = new TransformData();
            _transformData.LoadFromLexicon(_data.Get<Lexicon>("transform", new Lexicon()));

            SetupChildren(_data.Get<Lexicon>("children", new Lexicon()));
        }
        public void Build() {
            BuildGameObjects();
            SetupComponents();
            _transformData.Link(_transform);
            _transformData.Apply();

            foreach (var child in _children) {
                child.Build();
            }
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
            if (_transformData != null)
                _transformData.Apply();
        }

        // runtime refresh to get the nObject references back
        public void Refresh() {
            Log.Console("Refreshing: " + _name);
            Log.Console("Children count: " + _childrenNames.Count);

            _nGameObject = _gameObject.GetComponent<nWorldGameObject>();
            _children = new List<nObject>();
            _components = new List<nComponent>();

            if (_transform.parent != null) {
                nWorldGameObject parentNGameObject = _transform.parent.GetComponent<nWorldGameObject>();
                _parent = parentNGameObject.Object as nObject;
            }

            foreach (string childName in _childrenNames) {
                Transform childTransform = _transform.Find(childName);
                if (childTransform != null) {
                    nObject child = childTransform.GetComponent<nWorldGameObject>().Object as nObject;
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
        public nWorldGameObject NginGameObject {
            get {
            return _nGameObject;
            }
        }
        public Transform Transform {
            get {
            return _transform;
            }
        }
        public TransformData TransformData {
            get {
                return _transformData;
            }
        }

        public Vector3 Position {
            get {
                return _transformData.Position;
            }
            set {
                _transformData.Position = value;
            }
        }
        public Vector3 Scale {
            get {
                return _transformData.Scale;
            }
            set {
                _transformData.Scale = value;
            }
        }
        public Quaternion Rotation {
            get {
                return _transformData.Rotation;
            }
            set {
                _transformData.Rotation = value;
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
            foreach (nObject n in this._children)
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
        public nObject FindChild(string name, nObject parent = null) {
            if (parent == null) {
                parent = this;
            }
            foreach (nObject child in parent._children) {
                if (child._name == name) {
                    return child;
                }
            }
            foreach (nObject child in parent._children) {
                nObject result = FindChild(name, child);
                if (result != null) {
                    return result;
                }
            }
            return null;
        }

        public static IObject Spawn(string name, 
                                    Transform parent = null,
                                    bool isZeroOut = true) {
            // attempt to spawn from prefab
            GameObject prefab = Resources.Load<GameObject>("Object/" + name);
            if (prefab != null) {
                GameObject go = GameObject.Instantiate(prefab);
                Debug.Log("Spawned: " + go.name);
                IObject nObj = go.GetComponent<nWorldGameObject>().Object as IObject;
                Debug.Log("nObj: " + nObj);
                if (parent != null) {
                    go.transform.SetParent(parent);
                    if (isZeroOut) {
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                    }
                }
                return nObj;
            } else {
                Debug.LogError("Prefab not found: " + name);
            } 
            return null;
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
            _transform = _gameObject.transform;
            _transformData.Link(_transform);
            if (_parent != null) {
                _gameObject.transform.SetParent(_parent.Transform);
                _gameObject.transform.localPosition = Vector3.zero;
            }
            _nGameObject = _gameObject.AddComponent<nWorldGameObject>();
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
            _children =new List<nObject>();
            foreach (var child in children.Objects) {
                var childData = child.Value as Lexicon;
                var childName = child.Key;

                nObject nChild = new nObject(childName, childData, this);
                nChild.Setup();

                _children.Add(nChild);

                _childrenNames.Add(childName);
            }
        }
    }
}