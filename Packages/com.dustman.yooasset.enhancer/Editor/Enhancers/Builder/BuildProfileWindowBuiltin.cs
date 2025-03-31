using System;
using Sirenix.OdinInspector;
using UnityEditor;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class BuildProfileWindowBuiltin : BuildProfileWindow
    {
        private const string k_ToolsMenuItemPath = "YooAsset/Extensions/Builtin Window";
        private const int k_ToolsMenuItemPriority = 6002;

        [MenuItem(k_ToolsMenuItemPath, false, k_ToolsMenuItemPriority)]
        private static void OpenWindow()
        {
            var window = GetWindow<BuildProfileWindowBuiltin>("Builtin Window", false);
            window.Show();
        }

        public override AssetBundleBuildProfile Profile
        {
            get
            {
                if (IsException)
                {
                    return null;
                }
                if (m_Profile == null)
                {
                    if (m_Selectable)
                    {
                        try
                        {
                            var obj = Selection.activeObject;
                            if (obj != null && obj is AssetBundleBuildProfileBuiltin builder)
                            {
                                m_Profile = builder;
                                m_Exception = null;
                            }
                        }
                        catch (Exception e)
                        {
                            m_Exception = e.Message;
                        }
                    }
                }
                if (m_Profile == null && !IsException)
                {
                    m_Exception = $"{nameof(AssetBundleBuildProfileBuiltin)} was not found.";
                }
                return m_Profile;
            }
            set
            {
                m_Profile = value;
                m_Exception = null;
            }
        }

        private AssetBundleBuildProfileBuiltin BuiltinProfile
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return (AssetBundleBuildProfileBuiltin)Profile;
            }
        }

        [ShowInInspector]
        [PropertyOrder(-600 + 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Extension", Indent = true)]
        private ECompressOption CompressOption
        {
            get
            {
                if (!IsExist)
                {
                    return ECompressOption.Uncompressed;
                }
                return BuiltinProfile.CompressOption;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                BuiltinProfile.CompressOption = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-600 + 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Extension", Indent = true)]
        private bool DisableWriteTypeTree
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return BuiltinProfile.DisableWriteTypeTree;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                BuiltinProfile.DisableWriteTypeTree = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-600 + 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Extension", Indent = true)]
        private bool IgnoreTypeTreeChanges
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return BuiltinProfile.IgnoreTypeTreeChanges;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                BuiltinProfile.IgnoreTypeTreeChanges = value;
                OnValueChanged();
            }
        }
    }
}