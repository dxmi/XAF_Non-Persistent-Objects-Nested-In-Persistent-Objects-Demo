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
    public class Group : NonPersistentObjectImpl {
        private string _Name;
        [DevExpress.ExpressApp.Data.Key]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(ref _Name, value); }
        }
    }

    class NPGroupAdapter : NonPersistentObjectAdapter<Group, string> {
        public NPGroupAdapter(NonPersistentObjectSpace npos) : base(npos) { }
        protected override void GuardKeyNotEmpty(Group obj) {
        }
        protected override Group LoadObjectByKey(string key) {
            return new Group() { Name = key };
        }
        protected override IList<Group> GetObjects() {
            var pos = ObjectSpace.Owner as IObjectSpace;
            return pos.GetObjectsQuery<Product>().Where(o => o.GroupName != null).GroupBy(o => o.GroupName).Select(o => GetObjectByKey(o.Key)).ToList();
        }
    }

    /*
     * Also, see the overridden GetObjectSpaceToShowDetailViewFrom method
     * in the WebApplication descendant.
     * 
     */
}
