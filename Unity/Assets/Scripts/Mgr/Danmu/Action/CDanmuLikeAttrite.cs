using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CDanmuLikeAttrite : Attribute
{
    public string eventKey; // �¼���ֵ

    public CDanmuLikeAttrite(string value)
    {
        eventKey = value;
    }
}
