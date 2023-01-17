# Localisation
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
