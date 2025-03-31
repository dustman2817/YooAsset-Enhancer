using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using YooAsset.Enhancer.Runtime;

namespace YooAsset.Enhancer.Editor
{
    [CreateAssetMenu(fileName = nameof(AssetBundleBuilder), menuName = "YooAsset/Extensions/Create AssetBundle Builder")]
    [ESSConfig(ResourcesFolderPath = "Assets/Editor/Resources",
               AssetDatabaseFolderPath = "Assets/Editor/Resources",
               DefaultFileName = nameof(AssetBundleBuilder))]
    public class AssetBundleBuilder : EditorScriptableSingleton<AssetBundleBuilder>
    {
        [SerializeField]
        [HideInTables]
        [ListDrawerSettings(OnTitleBarGUI = nameof(OnTitleBarGUI))]
        private List<EncryptionServiceInfo> m_EncryptionServices;

        private void OnTitleBarGUI()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                RefreshEncryptionServices();
            }
        }

        public List<EncryptionServiceInfo> EncryptionServices
        {
            get
            {
                if (m_EncryptionServices == null)
                {
                    RefreshEncryptionServices();
                }
                return m_EncryptionServices;
            }
            set
            {
                m_EncryptionServices = value;
                EditorUtility.SetDirty(this);
            }
        }

        private void RefreshEncryptionServices()
        {
            if (m_EncryptionServices == null)
            {
                m_EncryptionServices = new List<EncryptionServiceInfo>();
            }
            var infos = EncryptionService.GetEncryptionServiceInfos(true);
            foreach (var service in m_EncryptionServices)
            {
                foreach (var info in infos)
                {
                    if (service.ServiceName != info.ServiceName)
                    {
                        continue;
                    }
                    if (service.ServiceType != info.ServiceType)
                    {
                        continue;
                    }
                    if (info.ServiceObject == service.ServiceObject)
                    {
                        continue;
                    }
                    if (info.ServiceObject == null && service.ServiceObject != null)
                    {
                        info.ServiceObject = service.ServiceObject;
                    }
                    else if (info.ServiceObject != null && service.ServiceObject != null)
                    {
                        info.ServiceObject = service.ServiceObject;
                    }
                }
            }
            m_EncryptionServices = infos;
        }

        [SerializeField]
        [HideInTables]
        private List<AssetBundleBuildProfile> m_Profiles;

        public List<AssetBundleBuildProfile> Profiles
        {
            get => m_Profiles;
            set
            {
                m_Profiles = value;
                EditorUtility.SetDirty(this);
            }
        }

        public static AssetBundleBuildProfile FindProfile(string packageName)
        {
            var profiles = Instance.Profiles;
            if (profiles != null && profiles.Count > 0)
            {
                foreach (var profile in profiles)
                {
                    if (!profile.PackageName.Equals(packageName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    return profile;
                }
            }
            return null;
            // throw new Exception($"Can't find profile. packageName: {packageName}");
        }

        // 提供给外部使用，外部可能会使用反射访问，请注意此函数的声明尽可能不变
        public static string GetPackageBuildVersion(string packageName)
        {
            var profile = FindProfile(packageName);
            if (profile == null)
            {
                return null;
            }
            return profile.PackageVersion;
        }
    }
}