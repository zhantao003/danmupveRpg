using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CDanmuCmdAttrite : Attribute
{
    public CDanmuEventConst eventKey; // �¼���ֵ

    public CDanmuCmdAttrite(CDanmuEventConst value)
    {
        eventKey = value;
    }
}
