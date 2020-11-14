using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardInitInfo))]
public class BoardInfoEditor : Editor
{
    SerializedProperty randomBoard, countX, countY, minValue, maxValue, board;
    int oldX=0, oldY=0;
    BoardArray boardArray;
    int[][] saved;

    void OnEnable()
    {
        randomBoard = serializedObject.FindProperty("randomBoard");
        countX = serializedObject.FindProperty("countX");
        countY = serializedObject.FindProperty("countY");
        minValue = serializedObject.FindProperty("minValue");
        maxValue = serializedObject.FindProperty("maxValue");
        board = serializedObject.FindProperty("board");
        boardArray = (target as BoardInitInfo).board;
        //InitBoard();
    }

    private void InitBoard()
    {
        boardArray.rows = new RowData[countY.intValue];
        for (int i = 0; i < countY.intValue; i++)
        {
            boardArray.rows[i].row = new int[countX.intValue];
            for (int j = 0; j < countX.intValue; j++)
            {
                boardArray.rows[i].row[j] = 1;
            }
        }
    }

    private void SaveBoard()
    {
        int y = boardArray.rows.Length;
        saved = new int[y][];
        for (int i = 0; i < y; i++)
        {
            int x = boardArray.rows[i].row.Length;
            saved[i] = new int[x];
            for (int j = 0; j < x; j++)
                saved[i][j] = boardArray.rows[i].row[j];
        }
    }

    private void ReloadBoard()
    {
        //Debug.Log("Reload Board");
        boardArray.rows = new RowData[countY.intValue];
        for (int i = 0; i < countY.intValue; i++)
        {
            boardArray.rows[i].row = new int[countX.intValue];
            for (int j = 0; j < countX.intValue; j++)
            {
                if (i >= saved.Length)
                    boardArray.rows[i].row[j] = 1;
                else if (j >= saved[i].Length)
                    boardArray.rows[i].row[j] = 1;
                else
                    boardArray.rows[i].row[j] = saved[i][j];
            }
        }

        //Debug.Log(boardArray.rows.Length);
    }


    public override void OnInspectorGUI()
    {
        boardArray = (target as BoardInitInfo).board;
        EditorGUILayout.PropertyField(countX);
        EditorGUILayout.PropertyField(countY);
        
        EditorGUILayout.PropertyField(randomBoard);
        if (randomBoard.boolValue)
        {
            EditorGUILayout.PropertyField(minValue);
            EditorGUILayout.PropertyField(maxValue);
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
            if (oldX != countX.intValue || oldY != countY.intValue)
            {
                SaveBoard();
                ReloadBoard();
                serializedObject.Update();
            }
            oldX = countX.intValue;
            oldY = countY.intValue;
            EditorGUILayout.PropertyField(board);
        }   
        serializedObject.ApplyModifiedProperties();
    }

}
