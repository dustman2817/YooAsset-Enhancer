using System;
using UnityEngine;

namespace YooAsset.Enhancer.Runtime
{
    public abstract class RuntimeScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static readonly Lazy<T> s_Instance = new Lazy<T>(LoadOrCreateInstance);

        public static T Instance => s_Instance.Value;

        // private static T s_Instance;
        //
        // public static T Instance
        // {
        //     get
        //     {
        //         if (s_Instance == null)
        //         {
        //             s_Instance = LoadOrCreateInstance();
        //         }
        //         return s_Instance;
        //     }
        // }

        private static T LoadOrCreateInstance()
        {
            string resourcesFolderPath = TempResourcesFolderPath.Path;
            string fileFolder = null;
            string fileName = null;

            var attribute = GetConfigAttribute();
            if (attribute != null)
            {
                resourcesFolderPath = attribute.ResourcesFolderPath;
                fileFolder = attribute.DefaultFileFolder;
                fileName = attribute.DefaultFileName;
            }

            TempResourcesFolderPath.PushPath(resourcesFolderPath);
            T obj = null;
            try
            {
                obj = RuntimeScriptableObjectUtility.LoadOrCreateObject<T>(fileFolder, fileName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            TempResourcesFolderPath.PopPath();

            return obj;
        }

        private static RSSConfigAttribute GetConfigAttribute()
        {
            foreach (var attribute in typeof(T).GetCustomAttributes(true))
            {
                if (attribute is RSSConfigAttribute config)
                {
                    return config;
                }
            }
            return null;
        }
    }
}