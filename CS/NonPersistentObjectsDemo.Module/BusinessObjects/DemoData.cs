using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    class DemoDataCreator {
        private IObjectSpace ObjectSpace;
        public DemoDataCreator(IObjectSpace objectSpace) {
            this.ObjectSpace = objectSpace;
        }
        public void CreateDemoObjects() {
            var p1 = CreateProject("Project X", "B");
            var p2 = CreateProject("Project Y", "B");
            var p3 = CreateProject("Project Z", "AAA");
            var p4 = CreateProject("Project Unknown", "XHD");
        }
        private Project CreateProject(string name, string group) {
            var project = ObjectSpace.CreateObject<Project>();
            project.CodeName = name;
            project.Group = new Group() { Name = group };
            return project;
        }
    }
}
