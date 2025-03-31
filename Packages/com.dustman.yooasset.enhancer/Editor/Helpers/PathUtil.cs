using System;
using System.IO;

namespace YooAsset.Enhancer.Editor
{
    public static class PathUtil
    {
        /// <summary>
        /// 获取有效路径（Unity只识别斜杠）
        /// </summary>
        public static string GetValidPath(string path)
        {
            return path.Replace('\\', '/').Replace("\\", "/");
        }

        public static bool IsUnityPath(string path)
        {
            return path.Contains("Assets");
        }

        /// <summary>
        /// 获取 Unity 路径
        /// </summary>
        public static string GetUnityPath(string path)
        {
            path = GetValidPath(path);
            return IsUnityPath(path) ? path.Substring(path.IndexOf("Assets", StringComparison.CurrentCulture)) : path;
        }

        #region 文件夹

        /// <summary>
        /// 获取所处文件夹名称
        /// </summary>
        public static string GetDirectoryName(string path, bool forceParent = true)
        {
            var dirPath = GetDirectoryPath(path, forceParent);
            var index = dirPath.LastIndexOf('/');
            return dirPath.Substring(index + 1, dirPath.Length - index - 1);
        }

        /// <summary>
        /// 获取所处文件夹路径
        /// </summary>
        public static string GetDirectoryPath(string path, bool forceParent = true)
        {
            return Directory.Exists(path) && !forceParent ? GetUnityPath(path) : GetUnityPath(Path.GetDirectoryName(path));
        }

        #endregion

        #region 文件

        /// <summary>
        /// 获取文件名称
        /// </summary>
        public static string GetFileName(string path)
        {
            var filePath = GetUnityPath(Path.GetFileNameWithoutExtension(path));
            var index = filePath.LastIndexOf('/');
            return filePath.Substring(index + 1, filePath.Length - index - 1);
        }

        #endregion
    }
}