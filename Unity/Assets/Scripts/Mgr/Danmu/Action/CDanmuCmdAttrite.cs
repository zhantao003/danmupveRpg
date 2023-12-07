using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CDanmuCmdAttrite : Attribute
{
    public CDanmuEventConst eventKey; // ÊÂ¼þ¼üÖµ

    public CDanmuCmdAttrite(CDanmuEventConst value)
    {
        eventKey = value;
    }
}
