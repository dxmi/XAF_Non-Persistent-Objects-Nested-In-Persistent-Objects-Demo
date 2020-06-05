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

}
