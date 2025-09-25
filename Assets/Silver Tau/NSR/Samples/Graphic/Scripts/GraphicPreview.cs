using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Graphic
{
    public class GraphicPreview : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool debugInfo;
        
        [Header("Common")]
        [SerializeField] private GraphicProvider graphicProvider;
        [SerializeField] private RawImage image;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        
        [Header("Full screen view")]
        [SerializeField] private Button buttonOpenFullScreenView;
        [SerializeField] private Button buttonCloseFullScreenView;
        [SerializeField] private GameObject fullScreenView;
        [SerializeField] private RawImage fullScreenImage;
        [SerializeField] private AspectRatioFitter fullScreenRatioFitter;

        private float _ratio;
        
        private void Start()
        {
            buttonOpenFullScreenView.onClick.AddListener(OpenFullScreenView);
            buttonCloseFullScreenView.onClick.AddListener(CloseFullScreenView);
        }

        private void OnEnable()
        {
            graphicProvider.OnCreateImage += OnCreateImage;
        }
        
        private void OnDisable()
        {
            graphicProvider.OnCreateImage -= OnCreateImage;
        }

        private void OnCreateImage()
        {
            if(graphicProvider.GraphicSubsystems == null || graphicProvider.GraphicSubsystems.Count == 0) return;
            var graphicSubsystem = graphicProvider.GraphicSubsystems.First();
            
            if (graphicSubsystem == null)
            {
                Debug.Log("Graphic subsystem is missing.");
                return;
            }
            
            var sharedGraphic = graphicSubsystem.sharedGraphic;

            if (sharedGraphic == null)
            {
                Debug.Log("Shared Graphic is missing.");
                return;
            }
            
            image.texture = sharedGraphic.texture;
            var width = image.texture.width;
            var height = image.texture.height;

            _ratio = width / (float)height;
            aspectRatioFitter.aspectRatio = _ratio;

            if(!debugInfo) return;
            
            graphicSubsystem.OnSharedGraphicSaved = path =>
            {
                // Output file path => path
                WriteGraphicInfo(sharedGraphic);
            };
        }

        private void OpenFullScreenView()
        {
            if(fullScreenView == null) return;
            if(image.texture == null) return;

            fullScreenImage.texture = image.texture;
            fullScreenRatioFitter.aspectRatio = _ratio;
            
            fullScreenView.SetActive(true);
        }

        private void CloseFullScreenView()
        {
            if(fullScreenView == null) return;
            fullScreenView.SetActive(false);
        }

        private void WriteGraphicInfo(SharedGraphic sharedGraphic)
        {
            if (sharedGraphic == null)
            {
                Debug.Log("Shared Graphic is missing.");
                return;
            }
            
            Debug.Log(string.Format("Shared Graphic: {0}", sharedGraphic.id));
            Debug.Log(string.Format("Name: {0}", sharedGraphic.name));
            Debug.Log(string.Format("Output path: {0}", sharedGraphic.outputPath));
        }
    }
}
