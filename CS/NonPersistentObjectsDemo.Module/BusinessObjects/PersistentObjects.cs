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
        private BindingList<Feature> _Features;
        [Aggregated]
        public IList<Feature> Features {
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
                list[e.NewIndex].ID = ++Feature.Sequence;
            }
        }
        private string _FeatureList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string FeatureList {
            get { return _FeatureList; }
            set { SetPropertyValue<string>(nameof(FeatureList), ref _FeatureList, value); }
        }
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
        private BindingList<Resource> _Resources;
        [Aggregated]
        public IList<Resource> Resources {
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
        }
        private string _ResourceList;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string ResourceList {
            get { return _ResourceList; }
            set { SetPropertyValue<string>(nameof(ResourceList), ref _ResourceList, value); }
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if(propertyName == nameof(Group)) {
                GroupName = (newValue as Group)?.Name;
            }
            else if(propertyName == nameof(KillerFeature)) {
                KillerFeatureName = (newValue as Feature)?.Name;
            }
        }
        protected override void OnLoaded() {
            base.OnLoaded();
            Group = GroupName == null ? null : ObjectSpace.GetObject(new Group() { Name = GroupName });
            Load(Features, FeatureList, ref Feature.Sequence);
            Load(Resources, ResourceList, ref Resource.Sequence);
            KillerFeature = KillerFeatureName == null ? null : Features.FirstOrDefault(f => f.Name == KillerFeatureName);
        }
        protected override void OnSaving() {
            FeatureList = Save(Features);
            ResourceList = Save(Resources);
            base.OnSaving();
        }
        private IObjectSpace objectSpace;
        protected IObjectSpace ObjectSpace { get { return objectSpace; } }
        IObjectSpace IObjectSpaceLink.ObjectSpace {
            get { return objectSpace; }
            set {
                if(objectSpace != value) {
                    //OnObjectSpaceChanging();
                    objectSpace = value;
                    //OnObjectSpaceChanged();
                }
            }
        }

        #region NP Serialization
        private void Load<T>(IList<T> list, string data, ref int sequence) where T : NonPersistentObjectBaseWithKey {
            list.Clear();
            if(data == null) return;
            var objectSpace = (CompositeObjectSpace)BaseObjectSpace.FindObjectSpaceByObject(this);
            var itsObjectSpace = objectSpace.FindAdditionalObjectSpace(typeof(T));
            var serializer = new XmlSerializer(typeof(T).MakeArrayType());
            using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(data))) {
                var objs = serializer.Deserialize(stream) as IList<T>;
                foreach(var obj in objs) {
                    obj.ID = ++sequence;
                    ObjectSpace.GetObject(obj);
                    list.Add(obj);
                }
            }
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
