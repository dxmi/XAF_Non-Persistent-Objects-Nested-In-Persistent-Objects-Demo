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
    public class Resource : NonPersistentObjectBase, IAssignable<Resource> {
        //[Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [DevExpress.ExpressApp.Data.Key]
        public Guid ID { get; set; }
        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
        private string _URI;
        public string URI {
            get { return _URI; }
            set { SetPropertyValue<string>(nameof(URI), ref _URI, value); }
        }
        private int _Priority;
        public int Priority {
            get { return _Priority; }
            set { SetPropertyValue<int>(nameof(Priority), ref _Priority, value); }
        }
        private bool _Embed;
        public bool Embed {
            get { return _Embed; }
            set { SetPropertyValue<bool>(nameof(Embed), ref _Embed, value); }
        }
        [Browsable(false)]
        [XmlIgnore]
        public Guid OwnerKey { get; set; }
        public void Assign(Resource source) {
            ID = source.ID;
            OwnerKey = source.OwnerKey;
            Name = source.Name;
            URI = source.URI;
            Priority = source.Priority;
            Embed = source.Embed;
        }
        public Resource() {
            ID = Guid.NewGuid();
        }
    }

    class NPResourceAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<Guid, Resource> objectMap;

        public NPResourceAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectMap = new Dictionary<Guid, Resource>();
        }
        void GuardKeyNotEmpty(Resource obj) {
            if(obj.OwnerKey == Guid.Empty)
                throw new InvalidOperationException(); // DEBUG
            if(obj.ID == Guid.Empty)
                throw new InvalidOperationException(); // DEBUG
        }
        private void AcceptObject(Resource obj) {
            Resource result;
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
        private Resource GetObject(Guid key) {
            Resource result;
            if(!objectMap.TryGetValue(key, out result)) {
                throw new NotSupportedException();
            }
            return result;
        }
        private Project GetOwnerByKey(Guid key) {
            var ownerObjectSpace = objectSpace.Owner as CompositeObjectSpace;
            return (ownerObjectSpace ?? objectSpace).GetObjectByKey<Project>(key);
        }
        private Project ReloadOwner(Project owner) {
            var ownerObjectSpace = objectSpace.Owner as CompositeObjectSpace;
            return (Project)(ownerObjectSpace ?? objectSpace).ReloadObject(owner);
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Resource)) {
                    e.Object = GetObject((Guid)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Resource)) {
                throw new NotSupportedException();
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            var link = e.SourceObject as IObjectSpaceLink;
            if(e.SourceObject is Resource) {
                var obj = (Resource)e.SourceObject;
                GuardKeyNotEmpty(obj);
                if(link.ObjectSpace == null) {
                    //AcceptObject(obj);
                    Resource result;
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
                            //AcceptObject(obj);
                            Resource result;
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
                            Resource result;
                            if(!objectMap.TryGetValue(obj.ID, out result)) {
                                var owner = GetOwnerByKey(obj.OwnerKey);
                                result = GetFromOwner(owner, obj.ID);
                                if(result == null) {
                                    owner = ReloadOwner(owner);
                                    result = GetFromOwner(owner, obj.ID);
                                }
                                if(result != null) {
                                    AcceptObject(result);
                                }
                            }
                            e.TargetObject = result;
                        }
                    }
                }
            }
        }
        private Resource GetFromOwner(Project owner, Guid localKey) {
            if(owner == null) {
                throw new InvalidOperationException("Owner object is not found in the storage.");
            }
            return owner.Resources.FirstOrDefault(o => o.ID == localKey);
        }
    }
}
