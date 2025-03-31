using System.Collections.Generic;
using System.IO;
using System.Linq;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class FilterRule
    {
        private static List<string> m_GlobalFilterRuleNames;

        public static List<string> GlobalFilterRuleNames
        {
            get
            {
                if (m_GlobalFilterRuleNames == null)
                {
                    var ruleNames = AssetBundleCollectorSettingData.GetFilterRuleNames();
                    m_GlobalFilterRuleNames = new List<string>();
                    foreach (var ruleName in ruleNames)
                    {
                        m_GlobalFilterRuleNames.Add(ruleName.ClassName);
                    }
                    m_GlobalFilterRuleNames.Sort();
                }
                return m_GlobalFilterRuleNames;
            }
        }

        public static IFilterRule GetFilterRuleInstance(string ruleName)
        {
            return AssetBundleCollectorSettingData.GetFilterRuleInstance(ruleName);
        }
    }

    [DisplayName("收集空白")]
    public class CollectNone : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            return false;
        }
    }

    [DisplayName("收集视频")]
    public class CollectVideo : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            string fileExtension = Path.GetExtension(data.AssetPath);
            return AssetFileExtensions.Video.Contains(fileExtension);
        }
    }
}