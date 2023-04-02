using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController charController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Animator characterAnimator;
    
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float minLandHeight = 0.5f;
    [SerializeField] private float walkspeedAcceleration = 2f;

    [SerializeField] private Vector3 playerVelocity;
    [SerializeField][ReadOnly] private bool isGrounded;

    private bool wasGrounded;
    private bool isFalling;
    private float currentWalkspeed;
    private Vector2 movementInput;
    private Vector2 currentDirection;
    private float speedMultiplier;
    private float timeSinceJump;
    private float timeSinceUngrounded;
    private float jumpStartY;

    private const float DEFAULT_WALKSPEED = 2f; // 2f is the speed where feet don't slide. Use this to adjust walk animation speed.
    private const float SPEED_RECOVERY_VALUE = 1f;

    private void Start()
    {
        currentDirection = new Vector2(transform.forward.x, transform.forward.z);
    }
    
    private void Update()
    {
        if (speedMultiplier < 1f)
            speedMultiplier += Time.deltaTime * SPEED_RECOVERY_VALUE;

        speedMultiplier = Mathf.Clamp01(speedMultiplier);
        
        // TODO: fix isGrounded when jump on platform and is sliding. Landing animation is too slow.
        
        isGrounded = charController.isGrounded;
        if (isGrounded)
        {
            if (!wasGrounded)
            {
                wasGrounded = true;
                
                if (isFalling)
                    speedMultiplier = 0.1f;
                
                characterAnimator.SetTrigger("Landed");
            }

            if (playerVelocity.y < 0)
                playerVelocity.y = gravityValue * 0.1f;
        }

        if (!isGrounded && wasGrounded)
            timeSinceUngrounded = Time.time;

        isFalling = !isGrounded && Time.time - timeSinceUngrounded > 0.4f;

        characterAnimator.SetBool("isGrounded", isGrounded);
        
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        movementInput = new Vector2(horizontalInput, verticalInput);

        currentDirection = Vector2.Lerp(currentDirection, movementInput, turnSpeed * Time.deltaTime);
        
        Vector3 moveDir;
        if (cameraController.IsInFirstPerson())
        {
            moveDir = transform.right * horizontalInput + transform.forward * verticalInput;
        }
        else
        {
            var cameraDirection = cameraController.GetCameraDirectionNormalized();
            var directionRight = Quaternion.AngleAxis(90, Vector3.up) * cameraDirection;
            var goalRotation = Quaternion.LookRotation(cameraDirection, Vector3.up);

            moveDir = directionRight * horizontalInput + cameraDirection * verticalInput;

            if (movementInput.magnitude > 0f)
                transform.rotation = Quaternion.Lerp(transform.rotation, goalRotation, turnSpeed * Time.deltaTime);
        }
        
        HandleRun();

        // TODO: just normalize moveDir.
        var movementVector = Vector3.ClampMagnitude(moveDir * Time.deltaTime * currentWalkspeed * speedMultiplier, Time.deltaTime * currentWalkspeed * speedMultiplier);
        charController.Move(movementVector);

        HandleJump();
        AnimateMovement();
        
        wasGrounded = isGrounded;
    }
    
    private void HandleRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentWalkspeed = Mathf.MoveTowards(currentWalkspeed, movementSpeed * 2f, Time.deltaTime * walkspeedAcceleration);
        }
        else
        {
            currentWalkspeed = Mathf.MoveTowards(currentWalkspeed, movementSpeed, Time.deltaTime * walkspeedAcceleration);
        }
    }

    private void HandleJump()
    {
        var canJump = Time.time - timeSinceJump > jumpCooldown;
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            timeSinceJump = Time.time;
            jumpStartY = transform.position.y;
            
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            
            characterAnimator.SetTrigger("Jump");
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        charController.Move(playerVelocity * Time.deltaTime);
    }

    private void AnimateMovement()
    {
        characterAnimator.SetBool("isFalling", isFalling);
        characterAnimator.SetBool("isMoving", movementInput.magnitude > 0f);
            
        // Adjust WalkSpeed parameter so that feet don't slide.
        characterAnimator.SetFloat("WalkSpeed", (currentWalkspeed * speedMultiplier) / DEFAULT_WALKSPEED);
        characterAnimator.SetFloat("directionX", currentDirection.x);
        characterAnimator.SetFloat("directionY", currentDirection.y);
    }
}
