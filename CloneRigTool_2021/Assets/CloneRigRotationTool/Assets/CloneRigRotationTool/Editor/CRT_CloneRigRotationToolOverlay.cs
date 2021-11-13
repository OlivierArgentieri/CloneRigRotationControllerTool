using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        class NodeCNT
        {
            public float rot_x;
            public float rot_y;
            public float rot_z;
            public float rot_w;
            public string name;
            public int childNumber;
            public NodeCNT[] next = null;
            public NodeCNT(string _name, float _rot_x, float _rot_y, float _rot_z, float _rot_w, int _childNumber, NodeCNT[] _next = null)
            {
                rot_x = _rot_x;
                rot_y = _rot_y;
                rot_z = _rot_z;
                rot_w = _rot_w;
                name = _name;
                childNumber = _childNumber;
                next = _next;
            }
        }
        #region f/p

        private static string GITHUB_URL => "https://github.com/OlivierArgentieri/CloneRigRotationControllerTool";
        private static string CURRENT_RESOURCES_FOLDER_PATH => $"{Application.dataPath}/Resources";
        private string JSON_FILE_NAME => $"{CURRENT_RESOURCES_FOLDER_PATH}/{(IsValid ? selectedRootNode.name : "undefined" )}.json";
        private List<GameObject> selectedGameObjects => Selection.gameObjects.ToList();
        private GameObject selectedRootNode => selectedGameObjects.Count > 0 ? selectedGameObjects.First() : null;
        private Vector2 scrollPos;
        private bool unsecuredMode = false;
        private bool IsValid => selectedRootNode && Application.isEditor;

        #endregion

        #region unity methods

        void OnInspectorUpdate()
        {
        }
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
                EditoolsButton.ButtonWithConfirm($"X", Color.Lerp(Color.black, Color.white, 0.6f), () => RemoveFile(_file), $"Remove: {Path.GetFileName(_file)}",
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

            // check if directory exist
            if (!Directory.Exists(CURRENT_RESOURCES_FOLDER_PATH))
            {
                // and create it
                Directory.CreateDirectory(CURRENT_RESOURCES_FOLDER_PATH);
            }

            // get all ctn_hips go
            Transform _rootCtr = selectedRootNode.transform;

            if (!_rootCtr) return;

            NodeCNT _test = new NodeCNT(_rootCtr.name, 0, 0, 0, 0, _rootCtr.childCount);
            ControllerToNodeObject(_rootCtr, ref _test);

            //RecursiveTest(_test);
            string _json = JsonConvert.SerializeObject(_test);
            File.WriteAllText(JSON_FILE_NAME, _json);
            
            // first function instruction 'IsValid' protect from run outside editor mode
            AssetDatabase.Refresh();
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
            ApplyToSelectedGameObject(ref _rootCtr, _nodeFromJSon);
        }

        private void ControllerToNodeObject(Transform _rootTransform, ref NodeCNT _rootNode)
        {
            if (_rootTransform.childCount == 0)
            {
                // tail
                if (_rootNode == null)
                {
                    _rootNode = new NodeCNT(_rootTransform.name, _rootTransform.rotation.x, _rootTransform.rotation.y, _rootTransform.rotation.z, _rootTransform.rotation.w, _rootTransform.childCount);
                }
                return;
            }

            if (_rootNode == null)
            {
                _rootNode = new NodeCNT(_rootTransform.name, _rootTransform.rotation.x, _rootTransform.rotation.y, _rootTransform.rotation.z, _rootTransform.rotation.w, _rootTransform.childCount);
            }

            if (_rootTransform.childCount > 0)
                _rootNode.next = new NodeCNT[_rootTransform.childCount];

            for (int _i = 0; _i < _rootTransform.childCount; _i++)
            {
                ControllerToNodeObject(_rootTransform.GetChild(_i), ref _rootNode.next[_i]);
            }

            // not last
            _rootNode.name = _rootTransform.name;
            var _rotation = _rootTransform.rotation;
            _rootNode.rot_x = _rotation.x;
            _rootNode.rot_y = _rotation.y;
            _rootNode.rot_z = _rotation.z;
            _rootNode.rot_w = _rotation.w;
        }

        private void RemoveFile(string _path)
        {
            if (!File.Exists(_path)) return;
            File.Delete(_path);
            
            string _meta = $"{_path}.meta";
            if (!File.Exists(_meta)) return;
            File.Delete(_meta);
            
            AssetDatabase.Refresh();
        }

        private void ApplyToSelectedGameObject(ref Transform _rootTransform, NodeCNT _rootNode)
        {
            if (_rootNode.next == null)
            {
                _rootTransform.rotation = new Quaternion(
                    _rootNode.rot_x,
                    _rootNode.rot_y,
                    _rootNode.rot_z,
                    _rootNode.rot_w
                );
                return;
            }

            for (int _i = 0; _i < _rootNode.next.Length; _i++)
            {
                var rootTransform = _rootTransform.transform.GetChild(_i);
                _rootTransform.rotation = new Quaternion(
                    _rootNode.rot_x,
                    _rootNode.rot_y,
                    _rootNode.rot_z,
                    _rootNode.rot_w
                );
                ApplyToSelectedGameObject(ref rootTransform, _rootNode.next[_i]);
            }

        }
        #endregion
    }
}