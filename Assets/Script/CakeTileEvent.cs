using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeTileEvent : TileEvent
{
    private int matchCount;
    private int requiredAmount;

    public CakeTileEvent(int amount)
    {
        requiredAmount = amount;
    }

    public override void OnMatch()
    {
        matchCount++;
    }

    public override bool AchievementCompleted()
    {
        if (matchCount == requiredAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}