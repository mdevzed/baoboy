using UnityEngine;

public class CrouchJump : MonoBehaviour
{
    public KeyCode key = KeyCode.Space;

    [Header("Crouch Length")]
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
    float jumpPower;
    float jumpPowerChangePerSecond;
    float headYChangePerSecond;
    float moveSpeedChangePerSecond;

    public bool IsCrouched { get; private set; }
    public event System.Action CrouchStart, CrouchEnd;


    void Start()
    {   
        IsCrouched = false;

        defaultHeadYLocalPosition = headToLower.localPosition.y;
        defaultMovementSpeed = movement.speed;

        jumpPower = minJumpPower;


        headYChangePerSecond = (minCrouchYHeadPosition - defaultHeadYLocalPosition)/ crouchAnimationDuration;
        moveSpeedChangePerSecond = (minMoveSpeed - defaultMovementSpeed) / crouchAnimationDuration;
        jumpPowerChangePerSecond = (minJumpPower - maxJumpPower) / crouchAnimationDuration;
       
        print(headYChangePerSecond);
        print(moveSpeedChangePerSecond);
        print(jumpPowerChangePerSecond);    
    }

    void Reset()
    {
        // Try to get components.
        movement = GetComponentInParent<FirstPersonMovement>();
        headToLower = movement.GetComponentInChildren<Camera>().transform;
    }

    void LateUpdate()
    {
        if (Input.GetKey(key))
        {
            // Set IsCrouched state.

            if (!IsCrouched)
            {
                IsCrouched = true;
                CrouchStart?.Invoke();
                crouchStartTime = Time.time;
                crouchEndTime = crouchStartTime + crouchAnimationDuration;
                jumpPower = minJumpPower;
                headYChangePerSecond = (minCrouchYHeadPosition - defaultHeadYLocalPosition)/ crouchAnimationDuration;
                moveSpeedChangePerSecond = (minMoveSpeed - defaultMovementSpeed) / crouchAnimationDuration;
                jumpPowerChangePerSecond = (minJumpPower - maxJumpPower) / crouchAnimationDuration;
            }

            if(Time.time < crouchEndTime)
            {
                // Enforce a low head.
                if (headToLower)
                {
                    // Lower the head, but clamp it to the minimum position
                    float newHeadYPosition = Mathf.Clamp(headToLower.localPosition.y + headYChangePerSecond * Time.deltaTime, minCrouchYHeadPosition, defaultHeadYLocalPosition);
                    headToLower.localPosition = new Vector3(headToLower.localPosition.x, newHeadYPosition, headToLower.localPosition.z);
                }

                if (movement)
                {
                    float newMovementSpeed = Mathf.Clamp(movement.speed + moveSpeedChangePerSecond * Time.deltaTime, minMoveSpeed, defaultMovementSpeed);
                    movement.speed = newMovementSpeed;
                }

                jumpPower = Mathf.Clamp(jumpPower + jumpPowerChangePerSecond * Time.deltaTime, minJumpPower, maxJumpPower);
            }

        }
        else
        {
            if (IsCrouched)
            {
                // Rise the head back up.
                if (headToLower)
                {
                    headToLower.localPosition = new Vector3(headToLower.localPosition.x, defaultHeadYLocalPosition, headToLower.localPosition.z);
                }
                if (movement)
                {
                    movement.speed = defaultMovementSpeed;
                }
                // Reset IsCrouched.
                IsCrouched = false;
                CrouchEnd?.Invoke();
            }
        }
    }
}
    
