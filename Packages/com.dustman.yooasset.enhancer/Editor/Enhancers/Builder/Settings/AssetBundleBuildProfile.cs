using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public abstract class AssetBundleBuildProfile : ScriptableObject, ISearchFilterable
    {
        /// <summary>
        /// 分组对象
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-999)]
        [GUIColor(nameof(BuildPipelineColor))]
        public AssetBundleBuildProfile ProfileObject
        {
            get => this;
            set {}
        }

        [ShowInInspector]
        [PropertyOrder(-999)]
        [HideInTables]
        [GUIColor(nameof(BuildPipelineColor))]
        public string BuilderName
        {
            get
            {
                if (ProfileObject == null)
                {
                    return string.Empty;
                }
                return ProfileObject.name;
            }
        }

        [FormerlySerializedAs("PackageName")]
        [SerializeField]
        [PropertyOrder(-990)]
        [GUIColor(nameof(BuildPipelineColor))]
        [ValueDropdown(nameof(PackageNames))]
        private string m_PackageName;

        private List<string> PackageNames => AssetBundleAllocator.Instance.PackageNames;

        /// <summary>
        /// 构建的包裹名称
        /// </summary>
        public string PackageName
        {
            get => m_PackageName;
            set
            {
                m_PackageName = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("PackageVersion")]
        [SerializeField]
        [PropertyOrder(-980)]
        [TableColumnWidth(60)]
        [GUIColor(nameof(BuildPipelineColor))]
        private string m_PackageVersion;

        /// <summary>
        /// 构建的包裹版本
        /// </summary>
        public string PackageVersion // TODO: 建议改为自己项目的版本号规则
        {
            get => m_PackageVersion;
            set
            {
                m_PackageVersion = value;
                EditorUtility.SetDirty(this);
            }
        }

        // [FormerlySerializedAs("BuildPipeline")]
        // [SerializeField]
        // [ValueDropdown(nameof(BuildPipelines))]
        // private string m_BuildPipeline;

        // private string[] BuildPipelines => Enum.GetNames(typeof(EBuildPipeline));

        /// <summary>
        /// 构建管线
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-890)]
        [HideInTables]
        [GUIColor(nameof(BuildPipelineColor))]
        // [ValueDropdown(nameof(BuildPipelines))]
        public abstract EBuildPipeline BuildPipeline { get; }

        internal Color BuildPipelineColor => AssetBundleDrawer.GetBuildPipelineColor(BuildPipeline.ToString());

        protected string BuildPipelineName => BuildPipeline.ToString();

        [FormerlySerializedAs("BuildMode")]
        [SerializeField]
        [PropertyOrder(-880)]
        [ValueDropdown(nameof(BuildModes))]
        private EBuildMode m_BuildMode = EBuildMode.ForceRebuild;

        internal abstract List<EBuildMode> BuildModes { get; }

        /// <summary>
        /// 构建模式
        /// </summary>
        public EBuildMode BuildMode
        {
            get => m_BuildMode;
            set
            {
                m_BuildMode = value;
                EditorUtility.SetDirty(this);
            }
        }

        // [FormerlySerializedAs("BuildTarget")]
        // [SerializeField]
        // [HideInInspector]
        // private BuildTarget m_BuildTarget = BuildTarget.NoTarget;

        /// <summary>
        /// 构建的平台
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-870)]
        [HideInTables]
        public BuildTarget BuildTarget
        {
            get => EditorUserBuildSettings.activeBuildTarget;
            // get
            // {
            //     if (m_BuildTarget == BuildTarget.NoTarget)
            //     {
            //         return EditorUserBuildSettings.activeBuildTarget;
            //     }
            //     return m_BuildTarget;
            //     return EditorUserBuildSettings.activeBuildTarget;
            // }
            // set
            // {
            //     m_BuildTarget = value;
            //     EditorUtility.SetDirty(this);
            // }
        }

        [FormerlySerializedAs("BuildOutputRoot")]
        [SerializeField]
        [HideInInspector]
        private string m_BuildOutputRoot;

        /// <summary>
        /// 构建输出的根目录
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-790)]
        [HideInTables]
        [FolderPath]
        [LabelText("Output Path")]
        public string BuildOutputRoot
        {
            get
            {
                if (string.IsNullOrEmpty(m_BuildOutputRoot))
                {
                    return AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
                }
                return m_BuildOutputRoot;
            }
            set
            {
                m_BuildOutputRoot = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("BuildinFileRoot")]
        [SerializeField]
        [HideInInspector]
        private string m_BuildinFileRoot;

        /// <summary>
        /// 内置文件的根目录
        /// </summary>
        [ShowInInspector]
        [PropertyOrder(-780)]
        [HideInTables]
        [FolderPath]
        [LabelText("Builtin Path")]
        public string BuildinFileRoot
        {
            get
            {
                if (string.IsNullOrEmpty(m_BuildinFileRoot))
                {
                    return AssetBundleBuilderHelper.GetStreamingAssetsRoot();
                }
                return m_BuildinFileRoot;
            }
            set
            {
                m_BuildinFileRoot = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("EnableSharePackRule")]
        [SerializeField]
        [PropertyOrder(-650)]
        [HideInTables]
        private bool m_EnableSharePackRule = true;

        /// <summary>
        /// 是否启用共享资源打包
        /// </summary>
        public virtual bool EnableSharePackRule
        {
            get => m_EnableSharePackRule;
            set
            {
                m_EnableSharePackRule = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("VerifyBuildingResult")]
        [SerializeField]
        [PropertyOrder(-648)]
        [HideInTables]
        private bool m_VerifyBuildingResult = true;

        /// <summary>
        /// 验证构建结果
        /// </summary>
        public bool VerifyBuildingResult
        {
            get => m_VerifyBuildingResult;
            set
            {
                m_VerifyBuildingResult = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("FileNameStyle")]
        [SerializeField]
        [PropertyOrder(-647)]
        // [HideInTables]
        private EFileNameStyle m_FileNameStyle = EFileNameStyle.HashName;

        /// <summary>
        /// 资源包名称样式
        /// </summary>
        public EFileNameStyle FileNameStyle
        {
            get => m_FileNameStyle;
            set
            {
                m_FileNameStyle = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("BuildinFileCopyOption")]
        [SerializeField]
        [PropertyOrder(-646)]
        [HideInTables]
        private EBuildinFileCopyOption m_BuildinFileCopyOption = EBuildinFileCopyOption.None;

        /// <summary>
        /// 内置文件的拷贝选项
        /// </summary>
        public EBuildinFileCopyOption BuildinFileCopyOption
        {
            get => m_BuildinFileCopyOption;
            set
            {
                m_BuildinFileCopyOption = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("BuildinFileCopyParams")]
        [SerializeField]
        [PropertyOrder(-645)]
        [HideInTables]
        [ShowIf("ShowBuildinFileCopyParams")]
        [EnableIf("EnableBuildinFileCopyParams")]
        private string m_BuildinFileCopyParams;

        internal bool ShowBuildinFileCopyParams => EnableBuildinFileCopyParams;

        internal bool EnableBuildinFileCopyParams => m_BuildinFileCopyOption is EBuildinFileCopyOption.ClearAndCopyByTags or EBuildinFileCopyOption.OnlyCopyByTags;

        /// <summary>
        /// 内置文件的拷贝参数
        /// </summary>
        public string BuildinFileCopyParams
        {
            get => m_BuildinFileCopyParams;
            set
            {
                m_BuildinFileCopyParams = value;
                EditorUtility.SetDirty(this);
            }
        }

        [FormerlySerializedAs("EncryptionServices")]
        [SerializeField]
        [PropertyOrder(-590)]
        [GUIColor(nameof(EncryptionServiceNameColor))]
        [ValueDropdown(nameof(EncryptionServiceNames))]
        private string m_EncryptionServiceName = nameof(EncryptionNone);

        internal Color EncryptionServiceNameColor => AssetBundleDrawer.GetEncryptionServiceColor(m_EncryptionServiceName);

        internal List<string> EncryptionServiceNames => EncryptionService.GlobalEncryptionServiceNames;

        /// <summary>
        /// 资源包加密服务类
        /// </summary>
        public string EncryptionServiceName
        {
            get => m_EncryptionServiceName;
            set
            {
                m_EncryptionServiceName = value;
                EditorUtility.SetDirty(this);
            }
        }

        protected IEncryptionServices EncryptionServiceInstance
        {
            get
            {
                var instance = EncryptionService.GetEncryptionServiceInstance(m_EncryptionServiceName);
                if (instance == null)
                {
                    throw new Exception($"Not found {m_EncryptionServiceName} encryption service.");
                }
                return instance;
            }
        }

        [GUIColor("green")]
        [TableColumnWidth(100, false)]
        [Button(DrawResult = false)]
        public virtual BuildResult Build()
        {
            AssetBundleAllocator.Instance.Export(); // 每次构建前，强制导出一次

            var parameters = GetBuildParameters();
            if (parameters == null)
            {
                throw new Exception("BuildParameters is null.");
            }
            var pipeline = GetBuildPipeline();
            if (pipeline == null)
            {
                throw new Exception("BuildPipeline is null.");
            }
            var result = pipeline.Run(parameters, true);
            if (result.Success)
            {
                EditorUtility.RevealInFinder(result.OutputPackageDirectory);
            }
            return result;
        }

        protected abstract BuildParameters GetBuildParameters();

        protected abstract IBuildPipeline GetBuildPipeline();

        public virtual bool IsMatch(string searchString)
        {
            if (BuilderName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}