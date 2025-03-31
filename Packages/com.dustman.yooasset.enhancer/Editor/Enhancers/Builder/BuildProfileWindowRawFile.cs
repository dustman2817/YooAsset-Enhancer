using System;
using UnityEditor;

namespace YooAsset.Enhancer.Editor
{
    public class BuildProfileWindowRawFile : BuildProfileWindow
    {
        private const string k_ToolsMenuItemPath = "YooAsset/Extensions/RawFile Window";
        private const int k_ToolsMenuItemPriority = 6004;

        [MenuItem(k_ToolsMenuItemPath, false, k_ToolsMenuItemPriority)]
        private static void OpenWindow()
        {
            var window = GetWindow<BuildProfileWindowRawFile>("RawFile Window", false);
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
                            if (obj != null && obj is AssetBundleBuildProfileRawFile builder)
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
                    m_Exception = $"{nameof(AssetBundleBuildProfileRawFile)} was not found.";
                }
                return m_Profile;
            }
            set
            {
                m_Profile = value;
                m_Exception = null;
            }
        }

        private AssetBundleBuildProfileRawFile RawFileProfile
        {
            get
            {
                if (!IsExist)
                {
                    return null;
                }
                return (AssetBundleBuildProfileRawFile)Profile;
            }
        }
    }
}