using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CSlotBlindBoxComp : MonoBehaviour
{
    // 抽奖按钮
    //public Button DrowBtn;

    // 奖励图片
    public Image[] ArardImgArr;
    public GameObject selectedEff;

    // 转盘速度
    private float AniMoveSpeed = 0f;

    // 进度
    private float[] progress = new[] { 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f };

    // 转动动画位置
    //private Vector3[] AniPosV3 = new[]
    //      {Vector3.up * 280, Vector3.up * 140, Vector3.zero, Vector3.down * 140, Vector3.down * 280};

    private Vector3[] AniPosV3;
        //= new[]
        //{Vector3.left * 280,Vector3.left * 210,Vector3.left * 140, Vector3.left * 70,Vector3.zero,  Vector3.right * 70, Vector3.right * 140, Vector3.right * 210,Vector3.right*280};

    //private Vector3[] AniPosV3 = new[]
    //   {Vector3.right * 280,Vector3.right * 210,Vector3.right * 140, Vector3.right * 70,Vector3.zero,  Vector3.left * 70, Vector3.left * 140, Vector3.left * 210,Vector3.left*280};

    // 自动暂停标识
    private bool isAutoStop;
    // 抽奖结束 停止刷新界面UI
    public bool isStopUpdatePos;

    public bool isAnimEnd;

    public bool isRevert;

    public AnimationCurve curve;
    public AnimationCurve curveback;

    void Start()
    {
        //DrowBtn.onClick.AddListener(DrawFun);
        isAutoStop = false;
        isStopUpdatePos = false;
        if(isRevert)
            AniPosV3 = new[]
       {Vector3.right * 280,Vector3.right * 210,Vector3.right * 140, Vector3.right * 70,Vector3.zero,  Vector3.left * 70, Vector3.left * 140, Vector3.left * 210,Vector3.left*280};
        else
            AniPosV3 = new[]
        {Vector3.left * 280,Vector3.left * 210,Vector3.left * 140, Vector3.left * 70,Vector3.zero,  Vector3.right * 70, Vector3.right * 140, Vector3.right * 210,Vector3.right*280};

    }

    void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    DrawFun(Random.Range(0,3),2f);
        //}
        if (isStopUpdatePos) {
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

    int finalDrawIndex;
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
                //Debug.Log(AniPosV3[index]);
                // todo...获取奖励数据维护
                finalDrawIndex = i+4;
                //Debug.Log(" finalDrawIndex = " + finalDrawIndex);
                StartCoroutine(FinalDrawAnim());
            }
            return AniPosV3[index];
        }
        else
        {
            return Vector3.Lerp(AniPosV3[index], AniPosV3[index + 1], progress[i] - index);
        }
    }

    public IEnumerator FinalDrawAnim() {
        ArardImgArr[finalDrawIndex].transform.SetAsLastSibling();
        float scale = 1;
        float time = 0;
        while (scale < 1.5f)
        {
            ArardImgArr[finalDrawIndex].transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            scale = curve.Evaluate(time);
        }
        time =0f;
        while (scale >1f)
        {
            ArardImgArr[finalDrawIndex].transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            scale = curveback.Evaluate(time);
        }
        ArardImgArr[finalDrawIndex].transform.localScale = new Vector3(1, 1, 1);
        selectedEff.gameObject.SetActive(true);

        isAnimEnd = true;
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
    public IEnumerator DrawFun(int finalDraw, float drawTimeDur)
    {

       yield return SetMoveSpeed(finalDraw, drawTimeDur, 0.2f);
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
        yield return new WaitForSeconds(0.2f);
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

