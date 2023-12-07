using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CDanmuGiftAttrite : Attribute
{ 
    public CDanmuGiftConst eventKey; // ÊÂ¼þ¼üÖµ

    public CDanmuGiftAttrite(CDanmuGiftConst value)
    {
        eventKey = value;
    }
}
