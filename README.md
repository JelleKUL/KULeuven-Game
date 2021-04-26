# KULeuven-Game
A game made to help the students.  
The game is made in **Unity 3D v2019.4 LTS**.

## Table of contents

* Project Structure
* Database Structure
* Game Structure
* How to add levels

### Project structure
The project is structured like a Unity project. All the relevant files are in the [/assets](../blob/master/Assets) folder. 
The project uses Online Account system to use accounts linked to a database to store player records.



### Game structure
The game is set up to have a number of **chapters**. Each chapter has a certain theme and contains a number of **levels**.<br>
A level is one exercise where the player has to enter a correct answer to unlock the next exercise.<br>
Each chapter is a **scriptable object** which contains:
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


### Database Structure
The database files can be found at [/Server-Side Scripts](../blob/master/Assets/Online%20Account%20System/Server-Side%20Scripts). these files are uploaded to a PHP & Mysql compatible server.

#### accounts
All the accounts are stored in a database with the following structure.

id | username | password | studentnr | custominfo | creationdate
--- | --- | --- | --- | --- | ---
auto generated identifier | a unique username | the user password | the student nr for reference | all the game related information | the date when the account was created

#### custom info
the custominfo contains an XML-serialized string which holds all the relevant game info (can be changed in the game).

```C#
public class AS_CustomInfo
{
    public int totalScore = 0; // the total score a player has
    public List<ChapterInfo> chapters; // a list of all the chapters in the game
    {
        public string UID = ""; // a unique UID to identify the chapter in the game
        public List<int> scores = new List<int>(); //
    }
}
````