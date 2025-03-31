using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using YooAsset.Editor;
using YooAsset.Enhancer.Runtime;

namespace YooAsset.Enhancer.Editor
{
    public class GroupWindow : OdinEditorWindow
    {
        private EventHandler m_OnGroupChanged;

        public EventHandler OnGroupChanged
        {
            get => m_OnGroupChanged;
            set => m_OnGroupChanged = value;
        }

        private EventHandler m_OnGroupSaved;

        public EventHandler OnGroupSaved
        {
            get => m_OnGroupSaved;
            set => m_OnGroupSaved = value;
        }

        private bool m_Selectable = true;

        public bool Selectable
        {
            get => m_Selectable;
            set => m_Selectable = value;
        }

        private AssetBundleAllocatorGroup m_Group;
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

        public AssetBundleAllocatorGroup Group
        {
            get
            {
                if (IsException)
                {
                    return null;
                }
                if (m_Group == null && !IsException)
                {
                    m_Exception = $"{nameof(AssetBundleAllocatorGroup)} was not found.";
                }
                return m_Group;
            }
            set
            {
                m_Group = value;
                m_Exception = null;
                m_AssetInfos = null;
            }
        }

        private bool IsExist => Group != null;

        private bool IsDrawable => IsExist;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Object", Indent = true)]
        private string GroupName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Group.GroupName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Group.GroupName = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Object", Indent = true)]
        private string GroupDesc
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Group.GroupDesc;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Group.GroupDesc = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [GUIColor(nameof(ActiveRuleNameColor))]
        [ValueDropdown(nameof(ActiveRuleNames))]
        private string ActiveRuleName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Group.ActiveRuleName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Group.ActiveRuleName = value;
                OnValueChanged();
            }
        }

        private Color ActiveRuleNameColor
        {
            get
            {
                if (!IsExist)
                {
                    return GUI.color;
                }
                return Group.ActiveRuleNameColor;
            }
        }

        private List<string> ActiveRuleNames => ActiveRule.GlobalActiveRuleNames;

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
                return Group.AssetTags;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Group.AssetTags = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(500)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Collectors", Indent = true)]
        [HideLabel]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [RichTableList(ShowIndexLabels = true, DrawScrollView = false, AlwaysExpanded = true, CustomAddFunction = nameof(OnAddCollector))]
        [OnValueChanged("OnValueChanged", true)]
        private List<AssetBundleAllocatorCollector> Collectors
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Group.Collectors;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Group.Collectors = value;
                OnValueChanged();
            }
        }

        private bool OnAddCollector()
        {
            Collectors.Add(new AssetBundleAllocatorCollector());
            OnValueChanged();
            return true;
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
                    // RefreshPreview();
                    m_AssetInfos = new List<AllocateAssetInfo>();
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
                    // RefreshPreview();
                    m_BundleInfos = new List<AllocateBundleInfo>();
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
            GUIHelper.PushColor(Color.red);
            SdfIcons.DrawIcon(rectButton.AlignCenter(13f), SdfIconType.ArrowRepeat);
            GUIHelper.PopColor();
            return rect;
        }

        private void RefreshPreview()
        {
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
            group.CheckConfigError();
            var infos = group.GetAllCollectAssets(command);

            m_AssetInfos = AllocatorConversion.ConvertToAllocateAssetInfos(infos);
            m_BundleInfos = AllocatorConversion.ConvertToAllocateBundleInfos(m_AssetInfos);
        }

        private void OnValueChanged()
        {
            m_OnGroupChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnValueSaved()
        {
            m_OnGroupSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}