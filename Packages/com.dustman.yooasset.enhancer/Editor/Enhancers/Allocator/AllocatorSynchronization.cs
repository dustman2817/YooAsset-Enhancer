using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class AllocatorSynchronization
    {
        public static void Import(AssetBundleCollectorSetting source, AssetBundleAllocator target)
        {
            target.ShowEditorAlias = source.ShowEditorAlias;
            target.ShowPackageView = source.ShowPackageView;
            target.UniqueBundleName = source.UniqueBundleName;

            if (source.Packages == null || source.Packages.Count <= 0)
            {
                target.Packages.Clear();
                return;
            }

            target.Packages = AllocatorImport.ImportPackages(source.Packages);
        }

        public static void Export(AssetBundleAllocator source, AssetBundleCollectorSetting target)
        {
            target.ShowEditorAlias = source.ShowEditorAlias;
            target.ShowPackageView = source.ShowPackageView;
            target.UniqueBundleName = source.UniqueBundleName;

            if (source.Packages == null || source.Packages.Count <= 0)
            {
                target.Packages.Clear();
                return;
            }

            target.Packages = AllocatorExport.ExportPackages(source.Packages);
        }
    }
}