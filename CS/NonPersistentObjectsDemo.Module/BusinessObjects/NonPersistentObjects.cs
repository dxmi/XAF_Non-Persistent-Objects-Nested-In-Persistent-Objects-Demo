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
    public abstract class NonPersistentObjectBaseWithKey : NonPersistentObjectBase {
        //[Browsable(false)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [XmlIgnore]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
    }

    [DomainComponent]
    public class Feature : NonPersistentObjectBaseWithKey {
        public static int Sequence;
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

    [DomainComponent]
    public class Group : NonPersistentObjectBase {

        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
    }

    [DomainComponent]
    public class Platform : NonPersistentObjectBase {

        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
        private string _Description;
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>(nameof(Description), ref _Description, value); }
        }
    }

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

    class NPAdapter {
        private NonPersistentObjectSpace objectSpace;
        private bool isDirty = true;

        public NPAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Group)) {
                var pos = objectSpace.Owner as IObjectSpace;
                e.Objects = pos.GetObjectsQuery<Project>().GroupBy(o => o.GroupName).Select(o => new Group() { Name = o.Key }).ToList();
            }
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            isDirty = true;
        }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
        }
    }

}
