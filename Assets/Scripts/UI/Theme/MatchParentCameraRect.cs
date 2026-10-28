using UnityEngine;

/// <summary>
/// Держит у UI-камеры ту же область вывода и параметры проекции, что у основной камеры (родителя),
/// и глубину на единицу больше — чтобы клики по UI не перехватывала 3D-сцена.
/// Канвас Screen Space – Camera раскладывается по области своей камеры, а основная камера рисует
/// только в окне 3D-вида, и её область меняет AspectRatioEnforcer во время работы.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class MatchParentCameraRect : MonoBehaviour
{
    [SerializeField] private Camera source;

    private Camera _camera;

    private void OnEnable()
    {
        _camera = GetComponent<Camera>();
        if (source == null && transform.parent != null) source = transform.parent.GetComponent<Camera>();
        Sync();
    }

    private void LateUpdate() => Sync();

    private void Sync()
    {
        if (source == null || _camera == null) return;
        if (_camera.rect != source.rect) _camera.rect = source.rect;
        if (!Mathf.Approximately(_camera.fieldOfView, source.fieldOfView)) _camera.fieldOfView = source.fieldOfView;
        if (_camera.orthographic != source.orthographic) _camera.orthographic = source.orthographic;
        // Глубина выше основной: EventSystem отдаёт клик камере с большей глубиной, и UI не должен уступать 3D-сцене.
        if (_camera.depth <= source.depth) _camera.depth = source.depth + 1;
    }
}
