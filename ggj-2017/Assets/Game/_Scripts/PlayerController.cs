using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Rewired.Player PlayerInput { get { return m_rewiredPlayer; } }

  public event System.Action<MoodColor> ColorPressed;

  [SerializeField]
  private ColorPickerUI m_colorPickerUIPrefab;

  [SerializeField]
  private int m_rewiredPlayerID;

  [SerializeField]
  private DudeController m_character;

  private Rewired.Player m_rewiredPlayer;
  private MoodColorZone m_currentMoodZone;
  private ColorPickerUI m_colorPicker;

  private void Start()
  {
    GameGlobals.Instance.PlayerController = this;
    m_rewiredPlayer = Rewired.ReInput.players.GetPlayer(m_rewiredPlayerID);
  }

  private void Update()
  {
    if (!Rewired.ReInput.isReady)
    {
      return;
    }

    // Interaction input happens if we are in an interaction zone 
    if (m_rewiredPlayer.GetButtonDown("Interact") && m_currentMoodZone != null)
    {
      m_currentMoodZone.HideInteractionPrompt();
      m_colorPicker = Instantiate(m_colorPickerUIPrefab);
    }
    
    // Color input
    if (m_rewiredPlayer.GetButtonDown("ColorRed"))
    {
      if (ColorPressed != null)
        ColorPressed(MoodColor.Red);
    }

    if (m_rewiredPlayer.GetButtonDown("ColorBlue"))
    {
      if (ColorPressed != null)
        ColorPressed(MoodColor.Blue);
    }

    if (m_rewiredPlayer.GetButtonDown("ColorYellow"))
    {
      if (ColorPressed != null)
        ColorPressed(MoodColor.Yellow);
    }

    // Get input for movement direction
    Vector3 moveDir = Vector3.zero;
    if (m_colorPicker == null)
    {
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
    }


    // Apply movement direction to character 
    if (m_character != null)
    {
      m_character.MoveDirection = moveDir.normalized;
    }
  }

  private void OnTriggerEnter(Collider col)
  {
    MoodColorZone moodZone = col.GetComponent<MoodColorZone>();
    if (moodZone != null)
    {
      m_currentMoodZone = moodZone;
      m_currentMoodZone.ShowInteractionPrompt();
    }
  }

  private void OnTriggerExit(Collider col)
  {
    MoodColorZone moodZone = col.GetComponent<MoodColorZone>();
    if (moodZone != null && m_currentMoodZone == moodZone)
    {
      m_currentMoodZone.HideInteractionPrompt();
      m_currentMoodZone = null;
    }
  }
}