using UnityEngine;

[RequireComponent(typeof(DudeController))]
public class WaypointAI : MonoBehaviour
{
  public WaypointPath Path;
  public bool CanWalk = true;

  private int m_currentWaypointIndex;
  private Transform m_currentWaypoint;
  private DudeController m_dude;
  private float m_walkTime;
  private float m_idleTime;
  private bool m_walking;

  private void Start()
  {
    m_dude = GetComponent<DudeController>();
  }

  private void Update()
  {
    if (m_walking && CanWalk)
    {
      m_walkTime += Time.deltaTime;
      if (m_currentWaypoint == null && Path != null)
      {
        m_currentWaypoint = Path.Waypoints[0];
        m_currentWaypointIndex = 0;
      }

      if (m_currentWaypoint != null)
      {
        Vector3 toTarget = m_currentWaypoint.position - transform.position;
        m_dude.MoveDirection = toTarget.normalized;

        if (toTarget.magnitude < 1.0f)
        {
          ++m_currentWaypointIndex;
          if (m_currentWaypointIndex >= Path.Waypoints.Length)
            m_currentWaypoint = null;
          else
            m_currentWaypoint = Path.Waypoints[m_currentWaypointIndex];
        }
      }   

      if (m_walkTime > 10.0f)
      {
        m_walking = false;
        m_walkTime = 0;
      }
    }
    else 
    {
      m_idleTime += Time.deltaTime;
      m_dude.MoveDirection = Vector3.zero;

      if (m_idleTime > 15.0f)
      {
        m_walking = true;
        m_idleTime = 0;
      }
    }

  }
}