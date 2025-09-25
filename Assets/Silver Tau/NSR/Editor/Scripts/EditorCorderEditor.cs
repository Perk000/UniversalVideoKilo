using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR.Recorders
{
    [CustomEditor(typeof(NSREditorCorder))]
    public class EditorCorderEditor : NSRPackageEditor
    {
        private NSREditorCorder _target;
        
        private void OnEnable()
        {
            if (target) _target = (NSREditorCorder)target;
        }
        
        public override void OnInspectorGUI()
        {
	        helpBox = new GUIStyle(GUI.skin.FindStyle("HelpBox"))
	        {
		        alignment = TextAnchor.MiddleLeft,
		        richText = true,
		        fontSize = 12,
		        imagePosition = ImagePosition.ImageLeft,
		        padding = new RectOffset(10, 10, 0, 0),
		        normal = 
		        {
			        background = CreateTexture2D(2, 2, colorDarkBlue), 
			        textColor = Color.white
		        }
	        };
	        
	        BoxLogo(_target, "<b> <color=#ffffff>Editor Corder</color></b>");
	        
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(5);
            GUILayout.Box(new GUIContent("\n" 
                                         + "Editor Corder is an auxiliary recording system. This system is used to record video in Unity Editor."
                                         + "\n", EditorGUIUtility.IconContent("console.infoicon").image), helpBox);
            
            GUILayout.Space(10);
            //base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}