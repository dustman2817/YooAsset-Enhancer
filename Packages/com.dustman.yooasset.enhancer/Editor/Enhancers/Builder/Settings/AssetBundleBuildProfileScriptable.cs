using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    [CreateAssetMenu(fileName = nameof(AssetBundleBuildProfileScriptable), menuName = "YooAsset/Extensions/Create AssetBundle Build Profile - Scriptable")]
    public class AssetBundleBuildProfileScriptable : AssetBundleBuildProfile
    {
        [FormerlySerializedAs("CompressOption")]
        [SerializeField]
        [PropertyOrder(-600 + 1)]
        [HideInTables]
        private ECompressOption m_CompressOption = ECompressOption.Uncompressed;

        /// <summary>
        /// 压缩选项
        /// </summary>
        public ECompressOption CompressOption
        {
            get => m_CompressOption;
            set => m_CompressOption = value;
        }

        [FormerlySerializedAs("DisableWriteTypeTree")]
        [SerializeField]
        [PropertyOrder(-600 + 1)]
        [HideInTables]
        private bool m_DisableWriteTypeTree;

        /// <summary>
        /// 禁止写入类型树结构（可以降低包体和内存并提高加载效率）
        /// </summary>
        public bool DisableWriteTypeTree
        {
            get => m_DisableWriteTypeTree;
            set => m_DisableWriteTypeTree = value;
        }

        [FormerlySerializedAs("IgnoreTypeTreeChanges")]
        [SerializeField]
        [PropertyOrder(-600 + 1)]
        [HideInTables]
        private bool m_IgnoreTypeTreeChanges = true;

        /// <summary>
        /// 忽略类型树变化
        /// </summary>
        public bool IgnoreTypeTreeChanges
        {
            get => m_IgnoreTypeTreeChanges;
            set => m_IgnoreTypeTreeChanges = value;
        }

        [FormerlySerializedAs("WriteLinkXML")]
        [SerializeField]
        [PropertyOrder(-600 + 1)]
        [HideInTables]
        private bool m_WriteLinkXML = true;

        /// <summary>
        /// 生成代码防裁剪配置
        /// </summary>
        public bool WriteLinkXML
        {
            get => m_WriteLinkXML;
            set => m_WriteLinkXML = value;
        }

        [FormerlySerializedAs("CacheServerHost")]
        [SerializeField]
        [PropertyOrder(-600 + 1)]
        [HideInTables]
        private string m_CacheServerHost;

        /// <summary>
        /// 缓存服务器地址
        /// </summary>
        public string CacheServerHost
        {
            get => m_CacheServerHost;
            set => m_CacheServerHost = value;
        }

        [FormerlySerializedAs("CacheServerPort")]
        [SerializeField]
        [PropertyOrder(-600 + 1)]
        [HideInTables]
        private int m_CacheServerPort;

        /// <summary>
        /// 缓存服务器端口
        /// </summary>
        public int CacheServerPort
        {
            get => m_CacheServerPort;
            set => m_CacheServerPort = value;
        }

        public override EBuildPipeline BuildPipeline => EBuildPipeline.ScriptableBuildPipeline;

        internal override List<EBuildMode> BuildModes
        {
            get
            {
                var modes = new List<EBuildMode>
                {
                    EBuildMode.IncrementalBuild,
                    EBuildMode.SimulateBuild
                };
                return modes;
            }
        }

        protected override BuildParameters GetBuildParameters()
        {
            // return new EnhancedScriptableBuildParameters 可自定义扩展
            return new ScriptableBuildParameters
            {
                BuildOutputRoot = BuildOutputRoot,
                BuildinFileRoot = BuildinFileRoot,
                BuildPipeline = BuildPipelineName,
                BuildTarget = BuildTarget,
                BuildMode = BuildMode,
                PackageName = PackageName,
                PackageVersion = PackageVersion,
                EnableSharePackRule = EnableSharePackRule,
                VerifyBuildingResult = true,
                FileNameStyle = FileNameStyle,
                BuildinFileCopyOption = BuildinFileCopyOption,
                BuildinFileCopyParams = BuildinFileCopyParams,
                EncryptionServices = EncryptionServiceInstance,
                CompressOption = CompressOption
            };
        }

        protected override IBuildPipeline GetBuildPipeline()
        {
            // return new EnhancedScriptableBuildPipeline(); 可自定义扩展
            return new ScriptableBuildPipeline();
        }

        [PropertyOrder(int.MaxValue)]
        [TableColumnWidth(44, false)]
        [Button(SdfIconType.GearFill)]
        public virtual void Detail()
        {
            var window = CreateInstance<BuildProfileWindowScriptable>();
            window.Selectable = false;
            window.Profile = this;
            window.Show();
        }
    }
}