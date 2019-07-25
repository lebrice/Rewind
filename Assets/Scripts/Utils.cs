using System.Collections.Generic;
using System.IO;
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
}

