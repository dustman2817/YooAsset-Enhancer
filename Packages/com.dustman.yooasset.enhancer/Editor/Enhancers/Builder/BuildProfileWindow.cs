using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public abstract class BuildProfileWindow : OdinEditorWindow
    {
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

        protected bool m_Selectable = true;

        public bool Selectable
        {
            get => m_Selectable;
            set => m_Selectable = value;
        }

        protected AssetBundleBuildProfile m_Profile;
        protected string m_Exception;

        protected bool IsException => !string.IsNullOrEmpty(m_Exception);

        [ShowInInspector]
        [PropertyOrder(-999)]
        [InfoBox("$m_Exception", InfoMessageType.Error, "IsException")]
        [TitleGroup("Object", Indent = true)]
        [HorizontalGroup("Object/Builder", DisableAutomaticLabelWidth = true)]
        [GUIColor(nameof(BuildPipelineColor))]
        public abstract AssetBundleBuildProfile Profile { get; set; }

        [ShowIf("@IsDrawable")]
        [EnableIf("@IsDirty")]
        [TitleGroup("Object", Indent = true)]
        [HorizontalGroup("Object/Builder", Width = 100)]
        [Button("保存配置", ButtonSizes.Medium)]
        private void Save()
        {
            AssetDatabase.SaveAssetIfDirty(Profile);
            OnValueSaved();
        }

        [PropertySpace(10)]
        [ShowIf("@IsException")]
        [GUIColor("yellow")]
        [Button("刷新配置", ButtonSizes.Gigantic)]
        private void Refresh()
        {
            m_Profile = null;
            m_Exception = null;
            Repaint();
        }

        protected bool IsExist => Profile != null;

        protected bool IsDirty => IsExist && EditorUtility.IsDirty(Profile);

        protected bool IsDrawable => IsExist;

        [ShowInInspector]
        [PropertyOrder(-990)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Package", Indent = true)]
        [GUIColor(nameof(BuildPipelineColor))]
        [ValueDropdown(nameof(PackageNames))]
        private string PackageName
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Profile.PackageName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.PackageName = value;
                OnValueChanged();
            }
        }

        private List<string> PackageNames => AssetBundleAllocator.Instance.PackageNames;

        [ShowInInspector]
        [PropertyOrder(-980)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Package", Indent = true)]
        [GUIColor(nameof(BuildPipelineColor))]
        private string PackageVersion
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Profile.PackageVersion;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.PackageVersion = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-980)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [GUIColor(nameof(BuildPipelineColor))]
        private EBuildPipeline BuildPipeline
        {
            get
            {
                if (!IsExist)
                {
                    return default;
                }
                return Profile.BuildPipeline;
            }
        }

        internal Color BuildPipelineColor => AssetBundleDrawer.GetBuildPipelineColor(BuildPipeline.ToString());

        [ShowInInspector]
        [PropertyOrder(-880)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [ValueDropdown(nameof(BuildModes))]
        private EBuildMode BuildMode
        {
            get
            {
                if (!IsExist)
                {
                    return EBuildMode.ForceRebuild;
                }
                return Profile.BuildMode;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.BuildMode = value;
                OnValueChanged();
            }
        }

        private List<EBuildMode> BuildModes
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Profile.BuildModes;
            }
        }

        [ShowInInspector]
        [PropertyOrder(-790)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [FolderPath]
        [LabelText("Output Path")]
        private string BuildOutputRoot
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Profile.BuildOutputRoot;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.BuildOutputRoot = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-780)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Setting", Indent = true)]
        [FolderPath]
        [LabelText("Output Path")]
        private string BuildinFileRoot
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Profile.BuildinFileRoot;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.BuildinFileRoot = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-648)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Option", Indent = true)]
        private bool VerifyBuildingResult
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Profile.VerifyBuildingResult;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.VerifyBuildingResult = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-647)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Option", Indent = true)]
        private EFileNameStyle FileNameStyle
        {
            get
            {
                if (!IsExist)
                {
                    return EFileNameStyle.HashName;
                }
                return Profile.FileNameStyle;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.FileNameStyle = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-646)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Option", Indent = true)]
        private EBuildinFileCopyOption BuildinFileCopyOption
        {
            get
            {
                if (!IsExist)
                {
                    return EBuildinFileCopyOption.None;
                }
                return Profile.BuildinFileCopyOption;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.BuildinFileCopyOption = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-645)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Option", Indent = true)]
        [ShowIf("ShowBuildinFileCopyParams")]
        [EnableIf("EnableBuildinFileCopyParams")]
        private string BuildinFileCopyParams
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return Profile.BuildinFileCopyParams;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.BuildinFileCopyParams = value;
                OnValueChanged();
            }
        }

        private bool ShowBuildinFileCopyParams
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Profile.ShowBuildinFileCopyParams;
            }
        }

        private bool EnableBuildinFileCopyParams
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return Profile.EnableBuildinFileCopyParams;
            }
        }

        [ShowInInspector]
        [PropertyOrder(-590)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Option", Indent = true)]
        [ValueDropdown(nameof(EncryptionServiceNames))]
        private string EncryptionServiceName
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Profile.EncryptionServiceName;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                Profile.EncryptionServiceName = value;
                OnValueChanged();
            }
        }

        private List<string> EncryptionServiceNames
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return Profile.EncryptionServiceNames;
            }
        }

        [PropertySpace(10f)]
        [ShowIf("@IsDrawable")]
        [GUIColor("green")]
        [Button(ButtonSizes.Gigantic)]
        private void Build()
        {
            if (!IsExist)
            {
                return;
            }
            Profile.Build();
        }

        protected void OnValueChanged()
        {
            EditorUtility.SetDirty(Profile);
            m_OnBuilderChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnValueSaved()
        {
            m_OnBuilderSaved?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Selection.selectionChanged += OnSelectionChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            if (!m_Selectable)
            {
                return;
            }
            Refresh();
        }
    }
}