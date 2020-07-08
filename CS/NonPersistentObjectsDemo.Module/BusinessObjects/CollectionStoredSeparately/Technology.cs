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
    public class Technology : NonPersistentBaseObject, IAssignable<Technology> {
        public Technology() : base() { }
        public Technology(Guid oid) : base(oid) { }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(ref _Name, value); }
        }
        private string _Description;
        [FieldSize(FieldSizeAttribute.Unlimited)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>(ref _Description, value); }
        }
        public void Assign(Technology source) {
            Name = source.Name;
            Description = source.Description;
        }
    }

    class NPTechnologyAdapter : NonPersistentObjectAdapter<Technology, Guid> {
        private static Dictionary<Guid, Technology> storage;
        static NPTechnologyAdapter() {
            storage = new Dictionary<Guid, Technology>();
        }
        public NPTechnologyAdapter(NonPersistentObjectSpace npos) : base(npos) { }
        protected override Technology LoadObjectByKey(Guid key) {
            Technology result = null;
            Technology objData;
            if(storage.TryGetValue(key, out objData)) {
                result = new Technology(key);
                result.Assign(objData);
            }
            return result;
        }
        protected override Technology ReloadObject(Technology obj) {
            Technology objData = LoadData(obj.Oid);
            obj.Assign(objData);
            return obj;
        }
        protected override void CommitChanges(List<Technology> objects) {
            foreach(var obj in objects) {
                Technology objData;
                if(ObjectSpace.IsDeletedObject(obj)) {
                    storage.Remove(obj.Oid);
                }
                else if(ObjectSpace.IsNewObject(obj)) {
                    objData = new Technology(obj.Oid);
                    objData.Assign(obj);
                    storage.Add(obj.Oid, objData);
                }
                else {
                    objData = LoadData(obj.Oid);
                    objData.Assign(obj);
                }
            }
        }
        private Technology LoadData(Guid key) {
            Technology objData;
            if(!storage.TryGetValue(key, out objData)) {
                throw new InvalidOperationException("Object is not found in the storage.");
            }
            return objData;
        }
    }
}
