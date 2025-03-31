using System;
using Sirenix.OdinInspector;
using UnityEditor;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class BuildProfileWindowScriptable : BuildProfileWindow
    {
        private const string k_ToolsMenuItemPath = "YooAsset/Extensions/Scriptable Window";
        private const int k_ToolsMenuItemPriority = 6003;

        [MenuItem(k_ToolsMenuItemPath, false, k_ToolsMenuItemPriority)]
        private static void OpenWindow()
        {
            var window = GetWindow<BuildProfileWindowScriptable>("Scriptable Window", false);
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
                            if (obj != null && obj is AssetBundleBuildProfileScriptable profile)
                            {
                                m_Profile = profile;
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
                    m_Exception = $"{nameof(AssetBundleBuildProfileScriptable)} was not found.";
                }
                return m_Profile;
            }
            set
            {
                m_Profile = value;
                m_Exception = null;
            }
        }

        private AssetBundleBuildProfileScriptable ScriptableProfile
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return (AssetBundleBuildProfileScriptable)Profile;
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
                return ScriptableProfile.CompressOption;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                ScriptableProfile.CompressOption = value;
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
                return ScriptableProfile.DisableWriteTypeTree;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                ScriptableProfile.DisableWriteTypeTree = value;
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
                return ScriptableProfile.IgnoreTypeTreeChanges;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                ScriptableProfile.IgnoreTypeTreeChanges = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-600 + 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Extension", Indent = true)]
        private bool WriteLinkXML
        {
            get
            {
                if (!IsExist)
                {
                    return false;
                }
                return ScriptableProfile.WriteLinkXML;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                ScriptableProfile.WriteLinkXML = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-600 + 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Extension", Indent = true)]
        private string CacheServerHost
        {
            get
            {
                if (!IsExist)
                {
                    return string.Empty;
                }
                return ScriptableProfile.CacheServerHost;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                ScriptableProfile.CacheServerHost = value;
                OnValueChanged();
            }
        }

        [ShowInInspector]
        [PropertyOrder(-600 + 1)]
        [ShowIf("@IsDrawable")]
        [TitleGroup("Extension", Indent = true)]
        private int CacheServerPort
        {
            get
            {
                if (!IsExist)
                {
                    return 0;
                }
                return ScriptableProfile.CacheServerPort;
            }
            set
            {
                if (!IsExist)
                {
                    return;
                }
                ScriptableProfile.CacheServerPort = value;
                OnValueChanged();
            }
        }
    }
}