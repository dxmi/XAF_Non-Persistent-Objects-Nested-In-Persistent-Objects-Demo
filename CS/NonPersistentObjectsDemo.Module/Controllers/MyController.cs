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
    
    public class AgentListViewController: ObjectViewController<ListView, Agent> {
        private ListViewProcessCurrentObjectController lvpcoc;
        private NewObjectViewController novc;
        protected override void OnActivated() {
            base.OnActivated();
            lvpcoc = Frame.GetController<ListViewProcessCurrentObjectController>();
            if(lvpcoc != null) {
                //lvpcoc.CustomProcessSelectedItem += lvpcoc_CustomProcessSelectedItem;
                //lvpcoc.ProcessCurrentObjectAction.ProcessCreatedView += ProcessCurrentObjectAction_ProcessCreatedView;
                //lvpcoc.CustomizeShowViewParameters += Lvpcoc_CustomizeShowViewParameters;
            }
            novc = Frame.GetController<NewObjectViewController>();
            if(novc != null) {
                //novc.ObjectCreating += novc_ObjectCreating;
                //novc.NewObjectAction.ProcessCreatedView += NewObjectAction_ProcessCreatedView;
            }
        }
        protected override void OnDeactivated() {
            if(novc != null) {
                //novc.ObjectCreating -= novc_ObjectCreating;
                novc.NewObjectAction.ProcessCreatedView -= NewObjectAction_ProcessCreatedView;
            }
            if(lvpcoc != null) {
                //lvpcoc.CustomProcessSelectedItem -= lvpcoc_CustomProcessSelectedItem;
                lvpcoc.ProcessCurrentObjectAction.ProcessCreatedView -= ProcessCurrentObjectAction_ProcessCreatedView;
                lvpcoc.CustomizeShowViewParameters -= Lvpcoc_CustomizeShowViewParameters;
            }
            base.OnDeactivated();
        }
        private void Lvpcoc_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e) {
            if(e.ShowViewParameters.CreatedView?.ObjectTypeInfo?.Type == typeof(Agent)) {
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            }
        }
        private void NewObjectAction_ProcessCreatedView(object sender, ActionBaseEventArgs e) {
            if(e.ShowViewParameters.CreatedView?.ObjectTypeInfo?.Type == typeof(Agent)) {
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            }
        }
        private void ProcessCurrentObjectAction_ProcessCreatedView(object sender, ActionBaseEventArgs e) {
            if(e.ShowViewParameters.CreatedView?.ObjectTypeInfo?.Type == typeof(Agent)) {
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            }
        }
        //private void lvpcoc_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
        //    e.Handled = true;
        //    var detailView = Application.CreateDetailView(View.ObjectSpace,
        //        Application.FindDetailViewId(e.InnerArgs.CurrentObject, View), false, e.InnerArgs.CurrentObject);
        //    e.InnerArgs.ShowViewParameters.CreatedView = detailView;
        //    e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        //}
        //private void novc_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
        //}
    }
}
