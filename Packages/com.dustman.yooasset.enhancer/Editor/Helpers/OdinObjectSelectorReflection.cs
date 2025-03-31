using System;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace YooAsset.Enhancer.Editor
{
    public static class OdinObjectSelectorReflection
    {
        private static Type m_Type = typeof(OdinObjectSelector);

        public static void Show(Rect position, object key, int id, object value, Type valueType, Type baseType, bool allowSceneObjects = true, bool disallowNullValues = false, InspectorProperty property = null, bool useUnitySelector = false)
        {
            var types = new Type[] {typeof(Rect), typeof(object), typeof(int), typeof(object), typeof(Type), typeof(Type), typeof(bool), typeof(bool), typeof(InspectorProperty), typeof(bool)};
            var method = m_Type.GetMethod("Show", BindingFlags.Static | BindingFlags.NonPublic, null, types, null);
            var objects = new object[] {position, key, id, value, valueType, baseType, allowSceneObjects, disallowNullValues, property, useUnitySelector};
            method?.Invoke(null, objects);
        }
    }
}