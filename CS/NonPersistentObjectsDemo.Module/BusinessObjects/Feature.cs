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
    public class Feature : NonPersistentObjectBase {
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
        private void AcceptObject(Feature obj) {
            Feature result;
            if(obj.OwnerKey == Guid.Empty) throw new InvalidOperationException(); // DEBUG
            if(!objectMap.TryGetValue(obj.ID, out result)) {
                //((IObjectSpaceLink)obj).ObjectSpace = objectSpace; // remove?
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
                    throw new InvalidOperationException("Object is not found in the storage.");
                }
                var owner = objectSpace.GetObjectByKey<Project>(ownerKey);
                if(owner == null) {
                    throw new InvalidOperationException("Object is not found in the storage.");
                }
                result = owner.Features.FirstOrDefault(o => o.LocalKey == localKey);
                if(result == null) {
                    throw new InvalidOperationException("Object is not found in the storage.");
                }
                AcceptObject(result);
                throw new NotSupportedException();
            }
            return result;
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
                if(link.ObjectSpace == null) {
                    //AcceptObject(obj);
                    Feature result;
                    if(obj.OwnerKey == Guid.Empty) throw new InvalidOperationException(); // DEBUG
                    if(!objectMap.TryGetValue(obj.ID, out result)) {
                        objectMap.Add(obj.ID, obj);
                        e.TargetObject = obj;
                    }
                    else {
                        // if objectMap contains an object with the same key, assume SourceObject is a reloaded copy. (because link is null yet)
                        // then refresh contents of the found object and return it.
                        if(result != obj) {
                            Copy(obj, result);
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
                            //AcceptObject(obj);
                            Feature result;
                            if(obj.OwnerKey == Guid.Empty)
                                throw new InvalidOperationException(); // DEBUG
                            if(!objectMap.TryGetValue(obj.ID, out result)) {
                                objectMap.Add(obj.ID, obj);
                                e.TargetObject = obj;
                            }
                            else {
                                // if objectMap contains an object with the same key, assume SourceObject is an outdated copy.
                                // then return the cached object. (???)
                                if(result != obj) {
                                    Copy(obj, result);
                                }
                                e.TargetObject = result;
                            }
                        }
                        else {
                            e.TargetObject = GetObject(obj.ID);
                        }
                    }
                }
                //if(link.ObjectSpace == null || link.ObjectSpace == objectSpace) {
                //    AcceptObject(obj);
                //    return;
                //}
                //if(link.ObjectSpace.IsNewObject(obj)) { // implement in OS?
                //    e.TargetObject = null;
                //    return;
                //}
                //e.TargetObject = GetObject(obj.ID);
                //Feature result;
                //if(!objectMap.TryGetValue(obj.ID, out result)) {
                //    result = new Feature();
                //    Copy(obj, result);
                //    AcceptObject(result);
                //}
                //e.TargetObject = result;
            }
        }
        private void Copy(Feature source, Feature target) {
            target.OwnerKey = source.OwnerKey;
            target.LocalKey = source.LocalKey;
            target.Name = source.Name;
            target.Progress = source.Progress;
        }
    }
}
