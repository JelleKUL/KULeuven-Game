# How to add content
This game can be easily expanded either with new chapters or new levels.

## Adding a new Level

1. Create a new Scene in */Assets/Scenes/(Chapter)*.
2. Empty out the scene and add a ``ScenePrefab``from *Assets/Prefabs/ScenePrefabs* of the desired Chapter. [You can also create a new Chapter](##Adding-a-new-Chapter).
3. Navigate to the ``(Chapter)Manager`` (eg. ``WaterPassingManager``).
4. Create a new **key** and **values** in the [localisation.csv](../master/Assets/Resources/localisation.csv) and add them to the ``(Chapter)Questions``. 
5. You can add values from public variables in the respective ``(Chapter)Controller`` to the question title and explanation by inserting ``{VARIABLE_NAME}`` or ``{VARIABLE_NAME[i]}`` in the localised values. adding a ``*`` in between the ``{}`` will multiply the value by the ``worldScale`` e.g. "Plaats de meetbaken op de ``{nrOfPoints}`` meetpunten, met Coördinaten: ``{meetpunten[0]*}``,``{meetpunten[1]*}``."
5. You can also change the Error Margin & Unit, NrOfTries and Score Increase.
6. Choose the AnswerType to fit your excersice.
7. Change the parameters in the ``(Chapter)Controller`` to fit your excercise.
8. Add your **Level** to the relevant **Chapter** and press the **Update Build Settings** button in the [ChapterList](../master/Assets/Chapters) to add the scene to the build settings at their correct place.

## Adding a new Chapter

1. Create a new ``ChapterScriptableObject`` **Chapter** in */Assets/Chapters* from the Project **+** dropdown menu under *ScriptableObjects/chapter*.
2. Fill in the relevant information like the title and explanation (the UID is automatically generated)
3. Add a cover image in */Assets/Images/Banners* and link it to the **Chapter**
4. Create a new folder in */Assets/Scenes/* to house you new levels.
5. if your new **Chapter** doesn't fit the existing controllers do the following extra steps:
    - Create a new ``(Chapter)Questions`` script from the [QuestionTemplate](../master/Assets/Scripts/Templates)
    - Create a new ``(Chapter)Controller`` script to match the new gameplay (use other controllers as reference)
6. Create some new **Levels** in the new folder and add them to the list in the ``ChapterScriptableObject``.
7. Add your new **Chapter** to the [ChapterList](../master/Assets/Chapters) and press the **Update Build Settings** button to add the chapter and scenes to the build settings at their correct place.