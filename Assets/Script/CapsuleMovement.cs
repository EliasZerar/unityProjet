using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CapsuleMovement : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private Animator m_Animator;

    private Rigidbody m_Rigidbody;
    private bool m_IsMovementEnabled = true;
    private Vector2 m_RawInput;
    private Vector2 m_OscInput;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.freezeRotation = true;
    }

    private void Update()
    {
        if (!m_IsMovementEnabled)
        {
            m_RawInput = Vector2.zero;
            m_OscInput = Vector2.zero;
            return;
        }

        m_RawInput = ReadMovementInput();
    }

    private void FixedUpdate()
    {
        Vector2 combined = m_RawInput + m_OscInput;
        combined = Vector2.ClampMagnitude(combined, 1f);

        Vector3 movementDir = new Vector3(combined.x, 0f, combined.y).normalized;
        Vector3 velocity = movementDir * m_Speed;

        m_Rigidbody.linearVelocity = new Vector3(velocity.x, m_Rigidbody.linearVelocity.y, velocity.z);

        if (movementDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }

        if (m_Animator != null)
            m_Animator.SetBool("isWalking", movementDir != Vector3.zero);
    }

    public void EnableMovement() => m_IsMovementEnabled = true;

    public void DisableMovement()
    {
        m_IsMovementEnabled = false;
        m_Rigidbody.linearVelocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    public void OscMoveHorizontal(float value) => m_OscInput.x = value;
    public void OscMoveVertical(float value)   => m_OscInput.y = value;

    private Vector2 ReadMovementInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;

        return input;
    }
}