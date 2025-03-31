using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    [Serializable]
    public class AssetBundleAllocatorGroup : ISearchFilterable
    {
        /// <summary>
        /// 分组对象
        /// </summary>
        public AssetBundleAllocatorGroup GroupObject
        {
            get => this;
            set {}
        }

        [FormerlySerializedAs("GroupName")]
        [SerializeField]
        private string m_GroupName;

        /// <summary>
        /// 分组名称
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-999)]
        [HideInTables]
        public string GroupName
        {
            get => m_GroupName;
            set => m_GroupName = value;
        }

        [FormerlySerializedAs("GroupDesc")]
        [SerializeField]
        private string m_GroupDesc;

        /// <summary>
        /// 分组描述
        /// </summary>
        public string GroupDesc
        {
            get => m_GroupDesc;
            set => m_GroupDesc = value;
        }

        [FormerlySerializedAs("ActiveRuleName")]
        [SerializeField]
        [GUIColor(nameof(ActiveRuleNameColor))]
        [ValueDropdown(nameof(ActiveRuleNames))]
        private string m_ActiveRuleName = nameof(EnableGroup);

        internal Color ActiveRuleNameColor => AssetBundleDrawer.GetActiveRuleColor(m_ActiveRuleName);

        private List<string> ActiveRuleNames => ActiveRule.GlobalActiveRuleNames;

        /// <summary>
        /// 分组激活规则
        /// </summary>
        public string ActiveRuleName
        {
            get => m_ActiveRuleName;
            set => m_ActiveRuleName = value;
        }

        [FormerlySerializedAs("AssetTags")]
        [SerializeField]
        private string m_AssetTags;

        /// <summary>
        /// 资源分类标签
        /// TODO: 暂时先使用输入框，后续可以改造为下拉框多选，有如下方案：
        /// TODO:     1.使用 [Flags] 标记枚举，使用枚举的 Key 作为 Tag，缺点是修改枚举得改代码；
        /// TODO:     2.使用 List 定义新字段转化成 Tags，缺点是显示不友好；
        /// TODO:     3.自定义属性绘制，缺点是得花点时间，而且绘制可能比较费；
        /// </summary>
        public string AssetTags
        {
            get => m_AssetTags;
            set => m_AssetTags = value;
        }

        [FormerlySerializedAs("Collectors")]
        [SerializeField]
        [PropertyOrder(500)]
        [HideInTables]
        private List<AssetBundleAllocatorCollector> m_Collectors = new List<AssetBundleAllocatorCollector>();

        /// <summary>
        /// 分组的收集器列表
        /// </summary>
        public List<AssetBundleAllocatorCollector> Collectors
        {
            get => m_Collectors;
            set => m_Collectors = value;
        }

        [PropertyOrder(int.MaxValue)]
        [TableColumnWidth(44, false)]
        [Button(SdfIconType.GearFill)]
        public virtual void Detail()
        {
            var window = ScriptableObject.CreateInstance<GroupWindow>();
            window.Selectable = false;
            window.Allocator = null;
            window.Package = null;
            window.Group = this;
            window.Show();
        }

        public bool IsMatch(string searchString)
        {
            if (GroupName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            if (GroupDesc.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}