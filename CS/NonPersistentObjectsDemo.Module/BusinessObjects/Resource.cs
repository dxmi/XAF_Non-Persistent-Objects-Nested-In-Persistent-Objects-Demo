using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DomainComponent]
    public class Resource : NonPersistentObjectBaseWithKey {
        public static int Sequence;
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
    }

    class NPResourceAdapter {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<int, Resource> objectMap;

        public NPResourceAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectMap = new Dictionary<int, Resource>();
        }
        private Resource GetObject(int key) {
            Resource result;
            if(!objectMap.TryGetValue(key, out result)) {
                throw new NotImplementedException();
                objectMap.Add(key, result);
            }
            return result;
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Resource)) {
                    e.Object = GetObject((int)e.Key);
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
                //throw new NotImplementedException();
            }
        }
    }
}
