using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CDanmuChat))]
public class DmPropertyDrawer : StructPropertyDrawer
{
    public DmPropertyDrawer()
    {
        contents = new[]
        {
                new GUIContent(EditorConstants.kUid),
                new GUIContent(EditorConstants.kUserName),
                new GUIContent(EditorConstants.kUserFace),
                new GUIContent(EditorConstants.kMsg),
                new GUIContent(EditorConstants.kFansMedalLevel),
                new GUIContent(EditorConstants.kFansMedalName),
                new GUIContent(EditorConstants.kFansMedalWearingStatus),
                new GUIContent(EditorConstants.kGuardLevel),
                new GUIContent(EditorConstants.kRoomID),
            };
        rects = new Rect[contents.Length];
        propertyNames = new[]
        {
                nameof(CDanmuChat.uid),
                nameof(CDanmuChat.nickName),
                nameof(CDanmuChat.headIcon),
                nameof(CDanmuChat.content),
                nameof(CDanmuChat.fanLv),
                nameof(CDanmuChat.fanName),
                nameof(CDanmuChat.fanEquip),
                nameof(CDanmuChat.vipLv),
                nameof(CDanmuChat.roomId),
            };
    }

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        if (!DrawBase(position, property, label)) return;
        var dm = (CDanmuChat)property.GetValue();
        dm.Mock();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}