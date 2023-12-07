using UnityEditor;
using UnityEngine;

public abstract class StructPropertyDrawer : PropertyDrawer
{
    protected GUIContent[] contents;
    protected Rect[] rects;
    protected string[] propertyNames;

    protected bool DrawBase(Rect position, SerializedProperty property, GUIContent label)
    {
        StructDrawerTool.Draw(contents, rects, propertyNames, position, property);
#if UNITY_2021_2_OR_NEWER || NET5_0_OR_GREATER
            var lastRect = rects[^1];
#else
        var lastRect = rects[rects.Length - 1];
#endif

        var buttonRect = new Rect()
        {
            position = new Vector2(lastRect.x, lastRect.y + position.height),
            width = position.width / 4,
            height = EditorGUIUtility.singleLineHeight
        };
        return GUI.Button(buttonRect, EditorConstants.kTestButton);
    }

}