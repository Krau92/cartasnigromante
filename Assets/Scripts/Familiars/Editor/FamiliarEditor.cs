using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(Familiar))]
public class FamiliarEditor : Editor
{
    private SerializedProperty passivesProperty;
    private bool isPassivesListExpanded = true;

    private void OnEnable()
    {
        passivesProperty = serializedObject.FindProperty("passives");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "passives");

        EditorGUILayout.Space();

        isPassivesListExpanded = EditorGUILayout.Foldout(isPassivesListExpanded, "Passives", true, EditorStyles.foldoutHeader);

        if (isPassivesListExpanded)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < passivesProperty.arraySize; i++)
            {
                SerializedProperty element = passivesProperty.GetArrayElementAtIndex(i);
                
                string displayName = "Unassigned Passive";

                if (element.managedReferenceValue != null)
                {
                    var passive = element.managedReferenceValue as Passives;
                    displayName = passive.name; 
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, new GUIContent(displayName), true);

                if (GUILayout.Button(new GUIContent("X", "Remove Passive"), GUILayout.Width(25)))
                {
                    passivesProperty.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Passive"))
            {
                var menu = new GenericMenu();
                var baseType = typeof(Passives);

                var subclasses = TypeCache.GetTypesDerivedFrom(baseType)
                    .Where(t => !t.IsAbstract && !t.IsGenericType);

                foreach (var subclass in subclasses)
                {
                    menu.AddItem(new GUIContent(subclass.Name), false, () =>
                    {
                        passivesProperty.arraySize++; 
                        var newElement = passivesProperty.GetArrayElementAtIndex(passivesProperty.arraySize - 1);
                        newElement.managedReferenceValue = Activator.CreateInstance(subclass);
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}