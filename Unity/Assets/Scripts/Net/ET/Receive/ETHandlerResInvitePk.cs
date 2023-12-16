using ETModel;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResInvitePk : AMHandler<Actor_InvitePk_G2C>
    {
        protected override async ETTask Run(Session session, Actor_InvitePk_G2C message)
        {
            //邀请人的信息
            DUserListInfo userInfo = message.UserInfo;
            RefreshUI(userInfo);
        }

        void RefreshUI(DUserListInfo inviter)
        {
            UINetMatch match = UIManager.Instance.GetUI(UIResType.ETNetMatch) as UINetMatch;
            match.RecieveInvited(inviter);
            //UIIdleMainMenu.Instance.uiNetMathcing.RecieveInvited(inviter);
        }
    }
}
