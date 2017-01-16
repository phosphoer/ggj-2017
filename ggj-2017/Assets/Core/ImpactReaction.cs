using UnityEngine;

public class ImpactReaction : MonoBehaviour
{
  public enum CollisionType
  {
    Physical,
    Trigger,
    Both
  }

  public CollisionType Type;
  public LayerMask CollideWith;
  public string ComponentFilter;
  public bool DestroyComponentOnImpact;
  public bool DestroyGameObjectOnImpact;

  public event System.Action<GameObject, bool> ImpactBegin;

  private void OnTriggerEnter(Collider c)
  {
    if ((CollideWith.value & (1 << c.gameObject.layer)) != 0)
    {
      if ((Type == CollisionType.Trigger || Type == CollisionType.Both) && ValidateCollision(c.gameObject))
      {
        if (ImpactBegin != null)
          ImpactBegin(c.gameObject, true);

        BaseImpact();
      }
    }
  }

  private void OnCollisionEnter(Collision c)
  {
    if ((CollideWith.value & (1 << c.gameObject.layer)) != 0)
    {
      if ((Type == CollisionType.Physical || Type == CollisionType.Both) && ValidateCollision(c.gameObject))
      {
        if (ImpactBegin != null)
          ImpactBegin(c.gameObject, false);

        BaseImpact();
      }
    }
  }

  private void BaseImpact()
  {
    if (DestroyComponentOnImpact)
    {
      Destroy(this);
    }
    else if (DestroyGameObjectOnImpact)
    {
      Destroy(gameObject);
    }
  }

  protected virtual bool ValidateCollision(GameObject obj)
  {
    if (!string.IsNullOrEmpty(ComponentFilter))
    {
      return obj.GetComponent(ComponentFilter) != null;
    }
    return true;
  }

}