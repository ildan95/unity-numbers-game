using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSquaresMinValue : Target
{
    public int needMinValue;

    public override void Init()
    {
        base.Init();
    }

    public override void DoProgress(BoardStat boardStat)
    {
        done = needMinValue <= boardStat.minNumber;
    }
}