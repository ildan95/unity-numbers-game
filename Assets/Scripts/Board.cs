using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardMode
{
    NORMAL, X2, D2, DEL, GAMEOVER
}

public class Board : MonoBehaviour {

    public int countX = 10;
    public int countY = 7;
    int squareSize = 100;
    public float scale = 1f;

    public int minNumber = 1;
    public int maxNumber = 4;

    public BoardMode mode = BoardMode.NORMAL;

    private Square[,] squares;
    private GameController gameController;

    public RectTransform squareProjectile;
    public Sprite[] colorImgs;

    int actionAnimId = 0;

    public void Init()
    {
        gameController = gameObject.GetComponentInParent<GameController>();
        InitColorImgs();
    }

    void SetSizeAndPosition()
    {
        RectTransform r = gameObject.GetComponent<RectTransform>();
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, countX * squareSize);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, countY * squareSize);
        float xScale = parent.sizeDelta.x / r.sizeDelta.x;
        float yScale = parent.sizeDelta.y / r.sizeDelta.y;
        if (xScale < 1f || yScale < 1f)
        {
            float minScale = Mathf.Min(xScale, yScale);
            transform.localScale *= minScale;
        }
    }
    public void Init(int _countX, int _countY, int _minNumber, int _maxNumber)
    {
        Init();
        countX = _countX;
        countY = _countY;
        minNumber = Mathf.Max(_minNumber, 1);
        maxNumber = _maxNumber;
        CreateSquares();
        SetSizeAndPosition();
    }

    public void Init(BoardInitInfo boardInitInfo)
    {
        Init();
        countX = boardInitInfo.countX;
        countY = boardInitInfo.countY;

        if (boardInitInfo.randomBoard)
        {
            minNumber = Mathf.Max(boardInitInfo.minValue,1);
            maxNumber = boardInitInfo.maxValue;
            CreateSquares();
        }
        else
        {
            CreateSquares(boardInitInfo.board);
            CorrectMinMax();
        }
        SetSizeAndPosition();
    }

    private void CreateSquares()
    {
        squares = new Square[countY, countX];
        for (int i=0; i < countY; i++)
            for (int j = 0; j < countX; j++)
                CreateSquare(i,j);
        StartCoroutine(ShowCreatedSquares());
    }
    private void CreateSquares(BoardArray board)
    {
        squares = new Square[countY, countX];
        for (int i = 0; i < countY; i++)
            for (int j = 0; j < countX; j++)
                CreateSquare(i, j, board.rows[i].row[j]);
        StartCoroutine(ShowCreatedSquares());

    }

    private Dictionary<int, Square> NearSameSquares(ref Dictionary<int, Square> taken, int i, int j)
    {
        if (taken.Count == 0)
            taken.Add(i * countY + j, squares[i, j]);

        int value = squares[i, j].GetValue();

        int[,] indexs = new int[4,2] { {i-1,j}, {i+1, j}, {i, j-1}, {i, j+1} };
        for (var ii=0; ii < 4; ii++)
        {
            if (indexs[ii,0] >= 0 && indexs[ii,0] < countY && indexs[ii,1] >= 0 && indexs[ii,1] < countX && squares[indexs[ii, 0], indexs[ii, 1]]!=null && squares[indexs[ii,0], indexs[ii,1]].GetValue() == value)
            {
                int key = indexs[ii, 0] * countY + indexs[ii, 1];
                if (!taken.ContainsKey(key))
                {
                    taken.Add(key, squares[indexs[ii, 0], indexs[ii, 1]]);
                    NearSameSquares(ref taken, indexs[ii, 0], indexs[ii, 1]);
                }
            }
        }
        return taken;
    }

    public void OnClick(int i, int j, int v)
    {
        if (mode == BoardMode.GAMEOVER)
            return;

        BoardStat boardStat = new BoardStat(minNumber, maxNumber);
        int removedCount = 0;

        if (mode == BoardMode.NORMAL)
        {
            Dictionary<int, Square> taken = new Dictionary<int, Square>();
            NearSameSquares(ref taken, i, j);
            removedCount = taken.Count;
            if (removedCount > 1)
            {
                
                int sum = Mathf.FloorToInt(Mathf.Log(removedCount, 2)) + v;
                foreach (KeyValuePair<int, Square> keyValue in taken)
                {
                    //Square s = keyValue.Value;
                    //s.DestroyAfterMoveTo(dest).ParallelCoroutinesGroup(this, "DestroyAfterMoveTo");

                    //StartCoroutine(s.DestroyAfterMoveTo(dest));
                    squares[keyValue.Key / countX, keyValue.Key % countX] = null;
                }
                CreateSquare(i, j, sum);
                CheckFree();
                CorrectMinMax();
                int[] shifting = CreateSquaresForFree();
                
                ActionAnimations(taken, i, j, shifting).ParallelCoroutinesGroup(this, "actionAnim"+actionAnimId+1);
            }
        }
        else if (mode == BoardMode.X2)
        {
            removedCount = 1;
            DestroyImmediate(squares[i, j].gameObject);
            CreateSquare(i, j, v+1);
            if (v + 1 > maxNumber)
                maxNumber = v + 1;
        }
        else if (mode == BoardMode.D2)
        {
            if (v != 1)
            {
                removedCount = 1;
                DestroyImmediate(squares[i, j].gameObject);
                CreateSquare(i, j, v - 1);
                if (v - 1 < minNumber)
                    maxNumber = v - 1;
            }
        }
        else if (mode == BoardMode.DEL)
        {
            removedCount = 1;
            DestroyImmediate(squares[i, j].gameObject);
            CheckFree();
            CorrectMinMax();
            CreateSquaresForFree();
        }

        boardStat.SetInfo(minNumber, maxNumber, v, removedCount, mode);
        mode = BoardMode.NORMAL;
        gameController.OnStepEnd(boardStat);
    }

    private void CreateSquare(int i, int j, int v=0)
    {
        var instance = Instantiate(squareProjectile.gameObject);
        instance.transform.SetParent(transform, false);
        Square s = instance.GetComponent<Square>();
        if (v == 0)
            v = GetRandomPower();
        Debug.LogFormat("{0} {1} {2}",i, j, v);
        s.Init(i, j, v, colorImgs[(v - 1) % colorImgs.Length], squareSize);
        squares[i, j] = s;
    }

    IEnumerator ShowCreatedSquares()
    {
        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++)
                squares[j, i].ShowInPlace().ParallelCoroutinesGroup(this, "showInPlace"+i);
            while (CoroutineExtension.GroupProcessing("showInPlace" + i))
                yield return null;
        }
    }

    IEnumerator ActionAnimations(Dictionary<int, Square> taken, int destI, int destJ, int[] shifting)
    {
        while (CoroutineExtension.GroupProcessing("actionAnim" + actionAnimId))
            yield return null;
        actionAnimId++;
        squares[destI, destJ].SetPositionByIJ(destI, destJ);
        Vector3 destroyDest = squares[destI, destJ].transform.position;
        foreach (KeyValuePair<int, Square> keyValue in taken)
        {
            Square s = keyValue.Value;
            s.DestroyAfterMoveTo(destroyDest).ParallelCoroutinesGroup(this, "DestroyAfterMoveTo");
        }
        //while (CoroutineExtension.GroupProcessing("DestroyAfterMoveTo"))
        //    yield return null;
        //Debug.Log("DestroyAfterMoveTo");

        for (int j = 0; j < countX; j++)
            if (shifting[j] != 0)
                for (int i=shifting[j]; i > 0; i--)
                    squares[i - 1, j].SetPositionByIJ(-shifting[j]+i-1, j);

        for (int i = 0; i < countY; i++)
            for (int j = 0; j < countX; j++)
                if (squares[i, j] != null)
                {
                    Square s = squares[i, j];
                    s.MoveTo(s.GetPositionByIJ()).ParallelCoroutinesGroup(this, "movingToIJ");
                    if (s.currentTransparent < 1f)
                        s.SmoothShow(0f,1f).ParallelCoroutinesGroup(this, "movingToIJ");
                }
        while (CoroutineExtension.GroupProcessing("movingToIJ"))
            yield return null;
        Debug.Log("movingToIJ");
    }

    private int[] CreateSquaresForFree()
    {
        int[] shifting = new int[countX];
        for (int j = 0; j < countX; j++)
            shifting[j] = 0;

        for (int i = 0; i < countY; i++)
            for (int j = 0; j < countX; j++)
                if (squares[i, j] == null)
                {
                    CreateSquare(i, j);
                    shifting[j] += 1;
                }
        return shifting;
    }
    private void CheckFree()
    {
        for (int i = 0; i < countY; i++)
            for (int j = 0; j < countX; j++)
                if (squares[i, j] == null)
                    ShiftDown(i, j);
    }

    public bool HasSteps()
    {
        int v;
        for (int i = 0; i < countY; i++)
            for (int j = 0; j < countX; j++)
            {
                if (squares[i, j] != null)
                {
                    v = squares[i, j].GetValue();
                    if (i + 1 < countY && squares[i + 1, j].GetValue() == v)
                        return true;
                    if (j + 1 < countX && squares[i, j + 1].GetValue() == v)
                        return true;
                }
            }
        return false;
    }

    private void CorrectMinMax()
    {
        int min = maxNumber;
        int max = minNumber;
        for (int i = 0; i < countY; i++)
            for (int j = 0; j < countX; j++)
            {
                if (squares[i, j] != null)
                {
                    int v = squares[i, j].GetValue();
                    if (v > max)
                        max = v;
                    if (v < min)
                        min = v;
                }
            }
        maxNumber = max;
        minNumber = min;
    }

    private void ShiftDown(int iStart, int j)
    {
        for (int i = iStart-1; i >= 0; i--)
        {
            if (squares[i, j] != null)
            {
                squares[i, j].Down();
                squares[i + 1, j] = squares[i, j];
                squares[i, j] = null;
            }
        }
    }

    private int GetRandomPower()
    {
        int a = Mathf.RoundToInt(Mathf.Pow(2, minNumber));
        int b = Mathf.RoundToInt(Mathf.Pow(2, maxNumber+1));
        float r = Random.value * (b - a) + a;
        int v = Mathf.FloorToInt(Mathf.Log(r, 2));
        return minNumber + maxNumber - v;
    }

    private void InitColorImgs()
    {
        int count = 9;
        colorImgs = new Sprite[count];

        for (int i = 0; i < count; i++)
            colorImgs[i] = Resources.Load<Sprite>(string.Format("Sprites/Potions/{0}", i + 1));



    }

    public void SetX2Mode()
    {
        mode = BoardMode.X2;
    }

    public void SetD2Mode()
    {
        mode = BoardMode.D2;
    }

    public void SetDELMode()
    {
        mode = BoardMode.DEL;
    }

    public void SetNormalMode()
    {
        mode = BoardMode.NORMAL;
    }
}


public class BoardStat
{
    public int
        minNumber,
        maxNumber,
        lastMinNumber,
        lastMaxNumber,
        removedValue,
        removedCount,
        score=0;


    public BoardMode mode;
    public BoardStat(int _lastMinNumber, int _lastMaxNumber)
    {
        lastMinNumber = _lastMinNumber;
        lastMaxNumber = _lastMaxNumber;
    }

    public void SetInfo(int _minNumber, int _maxNumber, int _removedValue, int _removedCount, BoardMode _mode)
    {
        minNumber = _minNumber;
        maxNumber = _maxNumber;
        removedCount = _removedCount;
        removedValue = _removedValue;
        mode = _mode;
    }

    public bool IsReleased()
    {
        return minNumber > lastMinNumber;
    }
    override public string ToString()
    {
        return string.Format("removed {0} with values {1}", removedCount, removedValue); 
    }
}
