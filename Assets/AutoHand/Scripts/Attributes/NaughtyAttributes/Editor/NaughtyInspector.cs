using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UnityEngine.Object), true)]
	public class NaughtyInspector : UnityEditor.Editor
	{

		private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();
		private IEnumerable<FieldInfo> _nonSerializedFields;
		private IEnumerable<PropertyInfo> _nativeProperties;
		private IEnumerable<MethodInfo> _methods;
		private Dictionary<string, SavedBool> _foldouts = new Dictionary<string, SavedBool>();

		protected virtual void OnEnable()
		{
			_nonSerializedFields = ReflectionUtility.GetAllFields(
				target, f => f.GetCustomAttributes(typeof(ShowNonSerializedFieldAttribute), true).Length > 0);

			_nativeProperties = ReflectionUtility.GetAllProperties(
				target, p => p.GetCustomAttributes(typeof(ShowNativePropertyAttribute), true).Length > 0);

			_methods = ReflectionUtility.GetAllMethods(
				target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
		}

		protected virtual void OnDisable()
		{
			ReorderableListPropertyDrawer.Instance.ClearCache();
		}

		double lastUpdateTime;
		public override void OnInspectorGUI()
		{
				GetSerializedProperties(ref _serializedProperties);

				bool anyNaughtyAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<INaughtyAttribute>(p) != null);
				if (!anyNaughtyAttribute)
				{
					DrawDefaultInspector();
				}
				else
				{
					DrawSerializedProperties();
				}

				DrawNativeProperties();
				DrawNonSerializedFields();
				DrawButtons();

		}

		protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
		{
			outSerializedProperties.Clear();
			using (var iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
					}
					while (iterator.NextVisible(false));
				}
			}
		}

		const string SCRIPT_NAME = "m_Script";
		protected void DrawSerializedProperties()
		{
			serializedObject.Update();


			var nongroups = GetNonGroupedProperties(_serializedProperties);
			// Draw non-grouped serialized properties
			foreach (var property in nongroups)
			{
				if (property.name.Equals(SCRIPT_NAME, System.StringComparison.Ordinal))
				{
					using (new EditorGUI.DisabledScope(disabled: true))
					{
						EditorGUILayout.PropertyField(property);
					}
				}
				else
				{
					NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
				}
			}
			
			IEnumerable<SerializedProperty> visibleProperties;
			var groups = GetGroupedProperties(_serializedProperties);
			// Draw grouped serialized properties
			foreach (var group in groups)
			{
				visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
				if (!visibleProperties.Any())
				{
					continue;
				}

				NaughtyEditorGUI.BeginBoxGroup_Layout(group.Key);
				foreach (var property in visibleProperties)
				{
					NaughtyEditorGUI.PropertyField_Layout(property, includeChildren: true);
				}

				NaughtyEditorGUI.EndBoxGroup_Layout();
			}


			var foldGroups = GetFoldoutProperties(_serializedProperties);
			// Draw foldout serialized properties
			foreach (var group in foldGroups)
			{
				visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
				if (!visibleProperties.Any())
				{
					continue;
				}

				if (!_foldouts.ContainsKey(group.Key))
				{
					_foldouts[group.Key] = new SavedBool($"{target.GetInstanceID()}.{group.Key}", false);
				}

				_foldouts[group.Key].Value = EditorGUILayout.Foldout(_foldouts[group.Key].Value, group.Key, true);
				if (_foldouts[group.Key].Value)
				{
					foreach (var property in visibleProperties)
					{
						NaughtyEditorGUI.PropertyField_Layout(property, true);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected void DrawNonSerializedFields(bool drawHeader = false)
		{
			if (_nonSerializedFields.Any())
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Non-Serialized Fields", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
				}

				foreach (var field in _nonSerializedFields)
				{
					NaughtyEditorGUI.NonSerializedField_Layout(serializedObject.targetObject, field);
				}
			}
		}

		protected void DrawNativeProperties(bool drawHeader = false)
		{
			if (_nativeProperties.Any())
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Native Properties", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
				}

				foreach (var property in _nativeProperties)
				{
					NaughtyEditorGUI.NativeProperty_Layout(serializedObject.targetObject, property);
				}
			}
		}

		protected void DrawButtons(bool drawHeader = false)
		{
			if (_methods.Count() > 0)
			{
				if (drawHeader)
				{
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Buttons", GetHeaderGUIStyle());
					NaughtyEditorGUI.HorizontalLine(
						EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
				}

				foreach (var method in _methods)
				{
					NaughtyEditorGUI.Button(serializedObject.targetObject, method);
				}
			}
		}

		private static IEnumerable<SerializedProperty> GetNonGroupedProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties.Where(p => PropertyUtility.GetAttribute<IGroupAttribute>(p) == null);
		}

		private static IEnumerable<IGrouping<string, SerializedProperty>> GetGroupedProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties
				.Where(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p) != null)
				.GroupBy(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p).Name);
		}

		private static IEnumerable<IGrouping<string, SerializedProperty>> GetFoldoutProperties(IEnumerable<SerializedProperty> properties)
		{
			return properties
				.Where(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p) != null)
				.GroupBy(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p).Name);
		}

		private static GUIStyle GetHeaderGUIStyle()
		{
			GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
			style.fontStyle = FontStyle.Bold;
			style.alignment = TextAnchor.UpperCenter;

			return style;
		}
	}
}
