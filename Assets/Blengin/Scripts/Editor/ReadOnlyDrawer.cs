// NOTE put in a Editor folder

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Ngin
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer 
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label);
			GUI.enabled = true;
		}
	}
}