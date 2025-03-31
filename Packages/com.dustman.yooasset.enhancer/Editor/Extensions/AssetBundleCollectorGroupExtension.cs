using System.Linq;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public static class AssetBundleCollectorGroupExtension
    {
        public static AssetBundleCollector GetCollector(this AssetBundleCollectorGroup group, string collectPath, string collectGUID)
        {
            return group.Collectors.FirstOrDefault(collector => collector.CollectPath == collectPath && collector.CollectorGUID == collectGUID);
        }
    }
}