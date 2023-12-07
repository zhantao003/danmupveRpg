using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CAysncImageDownload : CSingleCompBase<CAysncImageDownload>
{
    private Dictionary<string, Texture2D> spriteDic = new Dictionary<string, Texture2D>();

    private void Start()
    {
        init();

        InvokeRepeating("readLoadImageQueue", 0, 0.05f);
        InvokeRepeating("readSaveImageQueue", 0, 1);
    }

    private void readLoadImageQueue()
    {
        CAysncImageQueue.readLoadImageQueue();
    }

    private void readSaveImageQueue()
    {
        CAysncImageQueue.readSaveImageQueue();
    }

    private void init()
    {
        if (!Directory.Exists(imageCacheFolderPath))
        {
            Directory.CreateDirectory(imageCacheFolderPath);
        }

        spriteDic = new Dictionary<string, Texture2D>();
    }

    public void setAsyncImage(string url, RawImage image, bool isReload = false)
    {
        if (url == null) return;

        CAsyncImageInfo asyncImageInfo = new CAsyncImageInfo
        {
            asyncImageDownload = this,
            URL = url,
            image = image
        };

        if (!File.Exists(imageCacheFolderPath + url.GetHashCode() + ".png"))
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                if (!spriteDic.ContainsKey(imageCacheFolderPath + url.GetHashCode()))
                {
                    asyncImageInfo.type = EMAsyncImageType.net;
                }
                else
                {
                    asyncImageInfo.type = EMAsyncImageType.local;
                }
            }
        }
        else
        {
            asyncImageInfo.type = EMAsyncImageType.local;
        }

        if (asyncImageInfo.type == EMAsyncImageType.local)
        {
            localImageAction(asyncImageInfo.URL, asyncImageInfo.image);
        }
        else if (asyncImageInfo.type == EMAsyncImageType.net)
        {
            downloadImageAction(asyncImageInfo.URL, asyncImageInfo.image);
        }
    }

    public void downloadImageAction(string url, RawImage image)
    {
        StartCoroutine(downloadImage(url, image));
    }

    public void localImageAction(string url, RawImage image)
    {
        StartCoroutine(loadLocalImage(url, image));
    }

    private IEnumerator downloadImage(string url, RawImage image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }

        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

        try
        {
            CSaveImageInfo saveImageInfo = new CSaveImageInfo
            {
                pngData = texture.EncodeToPNG(),
                fileName = imageCacheFolderPath + url.GetHashCode() + ".png"
            };

            CAysncImageQueue.addSaveImageQueue(saveImageInfo);
        }
        catch
        {
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        //Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        if(image!=null)
            image.texture = texture;

        if (!spriteDic.ContainsKey(imageCacheFolderPath + url.GetHashCode()))
        {
            spriteDic.Add(imageCacheFolderPath + url.GetHashCode(), texture);
        }
    }

    private IEnumerator loadLocalImage(string url, RawImage image)
    {
        if (!spriteDic.ContainsKey(imageCacheFolderPath + url.GetHashCode()))
        {
            string filePath = "file:///" + imageCacheFolderPath + url.GetHashCode() + ".png";

            WWW www = new WWW(filePath);

            yield return www;

            Texture2D texture = www.texture;

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            //Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            image.texture = texture;

            if (!spriteDic.ContainsKey(imageCacheFolderPath + url.GetHashCode()))
            {
                spriteDic.Add(imageCacheFolderPath + url.GetHashCode(), texture);
            }
        }
        else
        {
            image.texture = spriteDic[imageCacheFolderPath + url.GetHashCode()];
        }
    }

    private string imageCacheFolderPath
    {
        get { return CAppPathMgr.LOCALSAVEDATA_DIR + "/ImageCaChe/"; }
    }
}
