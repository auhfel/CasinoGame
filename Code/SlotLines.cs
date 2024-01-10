using System.Collections.Generic;
using UnityEngine;


public static class SlotLines {
    static Vector2Int[][] lines;
    static List<Vector2Int> winningLines;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() {
        Vector2Int[] topLine = new Vector2Int[] {
        new Vector2Int(0,0),
        new Vector2Int(1,0),
        new Vector2Int(2,0),
        new Vector2Int(3,0),
        new Vector2Int(4,0),
        };
        Vector2Int[] midLine = new Vector2Int[] {
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(2,1),
        new Vector2Int(3,1),
        new Vector2Int(4,1),
        };
        Vector2Int[] bottomLine = new Vector2Int[] {
        new Vector2Int(0,2),
        new Vector2Int(1,2),
        new Vector2Int(2,2),
        new Vector2Int(3,2),
        new Vector2Int(4,2),
        };
        Vector2Int[] vLine = new Vector2Int[] {
        new Vector2Int(0,0),
        new Vector2Int(1,1),
        new Vector2Int(2,2),
        new Vector2Int(3,1),
        new Vector2Int(4,0),
        };
        Vector2Int[] invertedVLine = new Vector2Int[] {
        new Vector2Int(0,2),
        new Vector2Int(1,1),
        new Vector2Int(2,0),
        new Vector2Int(3,1),
            new Vector2Int(4,2),
        };
        lines = new Vector2Int[][] { topLine, bottomLine, midLine, vLine, invertedVLine };
        winningLines = new List<Vector2Int>();
    }

    public static float CalculateWinnings(ReelIconPrefab[][] results) {
        float winnings = 0;
        for (int i = 0; i < lines.Length; i++) {
            Vector2Int[] linePoints = lines[i];
            DetermineSlotIconToUse(linePoints, results, out ReelIconPrefab slotIconToUse, out bool allWilds);
            //If all icons in the line are wild, then use the first wild as the icon to use.
            if (allWilds)
                slotIconToUse = results[linePoints[0].x][linePoints[0].y];
            //Count the number of matches we find until we hit a non-wild or non-matching object.
            int matchCount = CountMatchesInLine(linePoints, results, slotIconToUse);
            //Use match count to lookup bet multiplier to apply for winnings
            float lineWinnings = slotIconToUse.modifier[matchCount] * Main.instance.gameState.currentBetAmount;
            if (lineWinnings > 0) {
                winningLines.Add(new Vector2Int(i, matchCount));
                winnings += lineWinnings;
            }

        }
        return winnings;
    }

    /// <summary>
    /// Iterates the winning lines up to how far they matched, used for animating the winning icons.
    /// </summary>
    /// <param name="results">The slot icons on the screen for each reel</param>
    /// <returns> Returns an iterator to the current winning icon</returns>
    public static IEnumerable<ReelIconPrefab> IterateWinningSlotIcons(ReelIconPrefab[][] results) {
        for (int i = 0; i < winningLines.Count; i++) {
            int winningLineIndex = winningLines[i].x;
            int matchCount = winningLines[i].y;
            Vector2Int[] linePoints = lines[winningLineIndex];
            for (int a = 0; a <= matchCount; a++) {
                yield return results[linePoints[a].x][linePoints[a].y];
            }
        }
        winningLines.Clear();
    }
    /// <summary>
    /// Counts the number of times the icon occurs consecutively in the line for scoring purposes.
    /// </summary>
    /// <param name="points">The points of the line to check</param>
    /// <param name="results">The slot icons on the screen for each reel</param>
    /// <param name="slotIconToUse"> The slot icon to use for scoring</param>
    /// <returns>Returns an integer representing the number of matches found</returns>
    private static int CountMatchesInLine(Vector2Int[] points, ReelIconPrefab[][] results, ReelIconPrefab slotIconToUse) {
        int matchCount = -1;
        for (int i = 0; i < points.Length; i++) {
            ReelIconPrefab current = results[points[i].x][points[i].y];
            if (current.id == slotIconToUse.id || current.isWild)
                matchCount++;
            else
                break;
        }
        return matchCount;
    }

    /// <summary>
    /// Loops through all results in the line, finding the first non-wild icon and if all icons in the line are wild if no non-wild icon is found.
    /// </summary>
    /// <param name="points">The points of the line to check</param>
    /// <param name="results">The slot icons on the screen for each reel</param>
    /// <param name="slotIconToUse"> The slot icon to use for scoring</param>
    /// <param name="allIconsAreWild">Boolean value showing if all icons are wild</param>
    private static void DetermineSlotIconToUse(Vector2Int[] points, ReelIconPrefab[][] results, out ReelIconPrefab slotIconToUse, out bool allIconsAreWild) {
        slotIconToUse = null;
        allIconsAreWild = true;
        for (int i = 0; i < points.Length; i++) {
            ReelIconPrefab slotObject = results[points[i].x][points[i].y];
            if (allIconsAreWild && slotObject.isWild == false)
                allIconsAreWild = false;
            if (slotObject.isWild == false && slotIconToUse == null) {
                slotIconToUse = slotObject;
            }
        }
    }
}
