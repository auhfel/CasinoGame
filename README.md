# Casino Slot Game Demo
A Casino slot game _made in 10 hours over two days_ to demonstrate my proficiency in Unity3D and C#.
_Has not been subjected to extensive quality assurance and bugs may exist._

## Notes
Only the code I've written for the game is included here. Other engine files (prefabs, assets, etc) are omitted as paid assets are used and shouldn't be included in a public repository.
Note that many values are initialized in the Unity Editor, so some initialization may not be apparent.

 ## Most Relevant Code
 Most of the game logic resides in Main.cs, Reel.cs, SlotLines.cs, and ReelIconPrefab.cs
 ### **Main.cs**
 Controls the entire game loop in a coroutine. Responsible for handling user input and controlling event (Coroutine) timing in an ordered fashion.
 ### **Reel.cs**
 Handles the logic of spinning and randomizing the icons of the reels.
 ### **SlotLines.cs**
 Creates the slot lines to use for scoring, and contains the scoring algorithm.
 ### **ReelIconPrefab.cs**
 Base class for Icons. Has virtual Animate coroutines that can be overrided to animate each icon prefab as desired.
