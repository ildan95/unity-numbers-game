using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameOverReason
{
    targetImpossible, noStepsOnBoard, noSteps, noTime
}
public class GameController : MonoBehaviour
{

    protected Board board;
    protected TextMeshProUGUI scoreText;
    protected int score = 0;
    protected Target[] targets;
    protected Game game;
    protected StarsCounter starCounter;
    protected GameStat gameStat;
    protected Player player;

    public void Init(int part, int level)
    {
        GameObject levelInstance = Instantiate(Resources.Load(string.Format("Levels/{0}/{1}",part,level), typeof(GameObject))) as GameObject;
        BoardInitInfo boardInitInfo = levelInstance.GetComponent<BoardInitInfo>();
        starCounter = levelInstance.GetComponent<StarsCounter>();

        gameStat = new GameStat();

        board = gameObject.GetComponentInChildren<Board>();
        player = FindObjectOfType<Player>();

        board.Init(boardInitInfo);
        score = 0;
        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        targets = levelInstance.GetComponents<Target>();
        game = levelInstance.GetComponent<Game>();
        game.Init();
    }
    public void OnStepEnd(BoardStat boardStat)
    {
        Debug.Log(boardStat);
        if (boardStat.removedCount <= 1)
            return;

        if (boardStat.mode == BoardMode.NORMAL)
            boardStat.score = UpdateScore(boardStat.removedCount * boardStat.removedValue);

        bool targetsDone = true;
        bool targetsIsImposible = false;
        foreach (var target in targets)
        {
            target.DoProgress(boardStat);
            if (!target.Done())
                targetsDone = false;
            if (target.IsImposible())
                targetsIsImposible = true;
        }
        game.DoProgress(boardStat);
        gameStat.score = score;
        gameStat.resoursesUsedRate = game.ResoursesUsedRate();

        if (targetsDone)
            OnWin();

        if (targetsIsImposible)
            OnGameOver(GameOverReason.targetImpossible); 

        if (!board.HasSteps())
            OnGameOver(GameOverReason.noStepsOnBoard);

        if (game.IsLoose())
            OnGameOver(game.gameOverReason);

    }

    private int UpdateScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
        return score;
    }

    protected void OnGameOver(GameOverReason reason)
    {
        board.mode = BoardMode.GAMEOVER;
        Debug.Log(string.Format("GameOver {0}", reason));
    }

    protected void OnWin()
    {
        int stars = 0;
        if (starCounter != null)
        {
            stars = starCounter.StarsCount(gameStat);
        }
        Debug.Log(string.Format("WIN! Stars = {0}", stars));
    }

    void Start()
    {
        Init(1,1);
    }
}