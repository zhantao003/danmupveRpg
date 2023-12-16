using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Runtime.InteropServices;

public class CCameraController : MonoBehaviour
{
    public static CCameraController Ins = null;

    

    public Vector3 vFirstOriPos;
    public float fFirstOriScale;

    public Transform targetCamPos;
    private Vector3 refV3;
    private float fScaleValue;
    public Camera pCam;

    public float fMoveSpeed;
    public float fScrollSpeed;

    public float fMoveTime = 1f;

    public float fDragSpeed;
    public bool bDragMove;                      //是否使用鼠标按住左键拖拽摄像机

    public bool bScroll = true;

    public Vector3 vGoTargetLerp;

    float fScroll;

    float fOrthographicSize = 0;

    public Vector2 vMapCheckBaseX;
    public float fMapCheckXLerp;
    public Vector2 vMapCheckBaseY;
    public float fMapCheckYLerp;
    public Vector2 vScaleRange;

    /// <summary>
    /// 是否锁定操作
    /// </summary>
    bool bLockController;

    public enum EMLookState
    {
        GoTarget,
        Stay,
        BackToOri,
    }

    public Camera pYYCam;

    public Vector3 vCameraLerp;

    public EMLookState emLookState;
    public Vector3 vOriPos;
    public float fOriScale;

    public Vector3 vTargetPos;
    public float fLookScale = 18f;
    public float fGoTargetTime = 0.5f;
    public float fStayTime = 1f;
    float fCurTime;
    float fLerp;
    float fTotalTime;


    public Transform tranLeftCheck;
    public Transform tranRightCheck;

    private void Start()
    {
        Ins = this;
        vFirstOriPos = transform.position;
        fFirstOriScale = pCam.orthographicSize;
        fOrthographicSize = pCam.orthographicSize;
    }

    /// <summary>
    /// 特写指定目标
    /// </summary>
    /// <param name="vTarget"></param>
    public void LookToTarget(Vector3 vTarget)
    {
        vOriPos = targetCamPos.position;
        fOriScale = pCam.orthographicSize;
        vTargetPos = vTarget + vCameraLerp;
        fTotalTime = fGoTargetTime;
        fCurTime = 0;
        emLookState = EMLookState.GoTarget;
        bLockController = true;
    }

    private void Update()
    {
        return;
        if (bLockController)
        {

            return;
        }
        fScroll = Input.GetAxis("Mouse ScrollWheel");
        if (bScroll)
        {
            if (fScroll > 0)
            {
                fOrthographicSize -= Time.deltaTime * fScrollSpeed;
            }
            else if (fScroll < 0)
            {
                fOrthographicSize += Time.deltaTime * fScrollSpeed;
            }
        }
        //判断缩放值是否超过范围
        fOrthographicSize = Mathf.Clamp(fOrthographicSize, vScaleRange.x, vScaleRange.y);
    }

    private void FixedUpdate()
    {
        return;
        if (bLockController)
        {
            if (emLookState == EMLookState.GoTarget)
            {
                fCurTime += CTimeMgr.FixedTimeUnScale;
                fLerp = fCurTime / fTotalTime;
                if (fCurTime < fTotalTime)
                {
                    targetCamPos.position = Vector3.Lerp(vOriPos, vTargetPos, fLerp);
                    fOrthographicSize = fOriScale + (fLookScale - fOriScale) * fLerp;
                }
                else
                {
                    targetCamPos.position = vTargetPos;
                    fOrthographicSize = fLookScale;
                    fTotalTime = fStayTime;
                    fCurTime = 0;
                    //CTimeMgr.fTimeScale = 0.5f;
                    emLookState = EMLookState.Stay;
                }
            }
            else if(emLookState == EMLookState.Stay)
            {
                fCurTime += CTimeMgr.FixedTimeUnScale;
                if (fCurTime >= fTotalTime)
                {
                    fTotalTime = fGoTargetTime;
                    fCurTime = 0;
                    //CTimeMgr.fTimeScale = 1f;
                    emLookState = EMLookState.BackToOri;
                }
            }
            else if (emLookState == EMLookState.BackToOri)
            {
                fCurTime += CTimeMgr.FixedTimeUnScale;
                fLerp = fCurTime / fTotalTime;
                if (fCurTime < fTotalTime)
                {
                    targetCamPos.position = Vector3.Lerp(vTargetPos, vOriPos, fLerp);
                    fOrthographicSize = fLookScale + (fOriScale - fLookScale) * fLerp;
                }
                else
                {
                    targetCamPos.position = vOriPos;
                    fOrthographicSize = fOriScale;
                    fCurTime = 0;
                    bLockController = false;
                }
            }
        }
    }

