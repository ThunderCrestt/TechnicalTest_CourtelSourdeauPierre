using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using com.technical.test;

/// <summary>
/// this class manages the editor of the rotators in the inspector, here it just adds a button to open the editor window
/// </summary>
[CustomEditor(typeof(Rotator))]
public class RotatorEditor : Editor
{
    Rotator rotator;
    public void OnEnable()
    {
        //get the current rotator
        rotator = (Rotator)target;
    }

    /// <summary>
    /// we ovveride the inspector gui to change what the inspector editor looks like, we just add a button
    /// </summary>
    /// 
    public override void OnInspectorGUI()
    {
        //We call base base.OnInspectorGUI(); so the variables in the editor do not change
        base.OnInspectorGUI();

        //the button to open the editor window that is created in RotatorEditorWindow
        if (GUILayout.Button("open editor window"))
        {
            RotatorEditorWindow editorWindow= RotatorEditorWindow.ShowWindow();
            //if we open the window from the inspector, the default values are from the selected rotator 
            editorWindow.rotatorsToEdit.Add(rotator);
            editorWindow.setValuesFromLastRotator();
        }
    }
}
