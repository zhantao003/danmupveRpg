using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CDanmuLike))]
public class LikePropertyDrawer : StructPropertyDrawer
{
    public LikePropertyDrawer()
    {
        contents = new[]
       {
                new GUIContent(EditorConstants.kUid),
                new GUIContent(EditorConstants.kUserName),
                new GUIContent(EditorConstants.kUserFace),
                new GUIContent(EditorConstants.kLike),
                new GUIContent(EditorConstants.kRoomID),
            };
        rects = new Rect[contents.Length];
        propertyNames = new[]
        {
                nameof(CDanmuLike.uid),
                nameof(CDanmuLike.nickName),
                nameof(CDanmuLike.headIcon),
                nameof(CDanmuLike.likeNum),
                nameof(CDanmuLike.roomId),
            };
    }

    public override void OnGUI(Rect position, SerializedProperty property,
       GUIContent label)
    {
        if (!DrawBase(position, property, label)) return;
        var dm = (CDanmuLike)property.GetValue();
        dm.Mock();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
