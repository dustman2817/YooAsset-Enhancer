using System.Linq;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public static class AssetBundleCollectorPackageExtension
    {
        public static AssetBundleCollectorGroup GetGroup(this AssetBundleCollectorPackage package, string groupName)
        {
            return package.Groups.FirstOrDefault(group => group.GroupName == groupName);
        }

        public static AssetBundleCollector GetCollector(this AssetBundleCollectorPackage package, string groupName, string collectPath, string collectGUID)
        {
            var group = package.GetGroup(groupName);
            return group?.GetCollector(collectPath, collectGUID);
        }
    }
}