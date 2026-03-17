// En BattleActions/Editor/AttackSOEditor.cs

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(AttackSO))]
public class AttackSOEditor : Editor
{
    private SerializedProperty battleEffectsProperty;
    private bool isEffectsListExpanded = true; // <-- ¡NUEVO! Variable para guardar el estado

    private void OnEnable()
    {
        battleEffectsProperty = serializedObject.FindProperty("battleEffects");
    }

    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, "battleEffects");
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Space();

        // --- DIBUJO DEL DESPLEGABLE (FOLDOUT) ---
        // EditorGUILayout.Foldout devuelve 'true' si está expandido y 'false' si está contraído.
        // Actualizamos nuestra variable con el resultado de la interacción del usuario.
        isEffectsListExpanded = EditorGUILayout.Foldout(isEffectsListExpanded, "Battle Effects", true, EditorStyles.foldoutHeader); 

        // --- DIBUJO PERSONALIZADO DE LA LISTA (SOLO SI ESTÁ EXPANDIDO) ---
        if (isEffectsListExpanded) 
        {
            EditorGUI.indentLevel++; // Aumentamos la indentación para que se vea anidado

            for (int i = 0; i < battleEffectsProperty.arraySize; i++)
            {
                SerializedProperty element = battleEffectsProperty.GetArrayElementAtIndex(i);
                
                string displayName = "Unassigned Effect";

                if (element.managedReferenceValue != null)
                {
                    var effect = element.managedReferenceValue as BattleEffect;
                    displayName = effect.EffectName; 
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(element, new GUIContent(displayName), true);

                if (GUILayout.Button(new GUIContent("X", "Remove Effect"), GUILayout.Width(25)))
                {
                    battleEffectsProperty.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            // El botón de añadir también va dentro de la condición
            if (GUILayout.Button("Add New Battle Effect"))
            {
                var menu = new GenericMenu();
                var baseType = typeof(BattleEffect);

                var subclasses = TypeCache.GetTypesDerivedFrom(baseType)
                    .Where(t => !t.IsAbstract && !t.IsGenericType);

                foreach (var subclass in subclasses)
                {
                    menu.AddItem(new GUIContent(subclass.Name), false, () =>
                    {
                        battleEffectsProperty.arraySize++; 
                        var newElement = battleEffectsProperty.GetArrayElementAtIndex(battleEffectsProperty.arraySize - 1);
                        newElement.managedReferenceValue = Activator.CreateInstance(subclass);
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            EditorGUI.indentLevel--; // Restauramos la indentación
        }

        serializedObject.ApplyModifiedProperties();
    }
}