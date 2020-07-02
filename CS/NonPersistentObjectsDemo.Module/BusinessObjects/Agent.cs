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

    class NPAgentAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<int, Agent> objectMap;

        public NPAgentAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectMap = new Dictionary<int, Agent>();
        }
        private void AcceptObject(Agent obj) {
            Agent result;
            if(!objectMap.TryGetValue(obj.ID, out result)) {
                objectMap.Add(obj.ID, obj);
            }
            else {
                if(result != obj) {
                    throw new InvalidOperationException();
                }
            }
        }
        private Agent GetObject(int key) {
            Agent result;
            if(!objectMap.TryGetValue(key, out result)) {
                throw new NotSupportedException();
            }
            return result;
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Agent)) {
                    e.Object = GetObject((int)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Agent)) {
                throw new NotSupportedException();
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            var link = e.SourceObject as IObjectSpaceLink;
            if(e.SourceObject is Agent) {
                var obj = (Agent)e.SourceObject;
                if(link.ObjectSpace == null || link.ObjectSpace == objectSpace) {
                    AcceptObject(obj);
                    return;
                }
                if(link.ObjectSpace.IsNewObject(obj)) { // implement in OS?
                    e.TargetObject = null;
                    return;
                }
                Agent result;
                if(!objectMap.TryGetValue(obj.ID, out result)) {
                    result = new Agent();
                    result.ID = obj.ID;
                    result.Name = obj.Name;
                    result.Progress = obj.Progress;
                    AcceptObject(result);
                }
                e.TargetObject = result;
            }
        }
    }
}
