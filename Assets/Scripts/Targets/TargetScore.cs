using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScore : Target {

    public int needMinScore;

    public override void Init()
    {
        base.Init();
    }

    public override void DoProgress(BoardStat boardStat)
    {
        done = needMinScore <= boardStat.score;
    }
}
