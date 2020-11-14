using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RowData
{
    public int[] row;
}

[System.Serializable]
public class BoardArray
{
    
    public RowData[] rows;
}

public class BoardInitInfo : MonoBehaviour {

    public int countX, countY, minValue, maxValue;

    public bool randomBoard = true;

    public BoardArray board;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
