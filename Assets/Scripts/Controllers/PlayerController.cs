using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    // Public fields
    
    [SerializeField] private CharacterController charController;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Renderer characterRenderer;
    [SerializeField] private Animator characterAnimator;
    
    [SerializeField] private float movementSpeed; 
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float minFallVelocity = -9f;
    [SerializeField] private float walkspeedAcceleration = 2f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private LayerMask groundedMask;
    
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField][ReadOnly] private bool isGrounded;

    // Properties

    public Transform CharacterTransform => characterTransform;
    public Renderer CharacterRenderer => characterRenderer;

    // Private fields

    private CameraController cameraController;
    private List<IPlayerPlugin> playerPlugins;
    private bool isFalling;
    private float currentWalkspeed;
    private Vector2 movementInput;
    private Vector2 currentDirection;
    private float speedMultiplier;
    private float timeSinceJump;
    private float lastLandingVelocity;
    private bool hasLanded;

    private bool isInitialized;

    // Constants
    
    private const float DEFAULT_WALKSPEED = 2f; // 2f is the speed where feet don't slide. Use this to adjust walk animation speed.
    private const float SPEED_RECOVERY_VALUE = 1f;

    // Animation parameters
    
    private readonly int ANIM_WALKSPEED_PARAM = Animator.StringToHash("WalkSpeed");
    private readonly int ANIM_GROUNDED_PARAM = Animator.StringToHash("isGrounded");
    private readonly int ANIM_MOVING_PARAM = Animator.StringToHash("isMoving");
    private readonly int ANIM_LANDED_PARAM = Animator.StringToHash("Landed");
    private readonly int ANIM_HARD_LANDED_PARAM = Animator.StringToHash("HardLanded");
    private readonly int ANIM_JUMP_PARAM = Animator.StringToHash("Jump");
    private readonly int ANIM_DIRECTION_X_PARAM = Animator.StringToHash("directionX");
    private readonly int ANIM_DIRECTION_Y_PARAM = Animator.StringToHash("directionY");

    // PlayerController
    
    public void Init()
    {
        cameraController = CameraController.Instance;
        currentDirection = new Vector2(transform.forward.x, transform.forward.z);

        GetPlugins();
        InitPlugins();
        
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        DoMovement();
    }

    public void DoMovement()
    {
        if (speedMultiplier < 1f)
            speedMultiplier += Time.deltaTime * SPEED_RECOVERY_VALUE;

        speedMultiplier = Mathf.Clamp01(speedMultiplier);
        
        // TODO: CLEAN UP CODE AND MAKE IT MORE READABLE/UNDERSTANDABLE/OPTIMIZED.
        
        CheckGroundedState();

        characterAnimator.SetBool(ANIM_GROUNDED_PARAM, isGrounded);
        
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput =Input.GetAxisRaw("Vertical");
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
        
        var movementVector = moveDir * Time.deltaTime * currentWalkspeed * speedMultiplier;
        charController.Move(movementVector);

        HandleJump();
        AnimateMovement();
    }

    private void FixedUpdate()
    {
        if (hasLanded || isGrounded) return;

        if (IsLandingNextPhysicsFrame())
        {
            lastLandingVelocity = charController.velocity.y;

            hasLanded = true;
        }
    }

    private void CheckGroundedState()
    {
        isGrounded = IsGrounded();
        
        CheckIfLanded();
        
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = gravityValue * 0.15f;
        }
    }

    private void CheckIfLanded()
    {
        if (hasLanded)
        {
            if (IsHardLanding())
            {
                speedMultiplier = 0.1f;
                    
                characterAnimator.SetTrigger(ANIM_HARD_LANDED_PARAM);
            }
            else
            {
                characterAnimator.SetTrigger(ANIM_LANDED_PARAM);
            }
            
            hasLanded = false;
        }
    }

    private bool IsLandingNextPhysicsFrame()
    {
        if (charController.velocity.y > 0f) return false;
        
        var startPosition = characterAnimator.transform.position;
        var dist = Mathf.Abs(charController.velocity.y * Time.fixedDeltaTime);

        return Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, dist, groundedMask);
    }
    
    private bool IsHardLanding()
    {
        return hasLanded && lastLandingVelocity < minFallVelocity;
    }

    private bool IsGrounded()
    {
        var startPosition = characterAnimator.transform.position;
        startPosition.y += charController.radius;
        
        return Physics.SphereCast(startPosition, charController.radius, Vector3.down, out RaycastHit hit, 0.15f, groundedMask);
    }

    private void OnDrawGizmos()
    {
        var startPosition = characterAnimator.transform.position;
        startPosition.y += charController.radius;
        
        Gizmos.DrawSphere(startPosition, charController.radius);
    }

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
            var runWalkspeed = movementSpeed * runSpeedMultiplier;
            currentWalkspeed = Mathf.MoveTowards(currentWalkspeed, runWalkspeed, Time.deltaTime * walkspeedAcceleration);
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

            playerVelocity.y += jumpPower;
            
            characterAnimator.SetTrigger(ANIM_JUMP_PARAM);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        charController.Move(playerVelocity * Time.deltaTime);
    }

    private void AnimateMovement()
    {
        characterAnimator.SetBool(ANIM_MOVING_PARAM, movementInput.magnitude > 0f);
            
        // Adjust WalkSpeed parameter so that feet don't slide.
        var walkspeed = (currentWalkspeed * speedMultiplier) / DEFAULT_WALKSPEED;
        characterAnimator.SetFloat(ANIM_WALKSPEED_PARAM, walkspeed);
        characterAnimator.SetFloat(ANIM_DIRECTION_X_PARAM, currentDirection.x);
        characterAnimator.SetFloat(ANIM_DIRECTION_Y_PARAM, currentDirection.y);
    }

    public float GetSpawnHeight() => (charController.height / 2f) + charController.skinWidth;

    private void GetPlugins()
    {
        playerPlugins = new List<IPlayerPlugin>();
        
        foreach (var childComponent in gameObject.GetComponentsInChildren<MonoBehaviour>())
        {
            if (childComponent is IPlayerPlugin plugin)
            {
                playerPlugins.Add(plugin);
            }
        }
    }
    
    private void InitPlugins()
    {
        foreach (var plugin in playerPlugins)         
        {
            plugin.Init(this);
        }
    }
}
