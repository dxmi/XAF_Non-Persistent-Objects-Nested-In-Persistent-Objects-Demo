using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(Name))]
    [DefaultClassOptions]
    public class Product : BaseObject {
        public Product(Session session) : base(session) { }

        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }
        private Group _Group;
        public Group Group {
            get { return _Group; }
            set { SetPropertyValue<Group>(nameof(Group), ref _Group, value); }
        }
        private string _GroupName;
        [Browsable(false)]
        public string GroupName {
            get { return _GroupName; }
            set { SetPropertyValue<string>(nameof(GroupName), ref _GroupName, value); }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if(propertyName == nameof(Group)) {
                GroupName = (newValue as Group)?.Name;
            }
        }
        protected override void OnLoaded() {
            base.OnLoaded();
            _Group = GroupName == null ? null : new Group() { Name = GroupName };
        }
    }
}
