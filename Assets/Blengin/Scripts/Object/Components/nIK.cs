namespace Ngin
{
    using UnityEngine;
    using System.Collections.Generic;

    [ExecuteAlways]
    public class nIK : nComponent
    {
        public List<GameObject> nodes;
        public GameObject effectorTarget;
        public GameObject effectorPole;
        public GameObject effectorRoot;
        
        public List<float> lengths;

        Lexicon args;
        public bool isEndRotate = true;
        public int iterations = 10;

        string _effectRootName;
        string _effectorPoleName;
        string _effectorRootName;
        List<string> _nodeNames;

        // root node wants to point to pole always
        protected override void AddClasses() {

        }
        protected override void StoreData(Lexicon data) {
            _effectorRootName = data.Get<string>("effectorRoot", "");
            _effectorPoleName = data.Get<string>("effectorPole", "");
            _effectRootName = data.Get<string>("effectorRoot", "");
            _nodeNames = data.Get<List<string>>("nodes", new List<string>());

            iterations = data.Get<int>("iterations", 10);
            isEndRotate = data.Get<bool>("isEndRotate", true);
        }
        protected override void Launch() {
            base.Launch();

            nodes = new List<GameObject>();
            foreach (var nodeName in _nodeNames) {
                nodes.Add(this.FindChildObject(nodeName));
            }
            effectorTarget = this.FindChildObject(_effectRootName);
            effectorPole = this.FindChildObject(_effectorPoleName);
            effectorRoot = this.FindChildObject(_effectorRootName);
        }

        protected override void Tick() {
            base.Tick();

            if (nodes.Count == 0 || effectorTarget == null || effectorPole == null || effectorRoot == null)
                return;

            SolveIk();
        }

        public void UpdateLengths(List<float> lengths)
        {
            this.lengths = lengths;
        }
        public void SolveIk()
        {
            Vector3 rootPoint = effectorRoot.transform.position;
            nodes[nodes.Count - 1].transform.up = -(effectorPole.transform.position - nodes[nodes.Count - 1].transform.position);
            
            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                nodes[i].transform.position = nodes[i + 1].transform.position + (-nodes[i + 1].transform.up * lengths[i + 1]);
                nodes[i].transform.up = -(effectorPole.transform.position - nodes[i].transform.position);
            }
            
            for (int i = 0; i < iterations; i++)
            {
                if (isEndRotate)
                    nodes[0].transform.up = -(effectorTarget.transform.position - nodes[0].transform.position);
                
                nodes[0].transform.position = effectorTarget.transform.position - (-nodes[0].transform.up * lengths[0]);
                
                for (int j = 1; j < nodes.Count; j++)
                {
                    nodes[j].transform.up = -(nodes[j - 1].transform.position - nodes[j].transform.position);
                    nodes[j].transform.position = nodes[j - 1].transform.position - (-nodes[j].transform.up * lengths[j]);
                }

                nodes[nodes.Count - 1].transform.position = rootPoint;
                for (int j = nodes.Count - 2; j >= 0; j--)
                {
                    nodes[j].transform.position = nodes[j + 1].transform.position + (-nodes[j + 1].transform.up * lengths[j + 1]);
                }
            }
        }
    }
}