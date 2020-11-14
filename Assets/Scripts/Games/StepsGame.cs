using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsGame : Game {

    private int leftSteps;

    public int 
        startStepsCount = 10,
        stepsForRelease = 5,
        bonusSquaresCount = 5,
        stepsForSquares = 1;
        

    public override void Init()
    {       
        leftSteps = startStepsCount;
        gameOverReason = GameOverReason.noSteps;
    }

    public override void DoProgress(BoardStat boardStat)
    {
        base.DoProgress(boardStat);

        if (boardStat.mode == BoardMode.NORMAL)
            leftSteps -= 1;
        if (boardStat.IsReleased())
            leftSteps += stepsForRelease;
        if (boardStat.removedCount >= bonusSquaresCount)
            leftSteps += stepsForSquares * boardStat.removedCount / bonusSquaresCount;
        isLoose = leftSteps <= 0;
    }

    public override float ResoursesUsedRate()
    {
        return leftSteps / startStepsCount;
    }
}
