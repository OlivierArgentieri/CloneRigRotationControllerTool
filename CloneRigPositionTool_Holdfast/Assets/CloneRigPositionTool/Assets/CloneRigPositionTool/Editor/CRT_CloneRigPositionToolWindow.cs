using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using EditoolsUnity;
using static System.String;

namespace CloneRigPositionTool.Assets.CloneRigPositionTool.Editor
{
    public class CRT_CloneRigPositionToolWindow : EditorWindow
    {
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

        private List<GameObject> selectedGameObjects => Selection.gameObjects.ToList();
        private GameObject selectedRootNode => selectedGameObjects.Count > 0 ? selectedGameObjects.First() : null;
        private string CURRENT_RESOURCES_FOLDER_PATH => $"{Application.dataPath}/Resources";
        private string JSON_FILE_NAME => $"{CURRENT_RESOURCES_FOLDER_PATH}/{(IsValid ? selectedRootNode.name : "undefined" )}.json";
        
        private Vector2 scrollPos;
        public bool IsValid => selectedRootNode;

        #endregion

        #region unity methods
        void OnInspectorUpdate()
        {
            // Repaint();
        }
        public void OnGUI()
        {
            List<GameObject> _selectedObjects = Selection.gameObjects.ToList();

            if (_selectedObjects.Count != 1)
                return;
            EditoolsLayout.Vertical(true); 
            EditoolsLayout.Horizontal(true);
            
            EditoolsButton.Button("Save Rig Controls", Color.white, SaveControlRigs );
            EditoolsLayout.Horizontal(false);
            
            EditoolsLayout.Space(3);

            string[] _files = Directory.GetFiles(CURRENT_RESOURCES_FOLDER_PATH)
                .Where(_f => Path.GetExtension(_f) == ".json").ToArray();
            
            // start scroll view
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(300), GUILayout.Height(400));
            foreach (var _file in _files)
            {
                EditoolsLayout.Horizontal(true);
                EditoolsButton.Button($"Load: {Path.GetFileNameWithoutExtension(_file)}", Color.white, () => LoadControlRigs(_file));
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

            if (!_rootCtr)
                return;

            NodeCNT _test = new NodeCNT(_rootCtr.name, 0, 0, 0, 0, _rootCtr.childCount);
            ControllerToNodeObject(_rootCtr, ref _test);

            //RecursiveTest(_test);
            string _json = Newtonsoft.Json.JsonConvert.SerializeObject(_test);
            File.WriteAllText(JSON_FILE_NAME, _json);


#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        }

        private void LoadControlRigs(string _file)
        {
            // get all ctn_hips go
            // Transform _rootCtr = currentGO.transform.Find("CNT_Character");
            Transform _rootCtr = selectedRootNode.transform;

            if (!_rootCtr)
                return;

            StreamReader _reader = new StreamReader(_file);
            NodeCNT _nodeFromJSon = JsonConvert.DeserializeObject<NodeCNT>(_reader.ReadToEnd());
            _reader.Close();

            if (_rootCtr.childCount != _nodeFromJSon.childNumber)
            {
                EditorUtility.DisplayDialog("Error", "Incompatible .json and gameObject", "OK");
                return;
            }
            
            for (int i = 0; i < 12; i++)
            {
                ApplyToSelectedGameObject(ref _rootCtr, _nodeFromJSon);
            }
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

        private int DeepNumber(NodeCNT _rootNode, int _nb = 0, int _max = 0)
        {
            if (_rootNode.next == null)
            {
                Debug.Log(_nb);
            }

            for (int _i = 0; _i < _rootNode.next.Length; _i++)
            {
                DeepNumber(_rootNode.next[_i], _nb + 1, _nb > _max ? _nb + 1 : _max);
            }

            Debug.Log(_nb);

            return _max;
        }
        
        private void RecursiveTest(NodeCNT _rootNode)
        {
            if (_rootNode.next == null)
            {
                Debug.Log(_rootNode.name);
                return;
            }

            for (int _i = 0; _i < _rootNode.next.Length; _i++)
            {
                RecursiveTest(_rootNode.next[_i]);
            }

            Debug.Log(_rootNode.name);
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
                
                ApplyToSelectedGameObject(ref rootTransform, _rootNode.next[_i]);
            }

            // not last
            _rootTransform.rotation = new Quaternion(
                _rootNode.rot_x,
                _rootNode.rot_y,
                _rootNode.rot_z,
                _rootNode.rot_w
                );
        }
        #endregion

    }
}