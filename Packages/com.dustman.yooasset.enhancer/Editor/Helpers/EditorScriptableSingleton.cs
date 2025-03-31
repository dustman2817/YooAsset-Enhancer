// -------------------------------------------------------------------------
// Copyright © 2010-2020 BOKE Technology Co.,Ltd. All rights reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
// 
// Copyright © 2024/07/04 tiansheng@boke.com. All rights reserved.
// -------------------------------------------------------------------------

using System;
using UnityEngine;
using YooAsset.Enhancer.Runtime;

namespace YooAsset.Enhancer.Editor
{
    public abstract class EditorScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
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

        protected static T LoadOrCreateInstance()
        {
            string resourcesFolderPath = TempResourcesFolderPath.Path;
            string assetDatabaseFolderPath = TempAssetDatabaseFolderPath.Path;
            string folderPath = null;
            string fileName = null;
            LoadFlags loadFlag = LoadFlags.All;

            var attribute = GetConfigAttribute();
            if (attribute != null)
            {
                resourcesFolderPath = attribute.ResourcesFolderPath;
                assetDatabaseFolderPath = attribute.AssetDatabaseFolderPath;
                folderPath = attribute.DefaultFileFolder;
                fileName = attribute.DefaultFileName;
                loadFlag = attribute.LoadFlag;
            }

            TempResourcesFolderPath.PushPath(resourcesFolderPath);
            TempAssetDatabaseFolderPath.PushPath(assetDatabaseFolderPath);
            var obj = EditorScriptableObjectUtility.LoadOrCreateObject<T>(folderPath, fileName, false, loadFlag);
            TempAssetDatabaseFolderPath.PopPath();
            TempResourcesFolderPath.PopPath();

            return obj;
        }

        protected static ESSConfigAttribute GetConfigAttribute()
        {
            foreach (var attribute in typeof(T).GetCustomAttributes(true))
            {
                if (attribute is ESSConfigAttribute config)
                {
                    return config;
                }
            }
            return null;
        }
    }
}