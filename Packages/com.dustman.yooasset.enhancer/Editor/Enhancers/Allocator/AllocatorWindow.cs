using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using YooAsset.Enhancer.Runtime;

namespace YooAsset.Enhancer.Editor
{
    public class AllocatorWindow : OdinEditorWindow
    {
        private const string k_ToolsMenuItemPath = "YooAsset/Extensions/Allocator Window";
        private const int k_ToolsMenuItemPriority = 4001;

        [MenuItem(k_ToolsMenuItemPath, false, k_ToolsMenuItemPriority)]
        private static void OpenWindow()
        {
            var window = GetWindow<AllocatorWindow>("Allocator Window", false);
            window.Show();
        }

        private EventHandler m_OnAllocatorChanged;

        public EventHandler OnAllocatorChanged
        {
            get => m_OnAllocatorChanged;
            set => m_OnAllocatorChanged = value;
        }

        private EventHandler m_OnAllocatorSaved;

        public EventHandler OnAllocatorSaved
        {
            get => m_OnAllocatorSaved;
            set => m_OnAllocatorSaved = value;
        }

        private AssetBundleAllocator m_Allocator;
        private string m_Exception;

        private bool IsException => !string.IsNullOrEmpty(m_Exception);

        [ShowInInspector]
        [InfoBox("$m_Exception", InfoMessageType.Error, "IsException")]
        [TitleGroup("Object", Indent = true)]
        [HorizontalGroup("Object/Allocator", DisableAutomaticLabelWidth = true)]
        public AssetBundleAllocator Allocator
        {
            get
            {
                if (IsException)
                {
                    return null;
                }
                if (m_Allocator == null)
                {
                    try
                    {
                        m_Allocator = AssetBundleAllocator.Instance;
                        m_Exception = null;
                    }
                    catch (Exception e)
                    {
                        m_Exception = e.Message;
                    }
                }
                if (m_Allocator == null && !IsException)
                {
                    m_Exception = $"{nameof(AssetBundleAllocator)} was not found.";
                }
                return m_Allocator;
            }
            set {}
        }

        [ShowIf("@IsDrawable")]
        [EnableIf("@IsDirty")]
        [TitleGroup("Object", Indent = true)]
        [HorizontalGroup("Object/Allocator", Width = 100)]
        [Button("保存配置", ButtonSizes.Medium)]
        private void Save()
        {
            AssetDatabase.SaveAssetIfDirty(Allocator);
            OnValueSaved();
        }

        [PropertySpace(10)]
        [ShowIf("@IsException")]
        [GUIColor("yellow")]
        [Button("刷新配置", ButtonSizes.Gigantic)]
        private void Refresh()
        {
            m_Allocator = null;
            m_Exception = null;
            Repaint();
        }

        private bool IsExist => Allocator != null;

        private bool IsDirty => IsExist && EditorUtility.IsDirty(Allocator);

        private bool IsDrawable => IsExist;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("是否展示资源包列表视图（自定义绘制后，已不需要此属性）")]
        [TitleGroup("Setting", Indent = true)]
        [ReadOnly]
        private bool ShowPackageView
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Allocator.ShowPackageView;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Allocator.ShowPackageView = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("是否显示为中文模式（自定义绘制后，已不需要此属性）")]
        [TitleGroup("Setting", Indent = true)]
        [ReadOnly]
        private bool ShowEditorAlias
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Allocator.ShowEditorAlias;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Allocator.ShowEditorAlias = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [PropertyTooltip("资源包名追加 PackageName 作为前缀")]
        [TitleGroup("Setting", Indent = true)]
        private bool UniqueBundleName
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Allocator.UniqueBundleName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Allocator.UniqueBundleName = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(500)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Packages", Indent = true)]
        [HideLabel]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [RichTableList(ShowIndexLabels = true, DrawScrollView = false, AlwaysExpanded = true, CustomAddFunction = nameof(OnAddPackage))]
        [OnValueChanged("OnValueChanged", true)]
        private List<AssetBundleAllocatorPackage> Packages
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Allocator.Packages;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Allocator.Packages = value;
                OnValueChanged();
            }
        }

        private bool OnAddPackage()
        {
            Packages.Add(new AssetBundleAllocatorPackage());
            OnValueChanged();
            return true;
        }

        private void OnValueChanged()
        {
            EditorUtility.SetDirty(Allocator);
            m_OnAllocatorChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnValueSaved()
        {
            m_OnAllocatorSaved?.Invoke(this, EventArgs.Empty);
        }

        [PropertySpace(10f)]
        [PropertyOrder(999)]
        [PropertyTooltip("注意！！！此操作会覆盖 AssetBundleAllocator 内数据！！！请谨慎操作！！！")]
        [ShowIf("@IsDrawable")]
        [GUIColor("red")]
        [Button("导入自 YooAsset -> AssetBundleCollector 配置", ButtonSizes.Large)]
        private void Import()
        {
            var result = EditorUtility.DisplayDialog("注意", "此操作会覆盖 AssetBundleAllocator 内数据！！！是否确认？", "确认", "取消");
            if (result)
            {
                Allocator.Import();
            }
        }

        [PropertySpace(10f)]
        [PropertyOrder(999)]
        [PropertyTooltip("因为 Allocator 并未完全重写 YooAsset 内部的构建逻辑，故需要把配置转换成 Collector")]
        [ShowIf("@IsDrawable")]
        [GUIColor("blue")]
        [Button("导出至 YooAsset -> AssetBundleCollector 配置", ButtonSizes.Gigantic)]
        private void Export()
        {
            Allocator.Export();
        }
    }
}