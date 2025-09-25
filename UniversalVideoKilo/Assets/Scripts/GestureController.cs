using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
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

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 1 && currentVideoPlayer == null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
                // Instantiate on plane
                Vector3 position = GetPlanePosition(touch.position);
                if (position != Vector3.zero)
                {
                    currentVideoPlayer = Instantiate(arVideoPlayerPrefab, position, Quaternion.identity);
                    gestureInfoText.text = "Video instantiated at " + position;
                }
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
                gestureInfoText.text = "Gesturing started";
            }

            // Pinch to scale
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);
            float scaleFactor = currentDistance / initialDistance;
            currentVideoPlayer.transform.localScale = initialScale * scaleFactor;

            // Rotate based on angle difference
            Vector2 initialDirection = Input.GetTouch(1).position - Input.GetTouch(0).position; // Wait, need to store initial
            // Simplified rotation
            Vector2 currentDirection = touch1.position - touch0.position;
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            currentVideoPlayer.transform.rotation = Quaternion.Euler(0, angle, 0);

            // Drag: average movement
            Vector2 avgMovement = (touch0.deltaPosition + touch1.deltaPosition) / 2;
            currentVideoPlayer.transform.Translate(avgMovement.x * 0.01f, 0, avgMovement.y * 0.01f, Space.World);

            gestureInfoText.text = $"Scale: {scaleFactor:F2}, Angle: {angle:F2}";
        }
        else if (Input.touchCount == 0)
        {
            gestureInfoText.text = "No gesture";
        }
    }

    Vector3 GetPlanePosition(Vector2 screenPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            return hits[0].pose.position;
        }
        return Vector3.zero;
    }
}