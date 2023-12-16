using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class UVControl : MonoBehaviour
{
    public int index;
    public int x, y;
    public float speed = 1;
    public float delayTime = 0;
    public bool boolHold = false;

    private int runtimeIndex;
    private float startTime;

    Renderer render;
    bool started;

    Material curMat
    {
        get
        {
            if (Application.isPlaying)
            {
                return render.material;
            }
            else
            {
                return render.sharedMaterial;
            }
        }
    }

    void Awake()
    {
        render = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            started = false;
            runtimeIndex = index;
            TexOffset(runtimeIndex);
            outTime = Time.time + (delayTime == 0 ? .0000001f : delayTime);
        }
    }

    private float outTime = 0;
    void Begin()
    {
        started = true;
        startTime = Time.realtimeSinceStartup;
        outTime = 0;
    }

    void Update()
    {
        if (outTime > 0 && Time.time >= outTime)
        {
            Begin();
        }

        if (started)
        {
            float time = Time.realtimeSinceStartup - startTime;
            runtimeIndex = (int)(time * speed);
            if (boolHold)
            {
                runtimeIndex = Mathf.Min(x * y - 1, runtimeIndex);
            }
            else
            {
                runtimeIndex = runtimeIndex % (x * y);
            }
            TexOffset(runtimeIndex);
        }
    }

    private void TexOffset(int index)
    {
        if (curMat == null)
        {
            return;
        }

        //Debug.LogError(index + "," + x);

        int xIndex = index % x;
        int yIndex = -index / x - 1;

        if (Application.isPlaying)
        {
            curMat.mainTextureOffset = new Vector2(xIndex * (1f / x), yIndex * (1f / y));
            curMat.mainTextureScale = new Vector2(1f / x, 1f / y);
        }
        else
        {
            curMat.mainTextureOffset = new Vector2(xIndex * (1f / x), yIndex * (1f / y));
            curMat.mainTextureScale = new Vector2(1f / x, 1f / y);
        }

    }
    public void OnDestroy()
    {
        if (Application.isPlaying)
        {
            DestroyImmediate(curMat);
        }
    }
}
