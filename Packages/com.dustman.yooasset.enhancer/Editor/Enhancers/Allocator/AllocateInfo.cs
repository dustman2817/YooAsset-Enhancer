using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    [Serializable]
    public class AllocateAssetInfo : ISearchFilterable
    {
        private Object m_AssetObject;

        [ShowInInspector]
        [PropertyOrder(-999)]
        [TableColumnWidth(250, false)]
        public Object AssetObject
        {
            get
            {
                if (m_AssetObject == null)
                {
                    m_AssetObject = AssetDatabase.LoadAssetAtPath<Object>(AssetPath);
                }
                return m_AssetObject;
            }
            set {}
        }

        /// <summary>
        /// 资源GUID
        /// </summary>
        private string m_AssetGUID;

        [ShowInInspector]
        [HideInTables]
        public string AssetGUID
        {
            get => m_AssetGUID;
            set => m_AssetGUID = value;
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        private string m_AssetPath;

        [ShowInInspector]
        [HideInTables]
        public string AssetPath
        {
            get => m_AssetPath;
            set => m_AssetPath = value;
        }

        private string AssetName => AssetUtil.GetAssetName(AssetPath);

        /// <summary>
        /// 资源包名称
        /// </summary>
        private string m_BundleName;

        [ShowInInspector]
        public string BundleName
        {
            get => m_BundleName;
            set => m_BundleName = value;
        }

        /// <summary>
        /// 寻址名称
        /// </summary>
        private string m_AddressName;

        [ShowInInspector]
        [HideInTables]
        public string AddressName
        {
            get => m_AddressName;
            set => m_AddressName = value;
        }

        public bool IsMatch(string searchString)
        {
            if (AssetName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            if (AddressName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class AllocateBundleInfo : ISearchFilterable
    {
        /// <summary>
        /// 资源包名称
        /// </summary>
        private string m_BundleName;

        public string BundleName
        {
            get => m_BundleName;
            set => m_BundleName = value;
        }

        private List<AllocateAssetInfo> m_AssetInfos;

        public List<AllocateAssetInfo> AssetInfos
        {
            get => m_AssetInfos;
            set => m_AssetInfos = value;
        }

        private List<Object> m_AssetObjects;

        [ShowInInspector]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DefaultExpandedState = false, IsReadOnly = true)]
        [LabelText("$BundleName")]
        public List<Object> AssetObjects
        {
            get
            {
                if (m_AssetInfos == null)
                {
                    return null;
                }
                if (m_AssetObjects == null)
                {
                    m_AssetObjects = new List<Object>();
                    foreach (var assetInfo in m_AssetInfos)
                    {
                        m_AssetObjects.Add(AssetDatabase.LoadAssetAtPath<Object>(assetInfo.AssetPath));
                    }
                }
                return m_AssetObjects;
            }
            set {}
        }

        public bool IsMatch(string searchString)
        {
            if (BundleName.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}