using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsCounterByUsedResourse : StarsCounter
{
    public float oneStar = 1f;
    public override void Init()
    {

    }

    public override int StarsCount(GameStat gameStat)
    {
        float percent = gameStat.resoursesUsedRate;
        if (percent > oneStar)
            return 0;
        else if (percent <= thirdStarRate)
            return 3;
        else if (percent <= secondStarRate)
            return 2;
        else
            return 1;
    }
}
