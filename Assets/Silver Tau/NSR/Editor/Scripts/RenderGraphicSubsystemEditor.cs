using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR.Graphic
{
    [CustomEditor(typeof(RenderGraphicSubsystem))]
    public class RenderGraphicSubsystemEditor : NSRPackageEditor
    {
        private RenderGraphicSubsystem _target;
        
        private void OnEnable()
        {
            if (target) _target = (RenderGraphicSubsystem)target;
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
	        
	        BoxLogo(_target, "<b> <color=#ffffff>Render Graphic Subsystem</color></b>");
	        
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(5);
            GUILayout.Box(new GUIContent("\n" 
                                         + "The Render Graphic Subsystem is an auxiliary system that allows you to take a screenshot and save the image using the render texture of the targeted camera."
                                         + "\n\n"
                                         + "The subsystem interacts with the Graphic Provider."
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