using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
 
 
public static class ExposeProperties
{	
	public static void Expose (string propertyName, PropertyField[] properties) {
		foreach (PropertyField field in properties) {
			if (field.Name.Replace (" ", "") == propertyName) {
				DrawProperty (field);
				return;
			}
		}
	}

	public static void Expose( PropertyField[] properties )
	{
 
		EditorGUILayout.BeginVertical();
 
		foreach ( PropertyField field in properties )
		{	
			EditorGUILayout.BeginHorizontal();
			DrawProperty (field);
			EditorGUILayout.EndHorizontal();
		}
 
		EditorGUILayout.EndVertical();
 
	}

	static void DrawProperty (PropertyField property) {

		GUILayoutOption[] emptyOptions = new GUILayoutOption[0];
		switch ( property.Type )
			{
			case SerializedPropertyType.Integer:
					property.SetValue( EditorGUILayout.IntField( property.Name, (int)property.GetValue(), emptyOptions ) ); 
				break;
 
			case SerializedPropertyType.Float:
					property.SetValue( EditorGUILayout.FloatField( property.Name, (float)property.GetValue(), emptyOptions ) );
				break;
 
			case SerializedPropertyType.Boolean:
					property.SetValue( EditorGUILayout.Toggle( property.Name, (bool)property.GetValue(), emptyOptions ) );
				break;
 
			case SerializedPropertyType.String:
					property.SetValue( EditorGUILayout.TextField( property.Name, (String)property.GetValue(), emptyOptions ) );
				break;
 
			case SerializedPropertyType.Vector2:
					property.SetValue( EditorGUILayout.Vector2Field( property.Name, (Vector2)property.GetValue(), emptyOptions ) );
				break;
 
			case SerializedPropertyType.Vector3:
					property.SetValue( EditorGUILayout.Vector3Field( property.Name, (Vector3)property.GetValue(), emptyOptions ) );
				break;
 
			case SerializedPropertyType.Enum:
   				    property.SetValue (EditorGUILayout.EnumPopup(property.Name, (Enum)property.GetValue(), emptyOptions));
				break;
 			
			default: break;
			}
	}
 
	public static PropertyField[] GetProperties (System.Object obj) {
 
		List< PropertyField > fields = new List<PropertyField>();
		PropertyInfo[] infos = obj.GetType().GetProperties (BindingFlags.Public | BindingFlags.Instance);
 
		foreach (PropertyInfo info in infos) {
			
			if (!(info.CanRead && info.CanWrite))
				continue;
 
			object[] attributes = info.GetCustomAttributes( true );
			bool isExposed = false;
 
			foreach (object o in attributes) {
				if (o.GetType() == typeof( ExposePropertyAttribute )) {
					isExposed = true;
					break;
				}
			}
 
			if (!isExposed)
				continue;
 
			SerializedPropertyType type = SerializedPropertyType.Integer;
			if (PropertyField.GetPropertyType (info, out type)) {
				PropertyField field = new PropertyField (obj, info, type);
				fields.Add (field);
			}
		}
 
		return fields.ToArray();
	}
 
}
 
public class PropertyField
{
	System.Object m_Instance;
	PropertyInfo m_Info;
	SerializedPropertyType m_Type;
 
	MethodInfo m_Getter;
	MethodInfo m_Setter;

	public SerializedPropertyType Type {
		get { return m_Type; }
	}
 
	public String Name {	
		get { return ObjectNames.NicifyVariableName( m_Info.Name );	}
	}
 
	public PropertyField (System.Object instance, PropertyInfo info, SerializedPropertyType type) {	
 
		m_Instance = instance;
		m_Info = info;
		m_Type = type;
 
		m_Getter = m_Info.GetGetMethod ();
		m_Setter = m_Info.GetSetMethod ();
	}
 
	public System.Object GetValue() {
		return m_Getter.Invoke (m_Instance, null);
	}
 
	public void SetValue (System.Object value) {
		m_Setter.Invoke (m_Instance, new System.Object[] { value });
	}
 
	public static bool GetPropertyType (PropertyInfo info, out SerializedPropertyType propertyType) {
 
		propertyType = SerializedPropertyType.Generic;
		Type type = info.PropertyType;
 
		if ( type == typeof( int ) )
		{
			propertyType = SerializedPropertyType.Integer;
			return true;
		}
 
		if ( type == typeof( float ) )
		{
			propertyType = SerializedPropertyType.Float;
			return true;
		}
 
		if ( type == typeof( bool ) )
		{
			propertyType = SerializedPropertyType.Boolean;
			return true;
		}
 
		if ( type == typeof( string ) )
		{
			propertyType = SerializedPropertyType.String;
			return true;
		}	
 
		if ( type == typeof( Vector2 ) )
		{
			propertyType = SerializedPropertyType.Vector2;
			return true;
		}
 
		if ( type == typeof( Vector3 ) )
		{
			propertyType = SerializedPropertyType.Vector3;
			return true;
		}
 
		if ( type.IsEnum )
		{
			propertyType = SerializedPropertyType.Enum;
			return true;
		}

		return false;
	}
}