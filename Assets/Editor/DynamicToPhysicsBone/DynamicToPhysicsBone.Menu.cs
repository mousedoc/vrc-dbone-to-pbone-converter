using UnityEditor;

namespace DynamicToPhysicsBone
{
    public class Menu
    {
        [MenuItem("Tools/DynamicToPhysicsBone/Open", priority = 15)]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<Window>();
            window.Show();
        }
    }
}