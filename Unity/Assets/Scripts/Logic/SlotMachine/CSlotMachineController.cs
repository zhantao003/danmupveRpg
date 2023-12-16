using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSlotMachineController : MonoBehaviour
{
    // 抽奖按钮
    //public Button DrowBtn;

    // 奖励图片
    public Image[] ArardImgArr;
    public GameObject seletedEff;

    // 转盘速度
    private float AniMoveSpeed = 1f;

    // 进度
    private float[] progress = new[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f };

    // 转动动画位置
    //private Vector3[] AniPosV3 = new[]
    //      {Vector3.up * 280, Vector3.up * 140, Vector3.zero, Vector3.down * 140, Vector3.down * 280};

    private Vector3[] AniPosV3 = new[]
        {Vector3.up * 420,Vector3.up * 280, Vector3.up * 140,Vector3.zero,  Vector3.down * 140, Vector3.down * 280, Vector3.down * 420};

    // 自动暂停标识
    private bool isAutoStop;
    // 抽奖结束 停止刷新界面UI
    public bool isStopUpdatePos;

    void Start()
    {
        //DrowBtn.onClick.AddListener(DrawFun);
        isAutoStop = false;
        isStopUpdatePos = false;
    }

    void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    DrawFun(Random.Range(0,3),2f);
        //}
        if (isStopUpdatePos)
        {
            SetRightPos();
            return;
        }

        float t = Time.fixedDeltaTime * AniMoveSpeed;
        for (int i = 0; i < ArardImgArr.Length; i++)
        {
            progress[i] += t;
            ArardImgArr[i].transform.localPosition = MovePosition(i);
        }
    }
    void SetRightPos()
    {
        for (int i = 0; i < ArardImgArr.Length; i++)
        {
            ArardImgArr[i].transform.localPosition = AniPosV3[Mathf.FloorToInt(progress[i])];
        }
    }
    // 获取下一个移动到的位置
    Vector3 MovePosition(int i)
    {
        int index = Mathf.FloorToInt(progress[i]);
        if (index > AniPosV3.Length - 2)
        {
            //保留其小数部分，不能直接赋值为0
            progress[i] -= index;
            index = 0;
            // 索引为2的到底了，索引为0的就在正中心
            if (i == drawIndex && isAutoStop)
            {
                isStopUpdatePos = true;
                //Debug.Log("展示奖励界面...");
                // todo...获取奖励数据维护
                seletedEff.gameObject.SetActive(true);

            }
            return AniPosV3[index];
        }
        else
        {
            return Vector3.Lerp(AniPosV3[index], AniPosV3[index + 1], progress[i] - index);
        }
    }

    /// <summary>
    /// 在奖励处停止
    /// </summary>
    int drawIndex;
    /// <summary>
    /// 点击抽奖
    /// </summary>
    /// <param name="finalDraw">结果</param>
    /// <param name="drawTimeDur">抽奖最高速度持续时间</param>
    public void DrawFun(int finalDraw, float drawTimeDur)
    {

        StartCoroutine(SetMoveSpeed(finalDraw, drawTimeDur, 0.5f));
        // DoTween 按钮下拉动画
        // Transform tran = DrowBtn.transform;
        //tran.DOLocalMoveY(-60, 0.2f).OnComplete(() =>
        //{
        //      tran.DOLocalMoveY(50, 0.2f);
        //
        //});
    }
    float v;
    /// <summary>
    /// 抽奖动画速度控制
    /// </summary>
    /// <param name="finalDraw"></param>
    /// <param name="drawTimeDur"></param>
    /// <param name="time">到达最高速度的时间，同到达最低速的时间</param>
    /// <returns></returns>
    IEnumerator SetMoveSpeed(int finalDraw, float drawTimeDur, float time)
    {
        AniMoveSpeed = 0;
        yield return new WaitForSeconds(1.5f);
        drawIndex = finalDraw;
        isAutoStop = false;
        isStopUpdatePos = false;
        float timeCount = 0;
        while (timeCount < time)
        {
            AniMoveSpeed = Mathf.SmoothDamp(AniMoveSpeed, 15f, ref v, time);
            yield return new WaitForEndOfFrame();
            timeCount += Time.deltaTime;
        }

        yield return new WaitForSeconds(drawTimeDur);
        timeCount = 0;
        while (timeCount < time)
        {
            AniMoveSpeed = Mathf.SmoothDamp(AniMoveSpeed, 1f, ref v, time);
            yield return new WaitForEndOfFrame();
            timeCount += Time.deltaTime;
        }
        yield return new WaitForSeconds(time);
        isAutoStop = true;
    }
}

