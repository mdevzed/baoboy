using UnityEngine;

public class CrouchJump : MonoBehaviour
{
    public KeyCode key = KeyCode.Space;

    [Header("Crouch Duration")]
    [Tooltip("How long it will take before fully crouched.")]
    public float crouchAnimationDuration = 5f;
    float crouchStartTime;
    float crouchEndTime;

    [Header("Head Lowering")]
    [Tooltip("Player head")]
    public Transform headToLower;
    float defaultHeadYLocalPosition; 
    [Tooltip("Minimum head position when crouched.")]
    public float minCrouchYHeadPosition = 0.5f; //Minimum head Y position

    [Header("Movement Slowing")]
    [Tooltip("Player movement variable")]
    public FirstPersonMovement movement;
    [Tooltip("Minimum movement speed when crouched.")]
    float defaultMovementSpeed;
    float movementSpeed;
    public float minMoveSpeed = 2;

    [Header("Jump Power")]
    [Tooltip("Maximum jump power when crouched.")]
    public float maxJumpPower = 2;
    [Tooltip("Minimum jump power when crouched.")]
    public float minJumpPower = 0.25f;
    [HideInInspector]
    public float jumpPower;

    GroundCheck groundCheck;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;


    void Start()
    {   
        IsCrouched = false;

        defaultHeadYLocalPosition = headToLower.localPosition.y;
        jumpPower = minJumpPower;  
    }

    void Reset()
    {
        // Try to get components.
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void LateUpdate()
    {
       // Debug.Log($"Movement Speed at start of LateUpdate: {movementSpeed}"); // Log at the beginning
        if (Input.GetKey(key))
        {
            // Set IsCrouched state.

            if (!IsCrouched)
            {
                IsCrouched = true;
                SetSpeedOverrideActive(true);
                CrouchStart?.Invoke();

                crouchStartTime = Time.time;
                crouchEndTime = crouchStartTime + crouchAnimationDuration;

                if (defaultMovementSpeed != movement.speed)
                {
                    defaultMovementSpeed = movement.speed;
                    movementSpeed = movement.speed;
                }

                jumpPower = minJumpPower;
                }

            if(Time.time < crouchEndTime)
            {
                // Smoothly lower the head
                float t = (Time.time - crouchStartTime) / crouchAnimationDuration; // Calculate normalized time
                float newHeadYPosition = Mathf.SmoothStep(defaultHeadYLocalPosition, minCrouchYHeadPosition, t);
                headToLower.localPosition = new Vector3(headToLower.localPosition.x, newHeadYPosition, headToLower.localPosition.z);

                // Smoothly decrease movement speed
                float newMovementSpeed = Mathf.SmoothStep(defaultMovementSpeed, minMoveSpeed, t);
                movementSpeed = newMovementSpeed;

                // Smoothly increase jump power
                jumpPower = Mathf.SmoothStep(minJumpPower, maxJumpPower, t); 
            }
        }
        else
        {
            if (IsCrouched && (!groundCheck || groundCheck.isGrounded))
            {
                // Rise the head back up.
                if (headToLower)
                {
                    headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition, headToLower.localPosition.z);
                }
                if (movement)
                {
                    movement.speed = defaultMovementSpeed;
                    movementSpeed = minMoveSpeed;
                }
                // Reset IsCrouched.
                IsCrouched = false;
                SetSpeedOverrideActive(false);
                CrouchEnd?.Invoke();
            }
        }
    }

    #region Speed override.
    void SetSpeedOverrideActive(bool state)
    {
        // Stop if there is no movement component.
        if(!movement)
        {
            return;
        }

        // Update SpeedOverride.
        if (state)
        {
            // Try to add the SpeedOverride to the movement component.
            if (!movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Add(SpeedOverride);
            }
        }
        else
        {
            // Try to remove the SpeedOverride from the movement component.
            if (movement.speedOverrides.Contains(SpeedOverride))
            {
                movement.speedOverrides.Remove(SpeedOverride);
            }
        }
    }

    float SpeedOverride() => movementSpeed;
    #endregion
}

    
