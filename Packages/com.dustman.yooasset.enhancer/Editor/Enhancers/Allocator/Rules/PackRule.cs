using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class PackRule
    {
        private static List<string> m_GlobalPackRuleNames;

        public static List<string> GlobalPackRuleNames
        {
            get
            {
                if (m_GlobalPackRuleNames == null)
                {
                    var ruleNames = AssetBundleCollectorSettingData.GetPackRuleNames();
                    m_GlobalPackRuleNames = new List<string>();
                    foreach (var ruleName in ruleNames)
                    {
                        m_GlobalPackRuleNames.Add(ruleName.ClassName);
                    }
                    m_GlobalPackRuleNames.Sort();
                }
                return m_GlobalPackRuleNames;
            }
        }

        public static IPackRule GetPackRuleInstance(string ruleName)
        {
            return AssetBundleCollectorSettingData.GetPackRuleInstance(ruleName);
        }
    }
}