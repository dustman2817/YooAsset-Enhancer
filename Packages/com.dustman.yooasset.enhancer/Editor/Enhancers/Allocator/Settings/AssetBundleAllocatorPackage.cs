using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    [Serializable]
    public class AssetBundleAllocatorPackage : ISearchFilterable
    {
        /// <summary>
        /// 包裹对象
        /// </summary>
        public AssetBundleAllocatorPackage PackageObject
        {
            get => this;
            set {}
        }

        [FormerlySerializedAs("PackageName")]
        [SerializeField]
        private string m_PackageName;

        /// <summary>
        /// 包裹名称
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-999)]
        [HideInTables]
        public string PackageName
        {
            get => m_PackageName;
            set => m_PackageName = value;
        }

        [FormerlySerializedAs("PackageDesc")]
        [SerializeField]
        private string m_PackageDesc;

        /// <summary>
        /// 包裹描述
        /// </summary>
        public string PackageDesc
        {
            get => m_PackageDesc;
            set => m_PackageDesc = value;
        }

        [FormerlySerializedAs("EnableAddressable")]
        [SerializeField]
        private bool m_EnableAddressable = true;

        /// <summary>
        /// 启用可寻址资源定位
        /// </summary>
        public bool EnableAddressable
        {
            get => m_EnableAddressable;
            set => m_EnableAddressable = value;
        }

        [FormerlySerializedAs("LocationToLower")]
        [SerializeField]
        private bool m_LocationToLower;

        /// <summary>
        /// 资源定位地址大小写不敏感
        /// </summary>
        public bool LocationToLower
        {
            get => m_LocationToLower;
            set => m_LocationToLower = value;
        }

        [FormerlySerializedAs("IncludeAssetGUID")]
        [SerializeField]
        private bool m_IncludeAssetGUID = true;

        /// <summary>
        /// 包含资源GUID数据
        /// </summary>
        public bool IncludeAssetGUID
        {
            get => m_IncludeAssetGUID;
            set => m_IncludeAssetGUID = value;
        }

        [FormerlySerializedAs("AutoCollectShaders")]
        [SerializeField]
        private bool m_AutoCollectShaders = true;

        /// <summary>
        /// 自动收集所有着色器（所有着色器存储在一个资源包内）
        /// </summary>
        public bool AutoCollectShaders
        {
            get => m_AutoCollectShaders;
            set => m_AutoCollectShaders = value;
        }

        [FormerlySerializedAs("IgnoreRuleName")]
        [SerializeField]
        [GUIColor(nameof(IgnoreRuleNameColor))]
        [ValueDropdown(nameof(IgnoreRuleNames))]
        private string m_IgnoreRuleName = nameof(NormalIgnoreRule);

        internal Color IgnoreRuleNameColor => AssetBundleDrawer.GetIgnoreRuleNameColor(m_IgnoreRuleName);

        private List<string> IgnoreRuleNames => IgnoreRule.GlobalIgnoreRuleNames;

        /// <summary>
        /// 资源忽略规则名
        /// </summary>
        public string IgnoreRuleName
        {
            get => m_IgnoreRuleName;
            set => m_IgnoreRuleName = value;
        }

        [FormerlySerializedAs("Groups")]
        [SerializeField]
        [HideInTables]
        private List<AssetBundleAllocatorGroup> m_Groups = new List<AssetBundleAllocatorGroup>();

        /// <summary>
        /// 分组列表
        /// </summary>
        public List<AssetBundleAllocatorGroup> Groups
        {
            get => m_Groups;
            set => m_Groups = value;
        }

        [PropertyOrder(int.MaxValue)]
        [TableColumnWidth(44, false)]
        [Button(SdfIconType.GearFill)]
        public virtual void Detail()
        {
            var window = ScriptableObject.CreateInstance<PackageWindow>();
            window.Selectable = false;
            window.Allocator = null;
            window.Package = this;
            window.Show();
        }

        public bool IsMatch(string searchString)
        {
            if (PackageName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            if (PackageDesc.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}