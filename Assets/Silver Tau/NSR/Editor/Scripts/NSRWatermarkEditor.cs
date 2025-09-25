using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR.Recorders.Watermark
{
    [CustomEditor(typeof(NSRWatermark))]
    public class NSRWatermarkEditor : NSRPackageEditor
    {
        private NSRWatermark _target;
        
        protected GUIStyle button;
        protected Color colorMain;
        protected Color colorMainActive;

        public override void Awake()
        {
	        base.Awake();
	        
	        ColorUtility.TryParseHtmlString("#212121", out colorMain);
	        ColorUtility.TryParseHtmlString("#ec3c63", out colorMainActive);
	        
	        button = new GUIStyle
	        {
		        alignment = TextAnchor.MiddleCenter,
		        fixedHeight = 26.0f,
		        richText = true,
		        stretchWidth = false,
		        padding = new RectOffset(8, 8, 8, 8),
		        normal = new GUIStyleState
		        {
			        background = CreateTexture2D(2, 2, colorMain),
			        textColor = Color.white
		        },
		        active = 
		        {
			        background = CreateTexture2D(2, 2, colorMainActive),
			        textColor = Color.white
		        }
	        };
        }

        private void OnEnable()
        {
            if (target) _target = (NSRWatermark)target;
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
	        
	        BoxLogo(_target, "<b> <color=#ffffff>Watermark</color></b>");
	        
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(10);
            base.OnInspectorGUI();
            
            GUILayout.Space(10);
            GUILayout.Space(2.5f);
            GUILayout.Label("<b>- - -</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleCenter, fontSize = 12, normal = { textColor = Color.white}});
            GUILayout.Space(2.5f);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("<b>Update settings in real time</b>", button))
            {
	            _target.UpdateSettingsRealtime();
            }
            
            GUILayout.FlexibleSpace();
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}