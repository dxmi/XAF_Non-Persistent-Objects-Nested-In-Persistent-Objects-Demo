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
    public class Department : BaseObject, IObjectSpaceLink {
        public Department(Session session) : base(session) { }

        private string _Name;
        public string Name {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }

        #region Agents
        private BindingList<Agent> _Agents;
        [Aggregated]
        public BindingList<Agent> Agents {
            get {
                if(_Agents == null) {
                    _Agents = new BindingList<Agent>();
                    _Agents.ListChanged += _Agents_ListChanged;
                }
                return _Agents;
            }
        }
        private void _Agents_ListChanged(object sender, ListChangedEventArgs e) {
            var list = (BindingList<Agent>)sender;
            if(e.ListChangedType == ListChangedType.ItemAdded) {
                list[e.NewIndex].ID = ++Agent.Sequence;
            }
            AgentList = Save(Agents);
        }
        private string _AgentList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string AgentList {
            get { return _AgentList; }
            set { SetPropertyValue<string>(nameof(AgentList), ref _AgentList, value); }
        }
        #endregion

        protected override void OnLoaded() {
            base.OnLoaded();
            Load(Agents, AgentList, o => { o.ID = ++Agent.Sequence; });
        }
        protected override void OnSaving() {
            AgentList = Save(Agents);
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
        private void Load<T>(BindingList<T> list, string data, Action<T> acceptor) {
            list.RaiseListChangedEvents = false;
            list.Clear();
            if(data != null) {
                var serializer = new XmlSerializer(typeof(T).MakeArrayType());
                using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(data))) {
                    var objs = serializer.Deserialize(stream) as IList<T>;
                    foreach(var obj in objs) {
                        acceptor?.Invoke(obj);
                        var tobj = ObjectSpace.GetObject(obj);
                        var aobj = tobj as IAssignable<T>;
                        if(aobj != null) {
                            aobj.Assign(obj);
                        }
                        list.Add(tobj);
                    }
                }
            }
            list.RaiseListChangedEvents = true;
            list.ResetBindings();
        }
        private string Save<T>(IList<T> list) {
            if(list == null || list.Count == 0) {
                return null;
            }
            var serializer = new XmlSerializer(typeof(T).MakeArrayType());
            using(var stream = new MemoryStream()) {
                serializer.Serialize(stream, list.ToArray());
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        #endregion
    }
}
