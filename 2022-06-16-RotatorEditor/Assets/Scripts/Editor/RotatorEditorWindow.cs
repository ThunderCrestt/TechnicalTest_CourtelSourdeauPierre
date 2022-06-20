using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using com.technical.test;
public class RotatorEditorWindow : EditorWindow
{
    bool toggleIdentifier;
    int identifier;
    bool toggleTimeBeforeStop;
    float timeBeforeStop;
    bool toggleReverseRotation;
    bool reverseRotation;
    bool toggleRotationSettings;
    [SerializeField]
    RotationSettings rotationSettings;
    bool toggleObjectToRotate;
    bool toggleAngleRotation;
    bool toggleTimeToRotate;
    [SerializeField]
    List<GameObject> rotatorsToEdit;

    [MenuItem("Window/Custom/ Rotators Multiple Setter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(RotatorEditorWindow));
    }


    //Draw a line to separate content
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    //TODO : ajouter des check pour venir choisir ce qu'on modifie ( et pouvoir les modifier par la suite )
    //TODO : modifier les valeurs quand un rotator est chargé, comment faire quand on en charge plusieurs ? peut être faire un toggle group dans la list pour choisir qui on modifie, et true de base
    //TODO : ajouter un bouton pour valider les changements
    //TODO : ajouter des fenêtres pour afficher les rotators qui vont être modifier ( par forcement possible de modifier à l'intérieur )
    private void OnGUI()
    {
        GUILayout.Label("Rotator Editor Window");
        //we search the SerializedProperty to add a custom list field in the editor window
        ScriptableObject target = this;
        SerializedObject serializableObjectTarget = new SerializedObject(target);
        SerializedProperty listProperty = serializableObjectTarget.FindProperty("rotatorsToEdit");
        EditorGUILayout.PropertyField(listProperty, new GUIContent("rotators to edit"), true);
        serializableObjectTarget.ApplyModifiedProperties();

        //draw a separator
        DrawUILine(Color.gray,1,5);

        //the identifier field in a toggle group
        EditorGUILayout.BeginHorizontal();
        toggleIdentifier = EditorGUILayout.BeginToggleGroup("Identifier :", toggleIdentifier);
        identifier = EditorGUILayout.IntField(" ", identifier);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndHorizontal();

        //the Time before Stopping in Seconds field in a toggle group
        EditorGUILayout.BeginHorizontal();
        toggleTimeBeforeStop = EditorGUILayout.BeginToggleGroup("Time before Stopping in Seconds :",toggleTimeBeforeStop);
        timeBeforeStop = EditorGUILayout.FloatField(" ", timeBeforeStop);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndHorizontal();

        //the Should Reverse Rotation field in a toggle group
        EditorGUILayout.BeginHorizontal();
        toggleReverseRotation = EditorGUILayout.BeginToggleGroup("Should Reverse Rotation :", toggleReverseRotation);
        reverseRotation = EditorGUILayout.Toggle(" ", reverseRotation);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndHorizontal();

        //the rotation settings in a toggle group with each of it's property in a toggle group
        SerializedProperty rotationsSettings = serializableObjectTarget.FindProperty("rotationSettings");
        toggleRotationSettings = EditorGUILayout.BeginToggleGroup("V rotation Settings :", toggleRotationSettings);
        EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            toggleObjectToRotate = EditorGUILayout.BeginToggleGroup("object to rotate :", toggleObjectToRotate);
            EditorGUILayout.PropertyField(rotationsSettings.FindPropertyRelative("ObjectToRotate"), GUIContent.none, true);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            toggleAngleRotation = EditorGUILayout.BeginToggleGroup("Angle Rotation :", toggleAngleRotation);
            EditorGUILayout.PropertyField(rotationsSettings.FindPropertyRelative("AngleRotation"), GUIContent.none, true);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            toggleTimeToRotate = EditorGUILayout.BeginToggleGroup("Time to Rotate in Seconds :", toggleTimeToRotate);
            EditorGUILayout.PropertyField(rotationsSettings.FindPropertyRelative("TimeToRotateInSeconds"), GUIContent.none, true);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
        EditorGUILayout.EndToggleGroup();


        EditorGUILayout.Space();
        if(GUILayout.Button("Validate Changes"))
        {
            ValidateChanges();
        }
        
    }

    private void ValidateChanges()
    {
        //TODO : Validate Changes
    }
}
