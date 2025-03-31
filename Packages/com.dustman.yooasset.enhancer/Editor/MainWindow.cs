using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public partial class MainWindow : OdinMenuEditorWindow, IHasCustomMenu
    {
        [Serializable]
        internal abstract class MainMenuItem
        {
            public string Path;
            public EditorWindow Window;
            public object Target;
            public SdfIconType IconType;
        }

        private const string k_ToolsMenuItemPath = "YooAsset/Extensions/Main Window";
        private const int k_ToolsMenuItemPriority = 2000;

        [MenuItem(k_ToolsMenuItemPath, false, k_ToolsMenuItemPriority)]
        private static void OpenWindow()
        {
            var window = (MainWindow)GetWindow(typeof(MainWindow), false, "YooAsset");
            window.Show();
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(EditorGUIUtility.TrTempContent("Refresh"), false, Refresh);
        }

        private void Refresh()
        {
            ForceMenuTreeRebuild();
        }

        private const string k_DrawerMenuPath = "绘制设置";
        private const string k_AllocatorMenuPath = "资源配置";
        private const string k_BuilderMenuPath = "资源构建";

        // private MainMenuItem m_CurMainMenuItem;

        private Dictionary<string, MainMenuItem> m_AllMainMenuItems;

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = CreateMenuTree();

            // 收集
            tree.Add(k_DrawerMenuPath, DrawerWindow, EditorIcons.StarPointer);
            tree.Add(k_AllocatorMenuPath, AllocatorWindow, EditorIcons.List);
            tree.Add(k_BuilderMenuPath, BuilderWindow, SdfIconType.FileZip);

            if (m_AllMainMenuItems == null)
            {
                m_AllMainMenuItems = new Dictionary<string, MainMenuItem>();
            }
            m_AllMainMenuItems.Clear();

            var allocatorMenuItems = GetAllocatorMenuItems();
            AddTreeItems(tree, allocatorMenuItems);

            var builderMenuItems = GetBuilderMenuItems();
            AddTreeItems(tree, builderMenuItems);

            // 事件
            tree.Selection.SelectionChanged += OnSelectionChanged;
            // tree.Selection.SelectionConfirmed += OnSelectionConfirmed;

            return tree;
        }

        private OdinMenuTree CreateMenuTree()
        {
            var tree = new OdinMenuTree(false)
            {
                Config = new OdinMenuTreeDrawingConfig()
                {
                    AutoHandleKeyboardNavigation = false,
                    // UseCachedExpandedStates = false,
                    DrawSearchToolbar = false
                },
                DefaultMenuStyle = new OdinMenuStyle()
                {
                    Height = 24,
                    Offset = 24,
                    IconPadding = 2,
                    IndentAmount = 15,
                    TriangleSize = 18,
                    TrianglePadding = 0,
                    AlignTriangleLeft = true,
                    // Borders = true,
                    // BorderPadding = 0,
                    // BorderAlpha = 1
                },
            };
            return tree;
        }

        private void AddTreeItems(OdinMenuTree tree, Dictionary<string, MainMenuItem> items)
        {
            if (items == null)
            {
                return;
            }
            foreach (var (key, value) in items)
            {
                AddTreeItem(tree, value);
                m_AllMainMenuItems.TryAdd(key, value);
            }
        }

        private void AddTreeItem(OdinMenuTree tree, MainMenuItem item)
        {
            if (item == null)
            {
                return;
            }
            var path = item.Path;
            var value = item.Window ?? item.Target;
            if (item.IconType != SdfIconType.None)
            {
                tree.Add(path, value, item.IconType);
            }
            else
            {
                tree.Add(path, value);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuItem in m_AllMainMenuItems.Values)
            {
                DestroyImmediate(menuItem?.Window);
            }
        }

        public void OnSelectionChanged(SelectionChangedType type)
        {
            // m_CurMainMenuItem = null;
            if (type == SelectionChangedType.SelectionCleared)
            {
                return;
            }

            var odinMenuItem = MenuTree.Selection[0];
            if (odinMenuItem == null)
            {
                return;
            }

            var path = odinMenuItem.GetFullPath();
            if (path == k_AllocatorMenuPath)
            {
                AllocatorWindow.OnAllocatorChanged = OnAllocatorChanged;
                AllocatorWindow.OnAllocatorSaved = OnAllocatorSaved;
                return;
            }
            if (path == k_BuilderMenuPath)
            {
                BuilderWindow.OnBuilderSaved = OnBuilderSaved;
                return;
            }

            if (m_AllMainMenuItems == null)
            {
                return;
            }
            m_AllMainMenuItems.TryGetValue(path, out var menuItem);
            if (menuItem == null)
            {
                return;
            }

            // Allocator
            if (menuItem is AllocatorMenuItem allocatorMenuItem)
            {
                OnAllocatorSelectionChanged(odinMenuItem, allocatorMenuItem);
            }

            // Builder
            if (menuItem is BuilderMenuItem builderMenuItem)
            {
                OnBuilderSelectionChanged(odinMenuItem, builderMenuItem);
            }

            // m_CurMainMenuItem = menuItem;
        }
    }

    // Drawer
    public partial class MainWindow
    {
        private AssetBundleDrawer DrawerWindow => AssetBundleDrawer.Instance;
    }

    // Allocator
    public partial class MainWindow
    {
        internal enum AllocatorType
        {
            Package,
            Group,
            Collector
        }

        [Serializable]
        internal class AllocatorMenuItem : MainMenuItem
        {
            public AllocatorType type;
        }

        private AllocatorWindow m_AllocatorWindow;

        private AllocatorWindow AllocatorWindow
        {
            get
            {
                if (m_AllocatorWindow == null)
                {
                    m_AllocatorWindow = CreateInstance<AllocatorWindow>();
                    m_AllocatorWindow.OnAllocatorChanged = OnAllocatorChanged;
                    m_AllocatorWindow.OnAllocatorSaved = OnAllocatorSaved;
                }
                return m_AllocatorWindow;
            }
        }

        private PackageWindow m_PackageWindow;

        private PackageWindow PackageWindow
        {
            get
            {
                if (m_PackageWindow == null)
                {
                    m_PackageWindow = CreateInstance<PackageWindow>();
                    m_PackageWindow.Selectable = false;
                    m_PackageWindow.OnPackageChanged = OnAllocatorChanged;
                    m_PackageWindow.OnPackageSaved = OnAllocatorSaved;
                }
                return m_PackageWindow;
            }
        }

        private GroupWindow m_GroupWindow;

        private GroupWindow GroupWindow
        {
            get
            {
                if (m_GroupWindow == null)
                {
                    m_GroupWindow = CreateInstance<GroupWindow>();
                    m_GroupWindow.Selectable = false;
                    m_GroupWindow.OnGroupChanged = OnAllocatorChanged;
                    m_GroupWindow.OnGroupSaved = OnAllocatorSaved;
                }
                return m_GroupWindow;
            }
        }

        private CollectorWindow m_CollectorWindow;

        private CollectorWindow CollectorWindow
        {
            get
            {
                if (m_CollectorWindow == null)
                {
                    m_CollectorWindow = CreateInstance<CollectorWindow>();
                    m_CollectorWindow.Selectable = false;
                    m_CollectorWindow.OnCollectorChanged = OnAllocatorChanged;
                    m_CollectorWindow.OnCollectorSaved = OnAllocatorSaved;
                }
                return m_CollectorWindow;
            }
        }

        private void OnAllocatorChanged(object sender, EventArgs args)
        {
            EditorUtility.SetDirty(AllocatorWindow.Allocator);
        }

        private void OnAllocatorSaved(object sender, EventArgs args)
        {
            Refresh();
        }

        private void OnAllocatorSelectionChanged(OdinMenuItem odinMenuItem, AllocatorMenuItem allocatorMenuItem)
        {
            PackageWindow.Package = null;
            GroupWindow.Group = null;
            CollectorWindow.Collector = null;
            switch (allocatorMenuItem.type)
            {
                case AllocatorType.Package:
                    PackageWindow.Allocator = AllocatorWindow.Allocator;
                    PackageWindow.Package = (AssetBundleAllocatorPackage)allocatorMenuItem.Target;
                    PackageWindow.OnPackageChanged = OnAllocatorChanged;
                    PackageWindow.OnPackageSaved = OnAllocatorSaved;
                    break;
                case AllocatorType.Group:
                    GroupWindow.Allocator = AllocatorWindow.Allocator;
                    GroupWindow.Group = (AssetBundleAllocatorGroup)allocatorMenuItem.Target;
                    GroupWindow.OnGroupChanged = OnAllocatorChanged;
                    GroupWindow.OnGroupSaved = OnAllocatorSaved;
                    foreach (var item in odinMenuItem.GetParentMenuItemsRecursive(false))
                    {
                        m_AllMainMenuItems.TryGetValue(item.GetFullPath(), out var parentMenuItem);
                        if (parentMenuItem is AllocatorMenuItem parentAllocatorMenuItem)
                        {
                            if (parentAllocatorMenuItem.type == AllocatorType.Package)
                            {
                                var parentPackage = (AssetBundleAllocatorPackage)parentAllocatorMenuItem.Target;
                                GroupWindow.Package = parentPackage.PackageObject;
                            }
                        }
                    }
                    break;
                case AllocatorType.Collector:
                    CollectorWindow.Allocator = AllocatorWindow.Allocator;
                    CollectorWindow.Collector = (AssetBundleAllocatorCollector)allocatorMenuItem.Target;
                    CollectorWindow.OnCollectorChanged = OnAllocatorChanged;
                    CollectorWindow.OnCollectorSaved = OnAllocatorSaved;
                    foreach (var item in odinMenuItem.GetParentMenuItemsRecursive(false))
                    {
                        m_AllMainMenuItems.TryGetValue(item.GetFullPath(), out var parentMenuItem);
                        if (parentMenuItem is AllocatorMenuItem parentAllocatorMenuItem)
                        {
                            if (parentAllocatorMenuItem.type == AllocatorType.Package)
                            {
                                var parentPackage = (AssetBundleAllocatorPackage)parentAllocatorMenuItem.Target;
                                CollectorWindow.Package = parentPackage.PackageObject;
                            }
                            else if (parentAllocatorMenuItem.type == AllocatorType.Group)
                            {
                                var parentGroup = (AssetBundleAllocatorGroup)parentAllocatorMenuItem.Target;
                                CollectorWindow.Group = parentGroup.GroupObject;
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Invalid allocator type.");
            }
        }

        private Dictionary<string, MainMenuItem> GetAllocatorMenuItems()
        {
            var allocator = AllocatorWindow.Allocator;
            if (allocator == null)
            {
                return null;
            }

            var menus = new Dictionary<string, MainMenuItem>();

            // Packages
            var packages = allocator.Packages;
            if (packages != null && packages.Count > 0)
            {
                foreach (var package in packages)
                {
                    var packagePath = $"{k_AllocatorMenuPath}/[P] {package.PackageName}";
                    if (menus.ContainsKey(packagePath))
                    {
                        // throw new Exception($"Custom menu item path '{packagePath}' is already exists.");
                        Debug.LogError($"Custom menu item path '{packagePath}' is already exists.");
                        continue;
                    }
                    menus.Add(packagePath, new AllocatorMenuItem()
                    {
                        Path = packagePath,
                        type = AllocatorType.Package,
                        Window = PackageWindow,
                        Target = package
                    });

                    // Groups
                    var groups = package.Groups;
                    if (groups != null && groups.Count > 0)
                    {
                        foreach (var group in groups)
                        {
                            var groupPath = $"{packagePath}/[G] {group.GroupName}";
                            if (menus.ContainsKey(groupPath))
                            {
                                // throw new Exception($"Custom menu item path '{groupPath}' is already exists.");
                                Debug.LogError($"Custom menu item path '{groupPath}' is already exists.");
                                continue;
                            }
                            menus.Add(groupPath, new AllocatorMenuItem()
                            {
                                Path = groupPath,
                                type = AllocatorType.Group,
                                Window = GroupWindow,
                                Target = group
                            });

                            // Collectors
                            var collectors = group.Collectors;
                            if (collectors != null && collectors.Count > 0)
                            {
                                foreach (var collector in collectors)
                                {
                                    if (collector.CollectTarget == null)
                                    {
                                        continue;
                                    }
                                    var collectorPath = $"{groupPath}/[C] {collector.CollectName} - {collector.CollectGUID}";
                                    if (menus.ContainsKey(collectorPath))
                                    {
                                        Debug.LogError($"Custom menu item path '{collectorPath}' is already exists.");
                                        continue;
                                    }
                                    menus.Add(collectorPath, new AllocatorMenuItem()
                                    {
                                        Path = collectorPath,
                                        type = AllocatorType.Collector,
                                        Window = CollectorWindow,
                                        Target = collector
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return menus;
        }
    }

    // Builder
    public partial class MainWindow
    {
        [Serializable]
        internal class BuilderMenuItem : MainMenuItem
        {
            public EBuildPipeline Type;
            public new AssetBundleBuildProfile Target;
        }

        private BuilderWindow m_BuilderWindow;

        private BuilderWindow BuilderWindow
        {
            get
            {
                if (m_BuilderWindow == null)
                {
                    m_BuilderWindow = CreateInstance<BuilderWindow>();
                    m_BuilderWindow.OnBuilderSaved = OnBuilderSaved;
                }
                return m_BuilderWindow;
            }
        }

        private BuildProfileWindowBuiltin m_BuildProfileWindowBuiltin;

        private BuildProfileWindowBuiltin BuildProfileWindowBuiltin
        {
            get
            {
                if (m_BuildProfileWindowBuiltin == null)
                {
                    m_BuildProfileWindowBuiltin = CreateInstance<BuildProfileWindowBuiltin>();
                    m_BuildProfileWindowBuiltin.Selectable = false;
                    m_BuildProfileWindowBuiltin.OnBuilderSaved = OnBuilderSaved;
                }
                return m_BuildProfileWindowBuiltin;
            }
        }

        private BuildProfileWindowScriptable m_BuildProfileWindowScriptable;

        private BuildProfileWindowScriptable BuildProfileWindowScriptable
        {
            get
            {
                if (m_BuildProfileWindowScriptable == null)
                {
                    m_BuildProfileWindowScriptable = CreateInstance<BuildProfileWindowScriptable>();
                    m_BuildProfileWindowScriptable.Selectable = false;
                    m_BuildProfileWindowScriptable.OnBuilderSaved = OnBuilderSaved;
                }
                return m_BuildProfileWindowScriptable;
            }
        }

        private BuildProfileWindowRawFile m_BuildProfileWindowRawFile;

        private BuildProfileWindowRawFile BuildProfileWindowRawFile
        {
            get
            {
                if (m_BuildProfileWindowRawFile == null)
                {
                    m_BuildProfileWindowRawFile = CreateInstance<BuildProfileWindowRawFile>();
                    m_BuildProfileWindowRawFile.Selectable = false;
                    m_BuildProfileWindowRawFile.OnBuilderSaved = OnBuilderSaved;
                }
                return m_BuildProfileWindowRawFile;
            }
        }

        private void OnBuilderSaved(object sender, EventArgs args)
        {
            Refresh();
        }

        private void OnBuilderSelectionChanged(OdinMenuItem odinMenuItem, BuilderMenuItem builderMenuItem)
        {
            BuildProfileWindowBuiltin.Profile = null;
            BuildProfileWindowScriptable.Profile = null;
            BuildProfileWindowRawFile.Profile = null;
            switch (builderMenuItem.Type)
            {
                case EBuildPipeline.BuiltinBuildPipeline:
                    BuildProfileWindowBuiltin.Profile = (AssetBundleBuildProfileBuiltin)builderMenuItem.Target;
                    BuildProfileWindowBuiltin.OnBuilderSaved = OnBuilderSaved;
                    break;
                case EBuildPipeline.ScriptableBuildPipeline:
                    BuildProfileWindowScriptable.Profile = (AssetBundleBuildProfileScriptable)builderMenuItem.Target;
                    BuildProfileWindowScriptable.OnBuilderSaved = OnBuilderSaved;
                    break;
                case EBuildPipeline.RawFileBuildPipeline:
                    BuildProfileWindowRawFile.Profile = (AssetBundleBuildProfileRawFile)builderMenuItem.Target;
                    BuildProfileWindowRawFile.OnBuilderSaved = OnBuilderSaved;
                    break;
                default:
                    throw new Exception("Invalid builder type.");
            }
        }

        private Dictionary<string, MainMenuItem> GetBuilderMenuItems()
        {
            var builder = BuilderWindow.Builder;
            if (builder == null)
            {
                return null;
            }

            var menus = new Dictionary<string, MainMenuItem>();

            var profiles = builder.Profiles;
            if (profiles != null && profiles.Count > 0)
            {
                foreach (var profile in profiles)
                {
                    string prefix;
                    BuildProfileWindow window;
                    switch (profile.BuildPipeline)
                    {
                        case EBuildPipeline.BuiltinBuildPipeline:
                            prefix = "[B]";
                            window = BuildProfileWindowBuiltin;
                            break;
                        case EBuildPipeline.ScriptableBuildPipeline:
                            prefix = "[S]";
                            window = BuildProfileWindowScriptable;
                            break;
                        case EBuildPipeline.RawFileBuildPipeline:
                            prefix = "[R]";
                            window = BuildProfileWindowRawFile;
                            break;
                        default:
                            throw new Exception("Invalid builder type.");
                    }
                    var itemMenuPath = $"{k_BuilderMenuPath}/{prefix} {profile.BuilderName}";
                    if (menus.ContainsKey(itemMenuPath))
                    {
                        throw new Exception($"Custom menu item path '{itemMenuPath}' is already exists.");
                    }
                    menus.Add(itemMenuPath, new BuilderMenuItem()
                    {
                        Path = itemMenuPath,
                        Type = profile.BuildPipeline,
                        Window = window,
                        Target = profile
                    });
                }
            }

            return menus;
        }
    }
}