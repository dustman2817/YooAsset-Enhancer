using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace YooAsset.Enhancer.Runtime
{
    public class TempResourcesFolderPath
    {
        private const string c_DefaultResourcesFolderPath = "Assets/Resources";
        private static string s_ResourcesFolderPath;
        private static readonly Stack<string> s_ResourcesFolderPaths = new Stack<string>();

        public static string Path
        {
            get
            {
                if (string.IsNullOrEmpty(s_ResourcesFolderPath))
                {
                    s_ResourcesFolderPath = c_DefaultResourcesFolderPath;
                }
                return s_ResourcesFolderPath;
            }
        }

        public static void PushPath(string path)
        {
            s_ResourcesFolderPaths.Push(path);
            s_ResourcesFolderPath = path;
        }

        public static void PopPath()
        {
            if (s_ResourcesFolderPaths.Count <= 0)
            {
                s_ResourcesFolderPath = c_DefaultResourcesFolderPath;
                return;
            }
            s_ResourcesFolderPath = s_ResourcesFolderPaths.Pop();
        }
    }

    public class RuntimeScriptableObjectUtility
    {
        private static string ResourcesFolderPath => TempResourcesFolderPath.Path;

        public static T LoadOrCreateObject<T>(string fileFolder = null, string fileName = null) where T : ScriptableObject
        {
            return (T)LoadOrCreateObject(typeof(T), fileFolder, fileName);
        }

        public static ScriptableObject LoadOrCreateObject(Type type, string fileFolder = null, string fileName = null)
        {
            fileName = string.IsNullOrEmpty(fileName) ? type.Name : fileName;
            var filePath = string.IsNullOrEmpty(fileFolder) ? fileName : $"{fileFolder}/{fileName}";

            // Search with Resources
            var obj = Resources.Load(filePath, type);
            if (obj != null)
            {
                Debug.Log($"Use Resources to find {fileName}.");
                return (ScriptableObject)obj;
            }

#if UNITY_EDITOR
            // Create with Resources
            if (obj == null)
            {
                Debug.Log($"Use Resources to create new {type.Name} asset.");
                obj = ScriptableObject.CreateInstance(type);
                var folderPath = string.IsNullOrEmpty(fileFolder) ? ResourcesFolderPath : $"{ResourcesFolderPath}/{fileFolder}";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var assetPath = $"{ResourcesFolderPath}/{filePath}.asset";
                UnityEditor.AssetDatabase.CreateAsset(obj, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
            }

            if (obj != null)
            {
                return (ScriptableObject)obj;
            }
#endif

            throw new Exception($"Load or create {fileName} files failed.");
        }
    }
}