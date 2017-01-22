using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
  public Transform FollowObject;
  public float MoveSpeed = 3.0f;

  private bool m_lerping;
  private bool m_following;
  private float m_lerpDist;
  private Vector3 m_lastFollowObjectPos;
  private float m_lastObjectMoveTime;
  private Vector3 m_startPos;
  private Quaternion m_startRot;
  private bool m_atTitle = true;

  [SerializeField]
  private GameObject m_titleRoot;

  [SerializeField]
  private Transform m_titleTransform;

  [SerializeField]
  private Transform m_cameraTransform;

  private void Start()
  {
    m_startPos = m_cameraTransform.localPosition;
    m_startRot = m_cameraTransform.localRotation;

    m_cameraTransform.LookAt(m_titleTransform, Vector3.up);
  }

  private void Update()
  {
    if (!Rewired.ReInput.isReady)
      return;

    if (m_atTitle)
    {
      var playerInput = GameGlobals.Instance.PlayerController.PlayerInput;
      if (playerInput.GetAnyButtonDown())
      {
        StartCoroutine(AnimateFromTitle());
      }
    }
  }

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

  private IEnumerator AnimateFromTitle()
  {
    const float duration = 2.0f;
    float startTime = Time.time;
    Quaternion rotBegin = m_cameraTransform.localRotation;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      m_cameraTransform.localRotation = Quaternion.Lerp(rotBegin, m_startRot, t);
      yield return null; 
    }

    m_titleRoot.SetActive(false);
  }
}