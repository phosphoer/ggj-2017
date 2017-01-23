using UnityEngine;
using System.Collections;

public class DateMood : MonoBehaviour
{
  public MoodColor MoodColor 
  {
    get { return m_moodColor; }
    set 
    {
      m_moodColor = value;
      m_moodUI.MoodColor = value;
    }
  }

  public int MoodIntensity 
  {
    get { return m_moodIntensity; }
    set 
    {
      m_moodIntensity = value;
      m_moodUI.MoodIntensity = value;
    }
  }

  public static event System.Action<DateMood> MoodCentered;
  public static event System.Action<DateMood> MoodOutOfControl;
  public static event System.Action<DateMood> GoodMood;

  private MoodColor m_moodColor;
  private int m_moodIntensity;
  private int m_currentTimeIcon;
  private float m_moodDriftTimer;

  [SerializeField]
  private AudioClip m_happySound;
  [SerializeField]
  private AudioClip m_sadSound;
  [SerializeField]
  private DateMoodUI m_moodUI;
  [SerializeField]
  private GameObject[] m_timeIcons;
  [SerializeField]
  private Animator m_animator;
  [SerializeField]
  private Renderer[] m_dateRenderers;

  public void ShowUI()
  {
    m_moodUI.RefreshUI();
  }

  public void ApplyMoodEffect(MoodColor appliedColor, int moodBaseIntensity)
  {
    Debug.Log(string.Format("Applying {0} to date's {1}", appliedColor, MoodColor));
    Debug.Log(string.Format("Date has intensity {0} and interaction intensity is {1}", MoodIntensity, moodBaseIntensity));

    // Get the angle arc of a single color wedge
    int colorCount = System.Enum.GetNames(typeof(MoodColor)).Length;
    float colorArcAngleSize = 2 * Mathf.PI / colorCount;

    // Get the angle wedge the current and applied colors are in
    float currentColorAngle = colorArcAngleSize * (int)MoodColor;
    float appliedColorAngle = colorArcAngleSize * (int)appliedColor;

    // Calculate the mood vector
    Vector2 currentMoodVector = new Vector2(Mathf.Cos(currentColorAngle), Mathf.Sin(currentColorAngle)).normalized;
    Vector2 appliedMoodVector = new Vector2(Mathf.Cos(appliedColorAngle), Mathf.Sin(appliedColorAngle)).normalized;
    Vector2 combinedVector = currentMoodVector + appliedMoodVector;

    int originalIntensity = MoodIntensity;

    // If the two vectors combine to form the zero vector then they are opposites
    if (combinedVector.magnitude <= 0.01f)
    {
      Debug.Log("The colors are direct opposites, just moving inward by " + moodBaseIntensity);

      // In this case we just lower our intensity by the zone intensity 
      int newIntensity = MoodIntensity - moodBaseIntensity;
      MoodIntensity = Mathf.Abs(newIntensity);
      if (newIntensity < 0)
      {
        MoodColor = appliedColor;
        Debug.Log("Went across the goal over to " + MoodColor.ToString());
      }
    }
    // Otherwise, they are analogous and we rotate around the color wheel + move outwards by the intensity
    else 
    {
      Debug.Log("The colors are not direct opposites");

      // We know if the applied mood is on the left or right of the existing mood based on the 2d cross product
      float moodCross = (currentMoodVector.x * appliedMoodVector.y) - (currentMoodVector.y * appliedMoodVector.x);
      int direction = (int)Mathf.Sign(moodCross);

      // How many 'moves' we have on the color wheel to reach our desired spot 
      int currentIntensity = MoodIntensity;
      int moveCount = moodBaseIntensity;
      int moodIndex = (int)MoodColor;
      int destMoodIndex = (int)appliedColor;

      // If the color is on the opposite side of the color wheel you have to 
      // move inwards first 
      float moodDot = Vector2.Dot(currentMoodVector, appliedMoodVector);
      if (moodDot < 0)
      {
        Debug.Log("..but the colors are on opposite sides of the wheel, so moving inward by 1");

        --moveCount;
        --currentIntensity;
        if (currentIntensity < 0)
        {
          moodIndex = (int)MoodColorZone.GetOppositeColor((MoodColor)moodIndex);
          currentIntensity = Mathf.Abs(currentIntensity);

          Debug.Log("Went across the goal over to " + ((MoodColor)moodIndex).ToString());
        }
      }

      // Now use the remaining moves to rotate towards the desired color
      while (moveCount > 0)
      {
        Debug.Log(string.Format("{0} moves remaining", moveCount));
        Debug.Log(string.Format("Date has intensity {0}", currentIntensity));

        --moveCount;

        // If we are not at the desired mood, rotate towards it
        if (moodIndex != destMoodIndex)
        {
          int prevMoodIndex = moodIndex;
          moodIndex = moodIndex + direction;
          if (moodIndex < 0) moodIndex = colorCount - 1;
          if (moodIndex >= colorCount) moodIndex = 0;

          Debug.Log(string.Format("Rotated color by {0} from {1} to {2}", direction, ((MoodColor)prevMoodIndex).ToString(), ((MoodColor)moodIndex).ToString()));
        }
        // Otherwise, move outwards with remaining move points
        else
        {
          currentIntensity = Mathf.Min(currentIntensity + 1, GameGlobals.Instance.MaxIntensity);
          Debug.Log("Moved outwards by 1");
        }
      }

      MoodColor = (MoodColor)moodIndex;
      MoodIntensity = currentIntensity;
    }

    // Do an emote based on whether this was a positive interaction 
    if (MoodIntensity >= originalIntensity)
    {
      if (m_animator != null) m_animator.SetTrigger("EmoteNegative");

      GetComponent<AudioSource>().PlayOneShot(m_sadSound);
    }
    else
    {
      if (m_animator != null) m_animator.SetTrigger("EmotePositive");

      GetComponent<AudioSource>().PlayOneShot(m_happySound);
    }

    StartCoroutine(AnimateColor());

    // Send mood events if necessary 
    if (MoodIntensity == 0)
    {
      if (MoodCentered != null)
        MoodCentered(this);
    }
    else if (MoodIntensity == GameGlobals.Instance.MaxIntensity)
    {
      if (MoodOutOfControl != null)
        MoodOutOfControl(this);
    }
    else if (MoodIntensity <= 1)
    {
      if (GoodMood != null)
        GoodMood(this);
    }

    Debug.Log("Final Mood");
    Debug.Log(MoodColor);
    Debug.Log(MoodIntensity);
  }

