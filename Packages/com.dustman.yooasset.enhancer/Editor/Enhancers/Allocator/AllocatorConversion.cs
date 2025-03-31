using System.Collections.Generic;
using System.Linq;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class AllocatorConversion
    {
        public static List<AllocateAssetInfo> ConvertToAllocateAssetInfos(List<CollectAssetInfo> source)
        {
            var target = new List<AllocateAssetInfo>();
            foreach (var info in source)
            {
                target.Add(ConvertToAllocateAssetInfo(info));
            }
            return target;
        }

        public static AllocateAssetInfo ConvertToAllocateAssetInfo(CollectAssetInfo source)
        {
            var target = new AllocateAssetInfo();
            target.AssetPath = source.AssetInfo.AssetPath;
            target.AssetGUID = source.AssetInfo.AssetGUID;
            target.BundleName = source.BundleName;
            target.AddressName = source.Address;
            return target;
        }

        public static List<AllocateBundleInfo> ConvertToAllocateBundleInfos(List<AllocateAssetInfo> source)
        {
            var target = new Dictionary<string, AllocateBundleInfo>();
            foreach (var info in source)
            {
                if (!target.TryGetValue(info.BundleName, out var bundleInfo))
                {
                    bundleInfo = new AllocateBundleInfo();
                    bundleInfo.BundleName = info.BundleName;
                    target.Add(info.BundleName, bundleInfo);
                }
                if (bundleInfo.AssetInfos == null)
                {
                    bundleInfo.AssetInfos = new List<AllocateAssetInfo>();
                }
                bundleInfo.AssetInfos.Add(info);
            }
            return target.Values.ToList();
        }
    }
}