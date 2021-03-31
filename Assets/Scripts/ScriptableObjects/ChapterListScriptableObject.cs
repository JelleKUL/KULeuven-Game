using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterList", menuName = "ScriptableObjects/ChapterList", order = 1)]
public class ChapterListScriptableObject : ScriptableObject
{
    public List<ChapterScriptableObject> chapters;
}
