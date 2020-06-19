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

    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(CodeName))]
    [DefaultClassOptions]
    public class Project : BaseObject, IObjectSpaceLink {
        public Project(Session session) : base(session) { }

        private string _CodeName;
        public string CodeName {
            get { return _CodeName; }
            set { SetPropertyValue<string>(nameof(CodeName), ref _CodeName, value); }
        }
        private Group _Group;
        //[DataSourceProperty(nameof(AllGroups))]
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
        //[Browsable(false)]
        //public IList<Group> AllGroups {
        //    get { return Session.Query<Project>().GroupBy(o => o.GroupName).Select(o => new Group() { Name = o.Key }).ToList(); }
        //}

        #region Features
        private BindingList<Feature> _Features;
        [Aggregated]
        public BindingList<Feature> Features {
            get {
                if(_Features == null) {
                    _Features = new BindingList<Feature>();
                    _Features.ListChanged += _Features_ListChanged;
                }
                return _Features;
            }
        }
        private void _Features_ListChanged(object sender, ListChangedEventArgs e) {
            var list = (BindingList<Feature>)sender;
            if(e.ListChangedType == ListChangedType.ItemAdded) {
                var obj = list[e.NewIndex];
                obj.OwnerKey = this.Oid;
                obj.LocalKey = e.NewIndex + 1;
            }
            FeatureList = Save(Features);
        }
        private string _FeatureList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string FeatureList {
            get { return _FeatureList; }
            set { SetPropertyValue<string>(nameof(FeatureList), ref _FeatureList, value); }
        }
        #endregion

        private Feature _KillerFeature;
        [DataSourceProperty(nameof(Features))]
        public Feature KillerFeature {
            get { return _KillerFeature; }
            set { SetPropertyValue<Feature>(nameof(KillerFeature), ref _KillerFeature, value); }
        }
        private string _KillerFeatureName;
        [Browsable(false)]
        public string KillerFeatureName {
            get { return _KillerFeatureName; }
            set { SetPropertyValue<string>(nameof(KillerFeatureName), ref _KillerFeatureName, value); }
        }

        #region Resources
        private BindingList<Resource> _Resources;
        [Aggregated]
        public BindingList<Resource> Resources {
            get {
                if(_Resources == null) {
                    _Resources = new BindingList<Resource>();
                    _Resources.ListChanged += _Resources_ListChanged;
                }
                return _Resources;
            }
        }
        private void _Resources_ListChanged(object sender, ListChangedEventArgs e) {
            var list = (BindingList<Resource>)sender;
            if(e.ListChangedType == ListChangedType.ItemAdded) {
                list[e.NewIndex].ID = ++Resource.Sequence;
            }
            ResourceList = Save(Resources);
        }
        private string _ResourceList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string ResourceList {
            get { return _ResourceList; }
            set { SetPropertyValue<string>(nameof(ResourceList), ref _ResourceList, value); }
        }
        #endregion

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

        #region Technologies
        private BindingList<Technology> _Technologies;
        //[Aggregated]
        public BindingList<Technology> Technologies {
            get {
                if(_Technologies == null) {
                    _Technologies = new BindingList<Technology>();
                    //_Technologies.ListChanged += _Technologies_ListChanged;
                }
                return _Technologies;
            }
        }
        //private void _Technologies_ListChanged(object sender, ListChangedEventArgs e) {
        //    var list = (BindingList<Technology>)sender;
        //    if(e.ListChangedType == ListChangedType.ItemAdded) {
        //        list[e.NewIndex].ID = ++Technology.Sequence;
        //    }
        //}
        private string _TechnologyList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string TechnologyList {
            get { return _TechnologyList; }
            set { SetPropertyValue<string>(nameof(TechnologyList), ref _TechnologyList, value); }
        }
        #endregion

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if(propertyName == nameof(Group)) {
                GroupName = (newValue as Group)?.Name;
            }
            else if(propertyName == nameof(KillerFeature)) {
                KillerFeatureName = (newValue as Feature)?.Name;
            }
            else if(propertyName == nameof(FeatureList)) {
                //Load(Features, FeatureList, o => { o.OwnerKey = this.Oid; });
            }
        }
        protected override void OnLoaded() {
            base.OnLoaded();
            _Group = GroupName == null ? null : ObjectSpace.GetObject(new Group() { Name = GroupName });
            Load(Features, FeatureList, o=> { o.OwnerKey = this.Oid; });
            Load(Resources, ResourceList, o => { o.ID = ++Resource.Sequence; });
            _KillerFeature = KillerFeatureName == null ? null : Features.FirstOrDefault(f => f.Name == KillerFeatureName);
            Load(Agents, AgentList, o => { o.ID = ++Agent.Sequence; });
            Load(Technologies, TechnologyList);
        }
        protected override void OnSaving() {
            FeatureList = Save(Features);
            ResourceList = Save(Resources);
            AgentList = Save(Agents);
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
        private void Load<T>(BindingList<T> list, string data, Action<T> acceptor) {
            list.RaiseListChangedEvents = false;
            list.Clear();
            if(data != null) {
                //var objectSpace = (CompositeObjectSpace)BaseObjectSpace.FindObjectSpaceByObject(this);
                //var itsObjectSpace = objectSpace.FindAdditionalObjectSpace(typeof(T));
                var serializer = new XmlSerializer(typeof(T).MakeArrayType());
                using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(data))) {
                    var objs = serializer.Deserialize(stream) as IList<T>;
                    foreach(var obj in objs) {
                        acceptor?.Invoke(obj);
                        var tobj = ObjectSpace.GetObject(obj);
                        var aobj = tobj as IAssignable<T>;
                        if(aobj!= null) {
                            aobj.Assign(obj); // always?
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
