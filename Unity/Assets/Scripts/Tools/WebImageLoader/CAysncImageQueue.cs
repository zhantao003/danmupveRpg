using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CAsyncImageInfo
{
    public CAysncImageDownload asyncImageDownload;
    public string URL;
    public RawImage image;
    public EMAsyncImageType type;
}

public enum EMAsyncImageType
{
    local,
    net
}


public class CSaveImageInfo
{
    public byte[] pngData;
    public string fileName;
}

public class CAysncImageQueue
{
    private static Queue loadImageQueue = new Queue();
    private static Queue saveImageQueue = new Queue();

    public static void addLoadImageQueue(CAsyncImageInfo asyncImageInfo)
    {
        loadImageQueue.Enqueue(asyncImageInfo);
    }

    public static void readLoadImageQueue()
    {
        if (loadImageQueue.Count > 0)
        {
            CAsyncImageInfo asyncImageInfo = (CAsyncImageInfo)loadImageQueue.Dequeue();

            if (asyncImageInfo.image)
            {
                switch (asyncImageInfo.type)
                {
                    case EMAsyncImageType.local:
                        asyncImageInfo.asyncImageDownload.localImageAction(asyncImageInfo.URL, asyncImageInfo.image);
                        break;
                    case EMAsyncImageType.net:
                        asyncImageInfo.asyncImageDownload.downloadImageAction(asyncImageInfo.URL, asyncImageInfo.image);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public static void addSaveImageQueue(CSaveImageInfo saveImageInfo)
    {
        saveImageQueue.Enqueue(saveImageInfo);
    }

    public static void readSaveImageQueue()
    {
        if (saveImageQueue.Count > 0)
        {
            CSaveImageInfo saveImageInfo = (CSaveImageInfo)saveImageQueue.Dequeue();

            File.WriteAllBytes(saveImageInfo.fileName, saveImageInfo.pngData);
        }
    }
}
