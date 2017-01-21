using UnityEngine;

public class FaceCamera : MonoBehaviour
{
  public bool FlipForward;

  private void Update()
  {
    Vector3 toCamera = Camera.main.transform.position - transform.position;
    toCamera.y = 0;

    transform.rotation = Quaternion.LookRotation(toCamera.normalized * (FlipForward ? -1 : 1), Vector3.up);
  }
}