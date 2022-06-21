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
    [SerializeField]
    string identifier;
    [SerializeField]
    float timeBeforeStop;
    bool reverseRotation;
    [SerializeField]
    RotationSettings rotationSettings;

    //the rotators list
    [SerializeField]
    public List<Rotator> rotatorsToEdit = new List<Rotator>();
    ReorderableList reorderableList = null;

    // Scriptable object and variable used for the propertyField
    ScriptableObject target;
    SerializedObject serializableObjectTarget;
    SerializedProperty listProperty;

    //Used in the scroll view in the rotators display
    Vector2 scrollPos = Vector2.zero;

    //create the menu item in the specified path
    [MenuItem("Window/Custom/ Rotators Multiple Setter")]

    /// <summary>
    /// open the editor window, returns itself to pass initial values when opened from inspector.
    /// </summary>
    public static RotatorEditorWindow ShowWindow()
    {
        return GetWindow(typeof(RotatorEditorWindow)) as RotatorEditorWindow;
    }

    /// <summary>
    /// work like an init, here we just define the SerializedObject to use find property to find the rotatorsToEdit list
    /// </summary>
    private void OnEnable()
    {
        target = this;
        serializableObjectTarget = new SerializedObject(target);
        listProperty = serializableObjectTarget.FindProperty("rotatorsToEdit");
        this.minSize = new Vector2(600, 600);

        reorderableList = new ReorderableList(serializableObjectTarget, listProperty, true, true, true, true);
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Rotators to edit");
        };
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            rect.y += 2;
            EditorGUI.BeginChangeCheck();
            rotatorsToEdit[index] = EditorGUI.ObjectField(new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height), rotatorsToEdit[index], typeof(Rotator),true) as Rotator;
            if(EditorGUI.EndChangeCheck())
            {
                setValuesFromLastRotator();
            }
        };
    }

    /// <summary>
    /// definitions of the fields, helpBoxes and button
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("Rotator Editor Window");
        //we search the SerializedProperty to add a custom list field in the editor window
        serializableObjectTarget.Update();
        reorderableList.DoLayoutList();

        //draw a separator
        DrawUILine(Color.gray, 1, 5);

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false),GUILayout.Width(Screen.width));

        EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Editor", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        //the identifier field in a toggle group
        EditorGUILayout.BeginHorizontal();
            toggleIdentifier = EditorGUILayout.BeginToggleGroup("Identifier :", toggleIdentifier);
                EditorGUI.BeginChangeCheck();
                    string newIdentifier = identifier;
                    newIdentifier = EditorGUILayout.TextField(" ", identifier);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "identifier Undo");
                    identifier = newIdentifier;
                }
            EditorGUILayout.EndToggleGroup();
        EditorGUILayout.EndHorizontal();

        //the Time before Stopping in Seconds field in a toggle group
        EditorGUILayout.BeginHorizontal();
            toggleTimeBeforeStop = EditorGUILayout.BeginToggleGroup("Time before Stopping in Seconds :", toggleTimeBeforeStop);
                EditorGUI.BeginChangeCheck();
                    float newTimeBeforeStop = timeBeforeStop;
                    newTimeBeforeStop = EditorGUILayout.FloatField(" ", timeBeforeStop);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "timeBeforeStop Undo");
                    timeBeforeStop = newTimeBeforeStop;
                }
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
                    EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(rotationsSettings.FindPropertyRelative("ObjectToRotate"), GUIContent.none, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(this, "object to rotate Undo");
                    }
                EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                toggleAngleRotation = EditorGUILayout.BeginToggleGroup("Angle Rotation :", toggleAngleRotation);
                    EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(rotationsSettings.FindPropertyRelative("AngleRotation"), GUIContent.none, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(this, "Angle Rotation Undo");
                    }
                EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                toggleTimeToRotate = EditorGUILayout.BeginToggleGroup("Time to Rotate in Seconds :", toggleTimeToRotate);
                    EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(rotationsSettings.FindPropertyRelative("TimeToRotateInSeconds"), GUIContent.none, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(this, "Time to Rotate in Seconds Undo");
                    }
                EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.Space();
        serializableObjectTarget.ApplyModifiedProperties();



        //We disable the button if the identifier field is empty, or if the rotator list is empty, or when the object to rotate in rotation setting is empty
        EditorGUI.BeginDisabledGroup(
            ((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleRotationSettings && !toggleTimeBeforeStop)
            || (rotatorsToEdit == null || rotatorsToEdit?.Count <= 0 || rotatorsToEdit?[0] == null)
            || (toggleIdentifier && (identifier == string.Empty || identifier == null))
            || (toggleRotationSettings && toggleObjectToRotate && rotationSettings.ObjectToRotate == null));

            if (GUILayout.Button("Validate Changes"))
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

        if (rotatorsToEdit?.Count > 0)
        {
            if (rotatorsToEdit[0] != null)
            {
                showRotatorsToEdit();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// set the values displayed in function of the last added rotator.
    /// </summary>
    public void setValuesFromLastRotator()
    {
        if (rotatorsToEdit?.Count > 0 && rotatorsToEdit[rotatorsToEdit.Count - 1] != null)
        {
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
        //GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

        GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Rotator display", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        int increment = 0;
        foreach (Rotator rotator in rotatorsToEdit)
        {
            if (increment%2==0)
            {
                //to create an other row when the previous one contains 2 rotators to display, GUILayout.Width(Screen.width) so it take all the space available
                EditorGUILayout.BeginHorizontal(GUILayout.Width(Screen.width));

                //GUI.Box(new Rect(increment % 2 * Screen.width / 2, 100, 100, 100), "truc machin");
            }
            //GUILayout.Width(Screen.width/2) so it take half of the available space
            EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width/2));

            if (rotator != null)
            {
                SerializedObject so = new SerializedObject(rotator);
                EditorGUILayout.PropertyField(so.FindProperty("_identifier"), new GUIContent("Identifier"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_timeBeforeStoppingInSeconds"), new GUIContent("Time before stopping"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_shouldReverseRotation"), new GUIContent("Should Reverse Rotation"), true);
                EditorGUILayout.PropertyField(so.FindProperty("_rotationsSettings"), new GUIContent("Rotations settings"), true);
                EditorGUILayout.Space();

            }
            EditorGUILayout.EndVertical();
            if (increment % 2 != 0)
            {
                //end of the row
                EditorGUILayout.EndHorizontal();
                DrawUILine(Color.gray, 2, 10, 0.8f);

            }
            increment++;

        }
        //GUI.EndGroup();
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

    /// <summary>
    /// Draw a line to separate content
    /// </summary>
    /// <param name="color"> the color of the line </param>
    /// <param name="thickness"> the ththickness of the line </param>
    /// <param name="padding">the padding between the next section and the current one</param>
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10, float percentOfScreenEmpty=0f)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        //r.x -= 2;
        r.x = percentOfScreenEmpty * Screen.width;
        r.width =(1f-2*percentOfScreenEmpty)*Screen.width;
        EditorGUI.DrawRect(r, color);
        EditorGUILayout.Space();
    }


}
