using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR.Graphic
{
    [CustomEditor(typeof(GraphicProvider))]
    public class GraphicProviderEditor : NSRPackageEditor
    {
        private GraphicProvider _target;
        
        private SerializedProperty _mainCamera;
        private SerializedProperty _graphicSettings;
        
        private void OnEnable()
        {
            if (target) _target = (GraphicProvider)target;
            
            _mainCamera = serializedObject.FindProperty("mainCamera");
            _graphicSettings = serializedObject.FindProperty("graphicSettings");
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
	        
	        BoxLogo(_target, "<b> <color=#ffffff>Graphic Provider</color></b>");
	        
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Box(new GUIContent("\n" 
                                         + "Graphic provider makes it easy to take a screenshot and save the image."
                                         + "\n", EditorGUIUtility.IconContent("console.infoicon").image), helpBox);
            
            GUILayout.Space(5);
            GUILayout.Box(new GUIContent("<b>Main Settings</b>", EditorGUIUtility.IconContent("d_MoreOptions@2x").image), labelHeader2);
            
            GUILayout.Space(10);
            
            _mainCamera.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" Target Camera", EditorGUIUtility.IconContent("Camera Icon").image), _mainCamera.objectReferenceValue, typeof(Camera), true);
            
            GUILayout.Space(5);
            _graphicSettings.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(" Settings Object", EditorGUIUtility.IconContent("d_ScriptableObject Icon").image), _graphicSettings.objectReferenceValue, typeof(GraphicSettings), true);
            
            //base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        
        private static GameObject CreateElementRoot(string name)
        {
	        var child = new GameObject(name);
	        Undo.RegisterCreatedObjectUndo(child, "Create " + name);
	        Selection.activeGameObject = child;
	        return child;
        }

        static GameObject CreateObject(string name, GameObject parent)
        {
	        var go = new GameObject(name);
	        GameObjectUtility.SetParentAndAlign(go, parent);
	        return go;
        }
        
        [MenuItem("GameObject/Silver Tau/NSR/Graphic Provider", false)]
        static public void AddGraphicProvider()
        {
	        var nsr = CreateElementRoot("Graphic Provider");
	        nsr.AddComponent<GraphicProvider>();
        }
    }
}