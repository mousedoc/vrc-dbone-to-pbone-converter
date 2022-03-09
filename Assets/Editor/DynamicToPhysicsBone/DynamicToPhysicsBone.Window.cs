using UnityEditor;
using UnityEngine;

namespace DynamicToPhysicsBone
{
    public class Window : EditorWindow
    {
        private GameObject targetObject = null;
        private ConvertOption option = new ConvertOption();

        private void OnGUI()
        {
            GUILayout.Label("DynamicBone To PhysicsBone Converter");
            targetObject = (GameObject)EditorGUILayout.ObjectField("Target Avatar", targetObject, typeof(GameObject), true);

            DrawOptions();

            ChangeBackgroundColor(Color.green);
            if (GUILayout.Button("Convert", GUILayout.Height(50)))
            {
                var core = new Core();
                core.Convert(targetObject, option);
            }
            ResetBackgroundColor();
        }

        private void DrawOptions()
        {
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            // Grab & Pose
            option.AllowGrab = EditorGUILayout.Toggle(nameof(option.AllowGrab), option.AllowGrab);
            option.AllowPose = EditorGUILayout.Toggle(nameof(option.AllowPose), option.AllowPose);
            option.MaxStretch = EditorGUILayout.FloatField(nameof(option.MaxStretch), option.MaxStretch);
            option.GrabMovement = EditorGUILayout.FloatField(nameof(option.GrabMovement), option.GrabMovement);

            // Offset
            option.PullOffset = EditorGUILayout.FloatField(nameof(option.PullOffset), option.PullOffset);
            option.SpringOffset = EditorGUILayout.FloatField(nameof(option.SpringOffset), option.SpringOffset);
            option.ImmobileOffset = EditorGUILayout.FloatField(nameof(option.ImmobileOffset), option.ImmobileOffset);

            // Clamp
            option.MaxAngle = EditorGUILayout.FloatField(nameof(option.MaxAngle), option.MaxAngle);
        }

        private void ChangeBackgroundColor(Color color) => GUI.backgroundColor = color;

        private void ResetBackgroundColor() => GUI.backgroundColor = Color.white;
    }
}