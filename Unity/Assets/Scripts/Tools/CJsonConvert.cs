using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CJsonConver
{
    //#region
    //// 将对象序列化为JSON字符串
    //public static string SerializeObject(object obj)
    //{
    //    // 如果对象为空，返回null
    //    if (obj == null) return null;
    //    JSONObject JSONObject = new JSONObject();
    //    Type objectType = obj.GetType();
    //    // 获取对象的所有属性
    //    PropertyInfo[] properties = ReflectionCache.GetProperties(objectType);

    //    // 遍历每个属性
    //    foreach (PropertyInfo property in properties)
    //    {
    //        // 如果属性有索引参数，跳过它
    //        if (property.GetIndexParameters().Length > 0)
    //        {
    //            continue;
    //        }
    //        // 获取属性的值
    //        object propertyValue = property.GetValue(obj);
    //        string propertyName = property.Name;
    //        try
    //        {
    //            // 序列化属性的值
    //            JSONNode jsonValue = SerializePropertyValue(propertyValue);
    //            JSONObject[propertyName] = jsonValue;
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogWarning($"Error while serializing property: {propertyName} of type: {property.PropertyType}. Error: {e.ToString()}");
    //            return null; // 发生错误时返回null
    //        }
    //    }

    //    return JSONObject.ToString();
    //}

    //// 序列化属性值
    //private static JSONNode SerializePropertyValue(object value)
    //{
    //    // 如果值为null，返回JSONNull
    //    if (value == null)
    //    {
    //        return null; // return JSONNull for null values
    //    }
    //    else
    //    {
    //        try
    //        {
    //            Type valueType = value.GetType();
    //            if (valueType.IsGenericType)
    //            {
    //                if (valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
    //                {
    //                    return SerializeDictionary((dynamic)value);
    //                }
    //                else if (valueType.GetGenericTypeDefinition() == typeof(List<>))
    //                {
    //                    return SerializeList((dynamic)value);
    //                }
    //                else
    //                {
    //                    throw new InvalidOperationException($"不支持的泛型类型：{valueType}");
    //                }
    //            }
    //            else if (IsSimpleType(valueType))
    //            {
    //                return SerializeSimpleType(value, valueType);
    //            }
    //            else
    //            {
    //                return JSON.Parse(SerializeObject(value));
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogWarning("Error while serializing: " + e.ToString());
    //            return null;
    //        }
    //    }
    //}
    //private static JSONObject SerializeDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    //{
    //    JSONObject JSONObject = new JSONObject();
    //    foreach (var kvp in dictionary)
    //    {
    //        JSONObject[kvp.Key.ToString()] = SerializePropertyValue(kvp.Value);
    //    }
    //    return JSONObject;
    //}
    //private static JSONArray SerializeList<T>(List<T> list)
    //{
    //    JSONArray JSONArray = new JSONArray();
    //    foreach (var item in list)
    //    {
    //        JSONArray.Add(SerializePropertyValue(item));
    //    }
    //    return JSONArray;
    //}

    //// 序列化简单类型的值
    //private static JSONNode SerializeSimpleType(object value, Type valueType)
    //{
    //    // 根据值的类型，采取不同的序列化方式
    //    switch (Type.GetTypeCode(valueType))
    //    {
    //        case TypeCode.Boolean:
    //        case TypeCode.Byte:
    //        case TypeCode.SByte:
    //        case TypeCode.Char:
    //        case TypeCode.Decimal:
    //        case TypeCode.Double:
    //        case TypeCode.Int16:
    //        case TypeCode.Int32:
    //        case TypeCode.Int64:
    //        case TypeCode.UInt16:
    //        case TypeCode.UInt32:
    //        case TypeCode.UInt64:
    //        case TypeCode.Single:
    //            return new JSONNumber(Convert.ToDouble(value));
    //        case TypeCode.DateTime:
    //            return new JSONString(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"));
    //        case TypeCode.String:
    //            return new JSONString(value.ToString());
    //        default:
    //            if (valueType.IsEnum)
    //            {
    //                return new JSONNumber(Convert.ToInt32(value));
    //            }
    //            else
    //            {
    //                return new JSONString(value.ToString());
    //            }
    //    }
    //}
    //public static T DeserializeObject<T>(string jsonString)
    //{
    //    return (T)DeserializeObject(jsonString, typeof(T));
    //}
    //// 将JSON字符串反序列化为对象
    //private static object DeserializeObject(string jsonString, Type type)
    //{
    //    // 解析JSON字符串
    //    JSONNode jsonNode = JSON.Parse(jsonString);
    //    if (jsonNode.IsNull) {
    //        return null;
    //    }
    //    // 创建对象实例
    //    object obj = Activator.CreateInstance(type);
    //    // 获取对象的所有属性
    //    PropertyInfo[] properties = ReflectionCache.GetProperties(type);

    //    // 遍历每个属性
    //    foreach (PropertyInfo property in properties)
    //    {
    //        string propertyName = property.Name;
    //        try
    //        {
    //            // 如果JSON对象包含该属性
    //            if (jsonNode.HasKey(propertyName))
    //            {
    //                // 获取JSON值
    //                JSONNode jsonValue = jsonNode[propertyName];
    //                // 反序列化属性值
    //                object propertyValue = DeserializePropertyValue(property.PropertyType, jsonValue);
    //                // 将反序列化得到的值赋给对象的属性
    //                property.SetValue(obj, propertyValue, null);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogWarning($"Error while deserializing property: {propertyName} of type: {property.PropertyType}. Error: {e.ToString()}");
    //            return null; // 发生错误时返回null
    //        }
    //    }

    //    return obj;
    //}

    //// 反序列化属性值
    //private static object DeserializePropertyValue(Type propertyType, JSONNode jsonValue)
    //{
    //    // 如果JSON值为null，返回null
    //    if (jsonValue.IsNull)
    //    {
    //        return null;
    //    }

    //    // 如果属性类型为简单类型
    //    if (IsSimpleType(propertyType))
    //    {
    //        try
    //        {
    //            // 如果属性类型为枚举
    //            if (propertyType.IsEnum)
    //            {
    //                // 将JSON值转化为枚举
    //                return Enum.Parse(propertyType, jsonValue.Value);
    //            }
    //            else
    //            {
    //                // 将JSON值转化为属性类型
    //                return Convert.ChangeType(jsonValue.Value, propertyType);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError("Error while deserializing: " + propertyType.ToString() + " " + e.ToString());
    //            return null;
    //        }
    //    }
    //    else if (propertyType.IsGenericType)
    //    {
    //        if (propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
    //        {
    //            return DeserializeDictionary(propertyType, jsonValue.AsObject);
    //        }
    //        else if (propertyType.GetGenericTypeDefinition() == typeof(List<>))
    //        {
    //            return DeserializeList(propertyType, jsonValue.AsArray);
    //        }
    //        else
    //        {
    //            throw new InvalidOperationException($"不支持的泛型类型：{propertyType}");
    //        }
    //    }
    //    else
    //    {
    //        // 如果属性类型不是简单类型，递归地反序列化该属性值
    //        return DeserializeObject(jsonValue.ToString(), propertyType);
    //    }
    //}
    //private static object DeserializeDictionary(Type type, JSONObject jsonObject)
    //{
    //    var method = typeof(CJsonConver).GetMethod("DeserializeDictionaryGeneric", BindingFlags.NonPublic | BindingFlags.Static);
    //    var genericMethod = method.MakeGenericMethod(type.GetGenericArguments());
    //    return genericMethod.Invoke(null, new object[] { jsonObject });
    //}

    //private static Dictionary<TKey, TValue> DeserializeDictionaryGeneric<TKey, TValue>(JSONObject jsonObject)
    //{
    //    Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
    //    foreach (var key in jsonObject.Keys)
    //    {
    //        TKey dictionaryKey = (TKey)Convert.ChangeType(key, typeof(TKey));
    //        TValue dictionaryValue = (TValue)DeserializePropertyValue(typeof(TValue), jsonObject[key]);
    //        dictionary.Add(dictionaryKey, dictionaryValue);
    //    }
    //    return dictionary;
    //}
    //private static object DeserializeList(Type type, JSONArray jsonArray)
    //{
    //    var method = typeof(CJsonConver).GetMethod("DeserializeListGeneric", BindingFlags.NonPublic | BindingFlags.Static);
    //    var genericMethod = method.MakeGenericMethod(type.GetGenericArguments());
    //    return genericMethod.Invoke(null, new object[] { jsonArray });
    //}
    //private static List<T> DeserializeListGeneric<T>(JSONArray jsonArray)
    //{
    //    List<T> list = new List<T>();
    //    foreach (var jsonNode in jsonArray)
    //    {
    //        list.Add((T)DeserializePropertyValue(typeof(T), jsonNode));
    //    }
    //    return list;
    //}
    //// 判断一个类型是否为简单类型
    //private static bool IsSimpleType(Type type)
    //{
    //    return ReflectionCache.GetIsSimpleType(type);
    //}

    //// 用于缓存反射结果的类
    //public static class ReflectionCache
    //{
    //    // 保存已经获取过的类型的属性
    //    private static Dictionary<Type, PropertyInfo[]> _propertyInfos = new Dictionary<Type, PropertyInfo[]>();
    //    // 保存已经判断过是否为简单类型的类型
    //    private static Dictionary<Type, bool> _simpleTypes = new Dictionary<Type, bool>();

    //    // 获取类型的所有属性
    //    public static PropertyInfo[] GetProperties(Type type)
    //    {
    //        if (!_propertyInfos.TryGetValue(type, out var properties))
    //        {
    //            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //            _propertyInfos[type] = properties;
    //        }
    //        return properties;
    //    }

    //    // 判断一个类型是否为简单类型
    //    public static bool GetIsSimpleType(Type type)
    //    {
    //        if (!_simpleTypes.TryGetValue(type, out var isSimple))
    //        {
    //            isSimple = type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type.IsValueType && !type.IsClass;
    //            _simpleTypes[type] = isSimple;
    //        }
    //        return isSimple;
    //    }
    //}
    //#endregion
}
