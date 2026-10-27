using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CopyObject : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject prefab;       // Перетащите префаб в инспекторе
    [SerializeField] private int count = 5;           // Количество копий
    public float spacing = 1900f;      // Расстояние между копиями по Z
    
    [SerializeField] private UIManager uimanager;

    [Header("Активная установка")]
    [Tooltip("Копия (0 — первая), с которой работают LoadSmoke (расчёт, пуск/остановка, а с ними мониторинг), " +
             "правая панель (матмодули) и стена камер. Аппараты остальных копий не считаются и не запускаются, " +
             "их камеры выключаются.")]
    [FormerlySerializedAs("cameraInstallationIndex")]
    [SerializeField] private int activeInstallationIndex = 0;
    [Tooltip("Пусто — ищется на сцене.")]
    [SerializeField] private CameraSequenceController cameraSequenceController;
    [Tooltip("Пусто — ищется на сцене.")]
    [SerializeField] private LoadSmoke loadSmoke;
    [Tooltip("Пусто — ищется на сцене.")]
    [SerializeField] private LoadRightPanel rightPanel;

    void Start()
    {
        if (prefab == null)
        {
            Debug.LogError("Префаб не назначен!");
            return;
        }

        int activeIndex = Mathf.Clamp(activeInstallationIndex, 0, Mathf.Max(count - 1, 0));
        Transform activeInstallation = null;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - i * spacing
            );

            GameObject sbor = Instantiate(prefab, spawnPosition, Quaternion.identity);
            uimanager.RegisterUI(sbor);
            PipeConnector pipe = sbor.GetComponent<PipeConnector>();

            StartCoroutine(ConnectAfterDelay(pipe));

            if (i == activeIndex) activeInstallation = sbor.transform;
        }

        if (activeInstallation == null) return;

        // После цикла: камеры в префабе включены, и выключить нужно камеры всех копий, а не только уже созданных.
        if (cameraSequenceController == null)
            cameraSequenceController = FindAnyObjectByType<CameraSequenceController>();
        if (cameraSequenceController != null)
            cameraSequenceController.SetCameraScope(activeInstallation);

        // Аппараты появились только сейчас — LoadSmoke мог собрать цепочку раньше, пустой.
        if (loadSmoke == null)
            loadSmoke = FindAnyObjectByType<LoadSmoke>();
        if (loadSmoke != null)
            loadSmoke.SetModuleScope(activeInstallation);

        // Матмодули правой панели — к менеджерам той же установки, что считает LoadSmoke.
        if (rightPanel == null)
            rightPanel = FindAnyObjectByType<LoadRightPanel>();
        if (rightPanel != null)
            rightPanel.SetParentComponent(activeInstallation.gameObject);
    }
    private IEnumerator ConnectAfterDelay(PipeConnector pipe)
    {
        yield return new WaitForSeconds(2f);
        pipe.ConnectComponents();
    }
}
