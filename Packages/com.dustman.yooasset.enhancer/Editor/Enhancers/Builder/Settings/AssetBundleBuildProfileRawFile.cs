using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    [CreateAssetMenu(fileName = nameof(AssetBundleBuildProfileRawFile), menuName = "YooAsset/Extensions/Create AssetBundle Build Profile - RawFile")]
    public class AssetBundleBuildProfileRawFile : AssetBundleBuildProfile
    {
        public override EBuildPipeline BuildPipeline => EBuildPipeline.RawFileBuildPipeline;

        internal override List<EBuildMode> BuildModes
        {
            get
            {
                var modes = new List<EBuildMode>
                {
                    EBuildMode.ForceRebuild,
                    EBuildMode.SimulateBuild
                };
                return modes;
            }
        }

        protected override BuildParameters GetBuildParameters()
        {
            // return new EnhancedRawFileBuildParameters 可自定义扩展
            return new RawFileBuildParameters
            {
                BuildOutputRoot = BuildOutputRoot,
                BuildinFileRoot = BuildinFileRoot,
                BuildPipeline = BuildPipelineName,
                BuildTarget = BuildTarget,
                BuildMode = BuildMode,
                PackageName = PackageName,
                PackageVersion = PackageVersion,
                VerifyBuildingResult = true,
                FileNameStyle = FileNameStyle,
                BuildinFileCopyOption = BuildinFileCopyOption,
                BuildinFileCopyParams = BuildinFileCopyParams,
                EncryptionServices = EncryptionServiceInstance
            };
        }

        protected override IBuildPipeline GetBuildPipeline()
        {
            // return new EnhancedRawFileBuildPipeline(); 可自定义扩展
            return new RawFileBuildPipeline();
        }

        [PropertyOrder(int.MaxValue)]
        [TableColumnWidth(44, false)]
        [Button(SdfIconType.GearFill)]
        public virtual void Detail()
        {
            var window = CreateInstance<BuildProfileWindowRawFile>();
            window.Selectable = false;
            window.Profile = this;
            window.Show();
        }
    }
}