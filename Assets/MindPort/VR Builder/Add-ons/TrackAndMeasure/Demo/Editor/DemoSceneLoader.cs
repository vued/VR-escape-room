using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRBuilder.Editor.Animations.DemoScene
{
    /// <summary>
    /// Menu item for loading the demo scene after checking the process file is in the StreamingAssets folder.
    /// </summary>
    public static class DemoSceneLoader
    {
        private const string demoScenePath = "Assets/MindPort/VR Builder/Add-ons/TrackAndMeasure/Demo/Scenes/VR Builder Demo - Track and Measure.unity";
        private const string demoProcessOrigin = "Assets/MindPort/VR Builder/Add-ons/TrackAndMeasure/Demo/StreamingAssets/Processes/Demo - Track and Measure/Demo - Track and Measure.json";
        private const string demoProcessDirectory = "Assets/StreamingAssets/Processes/Demo - Track and Measure";
        private const string demoProcessDestination = "Assets/StreamingAssets/Processes/Demo - Track and Measure/Demo - Track and Measure.json";

        [MenuItem("Tools/VR Builder/Demo Scenes/Track and Measure", false, 64)]
        public static void LoadDemoScene()
        {
            if (File.Exists(demoProcessDestination) == false)
            {
                if(EditorUtility.DisplayDialog("Demo Scene Setup", "Before opening the demo scene, the sample process needs to be copied in Assets/StreamingAssets. Press Ok to proceed.", "Ok"))
                {
                    Directory.CreateDirectory(demoProcessDirectory);
                    FileUtil.CopyFileOrDirectory(demoProcessOrigin, demoProcessDestination);
                }
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(demoScenePath);
        }
    }
}