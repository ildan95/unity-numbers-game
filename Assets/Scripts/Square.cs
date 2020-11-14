using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Square : MonoBehaviour, IPointerClickHandler
{
    private int value;
    public Text valueText;
    public Image img;
    public float moveTime;

    private int size;

    float moveSpeed;
    public float currentTransparent;

    private Board board;
    private RectTransform rect;

    public int i,j;
    public int I
    {
        get
        {
            return i;
        }
    }
    public int J
    {
        get
        {
            return j;
        }
    }

    public void Init(int _i, int _j, int _value, Sprite _sprite, int _size=100)
    {
        size = _size;
        if (img == null)
            img = GetComponent<Image>();
        SetPos(_i, _j);
        SetTransparent(0);
        SetValue(_value);
        SetColorImg(_sprite);
    }

    void SetTransparent(float a)
    {
        currentTransparent = a;
        img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        valueText.color = new Color(valueText.color.r, valueText.color.g, valueText.color.b, a);
    }

    public IEnumerator ShowInPlace()
    {
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, j * size, size);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, i * size, size);
        yield return StartCoroutine(SmoothShow(0f, 0.3f));
    }

    public void SetPos(int _i, int _j)
    {
        i = _i;
        j = _j;
        gameObject.name = string.Format("Square{0}_{1}", _i, _j);
    }

    public void SetPositionByIJ(int i, int j)
    {
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, j * size, size);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, i * size, size);
    }
    public Vector3 GetPositionByIJ()
    {
        Vector3 temp = rect.position;
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, j * size, size);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, i * size, size);
        Vector3 res = rect.position;
        rect.position = temp;
        return res;
    }

    public IEnumerator SmoothShow(float from, float to)
    {
        for (float a=from; a <= to; a += 0.05f)
        {
            SetTransparent(a);
            yield return null;//new WaitForSeconds(0.05f);
        }
        StartCoroutine(SmoothShow(to, 1f));
    }

    public IEnumerator MoveTo(Vector3 dest)
    {
        //Debug.Log(string.Format("MoveTo dest={0}, pos={1}", dest, rect.position));
        float effectSmooth = Time.deltaTime;
        float dist = Vector3.Distance(dest, rect.position);
        moveSpeed = dist / moveTime;
        if (dist <= 0.01)
            yield break;
        while (dist >= moveSpeed*Time.deltaTime)
        {
            rect.position = Vector3.MoveTowards(rect.position, dest, moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(dest, rect.position);
            yield return null;
        }
        rect.position = dest;
    }

    public IEnumerator DestroyAfterMoveTo(Vector3 dest)
    {
        yield return StartCoroutine(MoveTo(dest));
        SetTransparent(0f);
        Destroy(gameObject);
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int nvalue)
    {
        value = nvalue;
        valueText.text = Mathf.RoundToInt((Mathf.Pow(2, nvalue))).ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        board.OnClick(i, j, value);
    }

    void Awake () {
        rect = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        board = gameObject.GetComponentInParent<Board>();
    }

    public void SetColorImg(Sprite sprite)
    {
        img.sprite = sprite;
        //back.sprite = sprite;
    }

	public void Down()
    {
        SetPos(i+1, j);
    }
}
