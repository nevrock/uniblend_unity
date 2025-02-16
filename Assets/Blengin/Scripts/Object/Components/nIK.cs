namespace Ngin
{
    using UnityEngine;
    using System.Collections.Generic;

    [ExecuteAlways]
    public class nIK : nComponent
    {
        public int iterations = 10;
        public string target;
        public string poleTarget;
        public int chainLength;
        public float poleAngle;
        public float weightPosition;
        public float weightRotation;

        public bool isLinked = false;

        nIKController _controller;
        GameObject _effectorTarget;
        GameObject _effectorPole;
        GameObject _effectorRoot;
        List<GameObject> _nodes;
        List<float> _lengths;


        // root node wants to point to pole always
        protected override void AddClasses() {
        }
        protected override void StoreData(Lexicon data) {
            target = data.Get<string>("target", "");
            poleTarget = data.Get<string>("poleTarget", "");
            chainLength = data.Get<int>("chainLength", 0);
            poleAngle = data.Get<float>("poleAngle", 0);
            weightPosition = data.Get<float>("weightPosition", 0);
            weightRotation = data.Get<float>("weightRotation", 0);
            iterations = data.Get<int>("iterations", 10);
        }
        protected override void Launch() {
            base.Launch();
        }

        public void Link(nIKController controller, 
                        GameObject effectorTarget, 
                        GameObject effectorPole) {
            _controller = controller;
            _effectorTarget = effectorTarget;
            _effectorPole = effectorPole;

            _SetupChain();

            isLinked = true;
        }
        void _SetupChain() {
            _nodes = new List<GameObject>();
            _lengths = new List<float>();

            GameObject curr = this.gameObject;
            for (int i = 0; i < chainLength; i++) {
                Vector3 parentPosition = curr.transform.parent.position;
                Vector3 childPosition = curr.transform.position;

                _nodes.Add(curr.transform.parent.gameObject);
                _lengths.Add(Vector3.Distance(parentPosition, childPosition));

                curr = curr.transform.parent.gameObject;
            }
        }
        public string TargetName {
            get {
                return target;
            }
        }
        public string PoleName {
            get {
                return poleTarget;
            }
        }

        protected override void Tick() {
            base.Tick();

            if (_nodes.Count == 0 || _effectorTarget == null || _effectorPole == null || _effectorRoot == null)
                return;

            SolveIk();
        }
        public void SolveIk()
        {
            Vector3 rootPoint = _effectorRoot.transform.position;
            _nodes[_nodes.Count - 1].transform.up = -(_effectorPole.transform.position - _nodes[_nodes.Count - 1].transform.position);
            
            for (int i = _nodes.Count - 2; i >= 0; i--)
            {
                _nodes[i].transform.position = _nodes[i + 1].transform.position + (-_nodes[i + 1].transform.up * _lengths[i + 1]);
                _nodes[i].transform.up = -(_effectorPole.transform.position - _nodes[i].transform.position);
            }
            
            for (int i = 0; i < iterations; i++)
            {
                if (weightRotation > 0.1f)
                    _nodes[0].transform.up = -(_effectorTarget.transform.position - _nodes[0].transform.position);
                
                _nodes[0].transform.position = _effectorTarget.transform.position - (-_nodes[0].transform.up * _lengths[0]);
                
                for (int j = 1; j < _nodes.Count; j++)
                {
                    _nodes[j].transform.up = -(_nodes[j - 1].transform.position - _nodes[j].transform.position);
                    _nodes[j].transform.position = _nodes[j - 1].transform.position - (-_nodes[j].transform.up * _lengths[j]);
                }

                _nodes[_nodes.Count - 1].transform.position = rootPoint;
                for (int j = _nodes.Count - 2; j >= 0; j--)
                {
                    _nodes[j].transform.position = _nodes[j + 1].transform.position + (-_nodes[j + 1].transform.up * _lengths[j + 1]);
                }
            }
        }
    }
}