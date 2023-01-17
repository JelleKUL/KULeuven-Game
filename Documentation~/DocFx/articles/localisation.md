# Localisation
The project is localised and can handle multiple languages.
every textfield in the game has a **key** as a string, which is connected to a **value** in a CSV.

## Variable injection
It is possible to insert variables into the text by using the name of the variable in between `{}`.
- This works for sing variables like: `float`, `int`, ...
- This also supports arrays with indexing `variable[i]`
- Use an `*` after the variable name to scale the variable by the world scale defined by the game manager.

## Supported languages
Currently the game supports the following languages:
- *Dutch* (nl)
- *English* (en)

## Adding a new language

A new language can be added by inseeting a new column in [localisation.csv](../master/Assets/Resources/localisation.csv). The first value should be the name of the language. like in this example:

key | nl | en | ... 
--- | --- | --- | --- 
*ID_unique* | De waarde in het Nederlands | The value in English | ...
... |...|...|...