  private void Start()
  {
    RandomizeMood();
    MoodColorZone.MoodZoneActivated += OnMoodZoneActivated;
    MoodColor = MoodColor.Blue;
    MoodIntensity = 2;
    m_moodDriftTimer = -30.0f;

    GameEndCondition.MinuteElapsed += OnMinuteElapsed;

    StartCoroutine(MoodDriftRoutine());
    StartCoroutine(AnimateColor());
  }

  private void OnDestroy()
  {
    MoodColorZone.MoodZoneActivated -= OnMoodZoneActivated;
    GameEndCondition.MinuteElapsed -= OnMinuteElapsed;
  }

  private void Update()
  {
    m_moodDriftTimer += Time.deltaTime;
    if (m_moodDriftTimer > 20.0f)
    {
      ApplyMoodEffect(MoodColor, 1);      
      m_moodDriftTimer = 0;
    }
  }

  private void RandomizeMood()
  {
    int colorCount = System.Enum.GetNames(typeof(MoodColor)).Length;
    int moodChoice = Random.Range(0, colorCount);
    MoodColor = (MoodColor)moodChoice;
    MoodIntensity = Random.Range(3, 5);
  }

  private void OnMinuteElapsed()
  {
    StartCoroutine(ShowTimeIcon(m_timeIcons[m_currentTimeIcon]));
    m_currentTimeIcon = Mathf.Min(m_timeIcons.Length - 1, m_currentTimeIcon + 1);
  }

  private void OnMoodZoneActivated(MoodColorZone moodZone, MoodColor desiredColor)
  {
    StartCoroutine(WaitToDoMoodEffect(moodZone, desiredColor));
    m_moodDriftTimer = 0;
  }

  private IEnumerator ShowTimeIcon(GameObject timeIcon)
  {
    timeIcon.SetActive(true);
    yield return new WaitForSeconds(2.0f);
    timeIcon.SetActive(false);
  }

  private IEnumerator AnimateColor()
  {
    const float duration = 2.0f;
    float startTime = Time.time;
    Color toColor = GameGlobals.Instance.MoodColors[(int)MoodColor];
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      foreach (Renderer r in m_dateRenderers)
        r.sharedMaterial.SetColor("_Color", Color.Lerp(Color.white, toColor, t));

      yield return null; 
    }

    yield return new WaitForSeconds(2.0f);

    startTime = Time.time;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      foreach (Renderer r in m_dateRenderers)
        r.sharedMaterial.SetColor("_Color", Color.Lerp(toColor, Color.white, t));

      yield return null; 
    }
  }

  private IEnumerator MoodDriftRoutine()
  {
    while (true)
    {
      yield return new WaitForSeconds(35.0f);
    }
  }

  private IEnumerator WaitToDoMoodEffect(MoodColorZone moodZone, MoodColor desiredColor)
  {
    yield return new WaitForSeconds(2.0f);
    ApplyMoodEffect(desiredColor, moodZone.MoodIntensity);
  }
}