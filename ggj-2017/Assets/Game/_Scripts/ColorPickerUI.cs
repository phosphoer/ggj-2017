using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorPickerUI : MonoBehaviour
{
  [System.SerializableAttribute]
  public class ColorOption
  {
    public MoodColor MoodColor;
    public Transform Transform;
  }

  [SerializeField]
  private ColorOption[] m_colorOptions;

  private Dictionary<MoodColor, ColorOption> m_colorOptionsMap = new Dictionary<MoodColor, ColorOption>();

  private void Start()
  {
    GameGlobals.Instance.PlayerController.ColorPressed += OnColorPressed;

    foreach (ColorOption option in m_colorOptions)
    {
      m_colorOptionsMap.Add(option.MoodColor, option);
    }
  }

  private void OnDestroy()
  {
    GameGlobals.Instance.PlayerController.ColorPressed -= OnColorPressed;
  }

  private void OnColorPressed(MoodColor moodColor)
  {
    StartCoroutine(ColorSelectAnimation(m_colorOptionsMap[moodColor]));
  }

  private IEnumerator ColorSelectAnimation(ColorOption option)
  {
    const float duration = 1.0f;
    float animEndTime = Time.time + duration;
    Vector3 startScale = option.Transform.localScale;
    Vector3 endScale = startScale * 1.25f;
    while (Time.time < animEndTime)
    {
      float t = (animEndTime - Time.time) / duration;
      option.Transform.localScale = Vector3.Lerp(startScale, endScale, t);
      yield return null;
    }

    Destroy(gameObject);
  }
}