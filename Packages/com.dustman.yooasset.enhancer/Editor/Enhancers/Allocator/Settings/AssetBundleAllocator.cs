using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;
using YooAsset.Enhancer.Runtime;

namespace YooAsset.Enhancer.Editor
{
    [CreateAssetMenu(fileName = nameof(AssetBundleAllocator), menuName = "YooAsset/Extensions/Create AssetBundle Allocator")]
    [ESSConfig(ResourcesFolderPath = "Assets/Editor/Resources",
               AssetDatabaseFolderPath = "Assets/Editor/Resources",
               DefaultFileName = nameof(AssetBundleAllocator))]
    public class AssetBundleAllocator : EditorScriptableSingleton<AssetBundleAllocator>
    {
        [FormerlySerializedAs("ShowPackageView")]
        [SerializeField]
        private bool m_ShowPackageView = true;

        /// <summary>
        /// 显示包裹列表视图
        /// </summary>
        public bool ShowPackageView
        {
            get => m_ShowPackageView;
            set => m_ShowPackageView = value;
        }

        [FormerlySerializedAs("ShowEditorAlias")]
        [SerializeField]
        private bool m_ShowEditorAlias;

        /// <summary>
        /// 是否显示编辑器别名
        /// </summary>
        public bool ShowEditorAlias
        {
            get => m_ShowEditorAlias;
            set => m_ShowEditorAlias = value;
        }

        [FormerlySerializedAs("UniqueBundleName")]
        [SerializeField]
        private bool m_UniqueBundleName = true;

        /// <summary>
        /// 资源包名唯一化
        /// </summary>
        public bool UniqueBundleName
        {
            get => m_UniqueBundleName;
            set => m_UniqueBundleName = value;
        }

        [FormerlySerializedAs("Packages")]
        [SerializeField]
        private List<AssetBundleAllocatorPackage> m_Packages = new List<AssetBundleAllocatorPackage>();

        /// <summary>
        /// 包裹列表
        /// </summary>
        public List<AssetBundleAllocatorPackage> Packages
        {
            get => m_Packages;
            set => m_Packages = value;
        }

        public AssetBundleAllocator()
        {
            ShowPackageView = false;
        }

        public bool IsDirty
        {
            get
            {
                if (EditorUtility.IsDirty(this))
                {
                    return true;
                }
                if (Packages == null || Packages.Count <= 0)
                {
                    return false;
                }
                return false;
            }
        }

        public List<string> PackageNames
        {
            get
            {
                if (Instance == null)
                {
                    return null;
                }
                var names = new List<string>();
                var packages = Instance.Packages;
                if (packages != null && packages.Count > 0)
                {
                    names.AddRange(packages.Select(package => package.PackageName));
                }
                return names;
            }
        }

        [GUIColor("red")]
        [Button("导入自 YooAsset 配置", ButtonSizes.Medium)]
        internal void Import()
        {
            AllocatorSynchronization.Import(AssetBundleCollectorSettingData.Setting, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        [GUIColor("blue")]
        [Button("导出至 YooAsset 配置", ButtonSizes.Medium)]
        internal void Export()
        {
            AllocatorSynchronization.Export(this, AssetBundleCollectorSettingData.Setting);
            AssetBundleCollectorSettingData.SaveFile();
        }
    }
}