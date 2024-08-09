using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rigidbody;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;

    private CrouchJump crouchJump;

    void Reset()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Start()
    {
        // Get rigidbody.
        rigidbody = GetComponent<Rigidbody>();
        // Get the CrouchJump component
        crouchJump = GetComponentInParent<CrouchJump>();

        if (crouchJump != null)
        {
            crouchJump.CrouchEnd += PerformCrouchJump; 

        }
    }

    // No more LateUpdate() with Input.GetButtonDown("Jump")
    private void PerformCrouchJump()
    {
        if (!groundCheck || groundCheck.isGrounded)
        {
            rigidbody.AddForce(Vector3.up * 100 * crouchJump.jumpPower); 
        }
    }
}
