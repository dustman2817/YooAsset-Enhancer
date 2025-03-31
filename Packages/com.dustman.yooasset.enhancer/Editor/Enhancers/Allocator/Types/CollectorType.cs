using System;
using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class CollectorType
    {
        private static List<string> m_GlobalCollectorTypeNames;

        public static List<string> GlobalCollectorTypeNames
        {
            get
            {
                if (m_GlobalCollectorTypeNames == null)
                {
                    var typeNames = Enum.GetNames(typeof(ECollectorType));
                    m_GlobalCollectorTypeNames = new List<string>();
                    foreach (var typeName in typeNames)
                    {
                        m_GlobalCollectorTypeNames.Add(typeName);
                    }
                    m_GlobalCollectorTypeNames.Sort();
                }
                return m_GlobalCollectorTypeNames;
            }
        }
    }
}