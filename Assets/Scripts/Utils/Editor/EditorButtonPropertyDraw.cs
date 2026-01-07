using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EditorButton))]
public class EditorButtonPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);

        EditorButton btn = (EditorButton)(fieldInfo.GetValue(prop.serializedObject.targetObject));
        string methodName = btn.method;

        // Enable/disable button for runtime only functions
        if (btn.runtimeOnly)
        {
            GUI.enabled = Application.isPlaying;
        }
        else
        {
            GUI.enabled = true;
        }

        if (GUI.Button(pos, label))
        {
            CallMethod(prop, methodName);
        }

        GUI.enabled = true;
        EditorGUI.EndProperty();
    }

    private void CallMethod(SerializedProperty prop, string methodName)
    {
        System.Type type = prop.serializedObject.targetObject.GetType();
        MethodInfo method = type.GetMethod(methodName);

        if (method == null)
        {
            Debug.LogError("Method " + methodName + " not found (Check spelling and that it is public)");
        }
        else
        {
            // This will give us all the multi-selected components.
            Object[] objects = prop.serializedObject.targetObjects;
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length > 0)
            {
                Debug.LogError("Method " + methodName + " expects " + parameters.Length + " parameters. Editor buttons don't support methods with parameters.");
            }
            else
            {
                foreach (Object obj in objects)
                {
                    method.Invoke(obj, null);
                }
            }
        }
    }
}
