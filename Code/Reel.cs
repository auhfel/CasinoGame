using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reel : MonoBehaviour {
    static float reelIconSpacing = 1.75f;
    static int reelIconCount = 11;
    static float iconResetDepth = 0;
    public Transform origin;
    public List<Transform> slotIconTransforms = new List<Transform>();
    bool isSpinning = false;
    public bool isFullyStopped = false;
    // Start is called before the first frame update
    public ReelIconPrefab[] reelResults = new ReelIconPrefab[3];
    [RuntimeInitializeOnLoadMethod]
    static void Init() {
        //The depth at which icons should reset to the top after reaching
        iconResetDepth = (reelIconCount / 2.0f) * -reelIconSpacing;
    }

    void Awake() {
        origin = transform.Find("origin");
        CreateTransformsForSlotIcons();
    }

    /// <summary>
    /// Creates transforms to hold the slot icon prefabs for the reels, at the appropriate offsets
    /// </summary>
    private void CreateTransformsForSlotIcons() {
        Vector3 startPosition = origin.position;
        startPosition.y += reelIconCount / 2 * reelIconSpacing;
        for (int i = 0; i < reelIconCount; i++) {
            GameObject gameObject = new GameObject($"slotObjectTransform_{i}");
            gameObject.transform.position = startPosition;
            gameObject.transform.SetParent(this.transform, true);
            startPosition.y -= reelIconSpacing;
            slotIconTransforms.Add(gameObject.transform);
        }
    }

    public void SetReelIconAtIndex(int index, ReelIconPrefab prefab) {

        Transform slotObjectTransform = slotIconTransforms[index];
        //Destroy any existing icon prefabs. There should only ever be one, but let's do a loop just incase.
        for (int i = slotObjectTransform.childCount - 1; i >= 0; i--)
            Destroy(slotObjectTransform.GetChild(i).gameObject);
        
        ReelIconPrefab copy = Instantiate(prefab);

        //Set the layer of the copy to the same as this reel's layer so it gets rendered by the reel's camera.
        foreach (Transform t in copy.transform)
            t.gameObject.layer = gameObject.layer;

        copy.transform.SetParent(slotObjectTransform);
        copy.transform.localPosition = Vector3.zero;
    }

    public IEnumerator StartSpinning() {
        isSpinning = true;
        isFullyStopped = false;

        //Spin the wheel  for awhile. This used to use user input to stop each reel, but has been made to stop automatically for ease.
        //We could just yield return some amount of time here instead of using the isSpinning bool, but we're keeping the functionality
        //incase we want to both stop the reel automatically and by user input if desired to stop early.
        while (isSpinning) {
            SpinReel();
            yield return null;
        }

        //After the reel is signalled to stop, we don't actually stop it until it reaches the closest stop point so that
        //the reel stops very close to the correct position
        float nearestStopPoint = FloorValueToMultiple(slotIconTransforms[0].position.y, reelIconSpacing);
        float distanceToNearestStopPoint = Mathf.Abs(nearestStopPoint - slotIconTransforms[0].position.y);
        while (distanceToNearestStopPoint > 0) {
            SpinReel();
            float downwardMovement = Main.instance.gameSettings.reelSpinningSpeed * Time.deltaTime;
            distanceToNearestStopPoint -= downwardMovement;
            yield return null;
        }

        for (int i = 0; i < slotIconTransforms.Count; i++) {
            //This forces the stopping position to the nearest stop point, so we can compare it's position so as to set the reel spin results.
            Vector3 stoppingPos = RoundPositionToNearestStoppingPoint(i);
            slotIconTransforms[i].localPosition = stoppingPos;
            SetReelSpinResults(i, stoppingPos);
        }

        Main.instance.reelStoppingSound.Play();
        isFullyStopped = true;
        Vector3 RoundPositionToNearestStoppingPoint(int i) {
            float stopY = RoundValueToNearestMultiple(slotIconTransforms[i].localPosition.y, reelIconSpacing);
            Vector3 stoppingPos = slotIconTransforms[i].localPosition;
            stoppingPos.y = stopY;
            return stoppingPos;
        }
        void SetReelSpinResults(int i, Vector3 stoppingPos) {
            if (stoppingPos.y == reelIconSpacing)
                reelResults[0] = slotIconTransforms[i].GetComponentInChildren<ReelIconPrefab>();
            else if (stoppingPos.y == 0)
                reelResults[1] = slotIconTransforms[i].GetComponentInChildren<ReelIconPrefab>();
            else if (stoppingPos.y == -reelIconSpacing)
                reelResults[2] = slotIconTransforms[i].GetComponentInChildren<ReelIconPrefab>();
        }
    }

    private void SpinReel() {
        for (int i = 0; i < slotIconTransforms.Count; i++) {
            SpinIconDownward(i);
            if (IconHasPassedResetPoint(i)) {
                ResetIconAtIndexToTopOfReelAndRandomizeIt(i);
                RandomizeReelIconAtIndex(i);
            }
        }
        bool IconHasPassedResetPoint(int i) {
            return slotIconTransforms[i].localPosition.y < iconResetDepth;
        }
        void ResetIconAtIndexToTopOfReelAndRandomizeIt(int i) {
            float distancePastResetPoint = slotIconTransforms[i].localPosition.y - iconResetDepth;
            Vector3 positionToResetTo = slotIconTransforms[i].localPosition;
            positionToResetTo.y = -iconResetDepth + distancePastResetPoint;
            slotIconTransforms[i].localPosition = positionToResetTo;
        }
        void SpinIconDownward(int i) {
            float downwardMovement = Main.instance.gameSettings.reelSpinningSpeed * Time.deltaTime;
            Vector3 iconPosition = slotIconTransforms[i].localPosition;
            iconPosition.y -= downwardMovement;
            slotIconTransforms[i].localPosition = iconPosition;
        }
        void RandomizeReelIconAtIndex(int i) {
            SetReelIconAtIndex(i, Main.instance.GetRandomSlotObject());
        }
    }
    public static float RoundValueToNearestMultiple(float value, float multiple) {
        return Mathf.Round(value / multiple) * multiple;
    }
    public static float FloorValueToMultiple(float value, float multiple) {
        return Mathf.Floor(value / multiple) * multiple;
    }
    public void StopReel() {
        isSpinning = false;
    }
}
