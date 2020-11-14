using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    protected bool isLoose = false;
    public GameOverReason gameOverReason;
    public virtual void DoProgress(BoardStat boardStat)
    {
    }

    public virtual void Init()
    {

    }

    public bool IsLoose()
    {
        return isLoose;
    }
    
    public virtual float ResoursesUsedRate()
    {
        return 1;
    }

    
}

public class GameStat
{
    public int score;
    public float resoursesUsedRate;

    public GameStat(int _score, float _resoursesUsedRate)
    {
        score = _score;
        resoursesUsedRate = _resoursesUsedRate;
    }

    public GameStat()
    {
        score = 0;
        resoursesUsedRate = 0;
    }
}