using UnityEngine;
using System.Collections;

public static class Tween
{
  public static IEnumerator HermiteScale(Transform transform, Vector3 startScale, Vector3 endScale, float duration)
  {
    float startTime = Time.time;
    while (Time.time < startTime + duration)
    {
      float t = Mathf.Clamp01((Time.time - startTime) / duration);
      t = Mathfx.Hermite(0.0f, 1.0f, t);
      transform.localScale = Vector3.Lerp(startScale, endScale, t);
      yield return null;
    }
  }
}