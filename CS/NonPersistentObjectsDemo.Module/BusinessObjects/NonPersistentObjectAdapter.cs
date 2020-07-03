using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    public interface IAssignable<T> {
        void Assign(T source);
    }

    public abstract class NonPersistentObjectAdapter<TObject, TKey> {
        private NonPersistentObjectSpace objectSpace;
        private Dictionary<TKey, TObject> objectMap;
        protected NonPersistentObjectSpace ObjectSpace { get { return objectSpace; } }
        public NonPersistentObjectAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            objectSpace.ObjectGetting += ObjectSpace_ObjectGetting;
            objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
            objectSpace.Reloaded += ObjectSpace_Reloaded;
            objectSpace.ObjectReloading += ObjectSpace_ObjectReloading;
            objectSpace.CustomCommitChanges += ObjectSpace_CustomCommitChanges;
            objectMap = new Dictionary<TKey, TObject>();
        }
        protected virtual void GuardKeyNotEmpty(TObject obj) {
            if(Object.Equals(GetKeyValue(obj), default(TKey)))
                throw new InvalidOperationException(); // DEBUG
        }
        protected virtual TKey GetKeyValue(TObject obj) {
            return (TKey)objectSpace.GetKeyValue(obj);
        }
        private void AcceptObject(TObject obj) {
            TObject result;
            GuardKeyNotEmpty(obj);
            if(!objectMap.TryGetValue(GetKeyValue(obj), out result)) {
                objectMap.Add(GetKeyValue(obj), obj);
            }
            else {
                if(!Object.Equals(result, obj)) {
                    throw new InvalidOperationException();
                }
            }
        }
        protected TObject GetObjectByKey(TKey key) {
            TObject result;
            if(!objectMap.TryGetValue(key, out result)) {
                result = LoadObjectByKey(key);
                if(result != null) {
                    AcceptObject(result);
                }
            }
            return result;
        }
        protected virtual TObject LoadObjectByKey(TKey key) {
            throw new NotSupportedException();
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(TObject)) {
                    e.Object = GetObjectByKey((TKey)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(TObject)) {
                var objects = GetObjects();
                e.Objects = (System.Collections.IList)objects;
            }
        }
        protected virtual IList<TObject> GetObjects() {
            throw new NotSupportedException();
        }
        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            objectMap.Clear();
        }
        protected virtual bool ThrowOnAcceptingMismatchedObject { get { return false; } }
        private void ObjectSpace_ObjectGetting(object sender, ObjectGettingEventArgs e) {
            var link = e.SourceObject as IObjectSpaceLink;
            if(e.SourceObject is TObject) {
                var obj = (TObject)e.SourceObject;
                GuardKeyNotEmpty(obj);
                if(link.ObjectSpace == null) {
                    TObject result;
                    if(!objectMap.TryGetValue(GetKeyValue(obj), out result)) {
                        objectMap.Add(GetKeyValue(obj), obj);
                        e.TargetObject = obj;
                    }
                    else {
                        // if objectMap contains an object with the same key, assume SourceObject is a reloaded copy. (because link is null yet)
                        // then refresh contents of the found object and return it.
                        if(!Object.Equals(result, obj)) {
                            if(ThrowOnAcceptingMismatchedObject)
                                throw new InvalidOperationException();
                            if(result is IAssignable<TObject> a) {
                                a.Assign(obj);
                            }
                        }
                        e.TargetObject = result;
                    }
                }
                else {
                    if(link.ObjectSpace.IsNewObject(obj)) {
                        if(link.ObjectSpace == objectSpace) {
                            e.TargetObject = e.SourceObject;
                        }
                        else {
                            e.TargetObject = null;
                        }
                    }
                    else {
                        if(link.ObjectSpace == objectSpace) {
                            TObject result;
                            if(!objectMap.TryGetValue(GetKeyValue(obj), out result)) {
                                objectMap.Add(GetKeyValue(obj), obj);
                                e.TargetObject = obj;
                            }
                            else {
                                // if objectMap contains an object with the same key, assume SourceObject is a reloaded copy.
                                // then refresh contents of the found object and return it.
                                if(!Object.Equals(result, obj)) {
                                    if(ThrowOnAcceptingMismatchedObject)
                                        throw new InvalidOperationException();
                                    if(result is IAssignable<TObject> a) {
                                        a.Assign(obj);
                                    }
                                }
                                e.TargetObject = result;
                            }
                        }
                        else {
                            TObject result;
                            if(!objectMap.TryGetValue(GetKeyValue(obj), out result)) {
                                result = LoadSameObject(obj);
                                if(result != null) {
                                    AcceptObject(result);
                                }
                            }
                            e.TargetObject = result;
                        }
                    }
                }
            }
        }
        protected virtual TObject LoadSameObject(TObject obj) {
            return LoadObjectByKey(GetKeyValue(obj));
        }
        private void ObjectSpace_ObjectReloading(object sender, ObjectGettingEventArgs e) {
            if(e.SourceObject is TObject) {
                var tobj = (TObject)e.SourceObject;
                e.TargetObject = ReloadObject(tobj);
            }
        }
        protected virtual TObject ReloadObject(TObject obj) {
            return obj;
        }
        private void ObjectSpace_CustomCommitChanges(object sender, HandledEventArgs e) {
            foreach(var obj in objectSpace.ModifiedObjects) {
                if(obj is TObject) {
                    var tobj = (TObject)obj;
                    SaveObject(tobj);
                }
            }
        }
        protected virtual void SaveObject(TObject tobj) {
        }
    }

}
