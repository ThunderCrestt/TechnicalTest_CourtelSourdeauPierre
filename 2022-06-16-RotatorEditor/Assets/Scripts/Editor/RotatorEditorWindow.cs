using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using com.technical.test;

//this script manage the editor window for rotators
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

    // Scriptable object and variable used for the reordable list and rotation settings
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
        //display the editor window
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

        //min size of the editor window
        this.minSize = new Vector2(800, 650);

        //setup of the reorderableList and callbacks
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
        #region reorderableList field

        GUILayout.Label("Rotator Editor Window");
        //we update the serializable object and display the reorderableList
        serializableObjectTarget.Update();
        reorderableList.DoLayoutList();
        //apply the modification in the reorderableList

        //draw a separator
        DrawUILine(Color.gray, 1, 5);
        #endregion

        #region rotator field

        EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Editor", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        //the identifier field in a toggle group
        EditorGUILayout.BeginHorizontal();
            toggleIdentifier = EditorGUILayout.BeginToggleGroup("Identifier :", toggleIdentifier);
                //text field with undo
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
                // float field with undo
                EditorGUI.BeginChangeCheck();
                    float newTimeBeforeStop = timeBeforeStop;
                    newTimeBeforeStop = EditorGUILayout.FloatField(" ", timeBeforeStop,GUILayout.ExpandWidth(true));
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
        #endregion

        #region rotation settings field

        //the rotation settings in a toggle group with each of it's property in a toggle group
        SerializedProperty rotationsSettings = serializableObjectTarget.FindProperty("rotationSettings");
        toggleRotationSettings = EditorGUILayout.BeginToggleGroup("V rotation Settings :", toggleRotationSettings);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
                toggleObjectToRotate = EditorGUILayout.BeginToggleGroup("object to rotate :", toggleObjectToRotate);
                    //the object to rotate field with undo
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
                    //the angle rotation field with undo
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
                    //the time to rotate field with undo
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
        #endregion

        #region button

        //We disable the button if the identifier field is empty, or if the rotator list is empty, or when the object to rotate in rotation setting is empty or finally if all checkboxes are set to false
        EditorGUI.BeginDisabledGroup(
            ((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleRotationSettings && !toggleTimeBeforeStop)
            || (((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleTimeBeforeStop) && (toggleRotationSettings && !toggleObjectToRotate && !toggleTimeToRotate && !toggleAngleRotation))
            || (rotatorsToEdit == null || rotatorsToEdit?.Count <= 0 || rotatorsToEdit?[0] == null)
            || (toggleIdentifier && (identifier == string.Empty || identifier == null))
            || (toggleRotationSettings && toggleObjectToRotate && rotationSettings.ObjectToRotate == null));

            if (GUILayout.Button("Validate Changes"))
            {
                ValidateChanges();
            }
        EditorGUI.EndDisabledGroup();
        #endregion

        #region helpboxes

        //help boxes to display warning messages to the user
        if ((rotatorsToEdit == null || rotatorsToEdit?.Count <= 0 || rotatorsToEdit?[0] == null))
        {
            EditorGUILayout.HelpBox("Add and set a rotator to modify in the Rotators to edit List", MessageType.Warning);
        }
        if (
            ((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleRotationSettings && !toggleTimeBeforeStop)
            || (((rotatorsToEdit != null && rotatorsToEdit?.Count > 0 && rotatorsToEdit?[0] != null) && !toggleIdentifier && !toggleReverseRotation && !toggleTimeBeforeStop) && (toggleRotationSettings && !toggleObjectToRotate && !toggleTimeToRotate && !toggleAngleRotation))
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

        //a separator
        DrawUILine(Color.gray, 1, 5);
        #endregion

        #region display of the rotators

        //if there is a rotator set by the user, show them at the bottom of the editor window
        if (rotatorsToEdit?.Count > 0)
        {
            if (rotatorsToEdit[0] != null)
            {
                showRotatorsToEdit();
            }
        }
        #endregion
    }

    /// <summary>
    /// display the rotators added to the rotatorsToEdit list
    /// </summary>
    private void showRotatorsToEdit()
    {

        //we create a vertical layout to contains several rows
        EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.ExpandHeight(true));

                EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Rotator display", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                // is used to count the number of rotators display
                int increment = 0;
                foreach (Rotator rotator in rotatorsToEdit)
                {
                    if (increment % 2 == 0)
                    {
                        //to create an other row when the previous one contains 2 rotators to display, GUILayout.Width(Screen.width) so it take all the space available
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
                    }

                    //GUILayout.Width(Screen.width/2) so it take half of the available space, so 2 rotator can be displayed on the same row
                    EditorGUILayout.BeginVertical(GUILayout.Width(Screen.width / 2));
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
                if (increment % 2 !=0)
                {
                    EditorGUILayout.EndHorizontal();
                }
            EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

        /// <summary>
        /// manages the effect of the button "Validate Changes"
        /// </summary>
    private void ValidateChanges()
    {
        // for each rotator wich are not null, we apply the changes if the toggle concerned is true
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
    /// set the values displayed in function of the last added rotator.
    /// </summary>
    public void setValuesFromLastRotator()
    {
        //is used when a rotator is add, or if the window is opened from the inspector
        if (rotatorsToEdit?.Count > 0 && rotatorsToEdit[rotatorsToEdit.Count - 1] != null)
        {
            identifier = rotatorsToEdit[rotatorsToEdit.Count - 1]._identifier;
            timeBeforeStop = rotatorsToEdit[rotatorsToEdit.Count - 1]._timeBeforeStoppingInSeconds;
            reverseRotation = rotatorsToEdit[rotatorsToEdit.Count - 1]._shouldReverseRotation;
            rotationSettings = rotatorsToEdit[rotatorsToEdit.Count - 1]._rotationsSettings;
            Repaint();
        }
    }

    /// <summary>
    /// Draw a line to separate content
    /// </summary>
    /// <param name="color"> the color of the line </param>
    /// <param name="thickness"> the ththickness of the line </param>
    /// <param name="padding">the padding between the next section and the current one</param>
    /// <param name="percentOfScreenEmpty"> the percent of empty screen we want on the separator, 0 -> the separator take all the width of the window </param>
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
