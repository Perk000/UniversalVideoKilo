#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SilverTau.NSR.Recorders.Video
{
    [CustomEditor(typeof(UniversalVideoRecorder))]
    public class UniversalVideoRecorderEditor : NSRPackageEditor
    {
        private UniversalVideoRecorder _target;
        
        protected GUIStyle button;
        protected GUIStyle buttonChild;
        protected Color colorChild;
        protected Color colorChildHover;
        protected Color colorMain;
        protected Color colorMainHover;
        
        //private SerializedProperty id;
        //private SerializedProperty isDeselectLocked;
        //private SerializedProperty _proSettingsExpand;
        //private SerializedProperty _changeGameObjectExpand;
        //private SerializedProperty _changeSpriteExpand;
        //private SerializedProperty _changeTextExpand;
        //private SerializedProperty gameObjectComponents;
        //private SerializedProperty spriteComponents;
        //private SerializedProperty textComponents;

        //Common
        private SerializedProperty _cameraSettingExpand;
        private SerializedProperty _videoSettingExpand;
        private SerializedProperty _audioSettingExpand;
        private SerializedProperty _advancedSettingExpand;
        private SerializedProperty _editorSettingExpand;
        private SerializedProperty _infoResolution;
        private SerializedProperty _infoInputUILayer;
        private SerializedProperty _useFrameSkip;
        private SerializedProperty _InputUILayer;
        
        //Camera
        private SerializedProperty _mainCamera;
        private SerializedProperty _cameras;
        private SerializedProperty _useRenderTexture;
        private SerializedProperty _targetRenderTexture;
        
        //Video
        //private SerializedProperty _videoDataPath;
        //private SerializedProperty _videoFolderName;
        //private SerializedProperty _videoFileName;
        //private SerializedProperty _videoDateTimeFormat;
        private SerializedProperty _videoResolution;
        private SerializedProperty _useScreenDivide;
        private SerializedProperty _screenDivide;
        private SerializedProperty _frameRate;
        private SerializedProperty _frameSkip;
        private SerializedProperty _encodeTo;
        private SerializedProperty _HDR;
        private SerializedProperty _useCustomVideoResolution;
        private SerializedProperty _customVideoResolution;
        
        private SerializedProperty _advancedVideoSettings;
        private SerializedProperty _videoBitRate;
        
        private SerializedProperty _useFrameDescriptorSrgb;
        
        //Audio
        private SerializedProperty _recordMicrophone;
        private SerializedProperty _recordAllAudioSources;
        private SerializedProperty _audioListener;
        private SerializedProperty _recordOnlyOneAudioSource;
        private SerializedProperty _targetAudioSource;
        private SerializedProperty _audioReceiverMixer;
        private SerializedProperty _audioReceivers;
        
        private SerializedProperty _advancedAudioSettings;
        private SerializedProperty _sampleRate;
        private SerializedProperty _channelCount;
        private SerializedProperty _audioBitRate;
        private SerializedProperty _recordSeparateAudioFile;
        private SerializedProperty _separateAudioFileFormat;
        //private SerializedProperty _separateAudioDataPath;
        //private SerializedProperty _separateAudioFolderPath;
        //private SerializedProperty _separateAudioFileName;
        //private SerializedProperty _separateAudioDateTimeFormat;
        
        //Advanced
        private SerializedProperty _autoPauseResumeRecorder;
        private SerializedProperty _verifyingBeforeInitialization;
        
        //Editor
        private SerializedProperty _targetEditorEncoder;
        private SerializedProperty _targetVideoBitrateMode;
        private SerializedProperty _targetH264VideoEncodingProfile;
        private SerializedProperty _targetEditorIncludeAlpha;
        
        public override void Awake()
        {
            base.Awake();
            
            ColorUtility.TryParseHtmlString("#1b7fe3", out colorChild);
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
            
            buttonChild = new GUIStyle
            {
                padding = new RectOffset(15, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = 21.0f,
                richText = true,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorChild),
                    textColor = Color.white
                }, 
                hover = 
                {
                    background = CreateTexture2D(2, 2, colorChildHover),
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
            if (target) _target = (UniversalVideoRecorder)target;
            
            _infoResolution = serializedObject.FindProperty("_infoResolution");
            _infoInputUILayer = serializedObject.FindProperty("_infoInputUILayer");
            
            _cameraSettingExpand = serializedObject.FindProperty("_cameraSettingExpand");
            _mainCamera = serializedObject.FindProperty("mainCamera");
            _cameras = serializedObject.FindProperty("cameras");
            _useRenderTexture = serializedObject.FindProperty("useRenderTexture");
            _targetRenderTexture = serializedObject.FindProperty("targetRenderTexture");
            
            _videoSettingExpand = serializedObject.FindProperty("_videoSettingExpand");
            //_videoDataPath = serializedObject.FindProperty("videoDataPath");
            //_videoFolderName = serializedObject.FindProperty("videoFolderName");
            //_videoFileName = serializedObject.FindProperty("videoFileName");
            //_videoDateTimeFormat = serializedObject.FindProperty("videoDateTimeFormat");
            _videoResolution = serializedObject.FindProperty("videoResolution");
            _useScreenDivide = serializedObject.FindProperty("useScreenDivide");
            _screenDivide = serializedObject.FindProperty("screenDivide");
            _useCustomVideoResolution = serializedObject.FindProperty("useCustomVideoResolution");
            _customVideoResolution = serializedObject.FindProperty("customVideoResolution");
            _frameRate = serializedObject.FindProperty("frameRate");
            _useFrameSkip = serializedObject.FindProperty("useFrameSkip");
            _frameSkip = serializedObject.FindProperty("frameSkip");
            _encodeTo = serializedObject.FindProperty("encodeTo");
            _HDR = serializedObject.FindProperty("HDR");
            _InputUILayer = serializedObject.FindProperty("inputUILayer");
            
            _advancedVideoSettings = serializedObject.FindProperty("advancedVideoSettings");
            _videoBitRate = serializedObject.FindProperty("videoBitRate");
            
            _useFrameDescriptorSrgb = serializedObject.FindProperty("useFrameDescriptorSrgb");
            
            _audioSettingExpand = serializedObject.FindProperty("_audioSettingExpand");
            _recordMicrophone = serializedObject.FindProperty("recordMicrophone");
            _recordAllAudioSources = serializedObject.FindProperty("recordAllAudioSources");
            _audioListener = serializedObject.FindProperty("audioListener");
            _recordOnlyOneAudioSource = serializedObject.FindProperty("recordOnlyOneAudioSource");
            _targetAudioSource = serializedObject.FindProperty("targetAudioSource");
            _audioReceiverMixer = serializedObject.FindProperty("audioReceiverMixer");
            _audioReceivers = serializedObject.FindProperty("audioReceivers");
            
            _advancedAudioSettings = serializedObject.FindProperty("advancedAudioSettings");
            _sampleRate = serializedObject.FindProperty("sampleRate");
            _channelCount = serializedObject.FindProperty("channelCount");
            _audioBitRate = serializedObject.FindProperty("audioBitRate");
            _recordSeparateAudioFile = serializedObject.FindProperty("recordSeparateAudioFile");
            _separateAudioFileFormat = serializedObject.FindProperty("separateAudioFileFormat");
            //_separateAudioDataPath = serializedObject.FindProperty("separateAudioDataPath");
            //_separateAudioFolderPath = serializedObject.FindProperty("separateAudioFolderPath");
            //_separateAudioFileName = serializedObject.FindProperty("separateAudioFileName");
            //_separateAudioDateTimeFormat = serializedObject.FindProperty("separateAudioDateTimeFormat");
            
            _advancedSettingExpand = serializedObject.FindProperty("_advancedSettingExpand");
            _autoPauseResumeRecorder = serializedObject.FindProperty("autoPauseResumeRecorder");
            _verifyingBeforeInitialization = serializedObject.FindProperty("verifyingBeforeInitialization");
            
            _editorSettingExpand = serializedObject.FindProperty("_editorSettingExpand");
            
            _targetEditorEncoder = serializedObject.FindProperty("editorEncoder");
            _targetVideoBitrateMode = serializedObject.FindProperty("videoBitrateMode");
            _targetH264VideoEncodingProfile = serializedObject.FindProperty("h264VideoEncodingProfile");
            _targetEditorIncludeAlpha = serializedObject.FindProperty("editorIncludeAlpha");
            
            serializedObject.ApplyModifiedProperties();
        }
        
        public override void OnInspectorGUI()
        {
            BoxLogo(_target, " <b><color=#ffffff>Universal Video Recorder</color></b>");
            
            //base.OnInspectorGUI();
            
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            
            if (GUILayout.Button("<b>Camera Settings</b>", button))
            {
                _cameraSettingExpand.boolValue = !_cameraSettingExpand.boolValue;
            }

            if (_cameraSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                
                EditorGUILayout.PropertyField(_mainCamera);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_cameras);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_useRenderTexture);
                
                if (_useRenderTexture.boolValue)
                {
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_targetRenderTexture);
                }
                
                GUILayout.Space(10);
                
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Label("<b>UI Layer</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});

                if(GUILayout.Button("Info", new GUIStyle(GUI.skin.button){ fixedWidth = 50 }))
                {
                    _infoInputUILayer.boolValue = !_infoInputUILayer.boolValue;
                }
                
                EditorGUILayout.EndHorizontal();
                
                if(_infoInputUILayer.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "If you want to record the screen content along with the UI content, the Canvas interface needs to display the camera content." 
                                                 + "\n" 
                                                 + "To do this, do the following:"
                                                 + "\n\n" 
                                                 + "- Enable the InputUILayer option."
                                                 + "\n" 
                                                 + "- In the Canvas settings, set the rendering mode to Screen Space - Camera or World Space. This will allow you to add an interface to the content that your main camera is rendering on stage."
                                                 + "\n\n" 
                                                 + "If you want to record the screen content without the UI content, remember that the Canvas interface needs to display the content on top of the camera. "
                                                 + "\n" 
                                                 + "To do this, do the following:"
                                                 + "\n\n" 
                                                 + "- turn off the InputUILayer option."
                                                 + "\n" 
                                                 + "- in the Canvas settings in the viewer, select View Mode - Screen Space - Overlay."
                                                 + "\n\n" 
                                                 + "If you plan to change the state of the UI layer in real time, use the UniversalVideoRecorder.ConfigInputUILayer parameter and change the Render Mode of the target canvas."
                                                 + "\n" 
                                                 + "For an example, see the NSR - Screen Recorder_Record UI."
                                                 + "\n", MessageType.Info);
                }
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_InputUILayer);
                
                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            // Video
            if (GUILayout.Button("<b>Video Settings</b>", button))
            {
                _videoSettingExpand.boolValue = !_videoSettingExpand.boolValue;
            }

            if (_videoSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                
                //GUILayout.Label("<b>Path & Name</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});
                
                //GUILayout.Space(5);
                //EditorGUILayout.PropertyField(_videoDataPath);
                //GUILayout.Space(5);
                //EditorGUILayout.PropertyField(_videoFolderName);
                //GUILayout.Space(5);
                //EditorGUILayout.PropertyField(_videoFileName);
                //GUILayout.Space(5);
                //EditorGUILayout.PropertyField(_videoDateTimeFormat);
                //GUILayout.Space(10);
                
                EditorGUILayout.BeginHorizontal();
                
                GUILayout.Label("<b>Resolution</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});

                if(GUILayout.Button("Info", new GUIStyle(GUI.skin.button){ fixedWidth = 50 }))
                {
                    _infoResolution.boolValue = !_infoResolution.boolValue;
                }
                
                EditorGUILayout.EndHorizontal();
                
                if(_infoResolution.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "You can control the video resolution depending on the device's resolution using the Video Resolution enum." 
                                                 + "\n\n" 
                                                 + "If the video is portrait or landscape, the resolution will be calculated automatically."
                                                 + "\n\n" 
                                                 + "If you use RenderTexture, the video resolution will depend on the resolution of the RenderTexture."
                                                 + "\n", MessageType.Info);
                }
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_videoResolution);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_useScreenDivide);
                GUILayout.Space(5);

                if (_useScreenDivide.boolValue)
                {
                    _useCustomVideoResolution.boolValue = false;
                    EditorGUILayout.HelpBox("\n"
                                                 + "Screen divide option allow you to divide your current screen resolution by the value you set." 
                                                 + "\n", MessageType.Info);
                    
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_screenDivide);
                    GUILayout.Space(5);
                }
                
                EditorGUILayout.PropertyField(_useCustomVideoResolution);
                GUILayout.Space(5);

                if (_useCustomVideoResolution.boolValue)
                {
                    _useScreenDivide.boolValue = false;
                    EditorGUILayout.HelpBox("\n"
                                            + "The custom video resolution option allows you to set any value."
                                            + "\n"
                                            + "Just be careful with high values." 
                                            + "\n", MessageType.Info);
                    
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_customVideoResolution);
                    GUILayout.Space(5);
                }
                
                GUILayout.Space(5);
                GUILayout.Label("<b>Encode</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});
                
                GUILayout.Space(5);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_frameRate);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_useFrameSkip);
                GUILayout.Space(5);
                if (_useFrameSkip.boolValue)
                {
                    EditorGUILayout.HelpBox("\n"
                                            + "Control number of successive camera frames to skip while recording. Default value = 0." 
                                            + "\n", MessageType.Info);

                    EditorGUILayout.PropertyField(_frameSkip);
                    GUILayout.Space(5);
                }
                
                EditorGUILayout.PropertyField(_encodeTo);
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_advancedVideoSettings);

                if (_advancedVideoSettings.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n"
                                            + "Bitrate refers to how many bits can be transferred or processed within a certain amount of time. Default value = 10000000."
                                            + "\n", MessageType.Info);

                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_videoBitRate);
                    
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_useFrameDescriptorSrgb, new GUIContent("sRGB"));
                    
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_HDR);
                }
                
                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            //Audio
            if (GUILayout.Button("<b>Audio Settings</b>", button))
            {
                _audioSettingExpand.boolValue = !_audioSettingExpand.boolValue;
            }

            if (_audioSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                
                if (_recordMicrophone.boolValue)
                {
#if PLATFORM_IOS
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "For correct microphone recording on iOS devices, you need to enable the following settings in PlayerSettings -> Player -> Other Settings:" 
                                                 + "\n\n" 
                                                 + "Prepare iOS for Recording - Enable this option to initialize the microphone recording APIs. This lowers recording latency, but it also re-routes iPhone audio output via earphones."
                                                 + "\n\n"
                                                 + "Force iOS Speakers when Recording - Enable this option to send the phone’s audio output through the internal speakers, even when headphones are plugged in and recording."
                                                 + "\n", MessageType.Info);
#endif
                }

                if (_recordMicrophone.boolValue && (_recordAllAudioSources.boolValue || _recordOnlyOneAudioSource.boolValue))
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "If you're recording from a microphone and an audio source at the same time, we recommend that you reduce the main sound (such as music) to avoid echo." 
                                                 + "\n\n" 
                                                 + "This can happen when recording video on mobile devices where the speaker is near the microphone."
                                                 + "\n", MessageType.Info);
                }
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_recordMicrophone);
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_recordAllAudioSources);
                
                if (_recordAllAudioSources.boolValue)
                {
                    _recordOnlyOneAudioSource.boolValue = false;
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_audioListener);
                }
                
                GUILayout.Space(5);
                
                EditorGUILayout.PropertyField(_recordOnlyOneAudioSource);
                
                if (_recordOnlyOneAudioSource.boolValue)
                {
                    _recordAllAudioSources.boolValue = false;
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_targetAudioSource);
                }
                
                GUILayout.Space(10);
                GUILayout.Label("<b>Advanced:</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});
                GUILayout.Space(5);
                
                EditorGUILayout.PropertyField(_audioReceiverMixer);

                if (_audioReceiverMixer.boolValue)
                {
                    GUILayout.Space(5);

                    if (_recordAllAudioSources.boolValue || _recordMicrophone.boolValue ||
                        _recordOnlyOneAudioSource.boolValue)
                    {
                        EditorGUILayout.HelpBox(
                            "\n" + "If you are using Audio Receiver Mixer, the basic options *Record Microphone*, *Record All Audio Sources* and *Record Only One Audio Source* will be ignored. You can disable them. You need to add Audio Receiver to your audio source and add it to the list."
                                 + "\n", MessageType.Warning);
                    }

                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox(
                        "\n" + "Audio Receiver Mixer - This is a system that allows you to manually add any amount of audio to the recording. You can add any Audio Source, Audio Listener to use it."
                             + "\n\n"
                             + "Read more in the online documentation."
                             + "\n", MessageType.Info);
                    
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_audioReceivers);
                }
                
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_recordSeparateAudioFile);
                    
                if (_recordSeparateAudioFile.boolValue)
                {
                    GUILayout.Space(10);
                
                    GUILayout.Label("<b>Encode</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});

                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_separateAudioFileFormat);
                    GUILayout.Space(10);
                
                    //GUILayout.Label("<b>Path & Name</b>", new GUIStyle() {richText = true, alignment = TextAnchor.MiddleLeft, fontSize = 12, normal = { textColor = Color.white}});

                    //GUILayout.Space(10);
                    //EditorGUILayout.PropertyField(_separateAudioDataPath);
                    //GUILayout.Space(5);
                    //EditorGUILayout.PropertyField(_separateAudioFolderPath);
                    //GUILayout.Space(5);
                    //EditorGUILayout.PropertyField(_separateAudioFileName);
                    //GUILayout.Space(5);
                    //EditorGUILayout.PropertyField(_separateAudioDateTimeFormat);
                }
                
                GUILayout.Space(5);
                
                EditorGUILayout.PropertyField(_advancedAudioSettings);

                if (_advancedAudioSettings.boolValue)
                {
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n" + "Sample rate is the number of samples per second that are taken of a waveform to create a discete digital signal. The higher the sample rate, the more snapshots you capture of the audio signal. The audio sample rate is measured in kilohertz (kHz) and it determines the range of frequencies captured in digital audio. Default value = 48000." 
                                                 + "\n", MessageType.Info);
                
                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_sampleRate);
                    
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n"
                                                 + "The Channel count property is an enumerated value that describes how channels should be mapped between the inputs and outputs of the node. For example, 2 channels are for recording a stereo sound, 1 channel is for recording a mono sound. Default value = 2."
                                                 + "\n", MessageType.Info);

                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_channelCount);
                    
                    GUILayout.Space(5);
                    
                    EditorGUILayout.HelpBox("\n"
                                                 + "Bitrate is a term used to describe the amount of data transmitted in audio. The higher the bitrate, the better the sound quality. Default value = 96_000."
                                                 + "\n", MessageType.Info);

                    GUILayout.Space(5);
                    EditorGUILayout.PropertyField(_audioBitRate);
                }
                
                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            //Advanced
            if (GUILayout.Button("<b>Advanced Settings</b>", button))
            {
                _advancedSettingExpand.boolValue = !_advancedSettingExpand.boolValue;
            }

            if (_advancedSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_autoPauseResumeRecorder);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_verifyingBeforeInitialization);
                GUILayout.Space(10);
            }
            
            GUILayout.Space(10);
            
            //Editor
            if (GUILayout.Button("<b>Editor Recorder Settings</b>", button))
            {
                _editorSettingExpand.boolValue = !_editorSettingExpand.boolValue;
            }

            if (_editorSettingExpand.boolValue)
            {
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_targetEditorEncoder);
                GUILayout.Space(5);
                EditorGUILayout.PropertyField(_targetVideoBitrateMode);
                GUILayout.Space(5);
                if (_targetEditorEncoder.enumValueIndex == 0)
                {
                    EditorGUILayout.PropertyField(_targetH264VideoEncodingProfile);
                    GUILayout.Space(5);
                }
                EditorGUILayout.PropertyField(_targetEditorIncludeAlpha, new GUIContent("Include Alpha"));
                GUILayout.Space(10);
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
