using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CDanmuLikeAttrite : Attribute
{
    public string eventKey; // ÊÂ¼ş¼üÖµ

    public CDanmuLikeAttrite(string value)
    {
        eventKey = value;
    }
}
