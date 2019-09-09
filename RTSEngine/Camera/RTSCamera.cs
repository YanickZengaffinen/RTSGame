using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField]
    private KeyCode rotateKey = KeyCode.Mouse0;
    [SerializeField]
    private float rotationSpeed = 100;

    private Vector3? lastMousePos;

    void Update()
    {
        if(lastMousePos != null)
        {
            HandleRotation();
        }

        lastMousePos = Input.mousePosition;
    }

    private void HandleRotation()
    {
        float rotationDelta = (Input.mousePosition.x - lastMousePos.Value.x) / Screen.width;

        if (Input.GetKey(rotateKey))
        {
            transform.Rotate(Vector3.up, rotationDelta * rotationSpeed * Time.deltaTime);
        }
    }
}
