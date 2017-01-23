using UnityEngine;
using System.Collections;

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

  [SerializeField]
  private Renderer[] m_renderers;

  [SerializeField]
  private ParticleSystem m_interactionParticle;

  private Rewired.Player m_rewiredPlayer;
  private MoodColorZone m_currentMoodZone;
  private ColorPickerUI m_colorPicker;

  private void Start()
  {
    GameGlobals.Instance.PlayerController = this;
    m_rewiredPlayer = Rewired.ReInput.players.GetPlayer(m_rewiredPlayerID);

    foreach (Renderer r in m_renderers)
      r.sharedMaterial.SetColor("_Color", Color.white);
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
        MoodColor combined = MoodColorZone.CombineColors(MoodColor.Red, m_currentMoodZone.MoodColor);
        ParticleSystem particles = Instantiate(m_interactionParticle);
        particles.transform.position = m_currentMoodZone.FocusTransform.position;
        particles.transform.position = new Vector3(particles.transform.position.x, 0, particles.transform.position.z);
        var settings = particles.main;
        Destroy(particles.gameObject, 5.0f);
        settings.startColor = GameGlobals.Instance.MoodColors[(int)combined];

        m_colorPicker.ChooseColor(MoodColor.Red);
        m_currentMoodZone.ChooseMoodColor(MoodColor.Red);
        StartCoroutine(AnimateColor(MoodColor.Red));
        m_currentMoodZone = null;      
        if (m_animator != null) m_animator.SetTrigger("InteractRed");

        GetComponent<AudioSource>().PlayOneShot(m_soundRed);
      }

      if (m_rewiredPlayer.GetButtonDown("ColorBlue"))
      {
        MoodColor combined = MoodColorZone.CombineColors(MoodColor.Blue, m_currentMoodZone.MoodColor);
        ParticleSystem particles = Instantiate(m_interactionParticle);
        particles.transform.position = m_currentMoodZone.FocusTransform.position;
        particles.transform.position = new Vector3(particles.transform.position.x, 0, particles.transform.position.z);
        var settings = particles.main;
        Destroy(particles.gameObject, 5.0f);
        settings.startColor = GameGlobals.Instance.MoodColors[(int)combined];
        m_colorPicker.ChooseColor(MoodColor.Blue);
        m_currentMoodZone.ChooseMoodColor(MoodColor.Blue);
        StartCoroutine(AnimateColor(MoodColor.Blue));
        m_currentMoodZone = null;      
        if (m_animator != null) m_animator.SetTrigger("InteractBlue");

        GetComponent<AudioSource>().PlayOneShot(m_soundBlue);
      }

      if (m_rewiredPlayer.GetButtonDown("ColorYellow"))
      {
        MoodColor combined = MoodColorZone.CombineColors(MoodColor.Yellow, m_currentMoodZone.MoodColor);
        ParticleSystem particles = Instantiate(m_interactionParticle);
        particles.transform.position = m_currentMoodZone.FocusTransform.position;
        particles.transform.position = new Vector3(particles.transform.position.x, 0, particles.transform.position.z);
        var settings = particles.main;
        Destroy(particles.gameObject, 5.0f);
        settings.startColor = GameGlobals.Instance.MoodColors[(int)combined];
        m_colorPicker.ChooseColor(MoodColor.Yellow);
        m_currentMoodZone.ChooseMoodColor(MoodColor.Yellow);
        StartCoroutine(AnimateColor(MoodColor.Yellow));
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
  private IEnumerator AnimateColor(MoodColor moodColor)
  {
    const float duration = 2.0f;
    float startTime = Time.time;
    Color toColor = GameGlobals.Instance.MoodColors[(int)moodColor];
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      foreach (Renderer r in m_renderers)
        r.sharedMaterial.SetColor("_Color", Color.Lerp(Color.white, toColor, t));

      yield return null; 
    }

    yield return new WaitForSeconds(2.0f);

    startTime = Time.time;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      foreach (Renderer r in m_renderers)
        r.sharedMaterial.SetColor("_Color", Color.Lerp(toColor, Color.white, t));

      yield return null; 
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

    DateMood dateMood = col.GetComponent<DateMood>();
    if (dateMood != null)
    {
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