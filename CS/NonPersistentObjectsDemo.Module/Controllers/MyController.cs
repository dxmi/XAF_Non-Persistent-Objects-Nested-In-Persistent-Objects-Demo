using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using NonPersistentObjectsDemo.Module.BusinessObjects;

namespace NonPersistentObjectsDemo.Module.Controllers {
    public class FeatureListViewController : ObjectViewController<ListView, Feature> {
        protected override void OnActivated() {
            base.OnActivated();
            var filterController = Frame.GetController<FilterController>();
            if(filterController!= null) {
                filterController.AllowFilterNonPersistentObjects = true;
            }
        }
    }
}
