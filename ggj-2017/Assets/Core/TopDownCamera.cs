using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
  public Transform Parent;

  private float _initialYRotationOffset;
  private float _initialOffsetDistance;
  private float _initialHeight;

  private void Start()
  {
    Vector3 parentForwardFlat = new Vector3(Parent.forward.x, 0, Parent.forward.z);
    Vector3 offset = transform.position - Parent.position;
    Vector3 offsetFlat = new Vector3(offset.x, 0, offset.z);
    _initialHeight = offset.y;
    _initialOffsetDistance = offsetFlat.magnitude;
    _initialYRotationOffset = Vector3.Angle(parentForwardFlat.normalized, offsetFlat.normalized);
  }

  private void Update()
  {
    Vector3 parentForwardFlat = new Vector3(Parent.forward.x, 0, Parent.forward.z);
    Vector3 offsetPos = Quaternion.Euler(0, _initialYRotationOffset, 0) * (parentForwardFlat.normalized * _initialOffsetDistance);
    offsetPos += Vector3.up * _initialHeight;
    transform.position = Parent.position + offsetPos;
    transform.LookAt(Parent, Vector3.up);
  }
}