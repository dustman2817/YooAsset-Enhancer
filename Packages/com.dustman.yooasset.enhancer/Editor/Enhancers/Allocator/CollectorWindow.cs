using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using YooAsset.Editor;
using YooAsset.Enhancer.Runtime;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    public class CollectorWindow : OdinEditorWindow
    {
        private EventHandler m_OnCollectorChanged;

        public EventHandler OnCollectorChanged
        {
            get => m_OnCollectorChanged;
            set => m_OnCollectorChanged = value;
        }

        private EventHandler m_OnCollectorSaved;

        public EventHandler OnCollectorSaved
        {
            get => m_OnCollectorSaved;
            set => m_OnCollectorSaved = value;
        }

        private bool m_Selectable = true;

        public bool Selectable
        {
            get => m_Selectable;
            set => m_Selectable = value;
        }

        private AssetBundleAllocatorCollector m_Collector;
        private string m_Exception;

        private bool IsException => !string.IsNullOrEmpty(m_Exception);

        private AssetBundleAllocator m_Allocator;

        public AssetBundleAllocator Allocator
        {
            get => m_Allocator;
            set => m_Allocator = value;
        }

        private AssetBundleAllocatorPackage m_Package;

        public AssetBundleAllocatorPackage Package
        {
            get => m_Package;
            set => m_Package = value;
        }

        private AssetBundleAllocatorGroup m_Group;

        public AssetBundleAllocatorGroup Group
        {
            get => m_Group;
            set => m_Group = value;
        }

        public AssetBundleAllocatorCollector Collector
        {
            get
            {
                if (IsException)
                {
                    return null;
                }
                if (m_Collector == null && !IsException)
                {
                    m_Exception = $"{nameof(AssetBundleAllocatorCollector)} was not found.";
                }
                return m_Collector;
            }
            set
            {
                m_Collector = value;
                m_Exception = null;
                m_AssetInfos = null;
            }
        }

        private bool IsExist => Collector != null;

        private bool IsDrawable => IsExist;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Object", Indent = true)]
        [GUIColor(nameof(CollectorTypeColor))]
        private ECollectorType CollectorType
        {
            get
            {
                if (!IsExist)
                {
                    return ECollectorType.None;
                }
                return Collector.CollectorType;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.CollectorType = value;
                OnValueChanged();
            }
        }

        private Color CollectorTypeColor
        {
            get
            {
                if (!IsExist)
                {
                    return GUI.color;
                }
                return Collector.CollectorTypeColor;
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Target", Indent = true)]
        private Object CollectTarget
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Collector.CollectTarget;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.CollectTarget = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Target", Indent = true)]
        private string CollectGUID
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.CollectGUID;
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Target", Indent = true)]
        private string CollectPath
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.CollectPath;
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Target", Indent = true)]
        private string CollectName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.CollectName;
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [HorizontalGroup("Setting/Rule")]
        [HideLabel]
        [GUIColor(nameof(AddressRuleNameColor))]
        [ValueDropdown(nameof(AddressRuleNames))]
        private string AddressRuleName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.AddressRuleName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.AddressRuleName = value;
                OnValueChanged();
            }
        }

        private Color AddressRuleNameColor
        {
            get
            {
                if (!IsExist)
                {
                    return GUI.color;
                }
                return Collector.AddressRuleNameColor;
            }
        }

        private List<string> AddressRuleNames => AddressRule.GlobalAddressRuleNames;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [HorizontalGroup("Setting/Rule")]
        [HideLabel]
        [GUIColor(nameof(PackRuleNameColor))]
        [ValueDropdown(nameof(PackRuleNames))]
        private string PackRuleName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.PackRuleName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.PackRuleName = value;
                OnValueChanged();
            }
        }

        private Color PackRuleNameColor
        {
            get
            {
                if (!IsExist)
                {
                    return GUI.color;
                }
                return Collector.PackRuleNameColor;
            }
        }

        private List<string> PackRuleNames => PackRule.GlobalPackRuleNames;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [HorizontalGroup("Setting/Rule")]
        [HideLabel]
        [GUIColor(nameof(FilterRuleNameColor))]
        [ValueDropdown(nameof(FilterRuleNames))]
        private string FilterRuleName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.FilterRuleName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.FilterRuleName = value;
                OnValueChanged();
            }
        }

        private Color FilterRuleNameColor
        {
            get
            {
                if (!IsExist)
                {
                    return GUI.color;
                }
                return Collector.FilterRuleNameColor;
            }
        }

        private List<string> FilterRuleNames => FilterRule.GlobalFilterRuleNames;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        private string AssetTags
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.AssetTags;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.AssetTags = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        private string UserData
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Collector.UserData;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Collector.UserData = value;
                OnValueChanged();
            }
        }

        private List<AllocateAssetInfo> m_AssetInfos;

        [ShowInInspector]
        [PropertyOrder(900)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Preview", Indent = true)]
        [TabGroup("Preview/List", "Asset List")]
        [HideLabel]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [RichTableList(AlwaysExpanded = true, ShowPaging = true, DrawScrollView = false, ShowIndexLabels = true, HideAddButton = true, HideRemoveButton = true, OnTitleBarGUI = nameof(OnTitleBarGUI))]
        private List<AllocateAssetInfo> AssetInfos
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                if (m_AssetInfos == null)
                {
                    RefreshPreview();
                }
                return m_AssetInfos;
            }
            set {}
        }

        private List<AllocateBundleInfo> m_BundleInfos;

        [ShowInInspector]
        [PropertyOrder(900 - 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Preview", Indent = true)]
        [TabGroup("Preview/List", "Bundle List")]
        [HideLabel]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [RichTableList(AlwaysExpanded = true, ShowPaging = true, DrawScrollView = false, ShowIndexLabels = true, HideAddButton = true, HideRemoveButton = true, OnTitleBarGUI = nameof(OnTitleBarGUI))]
        private List<AllocateBundleInfo> BundleInfos
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                if (m_BundleInfos == null)
                {
                    RefreshPreview();
                }
                return m_BundleInfos;
            }
            set {}
        }

        private Rect OnTitleBarGUI(Rect rect)
        {
            var rectButton = rect.AlignRight(23f);
            rect.xMax = rectButton.xMin;
            if (GUI.Button(rectButton, GUIContent.none, SirenixGUIStyles.ToolbarButton))
            {
                try
                {
                    RefreshPreview();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
                Repaint();
            }
            // GUIHelper.PushColor(m_AssetInfos == null || m_AssetInfos.Count <= 0 ? Color.yellow : GUI.color);
            GUIHelper.PushColor(Color.yellow);
            SdfIcons.DrawIcon(rectButton.AlignCenter(13f), SdfIconType.ArrowRepeat);
            GUIHelper.PopColor();
            return rect;
        }

        private void RefreshPreview()
        {
            if (Package == null)
            {
                return;
            }
            var ignoreRule = AssetBundleCollectorSettingData.GetIgnoreRuleInstance(Package.IgnoreRuleName);
            var command = new CollectCommand(EBuildMode.SimulateBuild,
                                             Package.PackageName,
                                             Package.EnableAddressable,
                                             Package.LocationToLower,
                                             Package.LocationToLower,
                                             Package.AutoCollectShaders,
                                             Allocator.UniqueBundleName,
                                             ignoreRule);
            var group = AllocatorExport.ExportGroup(Group);
            var collector = group.GetCollector(Collector.CollectPath, Collector.CollectGUID);
            collector.CheckConfigError();
            var infos = collector.GetAllCollectAssets(command, group);

            m_AssetInfos = AllocatorConversion.ConvertToAllocateAssetInfos(infos);
            m_BundleInfos = AllocatorConversion.ConvertToAllocateBundleInfos(m_AssetInfos);
        }

        private void OnValueChanged()
        {
            m_OnCollectorChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnValueSaved()
        {
            m_OnCollectorSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}