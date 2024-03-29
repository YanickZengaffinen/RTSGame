﻿using System;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField]
    private bool rotationEnabled = true;
    [SerializeField]
    private KeyCode rotationKey = KeyCode.Mouse0;
    [SerializeField]
    private float rotationSpeed = 10000;

    private Vector3? rotationLastMousePosition;

    [Header("Autoscroll")]
    [SerializeField]
    private bool autoScrollEnabled = true;
    [SerializeField, Range(0f, 0.5f)]
    private float autoScrollScreenBorderThreshold = 0.1f; //autoscrolling will be on whenever the mouse is 0.x away from the screen border
    [SerializeField]
    private float autoScrollBaseSpeed = 25;
    [SerializeField]
    private AnimationCurve autoScrollSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Moving")]
    [SerializeField]
    private bool movingEnabled = true;
    [SerializeField]
    private KeyCodeGroup movingLeft = new KeyCodeGroup(KeyCode.A, KeyCode.LeftArrow);
    [SerializeField]
    private KeyCodeGroup movingForward = new KeyCodeGroup(KeyCode.W, KeyCode.UpArrow);
    [SerializeField]
    private KeyCodeGroup movingRight = new KeyCodeGroup(KeyCode.D, KeyCode.RightArrow);
    [SerializeField]
    private KeyCodeGroup movingBackward = new KeyCodeGroup(KeyCode.S, KeyCode.DownArrow);
    [SerializeField]
    private float movingBaseSpeed = 25;
    [SerializeField]
    private AnimationCurve movingSpeedCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    private float movingTimeUntilMaxSpeed = 3; // in seconds

    private float movingTimeSinceStart = 0;

    [Header("Ground Distance")]
    [SerializeField]
    private bool groundDistanceEnabled = true;
    [SerializeField]
    private float maxWorldHeight = 10000;
    [SerializeField]
    private float groundDistanceMin = 10f;
    [SerializeField]
    private float groundDistanceMinHard = 5f;
    [SerializeField]
    private float groundDistanceAdjustSpeed = 2f;
    [SerializeField]
    private LayerMask groundDistanceLayer = ~0;

    [Header("Zooming")]
    [SerializeField]
    private bool zoomingEnabled = true;
    [SerializeField]
    private float zoomingBaseSpeed = 100f;
    [SerializeField]
    private AnimationCurve zoomingSpeedCurve = AnimationCurve.Constant(0, 1, 1);

    private float currentPreferredHeight;

    [SerializeField]
    private float zoomingMax = 100f;

    private void Start()
    {
        currentPreferredHeight = this.transform.position.y;
    }

    void Update()
    {
        if (rotationEnabled)
        {
            HandleRotation();
        }

        if (autoScrollEnabled)
        {
            HandleAutoscroll();
        }

        if(movingEnabled)
        {
            HandleMoving();
        }

        if (zoomingEnabled)
        {
            HandleZooming();
        }

        if (groundDistanceEnabled)
        {
            HandleGroundDistance();
        }
    }

    private void HandleRotation()
    {
        bool rotationKeyPressed = Input.GetKey(rotationKey);
        if(rotationLastMousePosition.HasValue)
        {
            float rotationDelta = (Input.mousePosition.x - rotationLastMousePosition.Value.x) / Screen.width;

            if (rotationKeyPressed)
            {
                transform.Rotate(Vector3.up, rotationDelta * rotationSpeed * Time.deltaTime, Space.World);
            }
        }

        if(rotationKeyPressed)
        {
            rotationLastMousePosition = Input.mousePosition;
        }
        else
        {
            rotationLastMousePosition = null;
        }
    }

    private void HandleAutoscroll()
    {
        float currentX = Input.mousePosition.x / Screen.width;
        float currentZ = Input.mousePosition.y / Screen.height;

        float deltaX = 0;
        float deltaZ = 0;

        // x
        if(currentX <= autoScrollScreenBorderThreshold)
        {
            deltaX -= autoScrollSpeedCurve.Evaluate(1.0f - currentX / autoScrollScreenBorderThreshold);
        }

        if(currentX >= 1.0f - autoScrollScreenBorderThreshold)
        {
            deltaX += autoScrollSpeedCurve.Evaluate(1.0f - (1.0f - currentX) / autoScrollScreenBorderThreshold);
        }

        // z
        if (currentZ <= autoScrollScreenBorderThreshold)
        {
            deltaZ -= autoScrollSpeedCurve.Evaluate(1.0f - currentZ / autoScrollScreenBorderThreshold);
        }

        if (currentZ >= 1.0f - autoScrollScreenBorderThreshold)
        {
            deltaZ += autoScrollSpeedCurve.Evaluate(1.0f - (1.0f - currentZ) / autoScrollScreenBorderThreshold);
        }

        transform.position += Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)) * new Vector3(deltaX, 0, deltaZ) 
            * autoScrollBaseSpeed * Time.deltaTime;
    }

    private void HandleMoving()
    {
        float deltaX = 0;
        float deltaZ = 0;

        // x
        if(movingLeft.Any())
        {
            deltaX -= 1;
        }

        if(movingRight.Any())
        {
            deltaX += 1;
        }

        // z
        if(movingForward.Any())
        {
            deltaZ += 1;
        }

        if(movingBackward.Any())
        {
            deltaZ -= 1;
        }

        if(deltaX != 0 || deltaZ != 0)
        {
            transform.position += Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0)) * new Vector3(deltaX, 0, deltaZ) 
                * movingBaseSpeed * movingSpeedCurve.Evaluate(movingTimeSinceStart / movingTimeUntilMaxSpeed) * Time.deltaTime;

            if(movingTimeSinceStart < movingTimeUntilMaxSpeed)
            {
                movingTimeSinceStart = Math.Min(movingTimeSinceStart + Time.deltaTime, movingTimeUntilMaxSpeed);
            }
        }
        else
        {
            movingTimeSinceStart = Math.Max(movingTimeSinceStart - Time.deltaTime, 0);
        }
    }

    private void HandleGroundDistance()
    {
        float groundHeight = GetHeightAt(transform.position.x, transform.position.z);
        float heightDelta = transform.position.y - groundHeight;
        if (heightDelta < groundDistanceMin)
        {
            if (heightDelta > groundDistanceMinHard)
            {
                float newHeight = Mathf.Lerp(transform.position.y, groundHeight + groundDistanceMin, groundDistanceAdjustSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, groundHeight + groundDistanceMinHard, transform.position.z);
            }
        }
        else if(transform.position.y != currentPreferredHeight)
        {
            float newHeight = Mathf.Lerp(transform.position.y, currentPreferredHeight, groundDistanceAdjustSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        }
    }

    private void HandleZooming()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        // zooming out
        if(scrollDelta < 0)
        {
            float groundHeight = GetHeightAt(transform.position.x, transform.position.z);
            float newHeight = Math.Min(transform.position.y + Math.Abs(scrollDelta) * zoomingBaseSpeed * zoomingSpeedCurve.Evaluate(transform.position.y / (maxWorldHeight - groundHeight) ), zoomingMax);
            currentPreferredHeight = newHeight;
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        }
        // zooming in
        else if (scrollDelta > 0)
        {
            float groundHeight = GetHeightAt(transform.position.x, transform.position.z);
            float newHeight = Math.Max(transform.position.y - Math.Abs(scrollDelta) * zoomingBaseSpeed * zoomingSpeedCurve.Evaluate(transform.position.y / (maxWorldHeight - groundHeight)), groundHeight + groundDistanceMin);
            currentPreferredHeight = newHeight;
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        }
    }

    private float GetHeightAt(float x, float z)
    {
        var ray = new Ray(new Vector3(x, maxWorldHeight, z), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, groundDistanceLayer))
        {
            return hit.point.y;
        }

        return 0;
    }
}
