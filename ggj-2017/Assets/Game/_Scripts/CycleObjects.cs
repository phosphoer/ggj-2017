using UnityEngine;

public class CycleObjects : MonoBehaviour
{
  public GameObject[] Objects;
  public float CycleTime = 1.0f;

  private float lastCycle;
  private int index;

  public void SetIndex(int newIndex)
  {
    Objects[index].SetActive(false);
    index = Mathf.Clamp(newIndex, 0, Objects.Length - 1);
    Objects[index].SetActive(true);
  }

  private void Start()
  {
    for (var i = 0; i < Objects.Length; ++i)
      Objects[i].SetActive(i == 0);
  }

  private void Update()
  {
    if (Objects.Length == 0 || CycleTime == 0.0f)
      return;

    if (Time.unscaledTime > lastCycle + CycleTime)
    {
      lastCycle = Time.unscaledTime;
      Objects[index].SetActive(false);

      ++index;
      if (index >= Objects.Length)
        index = 0;

      Objects[index].SetActive(true);
    }
  }
}