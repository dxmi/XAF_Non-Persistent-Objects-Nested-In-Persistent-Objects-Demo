﻿using System;
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

        private Feature _MainFeature;
        [DataSourceProperty(nameof(Features))]
        public Feature MainFeature {
            get { return _MainFeature; }
            set { SetPropertyValue<Feature>(nameof(MainFeature), ref _MainFeature, value); }
        }
        private string _MainFeatureName;
        [Browsable(false)]
        public string MainFeatureName {
            get { return _MainFeatureName; }
            set { SetPropertyValue<string>(nameof(MainFeatureName), ref _MainFeatureName, value); }
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
                list[e.NewIndex].OwnerKey = this.Oid;
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

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if(propertyName == nameof(MainFeature)) {
                MainFeatureName = (newValue as Feature)?.Name;
            }
        }
        protected override void OnLoaded() {
            base.OnLoaded();
            int counter = 0;
            Load(Features, FeatureList, o => { o.OwnerKey = this.Oid; o.LocalKey = ++counter; });
            Load(Resources, ResourceList, o => { o.OwnerKey = this.Oid; });
            _MainFeature = MainFeatureName == null ? null : Features.FirstOrDefault(f => f.Name == MainFeatureName);
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
        #endregion
    }
}
