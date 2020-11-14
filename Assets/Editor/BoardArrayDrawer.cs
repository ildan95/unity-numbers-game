using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoardArray))]
public class BoardArrayDrawer : PropertyDrawer
{
    private int arraySize = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        Rect newPos = position;
        newPos.height = 18f;
        newPos.y += 18f;
        SerializedProperty data = property.FindPropertyRelative("rows");
        arraySize = data.arraySize;
        //Debug.Log(string.Format("rows size = {0}", arraySize));

        for (int j=0; j< data.arraySize; j++)
        {
            SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
            newPos.width = position.width / row.arraySize;
            for (int i = 0; i < row.arraySize; i++)
            {
                EditorGUI.PropertyField(newPos, row.GetArrayElementAtIndex(i), GUIContent.none);
                newPos.x += newPos.width; 
            }
            newPos.x = position.x;
            newPos.y += 18f;
        }
        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 18f * arraySize;
    }
}
