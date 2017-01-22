using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DudeController : MonoBehaviour
{
  public float MoveSpeed = 1.0f;

  public Vector3 MoveDirection
  {
    get { return m_moveDirection; }
    set { m_moveDirection = value; }
  }

  [SerializeField]
  private Animator m_animator;

  private Rigidbody m_rb;
  private Vector3 m_facingDirection;
  private Vector3 m_moveDirection;

  private void Start()
  {
    m_rb = GetComponent<Rigidbody>();
  }

  private void Update()
  {
    // Move in our current direction
    m_moveDirection.y = 0;
    m_moveDirection = Vector3.Normalize(m_moveDirection);
    Vector3 newPosition = m_rb.position + MoveDirection * MoveSpeed * Time.deltaTime;
    m_rb.MovePosition(newPosition);

    // Change our facing direction if we are actually moving
    if (MoveDirection.sqrMagnitude > Mathf.Epsilon)
    {
      m_facingDirection = MoveDirection;
      if (m_animator != null) m_animator.SetBool("Walking", true);
    }
    else 
    {
      if (m_animator != null) m_animator.SetBool("Walking", false);
    }

    // Face in our movement direction
    if (m_facingDirection.sqrMagnitude > 0 && Vector3.Angle(m_facingDirection, transform.forward) > 5)
    {
      Quaternion desiredRot = Quaternion.LookRotation(m_facingDirection, Vector3.up);
      m_rb.rotation = Quaternion.Lerp(m_rb.rotation, desiredRot, Time.deltaTime * 5.0f);
    }
  }
}