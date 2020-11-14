using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsCounter : MonoBehaviour {

    public float secondStarRate = 2f;
    public float thirdStarRate = 4f;
    public virtual void Init()
    {

    }

    public virtual int StarsCount(GameStat gameStat)
    {
        return 0;
    }
}
