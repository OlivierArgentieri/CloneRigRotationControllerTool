using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif
namespace CloneRigPositionTool.Assets.CloneRigPositionTool.Editor
{
    public class CRT_CloneRigPositionToolWindowInit
    {
        [MenuItem("Rig Tool/ Open window")]
        public static void InitWindow()
        {
            CRT_CloneRigPositionToolWindow _window = EditorWindow.GetWindow<CRT_CloneRigPositionToolWindow>(false);
            _window.titleContent = new GUIContent("Clothing test tool");
        }
    }
}