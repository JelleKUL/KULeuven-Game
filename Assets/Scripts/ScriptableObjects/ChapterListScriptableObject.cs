using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "ChapterList", menuName = "ScriptableObjects/ChapterList", order = 1)]
public class ChapterListScriptableObject : ScriptableObject
{
    [Scene] public List<string> menuScenes = new List<string>();
    [Scene] public string chapterEndScene;
    public List<ChapterScriptableObject> chapters;

#if UNITY_EDITOR
    public void UpdateBuildSettings()
    {
        Debug.Log("Updating Build settings");

        List<EditorBuildSettingsScene> sceneSettings = new List<EditorBuildSettingsScene>();

        foreach (var menuScene in menuScenes)
        {
            sceneSettings.Add(SetScene(GetSceneObject(menuScene)));
        }

        foreach (var chapter in chapters)
        {
            foreach (var level in chapter.levels)
            {
                sceneSettings.Add(SetScene(GetSceneObject(level)));
            }
        }
        sceneSettings.Add(SetScene(GetSceneObject(chapterEndScene)));


        EditorBuildSettings.scenes = sceneSettings.ToArray();

    }
    EditorBuildSettingsScene SetScene(SceneAsset scene)
    {
        if (scene == null) return null;
        return new EditorBuildSettingsScene(AssetDatabase.GetAssetOrScenePath(scene), true);
    }

    protected SceneAsset GetSceneObject(string sceneObjectName)
    {
        if (string.IsNullOrEmpty(sceneObjectName))
        {
            return null;
        }

        foreach (var editorScene in EditorBuildSettings.scenes)
        {
            if (editorScene.path.IndexOf(sceneObjectName) != -1)
            {
                return AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
            }
        }
        Debug.LogWarning("Scene [" + sceneObjectName + "] cannot be used. Add this scene to the 'Scenes in the Build' in build settings.");
        return null;
    }
#endif
}
