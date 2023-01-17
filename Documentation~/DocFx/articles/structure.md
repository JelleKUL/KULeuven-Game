# Game structure
The most important parts of the game structure is strored in **scriptable objects** in [/Chapters](../master/Assets/Chapters). These scriptable object allow for prersystent data storage without needing scene representation.

## Game
The game is set up around menu scenes which let the user log in, check leaderboards and play the game.
The game contains a number of **chapters**. 
```C#
public class ChapterListScriptableObject : ScriptableObject
{
    [Scene] public List<string> menuScenes = new List<string>(); // the extra menu scenes
    [Scene] public string chapterEndScene; // the end scene to show at the end of every chapter
    public List<ChapterScriptableObject> chapters; // the list of all the chapters
}
```

## Chapter
Each chapter has a certain **topic** and contains a number of **levels**.
```C#
public class ChapterScriptableObject : ScriptableObject
{
    [ReadOnly] public string UID; // the unique identifier
    public string chapterName; // the name of the chapter
    public string chapterExplanation; // the explanation of the chapter
    public Sprite coverImage; // the cover image of the chapter
    [Scene] public List <string> levels; // a list of all the levels (as UnityScenes)
}
```
## Level
A level is one exercise where the player has to enter a correct answer to unlock the next exercise.
this is a unique **scene** where all the relevant parameters can be changed to the creators liking.