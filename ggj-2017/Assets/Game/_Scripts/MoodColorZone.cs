using UnityEngine;
using System.Collections;

public class MoodColorZone : MonoBehaviour
{
  public MoodColor MoodColor;
  public int MoodIntensity = 1;

  public static event System.Action<MoodColorZone, MoodColor> MoodZoneActivated;
  
  [SerializeField]
  private Transform m_focusTransform;

  [SerializeField]
  private Renderer[] m_moodZoneRenderers;

  private GameObject m_interactionPrompt;
  private Material m_sharedMaterial;

  public static MoodColor CombineColors(MoodColor a, MoodColor b)
  {
    // Handle each combination
    if (a == MoodColor.Red)
    {
      if (b == MoodColor.Blue)
        return MoodColor.Violet;
      else if (b == MoodColor.Yellow)
        return MoodColor.Orange;
    }
    else if (a == MoodColor.Yellow)
    {
      if (b == MoodColor.Red)
        return MoodColor.Orange;
      else if (b == MoodColor.Blue)
        return MoodColor.Green;
    }
    else if (a == MoodColor.Blue)
    {
      if (b == MoodColor.Red)
        return MoodColor.Violet;
      else if (b == MoodColor.Yellow)
        return MoodColor.Green;
    }
    
    // If none of the combinations matched, just return the original color 
    // because they must be the same
    return a;
  }

  public static MoodColor GetOppositeColor(MoodColor color)
  {
    int colorCount = System.Enum.GetNames(typeof(MoodColor)).Length;
    int newColorIndex = ((int)color + 3) % (colorCount - 1);
    return (MoodColor)newColorIndex;
  }

  public static MoodColor RotateColor(MoodColor color, int amount)
  {
    int colorCount = System.Enum.GetNames(typeof(MoodColor)).Length;
    int newColorIndex = ((int)color + amount) % (colorCount - 1);
    return (MoodColor)newColorIndex;
  }

  public void ShowInteractionPrompt()
  {
    m_interactionPrompt = Instantiate(GameGlobals.Instance.InteractPromptPrefab);
    m_interactionPrompt.transform.position = m_focusTransform.position;
  }

  public void HideInteractionPrompt()
  {
    if (m_interactionPrompt != null)
    {
      Destroy(m_interactionPrompt);
      m_interactionPrompt = null;
    }
  }

  public void ChooseMoodColor(MoodColor moodColorToMix)
  {
    MoodColor mixedColor = CombineColors(MoodColor, moodColorToMix);
    StartCoroutine(AnimateColorTo(mixedColor));

    if (MoodZoneActivated != null)
    {
      MoodZoneActivated(this, mixedColor);
    }
  }

  private void Start()
  {
    // Assign all child renderers a shared mood material
    if (m_moodZoneRenderers.Length > 0)
    {
      m_sharedMaterial = m_moodZoneRenderers[0].material;
      foreach (Renderer r in m_moodZoneRenderers)
        r.sharedMaterial = m_sharedMaterial;
    }

    // Initialize to our starting color
    SetRendererColors(MoodColor);
  }

  private void SetRendererColors(MoodColor moodColor)
  {
    m_sharedMaterial.SetColor("_Color", GameGlobals.Instance.MoodColors[(int)moodColor]);
  }

  private IEnumerator AnimateColorTo(MoodColor toMoodColor)
  {
    const float duration = 1.0f;
    float startTime = Time.time;
    Color currentColor = GameGlobals.Instance.MoodColors[(int)MoodColor];
    Color toColor = GameGlobals.Instance.MoodColors[(int)toMoodColor];
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      m_sharedMaterial.SetColor("_Color", Color.Lerp(currentColor, toColor, t));
      yield return null; 
    }

    startTime = Time.time + duration;
    while (Time.time < startTime + duration)
    {
      float t = (Time.time - startTime) / duration;
      m_sharedMaterial.SetColor("_Color", Color.Lerp(toColor, currentColor, t));
      yield return null; 
    }
  }
}