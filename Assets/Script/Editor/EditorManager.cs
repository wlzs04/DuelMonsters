using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Assets.Script.Editor
{
    class EditorManager
    {
        [MenuItem("BuildTools/PlayModeUseStartScene")]
        static void SetPlayModeUseStartScene()
        {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
            EditorSceneManager.playModeStartScene = scene;
        }
    }
}
