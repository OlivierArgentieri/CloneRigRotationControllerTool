using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif
namespace CloneRigRotationTool.Assets.CloneRigRotationTool.Editor
{
    public class CRT_CloneRigRotationToolWindowInit
    {
        [MenuItem("Rig Tool/ Open window")]
        public static void InitWindow()
        {
            CRT_CloneRigRotationToolWindow _window = EditorWindow.GetWindow<CRT_CloneRigRotationToolWindow>(false);
            _window.titleContent = new GUIContent("SaveController");
            
            Debug.Log("aaa");
        }

    }
}