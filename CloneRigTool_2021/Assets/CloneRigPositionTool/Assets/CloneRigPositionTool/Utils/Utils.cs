using System.IO;

namespace CloneRigPositionTool.Assets.CloneRigPositionTool.Utils
{
    public static class CRT_Utils
    {
        public static string[] GetFiles(string _path)
        {
            if (!Directory.Exists(_path))
                return new string[] {};

            return Directory.GetFiles(_path);
        }
    }
}