using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
  [SerializeField]
  private int m_rewiredPlayerID;

  [SerializeField]
  private DudeController m_character;

  private Rewired.Player m_rewiredPlayer;

  private void Start()
  {
    m_rewiredPlayer = Rewired.ReInput.players.GetPlayer(m_rewiredPlayerID);
  }

  private void Update()
  {
    // Get input for movement direction
    Vector3 moveDir = Vector3.zero;
    if (m_rewiredPlayer.GetButton("MoveForward"))
    {
      moveDir += Vector3.forward;
    }
    if (m_rewiredPlayer.GetButton("MoveBackward"))
    {
      moveDir += Vector3.back;
    }
    if (m_rewiredPlayer.GetButton("MoveLeft"))
    {
      moveDir += Vector3.left;
    }
    if (m_rewiredPlayer.GetButton("MoveRight"))
    {
      moveDir += Vector3.right;
    }

    // Apply movement direction to character 
    if (m_character != null)
    {
      m_character.MoveDirection = moveDir.normalized;
    }
  }
}