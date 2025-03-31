using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class IgnoreRule
    {
        private static List<string> m_GlobalIgnoreRuleNames;

        public static List<string> GlobalIgnoreRuleNames
        {
            get
            {
                if (m_GlobalIgnoreRuleNames == null)
                {
                    var ruleNames = AssetBundleCollectorSettingData.GetIgnoreRuleNames();
                    m_GlobalIgnoreRuleNames = new List<string>();
                    foreach (var ruleName in ruleNames)
                    {
                        m_GlobalIgnoreRuleNames.Add(ruleName.ClassName);
                    }
                    m_GlobalIgnoreRuleNames.Sort();
                }
                return m_GlobalIgnoreRuleNames;
            }
        }

        public static IIgnoreRule GetIgnoreRuleInstance(string ruleName)
        {
            return AssetBundleCollectorSettingData.GetIgnoreRuleInstance(ruleName);
        }
    }
}