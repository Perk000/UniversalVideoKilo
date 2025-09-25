using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SilverTau.NSR.Graphic
{
    [CustomEditor(typeof(GraphicSettings))]
    public class GraphicSettingsEditor : Editor
    {
        private GraphicSettings _target;
        
        protected GUIStyle helpBox;
        protected GUIStyle labelHeader;
        protected GUIStyle labelHeader2;
        protected GUIStyle labelHeader3;
        protected Color colorDarkBlue;
        protected Color colorBlack;
        protected Color colorDodgerBlue;
        
        private SerializedProperty _screenshotLayerMasks;
        private SerializedProperty _imageQuality;
        private SerializedProperty _filePath;
        
        private SerializedProperty _applicationDataPath;
        private SerializedProperty _encodeTo;
        private SerializedProperty _HDR;
        
        private SerializedProperty _addDateTimeToGraphicName;
        
        private SerializedProperty _dateTimeFormat;
        
        private SerializedProperty _graphicName;
        private SerializedProperty _autoClearMemory;
        
        private SerializedProperty _autoSaveSharedGraphic;
        private SerializedProperty _deleteImageAfterSave;
        
        private SerializedProperty _imageSize;
        
        private SerializedProperty _changeImageSizeIfWidthMore;
        
        private SerializedProperty _changeImageSizeIfHeightMore;
        
        private void Awake()
        {
            if (target) _target = (GraphicSettings)target;
            
            ColorUtility.TryParseHtmlString("#212121", out colorBlack);
            ColorUtility.TryParseHtmlString("#174ead", out colorDodgerBlue);
            ColorUtility.TryParseHtmlString("#122d4a", out colorDarkBlue);
            
            labelHeader = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 26.0f,
                richText = true,
                fontSize = 14,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorBlack),
                    textColor = Color.white
                }
            };
            
            labelHeader2 = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 30.0f,
                richText = true,
                fontSize = 14,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorDodgerBlue),
                    textColor = Color.white
                }
            };
            
            labelHeader3 = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 30.0f,
                richText = true,
                fontSize = 16,
                normal = new GUIStyleState
                {
                    background = CreateTexture2D(2, 2, colorBlack),
                    textColor = Color.white
                }
            };
        }

        private Texture2D CreateTexture2D(int width, int height, Color col)
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
            
            _screenshotLayerMasks = serializedObject.FindProperty("screenshotLayerMasks");
            _imageQuality = serializedObject.FindProperty("imageQuality");
            _filePath = serializedObject.FindProperty("filePath");
            
            _applicationDataPath = serializedObject.FindProperty("applicationDataPath");
            _encodeTo = serializedObject.FindProperty("encodeTo");
            _HDR = serializedObject.FindProperty("HDR");
            
            _addDateTimeToGraphicName = serializedObject.FindProperty("addDateTimeToGraphicName");
            
            _dateTimeFormat = serializedObject.FindProperty("dateTimeFormat");
            
            _graphicName = serializedObject.FindProperty("graphicName");
            _autoClearMemory = serializedObject.FindProperty("autoClearMemory");
            
            _autoSaveSharedGraphic = serializedObject.FindProperty("autoSaveSharedGraphic");
            _deleteImageAfterSave = serializedObject.FindProperty("deleteImageAfterSave");
            
            _imageSize = serializedObject.FindProperty("imageSize");
            
            _changeImageSizeIfWidthMore = serializedObject.FindProperty("changeImageSizeIfWidthMore");
            
            _changeImageSizeIfHeightMore = serializedObject.FindProperty("changeImageSizeIfHeightMore");
            
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(10);
            
            GUILayout.Box(new GUIContent("<b>Graphic Settings</b>", EditorGUIUtility.IconContent("GameObject On Icon").image), labelHeader3);
            GUILayout.Space(5);
            
            GUILayout.Box(new GUIContent("\n" 
                                         + "<b>Graphic Settings</b> is a script object that allows you to customize the output image file. "
                                         + "You can change the settings at runtime and for different tasks."
                                         + "\n", EditorGUIUtility.IconContent("console.infoicon").image), helpBox);
            
            
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_screenshotLayerMasks);
            GUILayout.Space(5);
            
            GUILayout.Box(new GUIContent("<b>Output File</b>", EditorGUIUtility.IconContent("d_Preset.Context@2x").image), labelHeader);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_filePath);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_applicationDataPath);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_encodeTo);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_HDR);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_imageQuality);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_addDateTimeToGraphicName);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_dateTimeFormat);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_graphicName);
            GUILayout.Space(5);
            
            GUILayout.Box(new GUIContent("<b>Additional settings</b>", EditorGUIUtility.IconContent("d_Preset.Context@2x").image), labelHeader);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_autoClearMemory);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_autoSaveSharedGraphic);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_deleteImageAfterSave);
            GUILayout.Space(5);
                
            GUILayout.Box(new GUIContent("<b>XR</b>", EditorGUIUtility.IconContent("d_Preset.Context@2x").image), labelHeader);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_imageSize);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_changeImageSizeIfHeightMore);
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(_changeImageSizeIfWidthMore);
            
            //base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}