using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

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
    [SerializeField] private float minLandTime = 0.5f;
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

    private const float DEFAULT_WALKSPEED = 2f; // 2f is the speed where feet don't slide. Use this to adjust walk animation speed.
    private const float SPEED_RECOVERY_VALUE = 1f;

    private readonly int ANIM_WALKSPEED_PARAM = Animator.StringToHash("WalkSpeed");
    private readonly int ANIM_GROUNDED_PARAM = Animator.StringToHash("isGrounded");
    private readonly int ANIM_FALLING_PARAM = Animator.StringToHash("isFalling");
    private readonly int ANIM_MOVING_PARAM = Animator.StringToHash("isMoving");
    private readonly int ANIM_LANDED_PARAM = Animator.StringToHash("Landed");
    private readonly int ANIM_JUMP_PARAM = Animator.StringToHash("Jump");
    private readonly int ANIM_DIRECTION_X_PARAM = Animator.StringToHash("directionX");
    private readonly int ANIM_DIRECTION_Y_PARAM = Animator.StringToHash("directionY");
    
    private void Start()
    {
        currentDirection = new Vector2(transform.forward.x, transform.forward.z);
    }
    
    private void Update()
    {
        DoMovement();
    }

    private void DoMovement()
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
                
                characterAnimator.SetTrigger(ANIM_LANDED_PARAM);
            }

            if (playerVelocity.y < 0)
                playerVelocity.y = gravityValue * 0.1f;
        }

        if (!isGrounded && wasGrounded)
            timeSinceUngrounded = Time.time;

        isFalling = CheckIsFalling();

        characterAnimator.SetBool(ANIM_GROUNDED_PARAM, isGrounded);
        
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        movementInput = new Vector2(horizontalInput, verticalInput);

        if (cameraController.IsInFirstPerson())
        {
            currentDirection = movementInput;
        }
        else
        {
            currentDirection = Vector2.Lerp(currentDirection, movementInput, turnSpeed * Time.deltaTime);
        }

        CalculateMoveDirection(out Vector3 moveDir, horizontalInput, verticalInput);
        
        HandleRun();

        // TODO: just normalize moveDir.
        var movementVector = moveDir * Time.deltaTime * currentWalkspeed * speedMultiplier; // Vector3.ClampMagnitude(moveDir * Time.deltaTime * currentWalkspeed * speedMultiplier, Time.deltaTime * currentWalkspeed * speedMultiplier);
        charController.Move(movementVector);

        HandleJump();
        AnimateMovement();
        
        wasGrounded = isGrounded;
    }

    private bool CheckIsFalling() => !isGrounded && Time.time - timeSinceUngrounded > minLandTime;

    private void CalculateMoveDirection(out Vector3 moveDir, float horizontalInput, float verticalInput)
    {
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
        
        moveDir.Normalize();
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
            
            characterAnimator.SetTrigger(ANIM_JUMP_PARAM);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        charController.Move(playerVelocity * Time.deltaTime);
    }

    private void AnimateMovement()
    {
        characterAnimator.SetBool(ANIM_FALLING_PARAM, isFalling);
        characterAnimator.SetBool(ANIM_MOVING_PARAM, movementInput.magnitude > 0f);
            
        // Adjust WalkSpeed parameter so that feet don't slide.
        var walkspeed = (currentWalkspeed * speedMultiplier) / DEFAULT_WALKSPEED;
        characterAnimator.SetFloat(ANIM_WALKSPEED_PARAM, walkspeed);
        characterAnimator.SetFloat(ANIM_DIRECTION_X_PARAM, currentDirection.x);
        characterAnimator.SetFloat(ANIM_DIRECTION_Y_PARAM, currentDirection.y);
    }
}
