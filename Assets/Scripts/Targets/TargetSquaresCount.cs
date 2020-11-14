using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetSquaresCount : Target
{
    protected int squaresNumber = 0;
    public int squaresNeedNumber;

    public override void Init()
    {
        base.Init();
    }

    public override void DoProgress(BoardStat boardStat)
    {
        squaresNumber += boardStat.removedCount;
        done = squaresNumber >= squaresNeedNumber;
    }
}


