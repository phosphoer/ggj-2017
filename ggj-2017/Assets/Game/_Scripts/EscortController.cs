using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EscortController : MonoBehaviour
{
  public Transform FollowObject;

  [SerializeField]
  private DudeController m_character;

  [SerializeField]
  private Animator m_animator;

  private NavMeshPath m_path;
  private int m_currentPathIndex;
  private bool m_alive = true;

  private const float kDesiredDistToTarget = 2.0f;

  private void Start()
  {
    m_path = new NavMeshPath();

    StartCoroutine(RecalcPath());
  }

  private void OnDestroy()
  {
    m_alive = false;
  }

  private void Update()
  {
    if (FollowObject == null)
      return;

    // If we are close to our destination, don't do anything 
    if (Vector3.Distance(transform.position, FollowObject.position) < kDesiredDistToTarget)
    {
      m_character.MoveDirection = Vector3.zero;
      return;
    }

    if (m_path.corners.Length > 0)
    {
      // Get vector to next waypoint
      Vector3 nextPoint = m_path.corners[m_currentPathIndex];
      Vector3 toNextPoint = nextPoint - transform.position;

      // Move towards the current waypoint
      m_character.MoveDirection = toNextPoint.normalized;
      Debug.DrawLine(transform.position, nextPoint);

      // If we are close enough, go to the next waypoint
      if (toNextPoint.magnitude < kDesiredDistToTarget)
      {
        m_currentPathIndex = Mathf.Min(m_path.corners.Length - 1, m_currentPathIndex + 1);
      }
    }

  }

  private IEnumerator RecalcPath()
  {
    while (m_alive)
    {
      if (FollowObject != null)
      {
        NavMesh.CalculatePath(transform.position, FollowObject.transform.position, NavMesh.AllAreas, m_path);
        m_currentPathIndex = 1;
      }

      yield return new WaitForSeconds(1.0f);
    }
  }
}