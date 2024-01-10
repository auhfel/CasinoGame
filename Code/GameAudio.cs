using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameAudio", menuName = "ScriptableObjects/GameAudio", order = 1)]
public class GameAudio : ScriptableObject
{
    public AudioSource reelStopSound;
    public AudioSource reelSpinningSound;
    public AudioSource smallWinSound, mediumWinSound, bigWinSound;
}
