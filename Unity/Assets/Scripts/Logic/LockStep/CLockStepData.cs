using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockStepData
{
    //Ԥ����ÿ֡��ʱ�䳤��
    public static Fix64 g_fixFrameLen = (Fix64)0.066f;

    //��Ϸ���߼�֡
    public static long g_uGameLogicFrame = 0;

    //����1֡�͸����߼���С��1֡���ȴ�������
    public static long g_uServerWaitFrame = 2;

    //�ȴ�����������3֡
    public static long g_uServerStartFrame = 3;  

    //��������ǰ���߼�֡
    public static long g_uServerLogicFrame = 0;

    //��������ǰ����Ϸʣ��ʱ��
    public static long g_uServerGameTime = 0;

    public static SRandom pRand = new SRandom(100);
}
