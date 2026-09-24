using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SxemaElectrofilter : MonoBehaviour
{
    [SerializeField] private Animator sxemElectroAnim;
    [SerializeField] private GameObject sxema;
    [SerializeField] private GameObject sxemaOriginal;
    [SerializeField] private Material electrofilterMat;

    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI dustCountText;
    [SerializeField] private TextMeshProUGUI TensionText;

    public GameObject prefab;
    public Transform target;
    public Slider slider;
    public Slider sliderMat;

    public bool movePrefabs = false;
    public bool goDown = false;

    public Transform[] stopPositions;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    // Префабы, которые должны двигаться вправо
    private HashSet<GameObject> rightMovingPrefabs = new HashSet<GameObject>();
    private List<Vector3> originalPrefabPositions = new List<Vector3>();
    private int previousSliderValue = 0;

    private float tensionCount = 0;
    private int sliderParam = 0;
    private bool isOff = false;

    private bool isActivated = false;
    private bool isPaused = false;


    public void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        sliderMat.onValueChanged.AddListener(OnSliderValueChangedTension);
    }


    // =========================================================
    // НАПРЯЖЕНИЕ
    // =========================================================

    void OnSliderValueChangedTension(float value)
    {
        TensionText.text = value.ToString("0");

        if (value <= 14)
        {
            electrofilterMat.SetColor(
                "_EmissionColor",
                new Color(0, 102, 191, 0) * 0f
            );
        }
        else if (value >= 14 && value <= 28)
        {
            electrofilterMat.SetColor(
                "_EmissionColor",
                new Color(0, 102, 191, 0) * 0.01f
            );
        }
        else if (value >= 28 && value <= 42)
        {
            electrofilterMat.SetColor(
                "_EmissionColor",
                new Color(0, 102, 191, 0) * 0.05f
            );
        }
        else if (value >= 42 && value <= 70)
        {
            electrofilterMat.SetColor(
                "_EmissionColor",
                new Color(0, 102, 191, 0) * 0.1f
            );
        }
        else if (value >= 70 && value <= 98)
        {
            electrofilterMat.SetColor(
                "_EmissionColor",
                new Color(0, 102, 191, 0) * 0.5f
            );
        }
        else if (value >= 98 && value <= 100)
        {
            electrofilterMat.SetColor(
                "_EmissionColor",
                new Color(0, 102, 191, 0) * 1f
            );
        }

        // Пересчитываем, какие частицы идут вправо
        CalculateRightMovingPrefabs();
    }


    // =========================================================
    // КОЛИЧЕСТВО ПЫЛИ
    // =========================================================

    void OnSliderValueChanged(float value)
    {
        int newValue = Mathf.FloorToInt(value);

        dustCountText.text = newValue.ToString();

        if (newValue > previousSliderValue)
        {
            int prefabsToSpawn = newValue - previousSliderValue;

            for (int i = 0; i < prefabsToSpawn; i++)
            {
                SpawnPrefab();
            }
        }
        else if (newValue < previousSliderValue)
        {
            int prefabsToRemove = previousSliderValue - newValue;

            for (int i = 0; i < prefabsToRemove; i++)
            {
                RemovePrefab();
            }
        }

        previousSliderValue = newValue;

        // Пересчитываем движение вправо
        CalculateRightMovingPrefabs();
    }


    // =========================================================
    // ОПРЕДЕЛЕНИЕ ЧАСТИЦ, КОТОРЫЕ ИДУТ ВПРАВО
    // =========================================================

    private void CalculateRightMovingPrefabs()
    {
        rightMovingPrefabs.Clear();

        int dust = Mathf.FloorToInt(slider.value);
        int tension = Mathf.FloorToInt(sliderMat.value);

        if (dust <= 0)
            return;

        // Порог = напряжение + 5%
        float threshold = tension * 1.05f;

        // Если пыли не больше чем на 5% относительно напряжения,
        // частицы вправо не идут
        if (dust <= threshold)
            return;

        // Насколько пыль превышает напряжение
        float excessPercent = (dust - tension) / (float)dust;

        // Количество частиц, которые пойдут вправо
        int rightCount = Mathf.RoundToInt(
            dust * excessPercent
        );

        // Не больше существующих префабов
        rightCount = Mathf.Clamp(
            rightCount,
            0,
            spawnedPrefabs.Count
        );

        // Назначаем первые частицы движущимися вправо
        int added = 0;

        for (int i = 0; i < spawnedPrefabs.Count; i++)
        {
            if (spawnedPrefabs[i] != null)
            {
                rightMovingPrefabs.Add(spawnedPrefabs[i]);

                added++;

                if (added >= rightCount)
                    break;
            }
        }
    }


    // =========================================================
    // УДАЛЕНИЕ ПРЕФАБА
    // =========================================================

    private void RemovePrefab()
    {
        if (spawnedPrefabs.Count > 0)
        {
            int lastIndex = spawnedPrefabs.Count - 1;

            GameObject prefabToRemove =
                spawnedPrefabs[lastIndex];

            rightMovingPrefabs.Remove(prefabToRemove);

            spawnedPrefabs.RemoveAt(lastIndex);

            if (originalPrefabPositions.Count > lastIndex)
            {
                originalPrefabPositions.RemoveAt(lastIndex);
            }

            Destroy(prefabToRemove);
        }
    }


    // =========================================================
    // КНОПКА ДВИЖЕНИЯ
    // =========================================================

    public void boolOnMove()
    {
        movePrefabs = true;
    }


    // =========================================================
    // КНОПКА ДВИЖЕНИЯ ВНИЗ
    // =========================================================

    public void boolOnDown()
    {
        goDown = true;
    }


    // =========================================================
    // ОСТАНОВКА
    // =========================================================

    public void boolsOff()
    {
        movePrefabs = false;
        goDown = false;

        slider.value = 0;
        sliderMat.value = 0;

        sxemElectroAnim.Play("Stop");
    }


    // =========================================================
    // СОЗДАНИЕ ПРЕФАБА
    // =========================================================

    void SpawnPrefab()
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            float randomX = Random.Range(-width / 2, width / 2);
            float randomY = Random.Range(-height / 2, height / 2);

            Vector3 spawnPosition =
                target.position +
                new Vector3(
                    randomX / 1000,
                    randomY / 1000,
                    0
                );

            GameObject newPrefab = Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity,
                target
            );

            RectTransform prefabRectTransform =
                newPrefab.GetComponent<RectTransform>();

            // Rotation = 0
            prefabRectTransform.localRotation = Quaternion.identity;

            // Z = 0
            prefabRectTransform.localPosition =
                new Vector3(
                    prefabRectTransform.localPosition.x,
                    prefabRectTransform.localPosition.y,
                    0
                );

            spawnedPrefabs.Add(newPrefab);

            // Запоминаем исходную позицию
            originalPrefabPositions.Add(
                prefabRectTransform.localPosition
            );
        }
    }


    // =========================================================
    // ВКЛЮЧЕНИЕ / ВЫКЛЮЧЕНИЕ СХЕМЫ
    // =========================================================

    public void OnOffSxema()
    {
        if (!isActivated)
        {
            sxema.SetActive(true);
            sxemaOriginal.SetActive(false);

            sxemElectroAnim.Play("Stop");

            isActivated = true;
        }
        else
        {
            isOff = true;

            sxema.SetActive(false);
            sxemaOriginal.SetActive(true);

            sxemElectroAnim.Play("Stop");

            isActivated = false;
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (isPaused)
            return;

        if (movePrefabs)
        {
            MovePrefabsToStopPositions();
        }

        if (isOff)
        {
            sxemElectroAnim.Play("Stop");
            isOff = false;
        }
    }


    // =========================================================
    // ДВИЖЕНИЕ ПРЕФАБОВ
    // =========================================================

    void MovePrefabsToStopPositions()
    {
        for (int i = 0; i < spawnedPrefabs.Count; i++)
        {
            GameObject currentPrefab = spawnedPrefabs[i];

            if (currentPrefab == null)
                continue;

            RectTransform prefabRectTransform =
                currentPrefab.GetComponent<RectTransform>();


            // =================================================
            // ЧАСТИЦЫ, КОТОРЫЕ ДВИЖУТСЯ ВПРАВО
            // =================================================

            if (rightMovingPrefabs.Contains(currentPrefab))
            {
                float newX =
                    prefabRectTransform.localPosition.x +
                    Time.deltaTime * 100f;

                prefabRectTransform.localPosition =
                    new Vector3(
                        newX,
                        prefabRectTransform.localPosition.y,
                        0
                    );

                continue;
            }


            // =================================================
            // ОСТАЛЬНЫЕ ЧАСТИЦЫ
            // =================================================

            if (goDown)
            {
                // Движение вниз

                float newY = Mathf.MoveTowards(
                    prefabRectTransform.localPosition.y,
                    -Screen.height,
                    Time.deltaTime * 100f
                );

                prefabRectTransform.localPosition =
                    new Vector3(
                        prefabRectTransform.localPosition.x,
                        newY,
                        0
                    );

                if (prefabRectTransform.localPosition.y <= -Screen.height)
                {
                    spawnedPrefabs[i] = null;
                }
            }
            else
            {
                // Движение к stopPosition

                Transform stopPosition =
                    stopPositions[i % stopPositions.Length];

                float newX = Mathf.MoveTowards(
                    prefabRectTransform.localPosition.x,
                    stopPosition.localPosition.x,
                    Time.deltaTime * 100f
                );

                prefabRectTransform.localPosition =
                    new Vector3(
                        newX,
                        prefabRectTransform.localPosition.y,
                        0
                    );

                if (Vector3.Distance(
                    prefabRectTransform.localPosition,
                    stopPosition.localPosition
                ) < 0.1f)
                {
                    spawnedPrefabs[i] = null;
                }
            }
        }
    }


    // =========================================================
    // ЗАПУСК АНИМАЦИИ
    // =========================================================

    public void SxemaStart()
    {
        sxemElectroAnim.Play("Main");
    }


    // =========================================================
    // PAUSE
    // =========================================================

    public void PauseSxema()
    {
        if (!isPaused)
        {
            sxemElectroAnim.speed = 0;

            isPaused = true;
        }
        else
        {
            sxemElectroAnim.speed = 1;

            isPaused = false;
        }
    }


    // =========================================================
    // STOP
    // =========================================================

    public void SxemasStop()
    {
        movePrefabs = false;
        goDown = false;

        for (int i = 0; i < spawnedPrefabs.Count; i++)
        {
            if (spawnedPrefabs[i] != null &&
                i < originalPrefabPositions.Count)
            {
                RectTransform prefabRectTransform =
                    spawnedPrefabs[i].GetComponent<RectTransform>();

                prefabRectTransform.localPosition =
                    originalPrefabPositions[i];

                // На всякий случай возвращаем Rotation = 0
                prefabRectTransform.localRotation =
                    Quaternion.identity;
            }
        }

        // Убираем назначение движения вправо
        rightMovingPrefabs.Clear();

        // Останавливаем анимацию
        sxemElectroAnim.Play("Stop");
    }
}