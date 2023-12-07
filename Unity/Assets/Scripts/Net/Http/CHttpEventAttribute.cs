using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CHttpEventAttribute : Attribute
{
    public string eventKey; // 事件键值

    public CHttpEventAttribute(string value)
    {
        eventKey = value;
    }
}
