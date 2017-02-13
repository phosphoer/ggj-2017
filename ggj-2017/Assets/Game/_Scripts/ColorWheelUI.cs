using UnityEngine;

public class ColorWheelUI : MonoBehaviour
{
  public int MoodIntensity
  {
    get { return m_moodIntensity; }
    set 
    {
      if (m_moodIntensity != value)
      {
        m_moodIntensity = value;
        m_uiNeedsUpdate = true;
      }
    }
  }

  public MoodColor MoodColor
  {
    get { return m_moodColor; }
    set 
    {
      if (m_moodColor != value)
      {
        m_moodColor = value;
        m_uiNeedsUpdate = true;
      }
    }
  }

  [SerializeField]
  private Transform m_dateIconUI;

  [SerializeField]
  private GameObject m_colorWheelUI;

  [SerializeField]
  private float[] m_intensityDistances = new float[]
  {
    0.0f,
    0.1f,
    0.2f,
    0.3f,
    0.4f,
    0.5f,
  };

  private MoodColor m_moodColor;
  private int m_moodIntensity;
  private bool m_uiNeedsUpdate;
  private bool m_animating;

  public void RefreshUI()
  {
    m_uiNeedsUpdate = true;
  }

  public void UpdateUINoAnimation()
  {
     m_dateIconUI.localPosition = GetVectorForMood(MoodColor, MoodIntensity);
  }

  private void Update()
  {
    if (m_uiNeedsUpdate && !m_animating)
    {
      StartCoroutine(AnimateUI());
    }
  }

  private Vector3 GetVectorForMood(MoodColor mood, int intensity)
  {
    // Get the angle arc of a single color wedge
    int colorCount = System.Enum.GetNames(typeof(MoodColor)).Length;
    float colorArcAngleSize = 2 * Mathf.PI / colorCount;

    // Get the angle wedge the current and applied colors are in
    float currentColorAngle = colorArcAngleSize * (int)mood;

    // Calculate the mood vector
    Vector2 currentMoodVector = new Vector2(Mathf.Cos(currentColorAngle), Mathf.Sin(currentColorAngle)).normalized;
    currentMoodVector *= m_intensityDistances[intensity];
    currentMoodVector.y *= -1;

    return currentMoodVector;
  }

  private System.Collections.IEnumerator AnimateUI()
  {
    m_animating = true;
    m_uiNeedsUpdate = false;

    if (m_colorWheelUI != null)
      m_colorWheelUI.SetActive(true);

    const float duration = 2.0f;
    float startTime = Time.time;
    Vector3 startPos = m_dateIconUI.localPosition;
    Vector3 desiredPos = GetVectorForMood(MoodColor, MoodIntensity);
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      m_dateIconUI.localPosition = Vector3.Lerp(startPos, desiredPos, t);
      yield return null; 
    }

    m_animating = false;

    if (m_colorWheelUI != null)
      m_colorWheelUI.SetActive(false);
  }
}