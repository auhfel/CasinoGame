# Casino Slot Game Demo
A Casino slot game **made in 10 hours over two days** to demonstrate my proficiency in Unity3D and C#.
Has not been subjected to extensive quality assurance and bugs may exist.

## Notes
Only the code I wrote for the game is included here. Other engine files (prefabs, assets, etc) are omitted as I've used some paid assets and I don't want to include them in a public repository.
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
