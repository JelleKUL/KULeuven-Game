using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Chapter", menuName = "ScriptableObjects/Chapter", order = 1)]
public class ChapterScriptableObject : ScriptableObject
{
    [ReadOnly] public string UID;
    public string chapterName;
    public string chapterExplanation;
    public Sprite coverImage;
    [Scene] public List <string> levels;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (UID == "")
        {
            UID = GUID.Generate().ToString();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
