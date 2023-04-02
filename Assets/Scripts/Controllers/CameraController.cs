using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public enum CameraState
    {
        FIRST_PERSON,
        THIRD_PERSON,
        TRANSITION
    }
    
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Renderer characterRenderer;

    [Header("Camera Config")]
    [SerializeField] private float sensitivityX = 100f;
    [SerializeField] private float sensitivityY = 100f;
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float sphereRayRadius = 0.2f;
    [SerializeField] private LayerMask collisionLayers;

    private CameraState cameraState;
    private Vector3 cameraOffset;
    private Vector3 followVelocity;
    private float rotVertical;
    private float rotHorizontal;
    private float goalZoom;
    private float maxCollisionZoom;
    private float zoomVelocity;
    private bool isColliding;

    private readonly float minCameraAngle = -85f;
    private readonly float maxCameraAngle = 85f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        maxCollisionZoom = maxZoom;
        cameraOffset = cameraPivot.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && cameraState != CameraState.TRANSITION)
        {
            SwitchViewType();
        }
        
        RotateCamera();
    }

    private void FixedUpdate()
    {
        if (cameraState != CameraState.FIRST_PERSON)
            CameraCollisions();
    }

    private void LateUpdate()
    {
        if (cameraState != CameraState.FIRST_PERSON)
            CameraFollowCharacter();
    }

    private void HandleZoom()
    {
        goalZoom = Mathf.Clamp(goalZoom - Input.mouseScrollDelta.y, minZoom, maxZoom);
        var targetZoom = Mathf.Clamp(goalZoom, minZoom, maxCollisionZoom);
        var smoothZoom = Mathf.SmoothDamp(cameraTransform.localPosition.z, -targetZoom, ref zoomVelocity, zoomSpeed);

        cameraTransform.localPosition = new Vector3(0f, 0f, smoothZoom);
    }

    // TODO: when going from Third person view to first person, move pivot to head.
    private void CameraFollowCharacter()
    {
        var targetPosition = characterTransform.position + cameraOffset;
        var smoothFollow = Vector3.SmoothDamp(cameraPivot.position, targetPosition, ref followVelocity, followSpeed);

        cameraPivot.position = smoothFollow;

        if (cameraState == CameraState.THIRD_PERSON)
            HandleZoom();
    }

    private void SwitchViewType()
    {
        if (cameraState == CameraState.FIRST_PERSON)
        {
            cameraState = CameraState.TRANSITION;
            
            cameraPivot.SetParent(null);
            rotHorizontal = transform.eulerAngles.y;
            goalZoom = maxZoom;

            characterRenderer.enabled = true;

            cameraTransform.localPosition = new Vector3(0f, 0f, -0.1f);
            cameraTransform.DOLocalMove(new Vector3(0f, 0f, -goalZoom), 0.4f)
                .SetEase(Ease.InOutExpo)
                .OnComplete(() =>
                {
                    cameraState = CameraState.THIRD_PERSON;
                });
        }
        else
        {
            cameraState = CameraState.TRANSITION;
            
            cameraTransform.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.InOutExpo).OnComplete(() =>
            {
                cameraState = CameraState.FIRST_PERSON;
                
                // TODO: smoothly rotate character to camera direction.
                var lookVector = cameraTransform.forward;
                lookVector.y = 0;
                
                characterTransform.localRotation = Quaternion.LookRotation(lookVector);
                characterRenderer.enabled = false;
                
                cameraPivot.SetParent(characterTransform);
                cameraPivot.localPosition = cameraOffset;
                cameraPivot.localRotation = Quaternion.Euler(0, 0, 0);
            });
        }
    }

    private void RotateCamera()
    {
        var horizontalInput = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        var verticalInput = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        rotHorizontal += horizontalInput;
        rotVertical -= verticalInput;
        rotVertical = Mathf.Clamp(rotVertical, minCameraAngle, maxCameraAngle);
        
        if (cameraState == CameraState.FIRST_PERSON)
        {
            cameraPivot.localRotation = Quaternion.Euler(rotVertical, 0, 0);
            characterTransform.Rotate(Vector3.up * horizontalInput);
        }
        else
        {
            cameraPivot.localRotation = Quaternion.Euler(rotVertical, rotHorizontal, 0);
        }
    }

    public Vector3 GetCameraDirectionNormalized()
    {
        var dir = (cameraPivot.position - cameraTransform.position);
        dir.y = 0;

        return dir.normalized;
    }

    public bool IsInFirstPerson()
    {
        return cameraState == CameraState.FIRST_PERSON;
    }
    
    private void CameraCollisions()
    {
        var dirVector = cameraTransform.position - cameraPivot.position;
        var ray = new Ray(cameraPivot.position, dirVector.normalized);

        isColliding = Physics.SphereCast(ray, sphereRayRadius, out var hitData, goalZoom, collisionLayers);
        
        if (isColliding)
        {
            maxCollisionZoom = hitData.distance < maxZoom ? hitData.distance : maxZoom;

            cameraTransform.localPosition = new Vector3(0f, 0f, -hitData.distance);
        }
        else
        {
            maxCollisionZoom = maxZoom;
        }
    }
}
