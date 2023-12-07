using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CDanmuVipInfo))]
public class GuardPropertyDrawer : StructPropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        if (!DrawBase(position, property, label)) return;
        var guard = (CDanmuVipInfo)property.GetValue();
        guard.Mock();
    }

    public GuardPropertyDrawer()
    {
        contents = new[]
        {
                new GUIContent(EditorConstants.kGuardLevel),
                new GUIContent(EditorConstants.kGuardNum),
                //new GUIContent(EditorConstants.kGuardUnit),
                new GUIContent(EditorConstants.kFansMedalLevel),
                new GUIContent(EditorConstants.kFansMedalName),
                new GUIContent(EditorConstants.kFansMedalWearingStatus),
                new GUIContent(EditorConstants.kRoomID),

                new GUIContent(EditorConstants.kUid),
                new GUIContent(EditorConstants.kUserName),
                new GUIContent(EditorConstants.kUserFace),
            };
        rects = new Rect[contents.Length];
        propertyNames = new[]
        {
                 nameof(CDanmuVipInfo.vipLv),
                 nameof(CDanmuVipInfo.vipNum),
                 //nameof(CDanmuVipInfo.guardUnit),
                 nameof(CDanmuVipInfo.fanLv),
                 nameof(CDanmuVipInfo.fanName),
                 nameof(CDanmuVipInfo.fanEquip),
                 nameof(CDanmuVipInfo.roomId),
                 nameof(CDanmuVipInfo.uid),
                 nameof(CDanmuVipInfo.nickName),
                 nameof(CDanmuVipInfo.headIcon)
            };
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}