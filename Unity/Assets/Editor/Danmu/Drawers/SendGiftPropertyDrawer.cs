using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CDanmuGift))]
public class SendGiftPropertyDrawer : StructPropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        if (!DrawBase(position, property, label)) return;
        var sendGift = (CDanmuGift)property.GetValue();
        sendGift.Mock();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }

    public SendGiftPropertyDrawer()
    {
        contents = new[]
        {
                new GUIContent(EditorConstants.kRoomID),
                new GUIContent(EditorConstants.kUid),
                new GUIContent(EditorConstants.kUserName),
                new GUIContent(EditorConstants.kUserFace),
                new GUIContent(EditorConstants.kGiftID),
                new GUIContent(EditorConstants.kGiftName),
                new GUIContent(EditorConstants.kGiftNum),
                new GUIContent(EditorConstants.kPrice),
                new GUIContent(EditorConstants.kPaid),
                new GUIContent(EditorConstants.kFansMedalLevel),
                new GUIContent(EditorConstants.kFansMedalName),
                new GUIContent(EditorConstants.kFansMedalWearingStatus),
                new GUIContent(EditorConstants.kGuardLevel),
                //new GUIContent(EditorConstants.kAuId),
                //new GUIContent(EditorConstants.kAuName),
                //new GUIContent(EditorConstants.kAuFace)
            };
        rects = new Rect[contents.Length];
        propertyNames = new[]
        {
                nameof(CDanmuGift.roomId),
                nameof(CDanmuGift.uid),
                nameof(CDanmuGift.nickName),
                nameof(CDanmuGift.headIcon),
                nameof(CDanmuGift.giftId),
                nameof(CDanmuGift.giftName),
                nameof(CDanmuGift.giftNum),
                nameof(CDanmuGift.price),
                nameof(CDanmuGift.paid),
                nameof(CDanmuGift.fanLv),
                nameof(CDanmuGift.fanName),
                nameof(CDanmuGift.fanEquip),
                nameof(CDanmuGift.vipLv),
                //$"{nameof(SendGift.anchorInfo)}.{nameof(SendGift.anchorInfo.uid)}",
                //$"{nameof(SendGift.anchorInfo)}.{nameof(SendGift.anchorInfo.userName)}",
                //$"{nameof(SendGift.anchorInfo)}.{nameof(SendGift.anchorInfo.userFace)}",
            };
    }
}