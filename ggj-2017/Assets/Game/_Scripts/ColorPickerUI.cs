using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ColorPickerUI : MonoBehaviour
{
  public DateMood ForDate;

  [System.SerializableAttribute]
  public class ColorOption
  {
    public MoodColor MoodColor;
    public Transform Transform;
    public Image Image;
  }

  [SerializeField]
  private ColorOption[] m_colorOptions;

  [SerializeField]
  private Transform m_root;

  [SerializeField]
  private ColorWheelUI m_colorWheelUI;

  private Dictionary<MoodColor, ColorOption> m_colorOptionsMap = new Dictionary<MoodColor, ColorOption>();
  private MoodColor m_baseColor;

  public void ChooseColor(MoodColor moodColor)
  {
    StartCoroutine(ColorSelectAnimation(m_colorOptionsMap[moodColor]));
  }

  public void SetBaseColor(MoodColor baseColor)
  {
    m_baseColor = baseColor;
  }

  private void Start()
  {
    foreach (ColorOption option in m_colorOptions)
    {
      m_colorOptionsMap.Add(option.MoodColor, option);
    }

    m_colorWheelUI.MoodColor = ForDate.MoodColor;
    m_colorWheelUI.MoodIntensity = ForDate.MoodIntensity;
    m_colorWheelUI.UpdateUINoAnimation();
  }

  private IEnumerator ColorSelectAnimation(ColorOption option)
  {
    option.Transform.gameObject.SetActive(true);
    const float duration = 0.25f;
    float startTime = Time.time;
    Vector3 startScale = m_root.localScale;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
     m_root.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
      yield return null; 
    }
    Destroy(gameObject);
  }
}