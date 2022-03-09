using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DynamicToPhysicsBone.Window;

namespace DynamicToPhysicsBone
{

    public class Menu
    {
        [MenuItem("Tools/DynamicToPhysicsBone/Open", priority = 15)]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<DynamicToPhysicsBone.Window>();
            window.Show();
        }

    }
}
