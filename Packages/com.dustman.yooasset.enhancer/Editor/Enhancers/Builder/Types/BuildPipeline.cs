using System;
using System.Collections.Generic;
using YooAsset.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class BuildPipeline
    {
        private static List<string> m_GlobalBuildPipelineNames;

        public static List<string> GlobalBuildPipelineNames
        {
            get
            {
                if (m_GlobalBuildPipelineNames == null)
                {
                    var typeNames = Enum.GetNames(typeof(EBuildPipeline));
                    m_GlobalBuildPipelineNames = new List<string>();
                    foreach (var typeName in typeNames)
                    {
                        m_GlobalBuildPipelineNames.Add(typeName);
                    }
                    // m_GlobalBuildPipelineNames.Sort();
                }
                return m_GlobalBuildPipelineNames;
            }
        }
    }
}