using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    public static Main instance;

    //UI elements
    public TextMeshProUGUI creditsAmountUI, betAmountUI, winningsAmountUI;
    public Slider wildSlider;

    //Audio Elements
    public AudioSource reelStoppingSound, reelSpinningSound, smallWinSound, mediumWinSound, bigWinSound;

    //Scriptable Objects
    public GameSettings gameSettings;
    public GameState gameState;

    public List<ReelIconPrefab> slotIconPrefabs = new List<ReelIconPrefab>();
    public ReelIconPrefab wildIconPrefab;
    public List<Reel> reels = new List<Reel>();
    List<ReelIconPrefab> animatingIcons = new List<ReelIconPrefab>();
    ReelIconPrefab[][] results = new ReelIconPrefab[5][];
    bool spinReelsButton = false;

    private void Awake() {
        //Singleton pattern for ease of coding. Great for prototyping, bad for long-term project maintenance.
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start() {
        for (int i = 0; i < slotIconPrefabs.Count; i++) {
            slotIconPrefabs[i].SetId(i);
        }
        for (int i = 0; i < reels.Count; i++) {
            Reel reel = reels[i];
            results[i] = reels[i].reelResults;
            for (int a = 0; a < reel.slotIconTransforms.Count; a++) {
                reel.SetReelIconAtIndex(a, GetRandomSlotObject());
            }
        }
        StartCoroutine(GameLoop());
    }

    public ReelIconPrefab GetRandomSlotObject() {
        //We could cache this spawn rate, but if we calculate it every frame we can update it dynamically for debugging and testing purposes.
        float spawnWeightOfAllItems = 0;
        for (int i = 0; i < slotIconPrefabs.Count; i++)
            spawnWeightOfAllItems += slotIconPrefabs[i].spawnRate;
        float randomWeight = Random.Range(0, spawnWeightOfAllItems);
        float currentWeight = 0;
        for (int i = 0; i < slotIconPrefabs.Count; i++) {
            currentWeight += slotIconPrefabs[i].spawnRate;
            if (randomWeight <= currentWeight)
                return slotIconPrefabs[i];
        }
        //It should be impossible to not return something with the previous loop
        Debug.LogError("Somehow didn't get a random slot object");
        return null;
    }
    public void IncrementBet() {
        gameState.currentBetAmount += gameSettings.betIncrement;
        if (gameState.currentBetAmount > gameSettings.maximumBet)
            gameState.currentBetAmount = gameSettings.maximumBet;
        betAmountUI.text = $"{gameState.currentBetAmount}";
    }
    public void DecrementBet() {
        gameState.currentBetAmount -= gameSettings.betIncrement;
        if (gameState.currentBetAmount < gameSettings.betIncrement)
            gameState.currentBetAmount = gameSettings.betIncrement;
        betAmountUI.text = $"{gameState.currentBetAmount}";
    }
    void ChangeCredits(float amount) {
        gameState.currentCredits += amount;
        creditsAmountUI.text = $"{gameState.currentCredits}";
    }
    IEnumerator GameLoop() {
        ChangeCredits(10000);
        IncrementBet();
        while (true) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.S) || spinReelsButton == true) {
                spinReelsButton = false;
                yield return StartCoroutine(SpinReels());
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                DecrementBet();
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                IncrementBet();
            if (Input.GetKeyDown(KeyCode.Escape))
                yield break;
            wildIconPrefab.spawnRate = Mathf.Lerp(0, 100, wildSlider.value);
            yield return null;
        }
    }

    public void SpinReelsButton() {
        spinReelsButton = true;
    }
    IEnumerator PlayWinningSound(float winAmount) {
        float winAmountModifier = winAmount / gameState.currentBetAmount;
        if (winAmountModifier <= 5)
            smallWinSound.Play();
        else if (winAmountModifier > 5)
            mediumWinSound.Play();
        else if (winAmountModifier > 25)
            bigWinSound.Play();

        //Play the winning sound for 3 seconds or until the user spins the reel again
        float timer = 3f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.S) || spinReelsButton == true) {
                spinReelsButton = false;
                timer = 0;
            }
            yield return null;
        }
        if (winAmountModifier <= 5)
            smallWinSound.Stop();
        else if (winAmountModifier > 5)
            mediumWinSound.Stop();
        else if (winAmountModifier > 25)
            bigWinSound.Stop();
    }
    IEnumerator SpinReels() {
        winningsAmountUI.text = string.Empty;
        foreach (ReelIconPrefab slotIcon in animatingIcons)
            slotIcon.StopAnimation();
        animatingIcons.Clear();
        ChangeCredits(-gameState.currentBetAmount);
        reelSpinningSound.Play();
        for (int i = 0; i < reels.Count; i++)
            StartCoroutine(reels[i].StartSpinning());
        yield return new WaitForSecondsRealtime(1f);
        int currentReel = 0;
        while (currentReel < reels.Count) {
            yield return new WaitForSecondsRealtime(.40f);
            reels[currentReel].StopReel();
            currentReel++;
        }
        bool allReelsStopped = reels[0].isFullyStopped && reels[1].isFullyStopped && reels[2].isFullyStopped && reels[3].isFullyStopped && reels[4].isFullyStopped;
        while (allReelsStopped == false) {
            yield return null;
            allReelsStopped = reels[0].isFullyStopped && reels[1].isFullyStopped && reels[2].isFullyStopped && reels[3].isFullyStopped && reels[4].isFullyStopped;
        }
        reelSpinningSound.Stop();

        float winnings = SlotLines.CalculateWinnings(results);
        foreach (ReelIconPrefab winningIcon in SlotLines.IterateWinningSlotIcons(results)) {
            if (winningIcon.isAnimating == false) {
                StartCoroutine(winningIcon.StartAnimation());
                animatingIcons.Add(winningIcon);
            }
        }
        if (winnings > 0) {
            winningsAmountUI.text = $"{winnings}";
            StartCoroutine(PlayWinningSound(winnings));
            ChangeCredits(winnings);
        }
    }
}
