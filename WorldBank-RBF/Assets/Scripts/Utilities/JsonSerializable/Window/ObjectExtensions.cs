using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public static class ObjectExtensions
{
    public static T ToObject<T>(this IDictionary<string, object> source)
        where T : class, new()
    {
            T someObject = new T();
            System.Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
            	PropertyInfo info = someObjectType.GetProperty (item.Key);
            	if (info != null)
	            	info.SetValue(someObject, item.Value, null);

            }

            return someObject;
    }

    public static object ToObject2 (this IDictionary<string, object> source, System.Type type) {
    	object o = Activator.CreateInstance (type);
    	foreach (KeyValuePair<string, object> item in source) {
    		PropertyInfo info = type.GetProperty (item.Key);
    		info.SetValue (o, item.Value, null);
    	}
    	return o;
    }

    public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
    {
        return source.GetType().GetProperties(bindingAttr).ToDictionary
        (
            propInfo => propInfo.Name,
            propInfo => propInfo.GetValue(source, null)
        );

    }
}