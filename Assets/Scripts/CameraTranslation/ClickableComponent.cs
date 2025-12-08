using UnityEngine;

[DisallowMultipleComponent]
public class ClickableComponent : MonoBehaviour
{
    [Tooltip("Камера, которая показывает ТОЛЬКО этот компонент")]
    [SerializeField] private Camera linkedCamera;

    public Camera LinkedCamera => linkedCamera;
}