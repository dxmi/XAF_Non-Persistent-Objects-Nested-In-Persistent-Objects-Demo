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
    public class Group {
        [DevExpress.ExpressApp.Data.Key]
        public string Name { get; set; }
    }

    class NPGroupAdapter {
        private NonPersistentObjectSpace objectSpace;
        protected NonPersistentObjectSpace ObjectSpace { get { return objectSpace; } }
        private List<Group> objects;
        public NPGroupAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
        }
        protected Group GetObjectByKey(string key) {
            return new Group() { Name = key };
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Group)) {
                    e.Object = GetObjectByKey((string)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Group)) {
                if(objects == null) {
                    var pos = ObjectSpace.Owner as IObjectSpace;
                    objects = pos.GetObjectsQuery<Product>().Where(o => o.GroupName != null).GroupBy(o => o.GroupName).Select(o => GetObjectByKey(o.Key)).ToList();
                }
                e.Objects = objects;
            }
        }
    }
}
