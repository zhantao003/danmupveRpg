using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CDanmuGiftAttrite : Attribute
{ 
    public CDanmuGiftConst eventKey; // �¼���ֵ

    public CDanmuGiftAttrite(CDanmuGiftConst value)
    {
        eventKey = value;
    }
}
