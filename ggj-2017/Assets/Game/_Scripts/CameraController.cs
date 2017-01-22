using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Transform FollowObject;
  public float MoveSpeed = 3.0f;

  private bool m_lerping;
  private bool m_following;
  private float m_lerpDist;
  private Vector3 m_lastFollowObjectPos;
  private float m_lastObjectMoveTime;

  private void LateUpdate()
  {
    if (FollowObject != null)
    {
      Vector3 targetVelocity = FollowObject.position - m_lastFollowObjectPos;
      Vector3 toTarget = FollowObject.position - transform.position;
      float dist = toTarget.magnitude;
      if (dist > 1.0f)
      {
        m_following = true;
      }

      if (m_following)
      {
        if (!m_lerping)
        {
          m_lerping = true;
          m_lerpDist = dist;
        }
        else 
        {
          m_lerpDist = Mathf.Lerp(m_lerpDist, 0, Time.deltaTime * 0.5f);
          transform.position = FollowObject.position - toTarget.normalized * m_lerpDist;
        }

        if (targetVelocity.magnitude > 0.001f)
        {
          m_lastObjectMoveTime = Time.time;
        }

        if (Time.time - m_lastObjectMoveTime > 2.0f && dist < 0.01f)
        {
          m_lerping = false;
          m_following = false;
        }
      }

      m_lastFollowObjectPos = FollowObject.position;
    }
  }
}