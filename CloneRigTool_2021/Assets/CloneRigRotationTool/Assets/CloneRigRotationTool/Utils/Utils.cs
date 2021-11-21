using System.IO;
using UnityEditor;

namespace CloneRigRotationTool.Assets.CloneRigRotationTool.Utils
{
    public static class CRT_Utils
    {
        public static string[] GetFiles(string _path)
        {
            if (!Directory.Exists(_path))
                return new string[] {};

            return Directory.GetFiles(_path);
        }
        public static void RemoveFile(string _path)
        {
            if (!File.Exists(_path)) return;
            File.Delete(_path);
            
            string _meta = $"{_path}.meta";
            if (!File.Exists(_meta)) return;
            File.Delete(_meta);
            
            AssetDatabase.Refresh();
        }
    }
}