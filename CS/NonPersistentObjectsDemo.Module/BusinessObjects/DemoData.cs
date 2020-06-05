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
            p2.Features.Add(new Feature() { Name = "Feature 1", Progress = 3.5 });
            p2.Features.Add(new Feature() { Name = "Feature 2", Progress = 0 });
            p2.Features.Add(new Feature() { Name = "Feature 3", Progress = 1 });
            p2.Resources.Add(new Resource() { Name = "Resource A", URI = "a", Embed = true });
            p2.Resources.Add(new Resource() { Name = "Resource B", URI = "b", Priority = 2 });
            p2.Resources.Add(new Resource() { Name = "Resource C", URI = "c", Priority = 1 });
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
