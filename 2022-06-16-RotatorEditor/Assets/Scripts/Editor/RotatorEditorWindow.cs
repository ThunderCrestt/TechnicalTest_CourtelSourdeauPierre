using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using com.technical.test;
public class RotatorEditorWindow : EditorWindow
{
    //the several toggles to display and manage checkboxes
    bool toggleIdentifier;
    bool toggleTimeBeforeStop;
    bool toggleReverseRotation;
    bool toggleRotationSettings;
    bool toggleObjectToRotate;
    bool toggleAngleRotation;
    bool toggleTimeToRotate;

    //the variables of the rotator
    string identifier;
    float timeBeforeStop;
    bool reverseRotation;
    [SerializeField]
    RotationSettings rotationSettings;

    //the rotators list
    [SerializeField]
    public List<Rotator> rotatorsToEdit = new List<Rotator>();

    //
    ScriptableObject target;
    SerializedObject serializableObjectTarget;
    SerializedProperty listProperty;

    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Window/Custom/ Rotators Multiple Setter")]

    /// <summary>
    /// open the editor window, returns itself to pass initial values when opened from inspector.
    /// </summary>
    public static RotatorEditorWindow ShowWindow()
    {
         return GetWindow(typeof(RotatorEditorWindow)) as RotatorEditorWindow;
    }


    /// <summary>
    /// Draw a line to separate content
    /// </summary>
    /// <param name="color"> the color of the line </param>
    /// <param name="thickness"> the ththickness of the line </param>
    /// <param name="padding">the padding between the next section and the current one</param>
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    /// <summary>
    /// is called when a value is changed, here is used to update the fields when a rotator is set in the list. we use OnValidate because BeginChangeCheck did not work properly with list.
    /// </summary>
    private void OnValidate()
    {
        setValuesFromLastRotator();
    }

    /// <summary>
    /// work like an init, here we just define the SerializedObject to use find property to find the rotatorsToEdit list
    /// </summary>
    private void OnEnable()
    {
       target = this;
       serializableObjectTarget = new SerializedObject(target);
       listProperty = serializableObjectTarget.FindProperty("rotatorsToEdit");
    }

    //TODO : make undos
    //TODO : faire une jolie liste de rotator en bas.

    /// <summary>
    /// definitions of the fields, helpBoxes and button
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Rotator Editor Window");
        //we search the SerializedProperty to add a custom list field in the editor window
        serializableObjectTarget.Update();
        EditorGUILayout.PropertyField(listProperty, new GUIContent("Rotators to edit"), true);

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
        EditorGUI.BeginChangeCheck();
        string newIdentifier = EditorGUILayout.TextField(" ", identifier);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "change identifier");
            identifier = newIdentifier;
            serializableObjectTarget.Update();
        }
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
        //We disable the button if the identifier field is empty, or if the rotator list is empty, or when the object to rotate in rotation setting is empty
        EditorGUI.BeginDisabledGroup(
            ((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleRotationSettings && !toggleTimeBeforeStop)
            || (toggleRotationSettings &&(!toggleAngleRotation && !toggleTimeToRotate && !toggleObjectToRotate))
            || (rotatorsToEdit == null || rotatorsToEdit?.Count<=0 || rotatorsToEdit?[0]==null) 
            || (toggleIdentifier && (identifier == string.Empty || identifier == null)) 
            || (toggleRotationSettings && toggleObjectToRotate && rotationSettings.ObjectToRotate ==null)
                                        );
        if(GUILayout.Button("Validate Changes"))
        {
            ValidateChanges();
        }
        EditorGUI.EndDisabledGroup();

        //help boxes
        if ((rotatorsToEdit == null || rotatorsToEdit?.Count <= 0 || rotatorsToEdit?[0] == null))
        {
            EditorGUILayout.HelpBox("Add and set a rotator to modify in the Rotators to edit List", MessageType.Warning);
        }
        if (
            ((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleRotationSettings && !toggleTimeBeforeStop)
            || (toggleRotationSettings && (!toggleAngleRotation && !toggleTimeToRotate && !toggleObjectToRotate))
            )
        {
            EditorGUILayout.HelpBox("The rotator will not change, toggle changes with checkboxes", MessageType.Warning);
        }
        if ((toggleIdentifier && (identifier == string.Empty || identifier == null)))
        {
            EditorGUILayout.HelpBox("Assign a name (identifier) to the rotator", MessageType.Warning);
        }
        if ((toggleRotationSettings && toggleObjectToRotate && rotationSettings.ObjectToRotate == null))
        {
            EditorGUILayout.HelpBox("Add a object to rotate in the rotation settings", MessageType.Warning);
        }

        DrawUILine(Color.gray, 1, 5);

        if (rotatorsToEdit?.Count>0)
        {
            if(rotatorsToEdit[0]!=null)
            {
                showRotatorsToEdit();
            }
        }
    }

    /// <summary>
    /// set the values displayed in function of the last added rotator.
    /// </summary>
    public void setValuesFromLastRotator()
    {
        if (rotatorsToEdit?.Count > 0 && rotatorsToEdit[rotatorsToEdit.Count - 1] != null)
        {

            toggleIdentifier = false;
            toggleTimeBeforeStop = false;
            toggleReverseRotation = false;
            toggleRotationSettings = false;
            toggleObjectToRotate = false;
            toggleAngleRotation = false;
            toggleTimeToRotate = false;
            identifier = rotatorsToEdit[rotatorsToEdit.Count - 1]._identifier;
            timeBeforeStop = rotatorsToEdit[rotatorsToEdit.Count - 1]._timeBeforeStoppingInSeconds;
            reverseRotation = rotatorsToEdit[rotatorsToEdit.Count - 1]._shouldReverseRotation;
            rotationSettings = rotatorsToEdit[rotatorsToEdit.Count - 1]._rotationsSettings;
            Debug.Log("changes");
            Repaint();
        }
    }


    /// <summary>
    /// display the rotators added to the rotatorsToEdit list
    /// </summary>
    private void showRotatorsToEdit()
    {
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

    /// <summary>
    /// manages the effect of the button "Validate Changes"
    /// </summary>
    private void ValidateChanges()
    {
        foreach (Rotator rotator in rotatorsToEdit)
        {
            if (rotator != null)
            {
                if (toggleIdentifier) { rotator._identifier = identifier; }
                if (toggleTimeBeforeStop) { rotator._timeBeforeStoppingInSeconds = timeBeforeStop; }
                if (toggleReverseRotation) { rotator._shouldReverseRotation = reverseRotation; }
                if (toggleRotationSettings) 
                {
                    if (toggleObjectToRotate) rotator._rotationsSettings.ObjectToRotate = rotationSettings.ObjectToRotate;
                    if (toggleAngleRotation) rotator._rotationsSettings.AngleRotation = rotationSettings.AngleRotation;
                    if (toggleTimeToRotate) rotator._rotationsSettings.TimeToRotateInSeconds = rotationSettings.TimeToRotateInSeconds;
                }
            }
        }
        Repaint();
    }
}
