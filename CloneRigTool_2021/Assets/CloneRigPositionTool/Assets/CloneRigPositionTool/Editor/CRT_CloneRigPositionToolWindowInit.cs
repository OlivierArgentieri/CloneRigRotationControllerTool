
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif
namespace CloneRigPositionTool.Assets.CloneRigPositionTool.Editor
{
    public class CRT_CloneRigPositionToolWindowInit
    {
        [MenuItem("Rig aaTool/ Open window")]
        public static void InitWindow()
        {
            CRT_CloneRigPositionToolWindow _window = EditorWindow.GetWindow<CRT_CloneRigPositionToolWindow>(false);
            _window.titleContent = new GUIContent("SaveController");
            
            Debug.Log("aaa");
        }

    }
}