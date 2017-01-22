using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ColorPickerUI : MonoBehaviour
{
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
  private Image[] m_colorWheels;

  [SerializeField]
  private Transform m_root;

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

    foreach (Image color in m_colorWheels)
    {
      if (color != null)
        color.gameObject.SetActive(false);
    }

    m_colorWheels[(int)m_baseColor].gameObject.SetActive(true);
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