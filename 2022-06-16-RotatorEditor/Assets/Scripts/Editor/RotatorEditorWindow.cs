using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using com.technical.test;
public class RotatorEditorWindow : EditorWindow
{

    [SerializeField]
    List<GameObject> rotatorsToEdit;
    [MenuItem("Window/Custom/ Rotators Multiple Setter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(RotatorEditorWindow));
    }

    //TODO : ajouter un list pour choisir les rotators à changer
    //TODO : ajouter des check pour venir choisir ce qu'on modifie ( et pouvoir les modifier par la suite )
    //TODO : ajouter un bouton pour valider les changements
    //TODO : ajouter des fenêtres pour afficher les rotators qui vont être modifier ( par forcement possible de modifier à l'intérieur )
    private void OnGUI()
    {
        GUILayout.Label("Rotator Editor Window");
        //we search the SerializedProperty to add a custom list field in the editor window
        ScriptableObject target = this;
        SerializedObject list = new SerializedObject(target);
        SerializedProperty listProperty = list.FindProperty("rotatorsToEdit");
        EditorGUILayout.PropertyField(listProperty, new GUIContent("rotators to edit"), true);
        list.ApplyModifiedProperties();
    }
}
