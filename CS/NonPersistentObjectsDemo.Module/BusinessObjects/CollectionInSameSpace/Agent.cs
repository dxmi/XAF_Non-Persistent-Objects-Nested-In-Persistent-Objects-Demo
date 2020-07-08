using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DomainComponent]
    public class Agent : NonPersistentObjectImpl {
        public static int Sequence;
        public Agent() : base() { }
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [XmlIgnore]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(ref _Name, value); }
        }
        private double _Progress;
        public double Progress {
            get { return _Progress; }
            set { SetPropertyValue<double>(ref _Progress, value); }
        }
    }

    class NPAgentAdapter : NonPersistentObjectAdapter<Agent, int> {
        public NPAgentAdapter(NonPersistentObjectSpace npos) : base(npos) { }
        protected override Agent LoadSameObject(Agent obj) {
            Agent result = new Agent();
            result.ID = obj.ID;
            result.Name = obj.Name;
            result.Progress = obj.Progress;
            return result;
        }
        protected override bool ThrowOnAcceptingMismatchedObject => true;
    }

    /*
     * Also, see the overridden GetObjectSpaceToShowDetailViewFrom method
     * in the XafApplication (WinApplication / WebApplication) descendant.
     * 
     */
}
