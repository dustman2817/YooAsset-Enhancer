using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset.Enhancer.Runtime;

namespace YooAsset.Enhancer.Editor
{
    [CreateAssetMenu(fileName = nameof(AssetBundleDrawer), menuName = "YooAsset/Extensions/Create AssetBundle Drawer")]
    [ESSConfig(ResourcesFolderPath = "Assets/Editor/Resources",
               AssetDatabaseFolderPath = "Assets/Editor/Resources",
               DefaultFileName = nameof(AssetBundleDrawer))]
    public partial class AssetBundleDrawer : EditorScriptableSingleton<AssetBundleDrawer>
    {
        [Serializable]
        private abstract class KeyColor
        {
            [GUIColor(nameof(Color))]
            [ValueDropdown(nameof(Keys))]
            public string Key;

            public Color Color = Color.white;

            protected abstract List<string> Keys { get; }
        }

        private static Color GetColor<T>(List<T> colors, string key) where T : KeyColor
        {
            if (colors == null || colors.Count <= 0)
            {
                return Color.white;
            }
            foreach (var color in colors.Where(color => color.Key == key))
            {
                return color.Color;
            }
            return Color.white;
        }
    }

    // EBuildPipeline
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class BuildPipelineColor : KeyColor
        {
            protected override List<string> Keys => BuildPipeline.GlobalBuildPipelineNames;
        }

        [SerializeField]
        [TableList]
        private List<BuildPipelineColor> m_BuildPipelineColors;

        public static Color GetBuildPipelineColor(string key)
        {
            return GetColor(Instance.m_BuildPipelineColors, key);
        }
    }

    // IIgnoreRule
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class IgnoreRuleColor : KeyColor
        {
            protected override List<string> Keys => IgnoreRule.GlobalIgnoreRuleNames;
        }

        [SerializeField]
        [TableList]
        private List<IgnoreRuleColor> m_IgnoreRuleNameColors;

        public static Color GetIgnoreRuleNameColor(string key)
        {
            return GetColor(Instance.m_IgnoreRuleNameColors, key);
        }
    }

    // IActiveRule
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class ActiveRuleColor : KeyColor
        {
            protected override List<string> Keys => ActiveRule.GlobalActiveRuleNames;
        }

        [SerializeField]
        [TableList]
        private List<ActiveRuleColor> m_ActiveRuleColors;

        public static Color GetActiveRuleColor(string key)
        {
            return GetColor(Instance.m_ActiveRuleColors, key);
        }
    }

    // ECollectorType
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class CollectorTypeColor : KeyColor
        {
            protected override List<string> Keys => CollectorType.GlobalCollectorTypeNames;
        }

        [SerializeField]
        [TableList]
        private List<CollectorTypeColor> m_CollectorTypeColors;

        public static Color GetCollectorTypeColor(string key)
        {
            return GetColor(Instance.m_CollectorTypeColors, key);
        }
    }

    // IAddressRule
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class AddressRuleColor : KeyColor
        {
            protected override List<string> Keys => AddressRule.GlobalAddressRuleNames;
        }

        [SerializeField]
        [TableList]
        private List<AddressRuleColor> m_AddressRuleColors;

        public static Color GetAddressRuleColor(string key)
        {
            return GetColor(Instance.m_AddressRuleColors, key);
        }
    }

    // IPackRule
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class PackRuleColor : KeyColor
        {
            protected override List<string> Keys => PackRule.GlobalPackRuleNames;
        }

        [SerializeField]
        [TableList]
        private List<PackRuleColor> m_PackRuleColors;

        public static Color GetPackRuleColor(string key)
        {
            return GetColor(Instance.m_PackRuleColors, key);
        }
    }

    // IFilterRule
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class FilterRuleColor : KeyColor
        {
            protected override List<string> Keys => FilterRule.GlobalFilterRuleNames;
        }

        [SerializeField]
        [TableList]
        private List<FilterRuleColor> m_FilterRuleColors;

        public static Color GetFilterRuleColor(string key)
        {
            return GetColor(Instance.m_FilterRuleColors, key);
        }
    }

    // IEncryptionServices
    public partial class AssetBundleDrawer
    {
        [Serializable]
        private class EncryptionServiceColor : KeyColor
        {
            protected override List<string> Keys => EncryptionService.GlobalEncryptionServiceNames;
        }

        [SerializeField]
        [TableList]
        private List<EncryptionServiceColor> m_EncryptionServiceColors;

        public static Color GetEncryptionServiceColor(string key)
        {
            return GetColor(Instance.m_EncryptionServiceColors, key);
        }
    }
}