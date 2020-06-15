using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DomainComponent]
    public class Technology : NonPersistentObjectBase {
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [DevExpress.ExpressApp.Data.Key]
        public Guid Oid { get; set; }
        public Technology() : base() {
            Oid = Guid.NewGuid();
        }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
        private string _Description;
        [FieldSize(FieldSizeAttribute.Unlimited)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>(nameof(Description), ref _Description, value); }
        }
    }

    class NPTechnologyAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<Guid, Technology> objectMap;

        private static Dictionary<Guid, Technology> storage;

        static NPTechnologyAdapter() {
            storage = new Dictionary<Guid, Technology>();
        }

        public NPTechnologyAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
            objectSpace.CustomCommitChanges += ObjectSpace_CustomCommitChanges;
            objectMap = new Dictionary<Guid, Technology>();
        }
        private void AcceptObject(Technology obj) {
            Technology result;
            if(!objectMap.TryGetValue(obj.Oid, out result)) {
                ((IObjectSpaceLink)obj).ObjectSpace = objectSpace; // remove?
                objectMap.Add(obj.Oid, obj);
            }
            else {
                if(result != obj) {
                    throw new InvalidOperationException();
                }
            }
        }
        private Technology GetObject(Guid key) {
            Technology result;
            if(!objectMap.TryGetValue(key, out result)) {
                Technology objData;
                if(storage.TryGetValue(key, out objData)) {
                    result = new Technology();
                    Copy(objData, result);
                    AcceptObject(result);
                }
            }
            return result;
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Technology)) {
                    e.Object = GetObject((Guid)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Technology)) {
                throw new NotSupportedException();
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            var link = e.SourceObject as IObjectSpaceLink;
            if(e.SourceObject is Technology) {
                var obj = (Technology)e.SourceObject;
                if(link.ObjectSpace == null || link.ObjectSpace == objectSpace) {
                    AcceptObject(obj);
                    return;
                }
                if(link.ObjectSpace.IsNewObject(obj)) { // implement in OS?
                    e.TargetObject = null;
                    return;
                }
                e.TargetObject = GetObject(obj.Oid);
            }
        }
        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e) {
            if(e.SourceObject is Technology) {
                var tobj = (Technology)e.SourceObject;
                Technology objData;
                if(!storage.TryGetValue(tobj.Oid, out objData)) {
                    throw new InvalidOperationException("Object is not found in the storage.");
                }
                Copy(objData, tobj);
                e.TargetObject = tobj;
            }
        }
        private void ObjectSpace_CustomCommitChanges(object sender, HandledEventArgs e) {
            foreach(var obj in objectSpace.ModifiedObjects) {
                if(obj is Technology) {
                    var tobj = (Technology)obj;
                    Technology objData;
                    if(objectSpace.IsDeletedObject(obj)) {
                        storage.Remove(tobj.Oid);
                    }
                    else if(objectSpace.IsNewObject(obj)) {
                        objData = new Technology();
                        Copy(tobj, objData);
                        storage.Add(tobj.Oid, objData);
                    }
                    else {
                        if(!storage.TryGetValue(tobj.Oid, out objData)) {
                            throw new InvalidOperationException("Object is not found in the storage.");
                        }
                        Copy(tobj, objData);
                    }
                }
            }
        }
        private void Copy(Technology source, Technology target) {
            target.Oid = source.Oid;
            target.Name = source.Name;
            target.Description = source.Description;
        }
    }
}
