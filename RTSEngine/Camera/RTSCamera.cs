using System.Collections;
using System.Collections.Generic;
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
    private float autoScrollBaseSpeed = 10000;
    [SerializeField]
    private AnimationCurve autoScrollSpeedCurve;

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
    }

    private void HandleRotation()
    {
        bool rotationKeyPressed = Input.GetKey(rotationKey);
        if(rotationLastMousePosition.HasValue)
        {
            float rotationDelta = (Input.mousePosition.x - rotationLastMousePosition.Value.x) / Screen.width;

            if (rotationKeyPressed)
            {
                transform.Rotate(Vector3.up, rotationDelta * rotationSpeed * Time.deltaTime);
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
        float currentY = Input.mousePosition.y / Screen.height;

        float deltaX = 0;
        float deltaY = 0;

        // x
        if(currentX <= autoScrollScreenBorderThreshold)
        {
            deltaX -= autoScrollSpeedCurve.Evaluate(1.0f - currentX / autoScrollScreenBorderThreshold);
        }

        if(currentX >= 1.0f - autoScrollScreenBorderThreshold)
        {
            deltaX += autoScrollSpeedCurve.Evaluate(1.0f - (1.0f - currentX) / autoScrollScreenBorderThreshold);
        }

        // y
        if (currentY <= autoScrollScreenBorderThreshold)
        {
            deltaY -= autoScrollSpeedCurve.Evaluate(1.0f - currentY / autoScrollScreenBorderThreshold);
        }

        if (currentY >= 1.0f - autoScrollScreenBorderThreshold)
        {
            deltaY += autoScrollSpeedCurve.Evaluate(1.0f - (1.0f - currentY) / autoScrollScreenBorderThreshold);
        }

        transform.position += transform.TransformVector(new Vector3(deltaX, 0, deltaY) * autoScrollBaseSpeed) * Time.deltaTime;
    }
}
