using System;

namespace YooAsset.Enhancer.Runtime
{
    public class RSSConfigAttribute : Attribute
    {
        private string m_ResourcesFolderPath = "Assets/Resources";

        public virtual string ResourcesFolderPath
        {
            get => m_ResourcesFolderPath;
            set => m_ResourcesFolderPath = value;
        }

        private string m_DefaultFileFolder;

        public virtual string DefaultFileFolder
        {
            get => m_DefaultFileFolder;
            set => m_DefaultFileFolder = value;
        }

        private string m_DefaultFileName;

        public virtual string DefaultFileName
        {
            get => m_DefaultFileName;
            set => m_DefaultFileName = value;
        }

        private string m_CustomLoadFunction;

        public virtual string CustomLoadFunction
        {
            get => m_CustomLoadFunction;
            set => m_CustomLoadFunction = value;
        }
    }
}