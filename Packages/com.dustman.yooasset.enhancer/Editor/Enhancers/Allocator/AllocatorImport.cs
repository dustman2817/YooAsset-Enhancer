using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class AllocatorImport
    {
        public static List<AssetBundleAllocatorPackage> ImportPackages(List<AssetBundleCollectorPackage> source)
        {
            var target = new List<AssetBundleAllocatorPackage>();
            foreach (var package in source)
            {
                target.Add(ImportPackage(package));
            }
            return target;
        }

        public static AssetBundleAllocatorPackage ImportPackage(AssetBundleCollectorPackage source)
        {
            var target = new AssetBundleAllocatorPackage();
            target.PackageName = source.PackageName;
            target.PackageDesc = source.PackageDesc;
            target.EnableAddressable = source.EnableAddressable;
            target.LocationToLower = source.LocationToLower;
            target.IncludeAssetGUID = source.IncludeAssetGUID;
            target.AutoCollectShaders = source.AutoCollectShaders;
            target.IgnoreRuleName = source.IgnoreRuleName;

            if (source.Groups == null || source.Groups.Count <= 0)
            {
                target.Groups = new List<AssetBundleAllocatorGroup>();
            }
            else
            {
                target.Groups = new List<AssetBundleAllocatorGroup>();
                foreach (var group in source.Groups)
                {
                    target.Groups.Add(ImportGroup(group));
                }
            }
            return target;
        }

        public static AssetBundleAllocatorGroup ImportGroup(AssetBundleCollectorGroup source)
        {
            var target = new AssetBundleAllocatorGroup();
            target.GroupName = source.GroupName;
            target.GroupDesc = source.GroupDesc;
            target.AssetTags = source.AssetTags;
            target.ActiveRuleName = source.ActiveRuleName;

            if (source.Collectors == null || source.Collectors.Count <= 0)
            {
                target.Collectors = new List<AssetBundleAllocatorCollector>();
            }
            else
            {
                target.Collectors = new List<AssetBundleAllocatorCollector>();
                foreach (var collector in source.Collectors)
                {
                    target.Collectors.Add(ImportCollector(collector));
                }
            }
            return target;
        }

        public static AssetBundleAllocatorCollector ImportCollector(AssetBundleCollector source)
        {
            var target = new AssetBundleAllocatorCollector();
            target.CollectTarget = AssetDatabase.LoadAssetAtPath<Object>(source.CollectPath);
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