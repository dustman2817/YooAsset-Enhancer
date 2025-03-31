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
    public class PackageWindow : OdinEditorWindow
    {
        private EventHandler m_OnPackageChanged;

        public EventHandler OnPackageChanged
        {
            get => m_OnPackageChanged;
            set => m_OnPackageChanged = value;
        }

        private EventHandler m_OnPackageSaved;

        public EventHandler OnPackageSaved
        {
            get => m_OnPackageSaved;
            set => m_OnPackageSaved = value;
        }

        private bool m_Selectable = true;

        public bool Selectable
        {
            get => m_Selectable;
            set => m_Selectable = value;
        }

        private AssetBundleAllocatorPackage m_Package;
        private string m_Exception;

        private bool IsException => !string.IsNullOrEmpty(m_Exception);

        private AssetBundleAllocator m_Allocator;

        public AssetBundleAllocator Allocator
        {
            get => m_Allocator;
            set => m_Allocator = value;
        }

        public AssetBundleAllocatorPackage Package
        {
            get
            {
                if (IsException)
                {
                    return null;
                }
                if (m_Package == null && !IsException)
                {
                    m_Exception = $"{nameof(AssetBundleAllocatorPackage)} was not found.";
                }
                return m_Package;
            }
            set
            {
                m_Package = value;
                m_Exception = null;
                m_AssetInfos = null;
            }
        }

        private bool IsExist => Package != null;

        private bool IsDrawable => IsExist;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Object", Indent = true)]
        private string PackageName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Package.PackageName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.PackageName = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Object", Indent = true)]
        private string PackageDesc
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Package.PackageDesc;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.PackageDesc = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("启用可寻址资源定位系统")]
        [TitleGroup("Setting", Indent = true)]
        private bool EnableAddressable
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Package.EnableAddressable;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.EnableAddressable = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("资源定位地址大小写不敏感")]
        [TitleGroup("Setting", Indent = true)]
        private bool LocationToLower
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Package.LocationToLower;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.LocationToLower = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("资源清单里包含资源 GUID 信息")]
        [TitleGroup("Setting", Indent = true)]
        private bool IncludeAssetGUID
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Package.IncludeAssetGUID;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.IncludeAssetGUID = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("将所有着色器构建到独立的资源包内")]
        [TitleGroup("Setting", Indent = true)]
        private bool AutoCollectShaders
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Package.AutoCollectShaders;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.AutoCollectShaders = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("忽略引擎无法识别的文件")]
        [TitleGroup("Setting", Indent = true)]
        [GUIColor(nameof(IgnoreRuleNameColor))]
        [ValueDropdown(nameof(IgnoreRuleNames))]
        private string IgnoreRuleName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Package.IgnoreRuleName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.IgnoreRuleName = value;
                OnValueChanged();
            }
        }

        private Color IgnoreRuleNameColor
        {
            get
            {
                if (!IsExist)
                {
                    return GUI.color;
                }
                return Package.IgnoreRuleNameColor;
            }
        }

        private List<string> IgnoreRuleNames => IgnoreRule.GlobalIgnoreRuleNames;

        [ShowInInspector]
        [PropertyOrder(500)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Groups", Indent = true)]
        [HideLabel]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [RichTableList(ShowIndexLabels = true, DrawScrollView = false, AlwaysExpanded = true, CustomAddFunction = nameof(OnAddGroup))]
        [OnValueChanged("OnValueChanged", true)]
        private List<AssetBundleAllocatorGroup> Groups
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Package.Groups;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Package.Groups = value;
                OnValueChanged();
            }
        }

        private bool OnAddGroup()
        {
            Groups.Add(new AssetBundleAllocatorGroup());
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
            var package = AllocatorExport.ExportPackage(Package);
            package.CheckConfigError();
            var infos = package.GetAllCollectAssets(command);

            m_AssetInfos = AllocatorConversion.ConvertToAllocateAssetInfos(infos);
            m_BundleInfos = AllocatorConversion.ConvertToAllocateBundleInfos(m_AssetInfos);
        }

        private void OnValueChanged()
        {
            m_OnPackageChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnValueSaved()
        {
            m_OnPackageSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}