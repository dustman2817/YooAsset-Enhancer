using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class ActiveRule
    {
        private static List<string> m_GlobalActiveRuleNames;

        public static List<string> GlobalActiveRuleNames
        {
            get
            {
                if (m_GlobalActiveRuleNames == null)
                {
                    var ruleNames = AssetBundleCollectorSettingData.GetActiveRuleNames();
                    m_GlobalActiveRuleNames = new List<string>();
                    foreach (var ruleName in ruleNames)
                    {
                        m_GlobalActiveRuleNames.Add(ruleName.ClassName);
                    }
                    m_GlobalActiveRuleNames.Sort();
                }
                return m_GlobalActiveRuleNames;
            }
        }

        public static IActiveRule GetActiveRuleInstance(string ruleName)
        {
            return AssetBundleCollectorSettingData.GetActiveRuleInstance(ruleName);
        }
    }
}