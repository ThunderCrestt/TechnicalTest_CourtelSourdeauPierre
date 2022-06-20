using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using com.technical.test;
public class RotatorEditorWindow : EditorWindow
{
    bool toggleIdentifier;
    string identifier;
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
    List<Rotator> rotatorsToEdit;

    Vector2 scrollPos = Vector2.zero;

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

    //TODO : make undos
    //TODO : modifier les valeurs quand un rotator est chargé, comment faire quand on en charge plusieurs ? peut être faire un toggle group dans la list pour choisir qui on modifie, et true de base
    private void OnGUI()
    {
        //TODO : quand valeurs changés appelé callback, modif valeurs, repaint.
        GUILayout.Label("Rotator Editor Window");
        //we search the SerializedProperty to add a custom list field in the editor window
        ScriptableObject target = this;
        SerializedObject serializableObjectTarget = new SerializedObject(target);
        SerializedProperty listProperty = serializableObjectTarget.FindProperty("rotatorsToEdit");
        serializableObjectTarget.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(listProperty, new GUIContent("Rotators to edit"), true);
        if(rotatorsToEdit?.Count > 0 && EditorGUI.EndChangeCheck() && rotatorsToEdit[rotatorsToEdit.Count - 1] != null)
        {
            identifier = rotatorsToEdit[rotatorsToEdit.Count - 1]._identifier;
            timeBeforeStop = rotatorsToEdit[rotatorsToEdit.Count - 1]._timeBeforeStoppingInSeconds;
            reverseRotation = rotatorsToEdit[rotatorsToEdit.Count - 1]._shouldReverseRotation;
            rotationSettings = rotatorsToEdit[rotatorsToEdit.Count - 1]._rotationsSettings;
            Debug.Log("changes");
            Repaint();
        }
        //draw a separator
        DrawUILine(Color.gray,1,5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Editor");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //the identifier field in a toggle group
        EditorGUILayout.BeginHorizontal();
        toggleIdentifier = EditorGUILayout.BeginToggleGroup("Identifier :", toggleIdentifier);
        identifier = EditorGUILayout.TextField(" ", identifier);
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

        serializableObjectTarget.ApplyModifiedProperties();

        EditorGUILayout.Space();
        if(GUILayout.Button("Validate Changes"))
        {
            ValidateChanges();
        }


        //TODO : selected rotators ?
        DrawUILine(Color.gray, 1, 5);

        if (rotatorsToEdit?.Count>0)
        {
            if(rotatorsToEdit[0]!=null)
            {
                showRotatorsToEdit();
            }
        }
    }

    private void showRotatorsToEdit()
    {
        /*
        GUILayout.FlexibleSpace();
        GUILayout.BeginArea(new Rect(0, 1000, 256, 600));
        EditorGUILayout.BeginVertical();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos,false,true,GUILayout.ExpandHeight(true));
        GUILayout.Button("I am a button", GUILayout.MinWidth(150), GUILayout.MinHeight(150));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
        */
        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));
        foreach (Rotator rotator in rotatorsToEdit)
        {
            if (rotator != null)
            {
                //make each of them in a rect
                SerializedObject so = new SerializedObject(rotator);
                EditorGUILayout.PropertyField(so.FindProperty("_identifier"), new GUIContent("Identifier"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_timeBeforeStoppingInSeconds"), new GUIContent("Time before stopping in seconds"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_shouldReverseRotation"), new GUIContent("Should Reverse Rotation"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_rotationsSettings"), new GUIContent("Rotations settings"), true);
                EditorGUILayout.Space();

            }

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

    }

    private void ValidateChanges()
    {
        //TODO : Validate Changes
    }
}
