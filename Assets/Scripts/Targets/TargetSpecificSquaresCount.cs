using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpecificSquaresCount: TargetSquaresCount
{
    public int squaresNeedValue;

    public override void Init()
    {
        base.Init();
    }

    public override void DoProgress(BoardStat boardStat)
    {
        if (squaresNeedValue == boardStat.removedValue)
            base.DoProgress(boardStat);
        if (boardStat.minNumber > squaresNeedValue)
            isImposible = true;
    }
}


