# MSc Thesis Academic Games Tim Yeung 2024
 Academic test game made through collaboration between UT and MPI

### CHANGELOG 08/02/2025

- Updated settings menu:
    - Mapper menu for input buttons, ability to rebind controls to supported keyboard and game controller keys
    - Added option for FMRI await before scene
- Fixes for sound: wrong sounds were playing with wrong effects for certain scenes, loudness of sound was not consistent, ambient sound would not play in certain scenes
- Custom ambient sound import, users can now insert their own ambient sound into scenes by adding files within StreamingAssets/Ambient/*NAME_OF_ENVIRONMENT_IN_CAPS*.wav
- Game now continues directly to feedback/next object during an encounter after an input is registered, rather than waiting for the timer to run out.
- Replaced and added music and additional sounds for level end and cutscenes
- Added ending cutscene
- Added text at start of the level to indicate the current location (changeable within the localization file)
- Added credits
- Added exit button in main menu
- Escape key/ABORT key within levels will now return the player to the main menu.
- Added frame rate discrepancy logger for testing purposes and extra column in timing data file to set the threshold before a discrepancy is logged
</aside>

### Summary

**Legend of the Lunchbox** is an adventure “sorting” game where the player traverses a kingdom where inanimate objects, once cursed, seek to regain their lost properties. The player's mission is to recover their stolen lunchbox, journeying through three biomes: the grassy hills, the inner city market, and the castle. Each biome corresponds to a specific type of challenge (action, sound, or mixed), and the character moves automatically through these environments.

Throughout the journey, the player encounters enemies and potions. Before each encounter, a warning is given through a sound and visual cue. When the encounter begins, the player enters the main character's mind, where they visualize properties based on their perception of the enemy. The challenge is to determine whether these properties align with the enemy's characteristics. Through correct identifications, the player builds points. At the end of the encounter, the accumulated points are compared to the enemy's health. If successful, the player lifts the enemy’s curse, restoring them to vivid animation.

The kingdom is under a curse that has rendered its objects inanimate. As the player defeats enemies, they restore the world, making it more colorful and lifelike. However, failing encounters will result in losing imagination (essentially the life bar of the player) which causes the world to become increasingly grey, and lifeless.

Potions which can replenish the player’s imagination, are also encountered along the way.

At the end of the game, the player retrieves their lunchbox, concluding the journey with a sense of closure.

Multiple trials may occur within a single encounter, making the gameplay dynamic and challenging.

## Before you start

### Prerequisites:

In order to run the properly the game the game requires 3 files that are to be provided by the user. In the distribution version of the game, dummy files as examples will be provided. The 3 files are located within the folder of the game under the folders “Legend of the Lunchbox_Data/StreamingAssets” and are named as follows:

- LOTL_locale.csv
- LOTL_timing.csv
- LOTL_trials.csv

All files are formated as csv and can be read and edited using common spreadsheet programs like Microsoft Excel.

### LOTL_Locale

Contains the language data for different localizations of the game. An english and german version is provided. **Please do not change the order or ID of the entries as these are tied to specific text elements within the game. It is possible to add additional languages by creating a new column to the right of the existing columns and translating the provided entries.**

![image.png](attachment:5d7083c7-16dd-43b0-b649-4704a3624186:image.png)

### LOTL_timings.csv

Contains the variables relating to the duration of the states/screens within the game. Specific information on when these values are used can be found in the following video:

/missing

![image.png](attachment:38f397c9-2618-437e-910d-b7126de6d795:image.png)

### LOTL_trials

Contains the trial data to be used within the game. Each entry describes the presentation of a single trial. Thus it contains the presented object, the presented stimuli, the correctness of the combination, and timing data such as the delay before the start of a mini-block and the ITI which can be changed individually for jittering trials. The columns Level indicates in which level of the game the trial appears. Level 0 acts as a tutorial level. **The entries with Level 0 should retain their ObjectType and PropertyType so that the tutorial plays correctly. For all other levels at least one entry is required but more can be added without interference.**

![image.png](attachment:90c8e0d0-249d-4710-9370-75f0aa9c12ca:image.png)

### ASSETS folder

Within the previous folder, you may also find the ASSETS folder. Within this folder, external assets for the game should be put. **Each asset that is referenced within LOTL_trials should be present within the ASSETS folder, matched by the name stated under StimulusObject or StimulusProperty. I.e. an image “hammer_objekt.png” should be present for an IMAGE StimulusObject “hammer_objekt”. Only PNG and WAV files are accepted for external assets with PNG files requiring a size of 720 by 720 pixels**. Animations can be added as objects or properties within the game by providing a sequence of PNG images numbered and separated by a “-” or dash character. I.e. you could have an animation with multiple files called “hammer-01”, “hammer-02”, “hammer-03”, etc. The number of numbers after the dash do not matter as long as the files are named in ascending order to ensure that the correct image follows one after the other. The background of the images should be transparent.

### ERROR logs

Within the folder, you may also find error logs that are generated when the game runs into unexpected problems. If you find any irregularities within the game, please check these logs and submit them (over mail or otherwise) for further investigation.

## The Gameplay

Legend of the Lunchbox comprises of a main menu, an opening cutscene, a tutorial level, and 3 main levels. Within the levels, the player plays through “encounters” or mini-blocks of trials where they judge properties of a single object. 

[Legend of the Lunchbox 2025-01-25 22-36-17 - Trim.mp4](attachment:00730fa8-e94a-4f7a-9d19-5c3cb22fbc33:Legend_of_the_Lunchbox_2025-01-25_22-36-17_-_Trim.mp4)

Players approve or reject these properties using the two arrow keys of a keyboard LEFT and RIGHT. Additionally, players have a “life bar”, something that gives an indication of their performance. If they make too many mistakes during a mini-block, the bar will deplete by a small percentage. Making too many mistakes will lock the bar at a low percentage (although it will never entirely deplete). 

In order to increase their bar, players may encounter potions instead of objects. Players are then prompted to rapidly press both LEFT and RIGHT buttons to the loosen the cork and drink the potion. Sound and visual effects give the illusion of players progressing in this goal, although in reality it is impossible to fail this task. 

[Legend of the Lunchbox 2025-01-25 22-36-17 - Trim.mp4](attachment:67d3db68-bc94-4fdb-b006-0397608da8a1:Legend_of_the_Lunchbox_2025-01-25_22-36-17_-_Trim.mp4)

At the end of a “level” or block, the player is shown a screen showcasing their performance. They will then be able to take a break in real life if necessary and then progress by pressing either of the buttons. 

[Legend of the Lunchbox 2025-01-25 22-36-17 - Trim.mp4](attachment:ad17d20d-8dc6-4652-80c4-65d2075d6643:Legend_of_the_Lunchbox_2025-01-25_22-36-17_-_Trim.mp4)

## Controls

| CONTROL | DESCRIPTION |
| --- | --- |
| LEFT | “Untrue” in trials |
| RIGHT | “True” in trials |
| UP | Speed up time x4 (FOR TESTING PURPOSES). (Not yet implemented: menu navigation) |
| DOWN | Skip level (FOR TESTING PURPOSES). (Not yet implemented: menu navigation) |
| SUBMIT | (Not yet implemented: menu navigation) |
| CANCEL | Exit level and return to main menu (Not yet implemented: menu navigation) |
| FMRI | Key to listen to for FMRI pulses |

## Additional Settings

Legend of the Lunchbox provides additional options for academic purposes and to adapt the level of control necessary based on the setting. These include options for the following:

![image.png](attachment:71c5894d-96bc-40b6-857e-58bfa3442277:image.png)

### Sound:

Sounds can be turned on or of during levels/blocks. If turned off, the only sounds that will play during the levels will be that of trial-specific sounds.

### In-Trial Feedback:

In-trial feedback that occurs within a mini-block/”encounter” may be turned off.

### Prompt:

Text prompts that explicitly help the player in reminding their current task before presenting the property may be turned off.

### Language:

Any languages that have been provided in the LOTL_Locale file (see Set Up on the main page), may be chosen.

### Calibrate size:

This screen provides a preview of the first image asset found within the ASSETS folder. Using the slider and the numerical box, the base size of all images within the game can be tweaked.

### Check files:

Pressing this will run multiple tests on the input files provided to detect most of the major errors that will prevent the game from running as intended. Use the information from the tests displayed underneath the button to troubleshoot the issues within the input files.
