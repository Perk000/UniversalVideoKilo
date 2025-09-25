using UnityEditor;
using UnityEngine;

namespace SilverTau.NSR.Recorders.Audio
{
    [CustomEditor(typeof(AudioReceiver))]
    public class AudioReceiverEditor : NSRPackageEditor
    {
        private AudioReceiver _target;
        
        private SerializedProperty _isMute;
        
        private void OnEnable()
        {
            if (target) _target = (AudioReceiver)target;
            
            _isMute = serializedObject.FindProperty("isMute");
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
	        
	        BoxLogo(_target, "<b> <color=#ffffff>Audio Receiver</color></b>");
	        
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(5);
            GUILayout.Box(new GUIContent("<b>Settings</b>", EditorGUIUtility.IconContent("d_MoreOptions@2x").image), labelHeader2);
            
            GUILayout.Space(10);
            
            EditorGUILayout.PropertyField(_isMute);
            
            //base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        [MenuItem("CONTEXT/AudioListener/Silver Tau/NSR - Screen Recorder/Audio Receiver", false)]
        public static void AddAudioReceiverToAudioListener(MenuCommand command)
        {
	        var obj = (AudioListener)command.context;
	        if (!obj.gameObject.TryGetComponent<AudioReceiver>(out var audioReceiver))
	        {
		        audioReceiver = obj.gameObject.AddComponent<AudioReceiver>();
	        }
        }
        
        [MenuItem("CONTEXT/AudioSource/Silver Tau/NSR - Screen Recorder/Audio Receiver", false)]
        public static void AddAudioReceiverToAudioSource(MenuCommand command)
        {
	        var obj = (AudioSource)command.context;
	        if (!obj.gameObject.TryGetComponent<AudioReceiver>(out var audioReceiver))
	        {
		        audioReceiver = obj.gameObject.AddComponent<AudioReceiver>();
	        }
        }
    }
}