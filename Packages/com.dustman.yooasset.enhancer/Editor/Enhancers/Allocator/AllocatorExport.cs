using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class AllocatorExport
    {
        public static List<AssetBundleCollectorPackage> ExportPackages(List<AssetBundleAllocatorPackage> source)
        {
            var target = new List<AssetBundleCollectorPackage>();
            foreach (var package in source)
            {
                target.Add(ExportPackage(package));
            }
            return target;
        }

        public static AssetBundleCollectorPackage ExportPackage(AssetBundleAllocatorPackage source)
        {
            var target = new AssetBundleCollectorPackage();
            target.PackageName = source.PackageName;
            target.PackageDesc = source.PackageDesc;
            target.EnableAddressable = source.EnableAddressable;
            target.LocationToLower = source.LocationToLower;
            target.IncludeAssetGUID = source.IncludeAssetGUID;
            target.AutoCollectShaders = source.AutoCollectShaders;
            target.IgnoreRuleName = source.IgnoreRuleName;

            if (source.Groups == null || source.Groups.Count <= 0)
            {
                target.Groups = new List<AssetBundleCollectorGroup>();
            }
            else
            {
                target.Groups = new List<AssetBundleCollectorGroup>();
                foreach (var group in source.Groups)
                {
                    target.Groups.Add(ExportGroup(group));
                }
            }
            return target;
        }

        public static AssetBundleCollectorGroup ExportGroup(AssetBundleAllocatorGroup source)
        {
            var target = new AssetBundleCollectorGroup();
            target.GroupName = source.GroupName;
            target.GroupDesc = source.GroupDesc;
            target.AssetTags = source.AssetTags;
            target.ActiveRuleName = source.ActiveRuleName;

            if (source.Collectors == null || source.Collectors.Count <= 0)
            {
                target.Collectors = new List<AssetBundleCollector>();
            }
            else
            {
                target.Collectors = new List<AssetBundleCollector>();
                foreach (var collector in source.Collectors)
                {
                    target.Collectors.Add(ExportCollector(collector));
                }
            }
            return target;
        }

        public static AssetBundleCollector ExportCollector(AssetBundleAllocatorCollector source)
        {
            var target = new AssetBundleCollector();
            target.CollectPath = source.CollectPath;
            target.CollectorGUID = source.CollectGUID;
            target.CollectorType = source.CollectorType;
            target.AddressRuleName = source.AddressRuleName;
            target.PackRuleName = source.PackRuleName;
            target.FilterRuleName = source.FilterRuleName;
            target.AssetTags = source.AssetTags;
            target.UserData = source.UserData;
            return target;
        }
    }
}