using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class AddressRule
    {
        private static List<string> m_GlobalAddressRuleNames;

        public static List<string> GlobalAddressRuleNames
        {
            get
            {
                if (m_GlobalAddressRuleNames == null)
                {
                    var ruleNames = AssetBundleCollectorSettingData.GetAddressRuleNames();
                    m_GlobalAddressRuleNames = new List<string>();
                    foreach (var ruleName in ruleNames)
                    {
                        m_GlobalAddressRuleNames.Add(ruleName.ClassName);
                    }
                    m_GlobalAddressRuleNames.Sort();
                }
                return m_GlobalAddressRuleNames;
            }
        }

        public static IAddressRule GetAddressRuleInstance(string ruleName)
        {
            return AssetBundleCollectorSettingData.GetAddressRuleInstance(ruleName);
        }
    }
}