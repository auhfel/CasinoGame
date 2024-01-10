# CasinoGame
 A Demo slot style casino game for demonstration purposes

## Notes
Only the code I wrote for the game is included here. Other engine files (prefabs, assets, etc) are omitted as I've used some paid assets and I don't want to include them in a repo.
Note that many values are initialized in the Unity Editor, so some initialization may not be apparent.
This game demo was** created in roughly two days, ~10 hours total.** It is only a quick mockup and** has not been subjected to extensive quality assurance**.

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
