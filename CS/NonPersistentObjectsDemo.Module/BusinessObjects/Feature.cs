using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [ListViewFilter("All", "", true)]
    [ListViewFilter("None", "1=0")]
    [ListViewFilter("Started", "Progress > 0")]
    [DomainComponent]
    public class Feature : NonPersistentObjectBaseWithKey {
        public static int Sequence;
        public Feature() : base() { }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
        private double _Progress;
        public double Progress {
            get { return _Progress; }
            set { SetPropertyValue<double>(nameof(Progress), ref _Progress, value); }
        }
    }

    class NPFeatureAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<int, Feature> objectMap;

        public NPFeatureAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectMap = new Dictionary<int, Feature>();
        }
        private void AcceptObject(Feature obj) {
            Feature result;
            if(!objectMap.TryGetValue(obj.ID, out result)) {
                ((IObjectSpaceLink)obj).ObjectSpace = objectSpace; // remove?
                objectMap.Add(obj.ID, obj);
            }
            else {
                if(result != obj) {
                    throw new InvalidOperationException();
                }
            }
        }
        private Feature GetObject(int key) {
            Feature result;
            if(!objectMap.TryGetValue(key, out result)) {
                //result = new Feature();
                //features.Add(key, result);
                throw new NotSupportedException();
            }
            return result;
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Feature)) {
                    e.Object = GetObject((int)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Feature)) {
                throw new NotSupportedException();
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            var link = e.SourceObject as IObjectSpaceLink;
            if(e.SourceObject is Feature) {
                var obj = (Feature)e.SourceObject;
                if(link.ObjectSpace == null || link.ObjectSpace == objectSpace) {
                    AcceptObject(obj);
                    return;
                }
                if(link.ObjectSpace.IsNewObject(obj)) { // implement in OS?
                    e.TargetObject = null;
                    return;
                }
                Feature result;
                if(!objectMap.TryGetValue(obj.ID, out result)) {
                    result = new Feature();
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
