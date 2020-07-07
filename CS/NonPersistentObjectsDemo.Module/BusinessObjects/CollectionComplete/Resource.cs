using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DomainComponent]
    public class Resource : NonPersistentObjectImpl, IAssignable<Resource> {
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [DevExpress.ExpressApp.Data.Key]
        public Guid Oid { get; set; }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(ref _Name, value); }
        }
        private string _URI;
        public string URI {
            get { return _URI; }
            set { SetPropertyValue<string>(ref _URI, value); }
        }
        private int _Priority;
        public int Priority {
            get { return _Priority; }
            set { SetPropertyValue<int>(ref _Priority, value); }
        }
        private bool _Embed;
        public bool Embed {
            get { return _Embed; }
            set { SetPropertyValue<bool>(ref _Embed, value); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Guid OwnerKey { get; set; }
        public void Assign(Resource source) {
            Oid = source.Oid;
            OwnerKey = source.OwnerKey;
            Name = source.Name;
            URI = source.URI;
            Priority = source.Priority;
            Embed = source.Embed;
        }
        public Resource() {
            Oid = Guid.NewGuid();
        }
    }

    class NPResourceAdapter : NonPersistentObjectAdapter<Resource, Guid> {
        public NPResourceAdapter(NonPersistentObjectSpace npos) : base(npos) { }
        protected override void GuardKeyNotEmpty(Resource obj) {
            if(obj.OwnerKey == Guid.Empty)
                throw new InvalidOperationException(); // DEBUG
            if(obj.Oid == Guid.Empty)
                throw new InvalidOperationException(); // DEBUG
        }
        protected override Resource LoadSameObject(Resource obj) {
            Resource result;
            var owner = GetOwnerByKey(obj.OwnerKey);
            result = GetFromOwner(owner, obj.Oid);
            if(result == null) {
                owner = ReloadOwner(owner);
                result = GetFromOwner(owner, obj.Oid);
            }
            return result;
        }
        private Resource GetFromOwner(Project owner, Guid localKey) {
            if(owner == null) {
                throw new InvalidOperationException("Owner object is not found in the storage.");
            }
            return owner.Resources.FirstOrDefault(o => o.Oid == localKey);
        }
        private Project GetOwnerByKey(Guid key) {
            var ownerObjectSpace = ObjectSpace.Owner as CompositeObjectSpace;
            return (ownerObjectSpace ?? ObjectSpace).GetObjectByKey<Project>(key);
        }
        private Project ReloadOwner(Project owner) {
            var ownerObjectSpace = ObjectSpace.Owner as CompositeObjectSpace;
            return (Project)(ownerObjectSpace ?? ObjectSpace).ReloadObject(owner);
        }
    }
}
