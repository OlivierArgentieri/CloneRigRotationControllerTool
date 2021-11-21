using System;
using System.IO;
using CloneRigRotationTool.Assets.CloneRigRotationTool.Models;
#if UNITY_2021
#endif
using UnityEditor;
using UnityEngine;

namespace CloneRigRotationTool.Assets.CloneRigRotationTool.Editor.CloneRigToolController
{
    public class CloneRigToolController
    {
        public static Version version = new (0,0,2);
        #region f/p
        private static string CURRENT_RESOURCES_FOLDER_PATH => $"{Application.dataPath}/Resources";
        #endregion
        
        #region methods
        private static void ControllerToNodeObject(Transform _rootTransform, ref NodeCNT _rootNode)
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
        
        public static void SaveControlRigs(GameObject _selectedRootNode)
        {
            // check if directory exist
            if (!Directory.Exists(CURRENT_RESOURCES_FOLDER_PATH))
            {
                // and create it
                Directory.CreateDirectory(CURRENT_RESOURCES_FOLDER_PATH);
            }

            // get all ctn_hips go
            Transform _rootCtr = _selectedRootNode.transform;

            if (!_rootCtr) return;

            NodeCNT _test = new NodeCNT(_rootCtr.name, 0, 0, 0, 0, _rootCtr.childCount);
            ControllerToNodeObject(_rootCtr, ref _test);

            //RecursiveTest(_test);
            string _json = Newtonsoft.Json.JsonConvert.SerializeObject(_test);
            File.WriteAllText(GetJsonFileName(_selectedRootNode.name), _json);
            
            // first function instruction 'IsValid' protect from run outside editor mode
            AssetDatabase.Refresh();
        }

        public static void ApplyToSelectedGameObject(Transform _rootTransform, NodeCNT _rootNode)
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
                ApplyToSelectedGameObject(rootTransform, _rootNode.next[_i]);
            }

        }
        
        private static string GetJsonFileName(string _rootName) => $"{CURRENT_RESOURCES_FOLDER_PATH}/{_rootName}.json";
        #endregion
    }
}