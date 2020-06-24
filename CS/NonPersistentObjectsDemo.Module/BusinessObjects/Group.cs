using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DomainComponent]
    [DefaultProperty(nameof(Name))]
    public class Group : NonPersistentObjectBase {
        private string _Name;
        [DevExpress.ExpressApp.Data.Key]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
    }

    class NPGroupAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<string, Group> objectMap;

        public NPGroupAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectMap = new Dictionary<string, Group>();
        }
        private Group GetObject(string key) {
            if(key == null)
                return null;
            Group result;
            if(!objectMap.TryGetValue(key, out result)) {
                result = new Group() { Name = key };
                objectMap.Add(key, result);
            }
            return result;
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Group)) {
                    e.Object = GetObject((string)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Group)) {
                var pos = objectSpace.Owner as IObjectSpace;
                e.Objects = pos.GetObjectsQuery<Project>().Where(o => o.GroupName != null).GroupBy(o => o.GroupName).Select(o => GetObject(o.Key)).ToList();
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            var link = e.SourceObject as IObjectSpaceLink;
            if(e.SourceObject is Group) {
                var obj = (Group)e.SourceObject;
                if(link.ObjectSpace == null) {
                    if(obj.Name != null) {
                        Group result;
                        if(!objectMap.TryGetValue(obj.Name, out result)) {
                            result = new Group() { Name = obj.Name };
                            objectMap.Add(obj.Name, result);
                        }
                        e.TargetObject = result;
                    }
                }
                else {
                    if(link.ObjectSpace == objectSpace) {
                        if(!link.ObjectSpace.IsNewObject(obj)) {
                            e.TargetObject = GetObject(obj.Name);
                        }
                    }
                    else {
                        if(link.ObjectSpace.IsNewObject(obj)) {
                            e.TargetObject = null;
                        }
                        else {
                            e.TargetObject = GetObject(((Group)e.SourceObject).Name);
                        }
                    }
                }
            }
        }
    }
}
