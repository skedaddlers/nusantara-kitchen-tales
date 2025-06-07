using UnityEngine;

[System.Serializable]
public abstract class Step : MonoBehaviour
{
    public virtual bool isActive { get; protected set; } = false;
    public abstract void DisableStep();
}