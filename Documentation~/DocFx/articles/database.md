# Saving Progress
The database files can be found at [/Server-Side~](https://github.com/JelleKUL/KULeuven-Game/tree/main/Server-Side~). These files are uploaded to a PHP & Mysql compatible server.

## accounts
Using SimplSamlPHP for the back end: [SimpleSamlPHP Documentation](https://simplesamlphp.org/docs/stable/)
Integrated with KULeuven Identity provider: [KULeuven integration Documentation](https://admin.kuleuven.be/icts/services/aai/documentation/sp/install-simplesamlphp-sp.html).
The service returns the student ID to linkk the progress to the student.
All the accounts are stored in a database with the following structure.

id | studentnr | custominfo | creationdate
--- | --- | --- | --- 
auto generated identifier | the unique student number | all the game related information | the date when the account was created

## custom info
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