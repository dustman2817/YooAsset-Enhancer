using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    [CreateAssetMenu(fileName = nameof(AssetBundleBuildProfileBuiltin), menuName = "YooAsset/Extensions/Create AssetBundle Build Profile - Builtin")]
    public class AssetBundleBuildProfileBuiltin : AssetBundleBuildProfile
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

        public override EBuildPipeline BuildPipeline => EBuildPipeline.BuiltinBuildPipeline;

        internal override List<EBuildMode> BuildModes
        {
            get
            {
                var modes = new List<EBuildMode>
                {
                    EBuildMode.ForceRebuild,
                    EBuildMode.IncrementalBuild,
                    EBuildMode.DryRunBuild,
                    EBuildMode.SimulateBuild
                };
                return modes;
            }
        }

        protected override BuildParameters GetBuildParameters()
        {
            // return new EnhancedBuiltinBuildParameters 可自定义扩展
            return new BuiltinBuildParameters
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
            // return new EnhancedBuiltinBuildPipeline(); 可自定义扩展
            return new BuiltinBuildPipeline();
        }

        [PropertyOrder(int.MaxValue)]
        [TableColumnWidth(44, false)]
        [Button(SdfIconType.GearFill)]
        public virtual void Detail()
        {
            var window = CreateInstance<BuildProfileWindowBuiltin>();
            window.Selectable = false;
            window.Profile = this;
            window.Show();
        }
    }
}