using UnityEngine;
using UnityEngine.UI;

public class DateMoodUI : MonoBehaviour
{
  public int MoodIntensity
  {
    get { return m_moodIntensity; }
    set 
    {
      if (m_moodIntensity != value)
      {
        m_moodIntensity = value;
        RefreshUI();
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
        RefreshUI();
      }
    }
  }

  [SerializeField]
  private Image m_moodColorImage;

  [SerializeField]
  private Image m_intensityIcon;

  private int m_moodIntensity;
  private MoodColor m_moodColor;

  private void Start()
  {
    RefreshUI();
  }

  private void RefreshUI()
  {
    float intensityRamp = (float)m_moodIntensity / GameGlobals.Instance.MaxIntensity;
    
    float h, s, v;
    Color baseColor = GameGlobals.Instance.MoodColors[(int)MoodColor];
    Color.RGBToHSV(baseColor, out h, out s, out v);
    s = intensityRamp;
    Color saturatedColor = Color.HSVToRGB(h, s, v);

    m_moodColorImage.color = saturatedColor;
    m_intensityIcon.transform.localScale = Vector3.one * (intensityRamp);
  }
}