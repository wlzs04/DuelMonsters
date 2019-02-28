using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Script.Editor
{
    [InitializeOnLoad]
    class EditorManager
    {
        static EditorManager()
        {
            Debug.Log("编辑器启动！");
            EditorApplication.delayCall += SetPlayModeUseStartScene;
        }

        //[MenuItem("BuildTools/PlayModeUseStartScene")]
        static void SetPlayModeUseStartScene()
        {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
            EditorSceneManager.playModeStartScene = scene;
        }
    }
}
