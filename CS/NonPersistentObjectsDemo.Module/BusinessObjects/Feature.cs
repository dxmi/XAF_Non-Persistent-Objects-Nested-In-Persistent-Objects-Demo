using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [ListViewFilter("All", "", true)]
    [ListViewFilter("None", "1=0")]
    [ListViewFilter("Started", "Progress > 0")]
    [DomainComponent]
    public class Feature : NonPersistentObjectBase, IAssignable<Feature> {
        //public static int Sequence;
        public Feature() : base() { }
        [Browsable(false)]
        public int LocalKey { get; set; }
        [Browsable(false)]
        [XmlIgnore]
        public Guid OwnerKey { get; set; }
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [DevExpress.ExpressApp.Data.Key]
        public string ID { get { return string.Format("{0:N}:{1:x8}", OwnerKey, LocalKey); } }
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
        public void Assign(Feature source) {
            OwnerKey = source.OwnerKey;
            LocalKey = source.LocalKey;
            Name = source.Name;
            Progress = source.Progress;
    }
    }

    class NPFeatureAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<string, Feature> objectMap;

        public NPFeatureAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectMap = new Dictionary<string, Feature>();
        }
        void GuardKeyNotEmpty(Feature obj) {
            if(obj.OwnerKey == Guid.Empty)
                throw new InvalidOperationException(); // DEBUG
            if(obj.LocalKey == 0)
                throw new InvalidOperationException(); // DEBUG
        }
        private void AcceptObject(Feature obj) {
            Feature result;
            GuardKeyNotEmpty(obj);
            if(!objectMap.TryGetValue(obj.ID, out result)) {
                objectMap.Add(obj.ID, obj);
            }
            else {
                if(result != obj) {
                    throw new InvalidOperationException();
                }
            }
        }
        private Feature GetObject(string key) {
            Feature result;
            if(!objectMap.TryGetValue(key, out result)) {
                Guid ownerKey;
                int localKey;
                if(!TryParseKey(key, out ownerKey, out localKey)) {
                    throw new InvalidOperationException("Invalid key.");
                }
                var owner = GetOwnerByKey(ownerKey);
                if(owner == null) {
                    throw new InvalidOperationException("Owner object is not found in the storage.");
                }
                result = owner.Features.FirstOrDefault(o => o.LocalKey == localKey);
                if(result == null) {
                    return null;
                }
                AcceptObject(result);
            }
            return result;
        }
        private Project GetOwnerByKey(Guid key) {
            var ownerObjectSpace = objectSpace.Owner as CompositeObjectSpace;
            return (ownerObjectSpace ?? objectSpace).GetObjectByKey<Project>(key);
        }
        private bool TryParseKey(string key, out Guid ownerKey, out int localKey) {
            ownerKey = Guid.Empty;
            localKey = 0;
            if(string.IsNullOrEmpty(key)) {
                return false;
            }
            var parts = key.Split(':');
            if(parts.Length != 2) {
                return false;
            }
            if(!Guid.TryParse(parts[0], out ownerKey)) {
                return false;
            }
            if(!Int32.TryParse(parts[1], out localKey)) {
                return false;
            }
            return true;
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Feature)) {
                    e.Object = GetObject((string)e.Key);
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
                GuardKeyNotEmpty(obj);
                if(link.ObjectSpace == null) {
                    Feature result;
                    if(!objectMap.TryGetValue(obj.ID, out result)) {
                        objectMap.Add(obj.ID, obj);
                        e.TargetObject = obj;
                    }
                    else {
                        // if objectMap contains an object with the same key, assume SourceObject is a reloaded copy. (because link is null yet)
                        // then refresh contents of the found object and return it.
                        if(result != obj) {
                            result.Assign(obj);
                        }
                        e.TargetObject = result;
                    }
                }
                else {
                    if(link.ObjectSpace.IsNewObject(obj)) {
                        if(link.ObjectSpace == objectSpace) {
                            e.TargetObject = e.SourceObject;
                        }
                        else {
                            e.TargetObject = null;
                        }
                    }
                    else {
                        if(link.ObjectSpace == objectSpace) {
                            Feature result;
                            if(!objectMap.TryGetValue(obj.ID, out result)) {
                                objectMap.Add(obj.ID, obj);
                                e.TargetObject = obj;
                            }
                            else {
                                // if objectMap contains an object with the same key, assume SourceObject is an outdated copy.
                                // then return the cached object. (???)
                                if(result != obj) {
                                    result.Assign(obj);
                                }
                                e.TargetObject = result;
                            }
                        }
                        else {
                            e.TargetObject = GetObject(obj.ID);
                        }
                    }
                }
        }
        }
    }
}
