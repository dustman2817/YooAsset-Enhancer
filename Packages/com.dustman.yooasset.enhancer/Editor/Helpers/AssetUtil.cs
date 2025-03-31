using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    public enum AssetSearchFilterType
    {
        None,
        AnimationClip,
        AssemblyDefinitionAsset,
        AudioClip,
        AudioMixer,
        Font,
        Material,
        Mesh,
        Model,
        PhysicMaterial,
        Prefab,
        Scene,
        Script,
        Shader,
        Sprite,
        SpriteAtlas,
        TextAsset,
        Texture,
        VideoClip,
        DefaultAsset, // 目录、动态库等
    }

    public class AssetSearchFilters
    {
        public static string GetFilter(AssetSearchFilterType type)
        {
            if (type == AssetSearchFilterType.None)
            {
                return string.Empty;
            }
            return $"t:{type.ToString()}";
        }
    }

    public enum AssetFileExtensionType
    {
        Unknow,
        Animation,
        Audio,
        Font,
        Material,
        Model,
        Prefab,
        Scene,
        Script,
        Shader,
        SpriteAtlas,
        Text,
        Texture,
        Video,
        UnityAsset,
    }

    public class AssetFileExtensions
    {
        public static string[] Animation = { ".anim", ".controller", ".overridecontroller", ".mask" };
        public static string[] Audio = { ".mp3", ".wav", ".ogg", ".aif", ".aiff", ".mod", ".it", ".s3m", ".xm" };
        public static string[] Font = { ".ttf", ".tmp" };
        public static string[] Material = { ".mat", ".cubemap", ".physicsmaterial" };
        public static string[] Model = { ".3df", ".3dm", ".3dmf", ".3dv", ".3dx", ".c5d", ".lwo", ".lws", ".ma", ".mb", ".mesh", ".vrl", ".wrl", ".wrz", ".fbx", ".dae", ".3ds", ".dxf", ".obj", ".skp", ".max", ".blend" };
        public static string[] Prefab = { ".prefab" };
        public static string[] Scene = { ".unity" };
        public static string[] Script = { ".cs", ".js", ".boo", ".h" };
        public static string[] Shader = { ".shader", ".cginc" };
        public static string[] SpriteAtlas = { ".spriteatlas", ".spriteatlasv2" };
        public static string[] Text = { ".cs", ".js", ".boo", ".h" };
        public static string[] Texture = { ".ai", ".apng", ".png", ".bmp", ".cdr", ".dib", ".eps", ".exif", ".ico", ".icon", ".j", ".j2c", ".j2k", ".jas", ".jiff", ".jng", ".jp2", ".jpc", ".jpe", ".jpeg", ".jpf", ".jpg", "jpw", "jpx", "jtf", ".mac", ".omf", ".qif", ".qti", "qtif", ".tex", ".tfw", ".tga", ".tif", ".tiff", ".wmf", ".psd", ".exr", ".rendertexture", ".hdr" };
        public static string[] Video = { "Video", ".asf", ".asx", ".avi", ".dat", ".divx", ".dvx", ".mlv", ".m2l", ".m2t", ".m2ts", ".m2v", ".m4e", ".m4v", "mjp", ".mov", ".movie", ".mp21", ".mp4", ".mpe", ".mpeg", ".mpg", ".mpv2", ".ogm", ".qt", ".rm", ".rmvb", ".wmv", ".xvid", ".flv" };

        public static string[] UnityAsset = { ".asset", ".guiskin", ".flare", ".fontsettings", ".prefs" };
        public static string[] LightingAsset = { ".lighting" };
        public static string[] TimelineAsset = { ".playable" };

        private static class Extensions
        {
            public static Dictionary<AssetFileExtensionType, string[]> Caches;

            static Extensions()
            {
                Caches = new Dictionary<AssetFileExtensionType, string[]>();
                foreach (AssetFileExtensionType type in Enum.GetValues(typeof(AssetFileExtensionType)))
                {
                    var key = type.ToString();
                    var field = typeof(AssetFileExtensions).GetField(key, BindingFlags.Static | BindingFlags.Public);
                    if (field != null)
                    {
                        var value = field.GetValue(null) as string[];
                        Caches.Add(type, value);
                    }
                }
            }
        }

        public static string[] GetExtensions(AssetFileExtensionType type)
        {
            if (type == AssetFileExtensionType.Unknow)
            {
                return null;
            }
            Extensions.Caches.TryGetValue(type, out var value);
            if (value == null)
            {
                throw new Exception($"Not found {type} extensions.");
            }
            return value;
        }

        public static AssetFileExtensionType GetExtensionType(string extension)
        {
            extension = extension.ToLower();
            foreach (var (type, extensions) in Extensions.Caches)
            {
                if (extensions.Contains(extension))
                {
                    return type;
                }
            }
            return AssetFileExtensionType.Unknow;
        }
    }

    public static class AssetUtil
    {
        /// <summary>
        /// 获取资源名称
        /// </summary>
        public static string GetAssetName(string path)
        {
            if (Directory.Exists(path))
            {
                return PathUtil.GetDirectoryName(path, false);
            }
            if (File.Exists(path))
            {
                return PathUtil.GetFileName(path);
            }
            return null;
        }

        public static void GetAssets<T>(string filter, string[] searchInFolders, out Dictionary<string, T> assets, Func<string, string, int, int, bool> progress = null) where T : Object
        {
            GetAssets(filter, searchInFolders, typeof(T), out var objects, progress);
            assets = objects.ToDictionary(obj => obj.Key, obj => obj.Value as T);
        }

        public static void GetAssets<T>(AssetSearchFilterType type, string[] searchInFolders, out Dictionary<string, T> assets, Func<string, string, int, int, bool> progress = null) where T : Object
        {
            GetAssets(AssetSearchFilters.GetFilter(type), searchInFolders, out assets, progress);
        }

        public static void GetAssets(string filter, string[] searchInFolders, Type type, out Dictionary<string, Object> assets, Func<string, string, int, int, bool> progress = null)
        {
            assets = new Dictionary<string, Object>();
            GetAssets(filter, searchInFolders, out var files, progress);
            if (files == null || files.Count <= 0)
            {
                return;
            }
            foreach (var file in files)
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(file);
                var objType = obj.GetType();
                if (type == objType || type.IsAssignableFrom(objType))
                {
                    assets.Add(file, obj);
                }
            }
        }

        public static void GetAssets(AssetSearchFilterType type, string[] searchInFolders, out List<string> files, Func<string, string, int, int, bool> progress = null)
        {
            GetAssets(AssetSearchFilters.GetFilter(type), searchInFolders, out files, progress);
        }

        /// <summary>
        /// 获取指定路径下所有资源
        /// </summary>
        public static void GetAssets(string filter, string[] searchInFolders, out List<string> files, Func<string, string, int, int, bool> progress = null)
        {
            files = new List<string>();
            filter = string.IsNullOrEmpty(filter) ? "" : filter;
            var guids = AssetDatabase.FindAssets(filter, searchInFolders); // 注意：目录也会被当成资源返回。
            var index = 0;
            var length = guids.Length;
            for (; index < length; index++)
            {
                var guid = guids[index];
                var unityPath = AssetDatabase.GUIDToAssetPath(guid);
                if (progress != null && progress.Invoke(nameof(GetAssets), Path.GetFileName(unityPath), index, length))
                {
                    break;
                }
                if (File.Exists(unityPath) && !files.Contains(unityPath))
                {
                    files.Add(unityPath);
                }
            }
        }

        public static void GetAssets(AssetFileExtensionType type, string[] searchInFolders, out List<string> files, Func<string, string, int, int, bool> progress = null)
        {
            GetAssets(AssetFileExtensions.GetExtensions(type), searchInFolders, out files, progress);
        }

        /// <summary>
        /// 获取指定路径下所有资源（扩展名）
        /// </summary>
        public static void GetAssets(string[] extensions, string[] searchInFolders, out List<string> files, Func<string, string, int, int, bool> progress = null)
        {
            files = new List<string>();
            var guids = AssetDatabase.FindAssets("", searchInFolders); // 注意：目录也会被当成资源返回。
            var index = 0;
            var length = guids.Length;
            for (; index < length; index++)
            {
                var guid = guids[index];
                var unityPath = AssetDatabase.GUIDToAssetPath(guid);
                if (progress != null && progress.Invoke(nameof(GetAssets), Path.GetFileName(unityPath), index, length))
                {
                    break;
                }
                if (!File.Exists(unityPath))
                {
                    continue;
                }
                var ext = Path.GetExtension(unityPath);
                if (extensions.Contains(ext) && !files.Contains(unityPath))
                {
                    files.Add(unityPath);
                }
            }
        }
    }
}