using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public float animationScalingSpeed = 5f;
    public float reelSpinningSpeed = 12.5f;
    public float maximumBet = 500;
    public float playerCredits = 0;
    public float betAmount = 25;
    public float betIncrement = 25;
}
