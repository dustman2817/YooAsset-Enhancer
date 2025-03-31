using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    [Serializable]
    public class AssetBundleAllocatorCollector : ISearchFilterable
    {
        [FormerlySerializedAs("CollectorType")]
        [SerializeField]
        [PropertyOrder(-900)]
        [GUIColor(nameof(CollectorTypeColor))]
        private ECollectorType m_CollectorType = ECollectorType.MainAssetCollector;

        internal Color CollectorTypeColor => AssetBundleDrawer.GetCollectorTypeColor(m_CollectorType.ToString());

        /// <summary>
        /// 收集器类型
        /// </summary>
        public ECollectorType CollectorType
        {
            get => m_CollectorType;
            set => m_CollectorType = value;
        }

        [FormerlySerializedAs("CollectTarget")]
        [SerializeField]
        [PropertyOrder(-800)]
        private Object m_CollectTarget;

        /// <summary>
        /// 收集目标
        /// 注意：支持文件夹或单个资源文件
        /// </summary>
        public Object CollectTarget
        {
            get => m_CollectTarget;
            set => m_CollectTarget = value;
        }

        /// <summary>
        /// 收集的GUID
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-800)]
        [HideInTables]
        public string CollectGUID
        {
            get
            {
                if (CollectTarget == null)
                {
                    return string.Empty;
                }
                var path = AssetDatabase.GetAssetPath(CollectTarget);
                return AssetDatabase.AssetPathToGUID(path);
            }
        }

        /// <summary>
        /// 收集路径
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-800)]
        [HideInTables]
        public string CollectPath
        {
            get
            {
                if (CollectTarget == null)
                {
                    return string.Empty;
                }
                return AssetDatabase.GetAssetPath(CollectTarget);
            }
        }

        /// <summary>
        /// 收集名称
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-800)]
        public string CollectName
        {
            get
            {
                if (CollectTarget == null)
                {
                    return string.Empty;
                }
                var collectName = CollectTarget.name;
                if (collectName.Contains("/") || collectName.Contains("\\"))
                {
                    collectName = AssetUtil.GetAssetName(collectName);
                }
                return collectName;
            }
        }

        [FormerlySerializedAs("AddressRuleName")]
        [SerializeField]
        [HideInTables]
        [GUIColor(nameof(AddressRuleNameColor))]
        [ValueDropdown(nameof(AddressRuleNames))]
        private string m_AddressRuleName = nameof(AddressDisable);

        internal Color AddressRuleNameColor => AssetBundleDrawer.GetAddressRuleColor(m_AddressRuleName);

        private List<string> AddressRuleNames => AddressRule.GlobalAddressRuleNames;

        /// <summary>
        /// 寻址规则类名
        /// </summary>
        public string AddressRuleName
        {
            get => m_AddressRuleName;
            set => m_AddressRuleName = value;
        }

        [FormerlySerializedAs("PackRuleName")]
        [SerializeField]
        [GUIColor(nameof(PackRuleNameColor))]
        [ValueDropdown(nameof(PackRuleNames))]
        private string m_PackRuleName = nameof(PackSeparately);

        internal Color PackRuleNameColor => AssetBundleDrawer.GetPackRuleColor(m_PackRuleName);

        private List<string> PackRuleNames => PackRule.GlobalPackRuleNames;

        /// <summary>
        /// 打包规则类名
        /// </summary>
        public string PackRuleName
        {
            get => m_PackRuleName;
            set => m_PackRuleName = value;
        }

        [FormerlySerializedAs("FilterRuleName")]
        [SerializeField]
        [GUIColor(nameof(FilterRuleNameColor))]
        [ValueDropdown(nameof(FilterRuleNames))]
        private string m_FilterRuleName = nameof(CollectAll);

        internal Color FilterRuleNameColor => AssetBundleDrawer.GetFilterRuleColor(m_FilterRuleName);

        private List<string> FilterRuleNames => FilterRule.GlobalFilterRuleNames;

        /// <summary>
        /// 过滤规则类名
        /// </summary>
        public string FilterRuleName
        {
            get => m_FilterRuleName;
            set => m_FilterRuleName = value;
        }

        [FormerlySerializedAs("AssetTags")]
        [SerializeField]
        private string m_AssetTags;

        /// <summary>
        /// 资源分类标签
        /// </summary>
        public string AssetTags
        {
            get => m_AssetTags;
            set => m_AssetTags = value;
        }

        [FormerlySerializedAs("UserData")]
        [SerializeField]
        private string m_UserData;

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public string UserData
        {
            get => m_UserData;
            set => m_UserData = value;
        }

        [PropertyOrder(int.MaxValue)]
        [TableColumnWidth(44, false)]
        [Button(SdfIconType.GearFill)]
        public virtual void Detail()
        {
            var window = ScriptableObject.CreateInstance<CollectorWindow>();
            window.Selectable = false;
            window.Allocator = null;
            window.Package = null;
            window.Group = null;
            window.Collector = this;
            window.Show();
        }

        public bool IsMatch(string searchString)
        {
            if (CollectGUID.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            if (CollectName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}