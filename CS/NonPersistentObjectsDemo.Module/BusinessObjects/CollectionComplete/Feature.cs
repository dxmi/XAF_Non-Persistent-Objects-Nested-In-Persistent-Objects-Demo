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
    public class Feature : NonPersistentObjectImpl, IAssignable<Feature> {
        public Feature() : base() { }
        [Browsable(false)]
        [XmlIgnore]
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
            set { SetPropertyValue<string>(ref _Name, value); }
        }
        private double _Progress;
        public double Progress {
            get { return _Progress; }
            set { SetPropertyValue<double>(ref _Progress, value); }
        }
        public void Assign(Feature source) {
            OwnerKey = source.OwnerKey;
            LocalKey = source.LocalKey;
            Name = source.Name;
            Progress = source.Progress;
        }
    }

    class NPFeatureAdapter : NonPersistentObjectAdapter<Feature, string> {
        public NPFeatureAdapter(NonPersistentObjectSpace npos) : base(npos) { }
        protected override void GuardKeyNotEmpty(Feature obj) {
            if(obj.OwnerKey == Guid.Empty)
                throw new InvalidOperationException(); // DEBUG
            if(obj.LocalKey == 0)
                throw new InvalidOperationException(); // DEBUG
        }
        protected override Feature LoadObjectByKey(string key) {
            Feature result;
            Guid ownerKey;
            int localKey;
            if(!TryParseKey(key, out ownerKey, out localKey)) {
                throw new InvalidOperationException("Invalid key.");
            }
            var owner = GetOwnerByKey(ownerKey);
            result = GetFromOwner(owner, localKey);
            if(result == null) {
                owner = ReloadOwner(owner);
                result = GetFromOwner(owner, localKey);
            }
            return result;
        }
        private Feature GetFromOwner(Project owner, int localKey) {
            if(owner == null) {
                throw new InvalidOperationException("Owner object is not found in the storage.");
            }
            return owner.Features.FirstOrDefault(o => o.LocalKey == localKey);
        }
        private Project GetOwnerByKey(Guid key) {
            var ownerObjectSpace = ObjectSpace.Owner as CompositeObjectSpace;
            return (ownerObjectSpace ?? ObjectSpace).GetObjectByKey<Project>(key);
        }
        private Project ReloadOwner(Project owner) {
            var ownerObjectSpace = ObjectSpace.Owner as CompositeObjectSpace;
            if(ownerObjectSpace.ModifiedObjects.Contains(owner)) {
                throw new NotSupportedException();
            }
            return (Project)(ownerObjectSpace ?? ObjectSpace).ReloadObject(owner);
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
    }
}
