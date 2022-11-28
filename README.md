# KULeuven-Game
A game made to help the students.  
The game is made in **Unity 3D v2019.4 LTS**.

## Table of contents
- [Project structure](#Project-structure)
    - [Online account system](#Online-account-system)
    - [Localisation](#Localisation)
- [Game structure](#Game-structure)
    - [Game](#Game)
    - [Chapter](#Chapter)
    - [Level](#Level)
- [Database Structure](#Database-Structure)
    - [accounts](#accounts)
    - [custom info](#custom-info)
- [How to add content](#How-to-add-content)
    - [Adding a new Level](#Adding-a-new-Level)
    - [Adding a new Chapter](#Adding-a-new-Chapter)

## Project structure
The project is structured like a Unity project. All the relevant files are in the [/assets](../master/Assets) folder.
You can clone this repo on your system. Be sure to have **Github LFS** installed to also get all the assets.
The projects is using version control through [GitHub for Unity](https://assetstore.unity.com/packages/tools/version-control/github-for-unity-118069).

### Online account system
The project uses Online Account system to use accounts linked to a database to store player records.
It uses an Asset from the AssetStore named: [Online Account System](https://assetstore.unity.com/packages/tools/network/online-account-system-18487).
Go to [Database Structure](#Database-Structure) for more info about the structure of the database.

### Localisation
The project is localised and can handle multiple languages.
every textfield in the game has a **key** as a string, which is connected to a **value** in a CSV.
Currently the game supports the following languages:
- *Dutch* (NL)
- *English* (EN)

New Languages can be added in the [LocalisationManager](../master/Assets/Scripts/Localisation/LocalisationManager.cs)
```C#
public class LocalisationManager : MonoBehaviour
{
    public enum Language { NL, EN } // add new languages here
    public static Language language = Language.NL;

    private static Dictionary<string, string> localisedNL;
    private static Dictionary<string, string> localisedEN;
    //add new languages here
    ...
    public static string GetLocalisedValue(string key)
    {
        ...
        switch (language)
        {
            case Language.NL:
                localisedNL.TryGetValue(key, out value);
                break;
            case Language.EN:
                localisedEN.TryGetValue(key, out value);
                break;
            // add new languages here
        }
        ...
        return value;
    }
    ...
}
```
Then add new values to [localisation.csv](../master/Assets/Resources/localisation.csv) to add the new language to the game.

key | nl | en | ... 
--- | --- | --- | --- 
*ID_unique* | De waarde in het Nederlands | The value in English | ...
... |...|...|...


## Game structure
The most important parts of the game structure is strored in **scriptable objects** in [/Chapters](../master/Assets/Chapters). These scriptable object allow for prersystent data storage without needing scene representation.

### Game
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

### Chapter
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
### Level
A level is one exercise where the player has to enter a correct answer to unlock the next exercise.
this is a unique **scene** where all the relevant parameters can be changed to the creators liking.


## Database Structure
The database files can be found at [/Server-Side Scripts](../master/Assets/Online%20Account%20System/Server-Side%20Scripts). these files are uploaded to a PHP & Mysql compatible server.

### accounts
All the accounts are stored in a database with the following structure.

id | studentnr | custominfo | creationdate
--- | --- | --- | --- 
auto generated identifier | the unique student number | all the game related information | the date when the account was created

### custom info
the custominfo contains an XML-serialized string which holds all the relevant game info (can be changed in the game).

```C#
public class AS_CustomInfo
{
    public int totalScore = 0; // the total score a player has
    public List<ChapterInfo> chapters; // a list of all the chapters in the game
    {
        public string UID = ""; // a unique UID to identify the chapter in the game
        public List<int> scores = new List<int>(); // a list of the score of each level. this also tracks if the player has competed the level when the score is higher then zero
    }
}
```

## Shibboleth
This project uses shibboleth as another way to authenticate.
Using SimplSamlPHP for the back end: [SimpleSamlPHP Documentation](https://simplesamlphp.org/docs/stable/)
Integrated with KULeuven Identity provider: [KULeuven integration Documentation](https://admin.kuleuven.be/icts/services/aai/documentation/sp/install-simplesamlphp-sp.html).

## How to add content
This game can be easily expanded either with new chapters or new levels.

### Adding a new Level

1. Create a new Scene in */Assets/Scenes/(Chapter)*.
2. Empty out the scene and add a ``ScenePrefab``from *Assets/Prefabs/ScenePrefabs* of the desired Chapter. [You can also create a new Chapter](##Adding-a-new-Chapter).
3. Navigate to the ``(Chapter)Manager`` (eg. ``WaterPassingManager``).
4. Create a new **key** and **values** in the [localisation.csv](../master/Assets/Resources/localisation.csv) and add them to the ``(Chapter)Questions``. 
5. You can add values from public variables in the respective ``(Chapter)Controller`` to the question title and explanation by inserting ``{VARIABLE_NAME}`` or ``{VARIABLE_NAME[i]}`` in the localised values. adding a ``*`` in between the ``{}`` will multiply the value by the ``worldScale`` e.g. "Plaats de meetbaken op de ``{nrOfPoints}`` meetpunten, met Coördinaten: ``{meetpunten[0]*}``,``{meetpunten[1]*}``."
5. You can also change the Error Margin & Unit, NrOfTries and Score Increase.
6. Choose the AnswerType to fit your excersice.
7. Change the parameters in the ``(Chapter)Controller`` to fit your excercise.
8. Add your **Level** to the relevant **Chapter** and press the **Update Build Settings** button in the [ChapterList](../master/Assets/Chapters) to add the scene to the build settings at their correct place.

### Adding a new Chapter

1. Create a new ``ChapterScriptableObject`` **Chapter** in */Assets/Chapters* from the Project **+** dropdown menu under *ScriptableObjects/chapter*.
2. Fill in the relevant information like the title and explanation (the UID is automatically generated)
3. Add a cover image in */Assets/Images/Banners* and link it to the **Chapter**
4. Create a new folder in */Assets/Scenes/* to house you new levels.
5. if your new **Chapter** doesn't fit the existing controllers do the following extra steps:
    - Create a new ``(Chapter)Questions`` script from the [QuestionTemplate](../master/Assets/Scripts/Templates)
    - Create a new ``(Chapter)Controller`` script to match the new gameplay (use other controllers as reference)
6. Create some new **Levels** in the new folder and add them to the list in the ``ChapterScriptableObject``.
7. Add your new **Chapter** to the [ChapterList](../master/Assets/Chapters) and press the **Update Build Settings** button to add the chapter and scenes to the build settings at their correct place.
