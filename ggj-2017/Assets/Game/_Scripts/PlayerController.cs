using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Rewired.Player PlayerInput { get { return m_rewiredPlayer; } }

  [SerializeField]
  private ColorPickerUI m_colorPickerUIPrefab;

  [SerializeField]
  private int m_rewiredPlayerID;

  [SerializeField]
  private DudeController m_character;

  [SerializeField]
  private Animator m_animator;

  [SerializeField]
  private AudioClip m_soundRed;

  [SerializeField]
  private AudioClip m_soundBlue;

  [SerializeField]
  private AudioClip m_soundYellow;

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
    if (m_rewiredPlayer.GetButtonDown("Interact") && m_currentMoodZone != null && m_colorPicker == null)
    {
      m_currentMoodZone.HideInteractionPrompt();
      m_currentMoodZone.BeginInteraction();
      m_colorPicker = Instantiate(m_colorPickerUIPrefab);
      m_colorPicker.SetBaseColor(m_currentMoodZone.MoodColor);
      if (m_animator != null) m_animator.SetTrigger("InteractGreen");
    }
    
    // Color input
    if (m_colorPicker != null && m_currentMoodZone != null)
    {
      if (m_rewiredPlayer.GetButtonDown("ColorRed"))
      {
        m_colorPicker.ChooseColor(MoodColor.Red);
        m_currentMoodZone.ChooseMoodColor(MoodColor.Red);
        m_currentMoodZone = null;      
        if (m_animator != null) m_animator.SetTrigger("InteractRed");

        GetComponent<AudioSource>().PlayOneShot(m_soundRed);
      }

      if (m_rewiredPlayer.GetButtonDown("ColorBlue"))
      {
        m_colorPicker.ChooseColor(MoodColor.Blue);
        m_currentMoodZone.ChooseMoodColor(MoodColor.Blue);
        m_currentMoodZone = null;      
        if (m_animator != null) m_animator.SetTrigger("InteractBlue");

        GetComponent<AudioSource>().PlayOneShot(m_soundBlue);
      }

      if (m_rewiredPlayer.GetButtonDown("ColorYellow"))
      {
        m_colorPicker.ChooseColor(MoodColor.Yellow);
        m_currentMoodZone.ChooseMoodColor(MoodColor.Yellow);
        m_currentMoodZone = null;      
        if (m_animator != null) m_animator.SetTrigger("InteractYellow");

        GetComponent<AudioSource>().PlayOneShot(m_soundYellow);
      }
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