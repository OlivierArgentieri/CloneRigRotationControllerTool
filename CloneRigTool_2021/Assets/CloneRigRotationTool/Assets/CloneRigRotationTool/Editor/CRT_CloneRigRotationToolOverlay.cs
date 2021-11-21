using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CloneRigRotationTool.Assets.CloneRigRotationTool.Models;
using CloneRigRotationTool.Assets.CloneRigRotationTool.Utils;
using EditoolsUnity;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;


namespace CloneRigRotationTool.Assets.CloneRigRotationTool.Editor
{
    [Overlay(typeof(SceneView), "Clone Rig Tool")]
    public class CRT_CloneRigRotationToolOverlay : IMGUIOverlay
    {
        Version version = new Version(0,0,1);
        #region f/p

        private static string GITHUB_URL => "https://github.com/OlivierArgentieri/CloneRigRotationControllerTool";
        private static string CURRENT_RESOURCES_FOLDER_PATH => $"{Application.dataPath}/Resources";
        private List<GameObject> selectedGameObjects => Selection.gameObjects.ToList();
        private GameObject selectedRootNode => selectedGameObjects.Count > 0 ? selectedGameObjects.First() : null;
        private Vector2 scrollPos;
        private bool unsecuredMode = false;
        private bool IsValid => selectedRootNode && Application.isEditor;

        #endregion

        #region unity methods

        public override void OnGUI()
        {
            List<GameObject> _selectedObjects = Selection.gameObjects.ToList();

            if (_selectedObjects.Count != 1)
                return;
            
            EditoolsLayout.Horizontal(true);
            
            EditoolsLayout.Vertical(true);
            EditoolsBox.HelpBoxInfo("Save Rig controls");
            EditoolsLayout.Vertical(false);
            
            EditoolsLayout.Vertical(true);
            EditoolsButton.Button("GitHub", Color.Lerp(Color.blue, Color.white, 0.4f), () => {  Application.OpenURL(GITHUB_URL);});
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField($"V{version}", style);
            EditoolsLayout.Vertical(false);
            EditoolsLayout.Horizontal(false);
            
            EditoolsLayout.Vertical(true);
            EditoolsLayout.Horizontal(true);
            EditoolsBox.HelpBoxInfo("To avoid .json verification");
            EditoolsField.Toggle("Unsecure mode", ref unsecuredMode);
            EditoolsLayout.Horizontal(false);
            EditoolsLayout.Vertical(false);
            
            EditoolsLayout.Space(2);
            EditoolsLayout.Vertical(true);
            EditoolsLayout.Horizontal(true);
            EditoolsButton.Button("Save Rig Controls", Color.Lerp(Color.blue, Color.white, 0.6f), SaveControlRigs );
            EditoolsLayout.Horizontal(false);

            EditoolsLayout.Space();

            string[] _files = CRT_Utils.GetFiles(CURRENT_RESOURCES_FOLDER_PATH)
                .Where(_f => Path.GetExtension(_f) == ".json").OrderBy(Path.GetFileNameWithoutExtension).ToArray();
            
            // start scroll view
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400), GUILayout.Height(300));
            foreach (string _file in _files)
            {
                EditoolsLayout.Horizontal(true);
                EditoolsButton.Button($"Load: {Path.GetFileNameWithoutExtension(_file)}", Color.Lerp(Color.green, Color.white, 0.6f), () => LoadControlRigs(_file));
                EditoolsButton.ButtonWithConfirm($"X", Color.Lerp(Color.black, Color.white, 0.6f), () => CRT_Utils.RemoveFile(_file), $"Remove: {Path.GetFileName(_file)}",
                    "Are you sure ?", "Yes", "No");
                EditoolsLayout.Horizontal(false);
            }
            EditorGUILayout.EndScrollView();
            EditoolsLayout.Vertical(false);
        }
        #endregion

        #region custom methods
        private void SaveControlRigs()
        {
            if (!IsValid) return;
            CloneRigToolController.CloneRigToolController.SaveControlRigs(selectedRootNode);
        }

        private void LoadControlRigs(string _file)
        {
            if (!IsValid) return;
            
            // get all ctn_hips go
            // Transform _rootCtr = currentGO.transform.Find("CNT_Character");
            Transform _rootCtr = selectedRootNode.transform;

            if (!_rootCtr)
                return;

            StreamReader _reader = new StreamReader(_file);
            NodeCNT _nodeFromJSon = JsonConvert.DeserializeObject<NodeCNT>(_reader.ReadToEnd());
            _reader.Close();
            
            if (_rootCtr.childCount != _nodeFromJSon.childNumber && !unsecuredMode)
            {
                 EditorUtility.DisplayDialog("Error", "Incompatible .json and gameObject", "OK");
                 return;
            }
            
            CloneRigToolController.CloneRigToolController.ApplyToSelectedGameObject(_rootCtr, _nodeFromJSon);
        }
        #endregion
    }
}