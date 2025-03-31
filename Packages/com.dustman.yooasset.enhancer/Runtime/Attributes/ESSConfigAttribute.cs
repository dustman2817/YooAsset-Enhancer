using System;

namespace YooAsset.Enhancer.Runtime
{
    [Flags]
    public enum LoadFlags
    {
        None = 0,
        All = ~0,
        Resources = 1 << 0,
        AssetDatabase = 1 << 1
    }

    public class ESSConfigAttribute : RSSConfigAttribute
    {
        private string m_ResourcesFolderPath = "Assets/Editor/Resources";

        public override string ResourcesFolderPath
        {
            get => m_ResourcesFolderPath;
            set => m_ResourcesFolderPath = value;
        }

        private string m_AssetDatabaseFolderPath = "Assets";

        public string AssetDatabaseFolderPath
        {
            get => m_AssetDatabaseFolderPath;
            set => m_AssetDatabaseFolderPath = value;
        }

        private LoadFlags m_LoadFlag = LoadFlags.All;

        public LoadFlags LoadFlag
        {
            get => m_LoadFlag;
            set => m_LoadFlag = value;
        }
    }
}