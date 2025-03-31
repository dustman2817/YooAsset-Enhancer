// --------------------------------------------------------------------------
//  Copyright © 2010-2020 BOKE Technology Co.,Ltd. All rights reserved.
//  Unauthorized copying of this file, via any medium is strictly prohibited.
//  Proprietary and confidential.
// 
//  Copyright © 2024/06/12 tiansheng@boke.com. All rights reserved.
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Enhancer.Runtime;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    public class TempAssetDatabaseFolderPath
    {
        private const string c_DefaultAssetDatabaseFolderPath = "Assets";
        private static string s_AssetDatabaseFolderPath;
        private static readonly Stack<string> s_AssetDatabaseFolderPaths = new Stack<string>();

        public static string Path
        {
            get
            {
                if (string.IsNullOrEmpty(s_AssetDatabaseFolderPath))
                {
                    s_AssetDatabaseFolderPath = c_DefaultAssetDatabaseFolderPath;
                }
                return s_AssetDatabaseFolderPath;
            }
        }

        public static void PushPath(string path)
        {
            s_AssetDatabaseFolderPaths.Push(path);
            s_AssetDatabaseFolderPath = path;
        }

        public static void PopPath()
        {
            if (s_AssetDatabaseFolderPaths.Count <= 0)
            {
                s_AssetDatabaseFolderPath = c_DefaultAssetDatabaseFolderPath;
                return;
            }
            s_AssetDatabaseFolderPath = s_AssetDatabaseFolderPaths.Pop();
        }
    }

    public class EditorScriptableObjectUtility
    {
        private static string ResourcesFolderPath => TempResourcesFolderPath.Path;

        private static string AssetDatabaseFolderPath => TempAssetDatabaseFolderPath.Path;

        public static TObject LoadOrCreateObject<TObject>(string fileFolder = null, string fileName = null, bool allowMultiple = true, LoadFlags loadFlag = LoadFlags.All) where TObject : ScriptableObject
        {
            return (TObject)LoadOrCreateObject(typeof(TObject), fileFolder, fileName, allowMultiple, loadFlag);
        }

        public static ScriptableObject LoadOrCreateObject(Type type, string fileFolder = null, string fileName = null, bool allowMultiple = true, LoadFlags loadFlag = LoadFlags.All)
        {
            fileName = string.IsNullOrEmpty(fileName) ? type.Name : fileName;
            var filePath = string.IsNullOrEmpty(fileFolder) ? fileName : $"{fileFolder}/{fileName}";

            Object obj = null;

            // Check
            string[] guids = null;
            if (!allowMultiple)
            {
                guids = AssetDatabase.FindAssets($"t:{type.Name}");
                if (guids.Length > 1)
                {
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        Debug.LogError($"Found multiple file: {path}.");
                    }
                    throw new Exception($"Found multiple {fileName} files.");
                }
            }

            // Search with Resources
            if ((loadFlag & LoadFlags.Resources) != 0)
            {
                obj = Resources.Load(filePath, type);
                if (obj != null)
                {
                    Debug.Log($"Use Resources to find {fileName}.");
                    return (ScriptableObject)obj;
                }
            }

            // Search with AssetDatabase
            if ((loadFlag & LoadFlags.AssetDatabase) != 0)
            {
                if (guids == null || guids.Length <= 0)
                {
                    guids = AssetDatabase.FindAssets($"t:{type.Name}");
                }
                if (guids.Length > 0)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    obj = AssetDatabase.LoadAssetAtPath(assetPath, type);
                }
                if (obj != null)
                {
                    Debug.Log($"Use AssetDatabase to find {fileName}.");
                    return (ScriptableObject)obj;
                }
            }

            // Create with AssetDatabase
            if ((loadFlag & LoadFlags.AssetDatabase) != 0)
            {
                Debug.Log($"Use AssetDatabase to create new {type.Name}.asset.");
                obj = ScriptableObject.CreateInstance(type);
                var folderPath = string.IsNullOrEmpty(fileFolder) ? AssetDatabaseFolderPath : $"{AssetDatabaseFolderPath}/{fileFolder}";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var assetPath = $"{AssetDatabaseFolderPath}/{filePath}.asset";
                AssetDatabase.CreateAsset(obj, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

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
                AssetDatabase.CreateAsset(obj, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (obj != null)
            {
                return (ScriptableObject)obj;
            }

            throw new Exception($"Get or create {fileName} files failed.");
        }

        public static TObject[] FindObjects<TObject>() where TObject : ScriptableObject
        {
            return (TObject[])FindObjects(typeof(TObject));
        }

        public static ScriptableObject[] FindObjects(Type type)
        {
            var guids = AssetDatabase.FindAssets($"t:{type.Name}");
            if (guids.Length <= 0)
            {
                return null;
            }

            var objs = new ScriptableObject[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                objs[i] = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, type);
            }

            return objs;
        }
    }
}