    Vector3 direction;
    private void LateUpdate()
    {
        return;
        if(bLockController)
        {

        }
        else
        {
            direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.up * fMoveSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction -= Vector3.up * fMoveSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction -= Vector3.right * fMoveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right * fMoveSpeed;
            }

            //Debug.LogError("DDD===" + direction);
            if (bDragMove &&
                Input.GetMouseButton(0))  //点击鼠标左键进行视角的拖拽移动
            {
                direction -= Vector3.right * Input.GetAxis("Mouse X") * fDragSpeed;// 计算X轴旋转
                direction -= Vector3.up * Input.GetAxis("Mouse Y") * fDragSpeed;  //计算Y轴旋转
            }
            Vector3 vLeft = Camera.main.WorldToViewportPoint(tranLeftCheck.position);
            Vector3 vRight = Camera.main.WorldToViewportPoint(tranRightCheck.position);
           
            Vector3 inputPos = targetCamPos.position + direction;
            if(direction.x < 0)
            {
                Vector3 vGetPos = Camera.main.ViewportToWorldPoint(vLeft + new Vector3(0.5f, 0, 0));
                if(inputPos.x < vGetPos.x)
                {
                    inputPos.x = vGetPos.x;
                }
            }
            else if(direction.x > 0)
            {
                Vector3 vGetPos = Camera.main.ViewportToWorldPoint(vRight - new Vector3(0.5f, 0, 0));
                if (inputPos.x > vGetPos.x)
                {
                    inputPos.x = vGetPos.x;
                }
            }
            if (Input.GetKey(KeyCode.Space))
            {
                inputPos = vFirstOriPos;
                fOrthographicSize = fFirstOriScale;
            }
            float fScaleLerp = 1f - (pCam.orthographicSize - vScaleRange.x) / (vScaleRange.y - vScaleRange.x);
            ////判断x轴是否超出范围
            //if (inputPos.x < vMapCheckBaseX.x - fMapCheckXLerp * fScaleLerp)
            //{
            //    inputPos.x = vMapCheckBaseX.x - fMapCheckXLerp * fScaleLerp;
            //}
            //else if (inputPos.x > vMapCheckBaseX.y + fMapCheckXLerp * fScaleLerp)
            //{
            //    inputPos.x = vMapCheckBaseX.y + fMapCheckXLerp * fScaleLerp;
            //}
            //判断y轴是否超出范围
            if (inputPos.y < vMapCheckBaseY.x - fMapCheckYLerp * fScaleLerp)
            {
                inputPos.y = vMapCheckBaseY.x - fMapCheckYLerp * fScaleLerp;
            }
            else if (inputPos.y > vMapCheckBaseY.y + fMapCheckYLerp * fScaleLerp)
            {
                inputPos.y = vMapCheckBaseY.y + fMapCheckYLerp * fScaleLerp;
            }

            if (vLeft.x > 0f && direction == Vector3.zero)
            {
                Vector3 vGetPos = Camera.main.ViewportToWorldPoint(vLeft + new Vector3(0.5f, 0, 0));
                //targetCamPos.position = new Vector3(vGetPos.x, targetCamPos.position.y, targetCamPos.position.z);
                targetCamPos.position = new Vector3(vGetPos.x, inputPos.y, inputPos.z);
                transform.position = targetCamPos.position;
            }
            else if (vRight.x < 1f && direction == Vector3.zero)
            {
                Vector3 vGetPos = Camera.main.ViewportToWorldPoint(vRight - new Vector3(0.5f, 0, 0));
                //targetCamPos.position = new Vector3(vGetPos.x, targetCamPos.position.y, targetCamPos.position.z);
                targetCamPos.position = new Vector3(vGetPos.x, inputPos.y, inputPos.z);
                transform.position = targetCamPos.position;
            }
            else
            {
                targetCamPos.position = inputPos;
            }
        }
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetCamPos.rotation, 5 * Time.deltaTime);
        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos.position, ref refV3, 0.2f);
        pCam.orthographicSize = Mathf.SmoothDamp(pCam.orthographicSize, fOrthographicSize, ref fScaleValue, 0.2f);
        if (pYYCam != null)
            pYYCam.orthographicSize = pCam.orthographicSize;
    }
}
