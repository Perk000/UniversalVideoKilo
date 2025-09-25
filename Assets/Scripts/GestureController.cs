using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class GestureController : MonoBehaviour
{
    public GameObject arVideoPlayerPrefab;
    public TextMeshProUGUI gestureInfoText;

    private ARRaycastManager raycastManager;
    private GameObject currentVideoPlayer;
    private Vector2 initialTouchPosition;
    private float initialDistance;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Vector2 initialDirection;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        if (raycastManager == null)
        {
            Debug.LogError("GestureController: ARRaycastManager not found in scene. AR features will not work.");
        }
    }

    void Update()
    {
        // Check if any touch is over UI
        bool touchOverUI = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                touchOverUI = true;
                break;
            }
        }

        if (touchOverUI)
        {
            // Skip gesture processing if any touch is over UI
            return;
        }

        if (Input.touchCount == 1 && currentVideoPlayer == null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
                // Instantiate on plane
                Vector3 position = GetPlanePosition(touch.position);
                if (position == Vector3.zero)
                {
                    // Fallback: instantiate in front of camera if no plane detected
                    Camera mainCam = Camera.main;
                    if (mainCam != null)
                    {
                        position = mainCam.transform.position + mainCam.transform.forward * 2f;
                        gestureInfoText.text = "No plane detected, instantiating at camera position";
                    }
                    else
                    {
                        gestureInfoText.text = "No plane detected and no main camera found";
                        return;
                    }
                }
                // Get video orientation from PlayerPrefs and apply appropriate rotation on Z-axis
                int orientation = PlayerPrefs.GetInt("videoOrientation", 0);
                Quaternion rotation = Quaternion.Euler(0, 0, -orientation);
                currentVideoPlayer = Instantiate(arVideoPlayerPrefab, position, rotation);
                gestureInfoText.text = "Video instantiated at " + position + " (Z-rotation: " + orientation + "°)";
            }
        }
        else if (Input.touchCount == 1 && currentVideoPlayer != null)
        {
            // Single finger drag to move on horizontal plane
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                currentVideoPlayer.transform.Translate(delta.x * 0.01f, 0, delta.y * 0.01f, Space.World);
                gestureInfoText.text = "Dragging video player";
            }
        }
        else if (Input.touchCount == 2 && currentVideoPlayer != null)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch0.position, touch1.position);
                initialRotation = currentVideoPlayer.transform.rotation;
                initialScale = currentVideoPlayer.transform.localScale;
                initialPosition = currentVideoPlayer.transform.position;
                initialDirection = touch1.position - touch0.position;
                gestureInfoText.text = "Gesturing started";
            }

            // Pinch to scale controls (assuming controls are part of the prefab, scale the UI canvas or controls)
            // For now, scale the whole prefab; to scale controls only, assign a specific transform
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);
            float scaleFactor = currentDistance / initialDistance;
            currentVideoPlayer.transform.localScale = initialScale * scaleFactor;

            // Rotate on Z-axis for video orientation
            Vector2 currentDirection = touch1.position - touch0.position;
            float angleDiff = Vector2.SignedAngle(initialDirection, currentDirection);
            currentVideoPlayer.transform.rotation = initialRotation * Quaternion.Euler(0, 0, angleDiff);

            gestureInfoText.text = $"Scale: {scaleFactor:F2}, Z-Rotation: {angleDiff:F2}";
        }
        else if (Input.touchCount == 0)
        {
            gestureInfoText.text = "No gesture";
        }
    }

    Vector3 GetPlanePosition(Vector2 screenPosition)
    {
        if (raycastManager == null)
        {
            Debug.LogWarning("GestureController: ARRaycastManager is null, cannot detect planes.");
            return Vector3.zero;
        }
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            return hits[0].pose.position;
        }
        return Vector3.zero;
    }
}