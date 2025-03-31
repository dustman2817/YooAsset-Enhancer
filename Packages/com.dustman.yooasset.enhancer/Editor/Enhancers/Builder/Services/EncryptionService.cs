using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    [Serializable]
    public class EncryptionServiceInfo
    {
        public string ServiceName;
        public string ServiceType;
        public Object ServiceObject;

        public Type Type => Type.GetType(ServiceType);
    }

    public class EncryptionService
    {
        private static Dictionary<string, Type> m_GlobalEncryptionServiceTypes;

        public static Dictionary<string, Type> GlobalEncryptionServiceTypes
        {
            get
            {
                if (m_GlobalEncryptionServiceTypes == null)
                {
                    var types = new List<Type>()
                    {
                        typeof(EncryptionNone)
                    };

                    var customTypes = EditorTools.GetAssignableTypes(typeof(IEncryptionServices));
                    types.AddRange(customTypes);

                    m_GlobalEncryptionServiceTypes = new Dictionary<string, Type>();
                    foreach (var type in types)
                    {
                        m_GlobalEncryptionServiceTypes.TryAdd(type.Name, type);
                    }
                }
                return m_GlobalEncryptionServiceTypes;
            }
        }

        private static List<string> m_GlobalEncryptionServiceNames;

        public static List<string> GlobalEncryptionServiceNames
        {
            get
            {
                if (m_GlobalEncryptionServiceNames == null)
                {
                    m_GlobalEncryptionServiceNames = new List<string>();
                    foreach (var pair in GlobalEncryptionServiceTypes)
                    {
                        m_GlobalEncryptionServiceNames.Add(pair.Key);
                    }
                }
                return m_GlobalEncryptionServiceNames;
            }
        }

        private static List<EncryptionServiceInfo> m_CacheEncryptionServiceInfos;

        public static List<EncryptionServiceInfo> GetEncryptionServiceInfos(bool recache = false)
        {
            if (recache)
            {
                m_CacheEncryptionServiceInfos = null;
            }
            if (m_CacheEncryptionServiceInfos == null)
            {
                m_CacheEncryptionServiceInfos = new List<EncryptionServiceInfo>();
                foreach (var pair in GlobalEncryptionServiceTypes)
                {
                    var name = pair.Key;
                    var type = pair.Value;
                    AssetUtil.GetAssets($"t:{nameof(MonoScript)} {name}", null, type, out var assets);
                    if (assets == null || assets.Count != 1)
                    {
                        // throw new Exception($"{name} is not found or duplicate.");
                        Debug.LogError($"{name} is not found or duplicate.");
                        m_CacheEncryptionServiceInfos.Add(new EncryptionServiceInfo()
                        {
                            ServiceName = name,
                            ServiceType = type.FullName,
                            ServiceObject = null
                        });
                    }
                    else
                    {
                        foreach (var asset in assets)
                        {
                            m_CacheEncryptionServiceInfos.Add(new EncryptionServiceInfo()
                            {
                                ServiceName = name,
                                ServiceType = type.FullName,
                                ServiceObject = asset.Value
                            });
                        }
                    }
                }
            }
            return m_CacheEncryptionServiceInfos;
        }

        private static Dictionary<string, IEncryptionServices> m_CacheEncryptionServiceInstances;

        public static IEncryptionServices GetEncryptionServiceInstance(string serviceName)
        {
            if (m_CacheEncryptionServiceInstances == null)
            {
                m_CacheEncryptionServiceInstances = new Dictionary<string, IEncryptionServices>();
            }

            if (m_CacheEncryptionServiceInstances.TryGetValue(serviceName, out IEncryptionServices instance))
            {
                return instance;
            }

            if (GlobalEncryptionServiceTypes.TryGetValue(serviceName, out var type))
            {
                instance = (IEncryptionServices)Activator.CreateInstance(type);
                m_CacheEncryptionServiceInstances.Add(serviceName, instance);
                return instance;
            }

            throw new Exception($"{serviceName} instance is not found.");
        }
    }
}