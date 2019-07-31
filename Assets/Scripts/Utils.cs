using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace utils
{
    public class SharedVariable<T> : ScriptableObject
    {
        public T Value;
        public static implicit operator T(SharedVariable<T> v) => v.Value;
    }

    public class VariableReference<T> : ScriptableObject
    {
        public bool UseConstant = true;
        public T ConstantValue;
        public SharedVariable<T> Variable;
        public T Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }
        public VariableReference() { }
        public VariableReference(T value)
        {
            UseConstant = true;
            ConstantValue = value;
        }
        public static implicit operator T(VariableReference<T> v) => v.Value;
    }

    public abstract class RuntimeSet<T> : ScriptableObject
    {
        public List<T> Items = new List<T>();
        public void Add(T t)
        {
            if (!Items.Contains(t)) Items.Add(t);
        }
        public void Remove(T t)
        {
            if (Items.Contains(t)) Items.Remove(t);
        }
    }
    namespace NativeListExtensionMethods
    {
        public static class NativeListExtensions
        {
            public static void Push<T>(this NativeList<T> list, T value) where T: struct
            {

            }
            
            public static T Pop<T>(this NativeList<T> list) where T: struct
            {
                T item = list[list.Length - 1];
                list.RemoveAtSwapBack(list.Length - 1);
                return item;
            }

            public static T Peek<T>(this NativeList<T> list) where T: struct
            {
                return list[list.Length - 1];
            }
        }
    }
    //public struct NativeStack<T> : NativeList<T>
    //{
    //    private NativeList<T> List;

    //    public NativeStack(int capacity, Allocator alloc)
    //    {
    //        List = new NativeList<T>(capacity, alloc);
    //    }

    //    public NativeStack(Allocator alloc)
    //    {
    //        List = new NativeList<T>(alloc);
    //    }

    //    public void Push(T obj)
    //    {
    //        List.Add(obj);
    //    }

    //    public T Pop()
    //    {
    //        T item = List[List.Length - 1];
    //        List.RemoveAtSwapBack(List.Length - 1);
    //        return item;
    //    }

    //    public T Peek()
    //    {
    //        return List[List.Length - 1];
    //    }

    //    public int Count { get => List.Length; }

    //    public void Dispose()
    //    {
    //        List.Dispose();
    //    }

    //    public JobHandle Dispose(JobHandle jobHandle)
    //    {
    //        return List.Dispose(jobHandle);
    //    }
    //}

    //public static class ScriptableObjectUtility
    //{
    //    /// <summary>
    //    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    //    /// </summary>
    //    public static void CreateAsset<T>() where T : ScriptableObject
    //    {
    //        T asset = ScriptableObject.CreateInstance<T>();

    //        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //        if (path == "")
    //        {
    //            path = "Assets";
    //        }
    //        else if (Path.GetExtension(path) != "")
    //        {
    //            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    //        }
    //        var bob = path + "/New " + typeof(T).ToString() + ".asset";
    //        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(bob);

    //        AssetDatabase.CreateAsset(asset, assetPathAndName);

    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh();
    //        EditorUtility.FocusProjectWindow();
    //        Selection.activeObject = asset;
    //    }
    //}
}

