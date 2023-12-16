using ETModel;

public static class ETHandlerReqAcceptInvitePk
{
    public static async ETVoid Request(long inviterUserId)
    {
        SessionComponent.Instance.Session.Send(new Actor_AcceptInvitePk_C2G()
        {
            InviterUserId = inviterUserId
        });

        await ETTask.CompletedTask;
    }
}
