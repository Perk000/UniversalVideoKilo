#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SilverTau.NSR.Recorders.Audio
{
    [CustomEditor(typeof(MicrophoneAudioInput), true)]
    public class MicrophoneAudioInputEditor : NSRPackageEditor
    {
        private MicrophoneAudioInput _target;
        
        protected GUIStyle button;
        protected Color colorChildHover;
        protected Color colorMain;
        protected Color colorMainHover;
        
        private SerializedProperty _audioFormats;
        private SerializedProperty _setAutoFrequency;
        private SerializedProperty _frequency;
        private SerializedProperty _setAutoChannels;
        private SerializedProperty _channels;
        private SerializedProperty _computeRMS;
        private SerializedProperty _computeDB;
        private SerializedProperty _bufferWindowLength;
        
        private SerializedProperty _mainSettingExpand;
        private SerializedProperty _audioSettingExpand;
        private SerializedProperty _advancedSettingExpand;
        
        public override void Awake()
        {
            base.Awake();
            
            ColorUtility.TryParseHtmlString("#ec3c63", out colorChildHover);
            ColorUtility.TryParseHtmlString("#212121", out colorMain);
            ColorUtility.TryParseHtmlString("#ec3c63", out colorMainHover);
            
            button = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 26.0f,
                richText = true,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorMain),
                    textColor = Color.white
                }, 
                hover = 
                {
                    background = CreateTexture2D(2, 2, colorMainHover),
                    textColor = Color.white
                }
            };
        }
        
        private new Texture2D CreateTexture2D(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for(var i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        private void OnEnable()
        {
            if (target) _target = (MicrophoneAudioInput)target;
            
            _audioFormats = serializedObject.FindProperty("audioFormats");
            _setAutoFrequency = serializedObject.FindProperty("setAutoFrequency");
            _frequency = serializedObject.FindProperty("frequency");
            _setAutoChannels = serializedObject.FindProperty("setAutoChannels");
            _channels = serializedObject.FindProperty("channels");
            _computeRMS = serializedObject.FindProperty("computeRMS");
            _computeDB = serializedObject.FindProperty("computeDB");
            _bufferWindowLength = serializedObject.FindProperty("bufferWindowLength");
            
            _mainSettingExpand = serializedObject.FindProperty("_mainSettingExpand");
            _audioSettingExpand = serializedObject.FindProperty("_audioSettingExpand");
            _advancedSettingExpand = serializedObject.FindProperty("_advancedSettingExpand");
            
            serializedObject.ApplyModifiedProperties();
        }
        
        public override void OnInspectorGUI()
        {
            BoxLogo(_target, " <b><color=#ffffff>Microphone Audio Recorder</color></b>");
            
            //base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            if (GUILayout.Button("<b>Main Settings</b>", button))
            {
                _mainSettingExpand.boolValue = !_mainSettingExpand.boolValue;
            }

            if (_mainSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_audioFormats);
            }
            
            GUILayout.Space(10);
            if (GUILayout.Button("<b>Audio Settings</b>", button))
            {
                _audioSettingExpand.boolValue = !_audioSettingExpand.boolValue;
            }

            if (_audioSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_setAutoFrequency);
            
                if (!_setAutoFrequency.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "Frequency for the audio file." 
                                                 + "\n" 
                                                 + "The default value is 48000."
                                                 + "\n" 
                                                 + "Be careful when changing it."
                                                 + "\n", MessageType.Info);
                
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_frequency);
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Box("", new GUIStyle{normal = new GUIStyleState{background = CreateTexture2D(2, 2, colorChildHover)}}, GUILayout.ExpandWidth(true), GUILayout.Height(2));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                }
            
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_setAutoChannels);
            
                if (!_setAutoChannels.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "The number of channels for the audio file." 
                                                 + "\n" 
                                                 + "The default value is 2."
                                                 + "\n" 
                                                 + "Be careful when changing it."
                                                 + "\n", MessageType.Info);
                
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_channels);
                    GUILayout.Space(10);
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("<b>Advanced Settings</b>", button))
            {
                _advancedSettingExpand.boolValue = !_advancedSettingExpand.boolValue;
            }

            if (_advancedSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_computeRMS);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_computeDB);
            
                if (_computeRMS.boolValue || _computeDB.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "The size of the buffer window for calculating RMS and Decibels." 
                                                 + "\n\n" 
                                                 + "The larger the size of the buffer window array, the more performance the device will use."
                                                 + "\n", MessageType.Info);
                
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_bufferWindowLength);
                    GUILayout.Space(10);
                }
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                EditorSceneManager.MarkSceneDirty(_target.gameObject.scene);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#endif
