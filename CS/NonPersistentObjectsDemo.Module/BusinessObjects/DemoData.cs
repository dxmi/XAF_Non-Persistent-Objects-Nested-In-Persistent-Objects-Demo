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
            if(ObjectSpace.CanInstantiate(typeof(Project))) {
                if(!ObjectSpace.CanInstantiate(typeof(Technology))) {
                    var typesInfo = ObjectSpace.TypesInfo;
                    var npos = new NonPersistentObjectSpace(typesInfo, ((DevExpress.ExpressApp.DC.TypesInfo)typesInfo).FindEntityStore(typeof(DevExpress.ExpressApp.DC.NonPersistentTypeInfoSource)));
                    ((CompositeObjectSpace)ObjectSpace).AdditionalObjectSpaces.Add(npos);
                    ((CompositeObjectSpace)ObjectSpace).AutoCommitAdditionalObjectSpaces = true;
                    new NPTechnologyAdapter(npos);
                }
                CreateProjects();
                CreateProducts();
                CreateEpochs();
                CreateDepartments();
            }
        }
        private void CreateProjects() {
            var p1 = CreateProject("Project X");
            var p2 = CreateProject("Project Y");
            p2.Features.Add(new Feature() { Name = "Feature 1", Progress = 3.5 });
            p2.Features.Add(new Feature() { Name = "Feature 2", Progress = 0 });
            p2.Features.Add(new Feature() { Name = "Feature 3", Progress = 1 });
            p2.Resources.Add(new Resource() { Name = "Resource A", URI = "a", Embed = true });
            p2.Resources.Add(new Resource() { Name = "Resource B", URI = "b", Priority = 2 });
            p2.Resources.Add(new Resource() { Name = "Resource C", URI = "c", Priority = 1 });
            var p3 = CreateProject("Project Z");
        }
        private Project CreateProject(string name) {
            var project = ObjectSpace.CreateObject<Project>();
            project.CodeName = name;
            return project;
        }
        private void CreateProducts() {
            var p1 = CreateProduct("Product 1", "B");
            var p2 = CreateProduct("Product 2", "B");
            var p4 = CreateProduct("Product 3", "XHD");
            var p3 = CreateProduct("Product 4", "AAA");
        }
        private Product CreateProduct(string name, string group) {
            var project = ObjectSpace.CreateObject<Product>();
            project.Name = name;
            project.Group = new Group() { Name = group };
            return project;
        }
        private void CreateEpochs() {
            var t1 = CreateTechnology("Tech 1", "Technology 1");
            var t2 = CreateTechnology("Tech 2", "Technology 2");
            var t3 = CreateTechnology("Tech 3", "Technology 3");
            var e1 = CreateEpoch("Stone Age");
            var e2 = CreateEpoch("Nowadays");
            var e3 = CreateEpoch("Future");
            e2.Technologies.Add(t1);
            e2.Technologies.Add(t2);
            e2.Technologies.Add(t3);
        }
        private Epoch CreateEpoch(string name) {
            var obj = ObjectSpace.CreateObject<Epoch>();
            obj.Name = name;
            return obj;
        }
        private Technology CreateTechnology(string name, string description) {
            var tech = ObjectSpace.CreateObject<Technology>();
            tech.Name = name;
            tech.Description = description;
            return tech;
        }
        private void CreateDepartments() {
            var e1 = CreateDepartment("Sales");
            var e2 = CreateDepartment("Research");
            var e3 = CreateDepartment("Communications");
            e2.Agents.Add(new Agent() { Name = "Agent X", Progress = 80 });
            e2.Agents.Add(new Agent() { Name = "Agent Orange", Progress = 0 });
        }
        private Department CreateDepartment(string name) {
            var obj = ObjectSpace.CreateObject<Department>();
            obj.Name = name;
            return obj;
        }
    }
}
