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

  private Dictionary<MoodColor, ColorOption> m_colorOptionsMap = new Dictionary<MoodColor, ColorOption>();

  public void ChooseColor(MoodColor moodColor)
  {
    StartCoroutine(ColorSelectAnimation(m_colorOptionsMap[moodColor]));
  }

  private void Start()
  {
    foreach (ColorOption option in m_colorOptions)
    {
      m_colorOptionsMap.Add(option.MoodColor, option);
      option.Image.color = GameGlobals.Instance.MoodColors[(int)option.MoodColor];
    }
  }

  private IEnumerator ColorSelectAnimation(ColorOption option)
  {
    const float duration = 1.0f;
    float startTime = Time.time;
    Vector3 startScale = option.Transform.localScale;
    Vector3 endScale = startScale * 1.25f;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      option.Transform.localScale = Vector3.Lerp(startScale, endScale, t);
      yield return null;
    }

    Destroy(gameObject);
  }
}