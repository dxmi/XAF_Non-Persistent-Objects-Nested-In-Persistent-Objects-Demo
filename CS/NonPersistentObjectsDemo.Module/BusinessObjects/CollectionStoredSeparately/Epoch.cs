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
    public class Epoch : BaseObject, IObjectSpaceLink {
        public Epoch(Session session) : base(session) { }

        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }

        #region Technologies
        private BindingList<Technology> _Technologies;
        public BindingList<Technology> Technologies {
            get {
                if(_Technologies == null) {
                    _Technologies = new BindingList<Technology>();
                }
                return _Technologies;
            }
        }
        private string _TechnologyList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string TechnologyList {
            get { return _TechnologyList; }
            set { SetPropertyValue<string>(nameof(TechnologyList), ref _TechnologyList, value); }
        }
        #endregion

        protected override void OnLoaded() {
            base.OnLoaded();
            Load(Technologies, TechnologyList);
        }
        protected override void OnSaving() {
            TechnologyList = Save(Technologies);
            base.OnSaving();
        }
        private IObjectSpace objectSpace;
        protected IObjectSpace ObjectSpace { get { return objectSpace; } }
        IObjectSpace IObjectSpaceLink.ObjectSpace {
            get { return objectSpace; }
            set {
                if(objectSpace != value) {
                    objectSpace = value;
                }
            }
        }

        #region NP Serialization
        private void Load(IList<Technology> list, string data) {
            list.Clear();
            if(data != null) {
                foreach(var s in data.Split(',')) {
                    Guid key;
                    if(Guid.TryParse(s, out key)) {
                        var obj = ObjectSpace.GetObjectByKey<Technology>(key);
                        if(obj != null) {
                            list.Add(obj);
                        }
                    }
                }
            }
        }
        private string Save(IList<Technology> list) {
            if(list == null || list.Count == 0) {
                return null;
            }
            return string.Join(",", list.Select(o => o.Oid.ToString("D")));
        }
        #endregion
    }
}
