using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsCounterByScore : StarsCounter {
    public int oneStar;

    public override void Init()
    {

    }

    public override int StarsCount(GameStat gameStat)
    {
        int score = gameStat.score;
        if (score < oneStar)
            return 0;
        else if (score < oneStar * secondStarRate)
            return 1;
        else if (score < oneStar * thirdStarRate)
            return 2;
        else 
            return 3;
    }
}
