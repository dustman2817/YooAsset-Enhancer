using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class BuilderWindow : OdinEditorWindow
    {
        private const string k_ToolsMenuItemPath = "YooAsset/Extensions/Builder Window";
        private const int k_ToolsMenuItemPriority = 6001;

        [MenuItem(k_ToolsMenuItemPath, false, k_ToolsMenuItemPriority)]
        private static void OpenWindow()
        {
            var window = GetWindow<BuilderWindow>("Builder Window", false);
            window.Show();
        }

        private EventHandler m_OnBuilderChanged;

        public EventHandler OnBuilderChanged
        {
            get => m_OnBuilderChanged;
            set => m_OnBuilderChanged = value;
        }

        private EventHandler m_OnBuilderSaved;

        public EventHandler OnBuilderSaved
        {
            get => m_OnBuilderSaved;
            set => m_OnBuilderSaved = value;
        }

        private AssetBundleBuilder m_Builder;
        private string m_Exception;

        private bool IsException => !string.IsNullOrEmpty(m_Exception);

        [ShowInInspector]
        [InfoBox("$m_Exception", InfoMessageType.Error, "IsException")]
        [TitleGroup("Object", Indent = true)]
        [HorizontalGroup("Object/Builder", DisableAutomaticLabelWidth = true)]
        public AssetBundleBuilder Builder
        {
            get
            {
                if (IsException)
                {
                    return null;
                }
                if (m_Builder == null)
                {
                    try
                    {
                        m_Builder = AssetBundleBuilder.Instance;
                        m_Exception = null;
                    }
                    catch (Exception e)
                    {
                        m_Exception = e.Message;
                    }
                }
                if (m_Builder == null && !IsException)
                {
                    m_Exception = $"{nameof(AssetBundleBuilder)} was not found.";
                }
                return m_Builder;
            }
            set {}
        }

        [ShowIf("@IsDrawable")]
        [EnableIf("@IsDirty")]
        [TitleGroup("Object", Indent = true)]
        [HorizontalGroup("Object/Builder", Width = 100)]
        [Button("保存配置", ButtonSizes.Medium)]
        private void Save()
        {
            AssetDatabase.SaveAssetIfDirty(Builder);
            OnValueSaved();
        }

        [PropertySpace(10)]
        [ShowIf("@IsException")]
        [GUIColor("yellow")]
        [Button("刷新配置", ButtonSizes.Gigantic)]
        private void Refresh()
        {
            m_Builder = null;
            m_Exception = null;
            Repaint();
        }

        private bool IsExist => Builder != null;

        private bool IsDirty => IsExist && EditorUtility.IsDirty(Builder);

        private bool IsDrawable => IsExist;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        private BuildTarget ActiveBuildTarget => EditorUserBuildSettings.activeBuildTarget;

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        private string DefaultBuildOutputRoot => AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();

        [ShowInInspector]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        private string DefaultBuildinFileRoot => AssetBundleBuilderHelper.GetStreamingAssetsRoot();

        [ShowInInspector]
        [PropertyOrder(int.MaxValue)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Option", Indent = true)]
        [HideLabel]
        [TableList(DrawScrollView = false, AlwaysExpanded = true, HideToolbar = true)]
        [OnValueChanged("OnValueChanged", true)]
        private List<EncryptionServiceInfo> EncryptionServices
        {
            get
            {
                if (Builder == null)
                {
                    return null;
                }
                return Builder.EncryptionServices;
            }
            set {}
            // set
            // {
            //     if (!IsExist)
            //     {
            //         return;
            //     }
            //     Builder.EncryptionServices = value;
            //     OnValueChanged();
            // }
        }

        [ShowInInspector]
        [PropertyOrder(int.MaxValue)]
        [ShowIf("@IsDrawableProfiles")]
        [TitleGroup("Profile", Indent = true)]
        [HideLabel]
        [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
        [TableList(ShowIndexLabels = true, DrawScrollView = false, AlwaysExpanded = true)]
        [OnValueChanged("OnValueChanged", true)]
        public List<AssetBundleBuildProfile> Profiles
        {
            get
            {
                if (Builder == null)
                {
                    return null;
                }
                return Builder.Profiles;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Builder.Profiles = value;
                OnValueChanged();
            }
        }

        private bool IsDrawableProfiles => IsExist && Profiles != null && Profiles.Count > 0;

        private void OnValueChanged()
        {
            EditorUtility.SetDirty(Builder);
            m_OnBuilderChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnValueSaved()
        {
            m_OnBuilderSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}