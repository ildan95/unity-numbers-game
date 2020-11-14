using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    
    protected bool done = false;
    protected bool isImposible = false;
    public virtual void Init()
    {

    }

    public virtual void DoProgress(BoardStat boardStat)
    {
    }

    //public virtual void DoProgress(BoardStat boardStat, int score){

    public virtual bool Done()
    {
        return done;
    }

    public virtual bool IsImposible()
    {
        return isImposible;
    }
}
