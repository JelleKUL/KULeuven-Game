using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChapterListScriptableObject))]
public class ChapterListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20);

        ChapterListScriptableObject chapterlist = (ChapterListScriptableObject)target;
        if(GUILayout.Button("Update Build Settings"))
        {
            chapterlist.UpdateBuildSettings();
        }
       

        
    }
}
