using System;
using System.IO;

namespace YooAsset.Enhancer.Editor
{
    /// <summary>
    /// 偏移量加密（纯防小白）
    /// </summary>
    public class EncryptionOffset : IEncryptionServices
    {
        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            EncryptResult result = new EncryptResult
            {
                Encrypted = true,
                EncryptedData = EncryInternal(fileInfo.FilePath, fileInfo.BundleName)
            };
            return result;
        }

        public static byte[] EncryInternal(string filePath, string bundleName)
        {
            var source = File.ReadAllBytes(filePath);
            var result = source;
            var offset = GetFileOffset(bundleName);
            if (offset > 0)
            {
                var sourceLength = source.Length;
                var resultLength = offset + sourceLength;
                result = new byte[resultLength];
                Array.Copy(source, 0, result, 0, offset);
                Array.Copy(source, 0, result, offset, source.Length);
            }
            return result;
        }

        private static int GetFileOffset(string bundleName)
        {
            return 32;
        }
    }
}