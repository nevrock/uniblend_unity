namespace Ngin {
    using UnityEngine;
    using System.Collections.Generic;
    public class nIKController : nComponent {

        protected override void AddClasses() {

        }
        protected override void StoreData(Lexicon data) {
            
        }
        protected override void Launch() {
            base.Launch();

            List<nIK> iks = this.GetComponentsInChildren<nIK>();
            foreach (var ik in iks) {
                ik.Link(
                    controller: this,
                    effectorTarget: this.FindChildObject(ik.TargetName),
                    effectorPole: this.FindChildObject(ik.PoleName)
                );
            }
        }
    }
}