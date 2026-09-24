using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SxemaSelectElectrofilter : MonoBehaviour
{
    [SerializeField] private Animator sxemElectroAnim;
    [SerializeField] private Material electrofilterMat;

    [SerializeField] private TextMeshProUGUI dustCountText;
    [SerializeField] private TextMeshProUGUI TensionText;

    public GameObject prefab;
    public Transform target;

    public Slider slider;
    public Slider sliderMat;

    public bool movePrefabs = false;
    public bool goDown = false;

    public GameObject child;

    public Transform[] stopPositions;

    // Точка, куда будут уходить частицы вправо
    [SerializeField] private Transform rightTarget;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    // Частицы, которые должны двигаться вправо
    private HashSet<GameObject> rightMovingPrefabs = new HashSet<GameObject>();

    private int previousSliderValue = 0;

    private bool isOff = false;


    public void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        sliderMat.onValueChanged.AddListener(OnSliderValueChangedTension);

        child.SetActive(false);
    }


    // =========================================================
    // НАПРЯЖЕНИЕ
    // =========================================================

    void OnSliderValueChangedTension(float value)
    {
        TensionText.text = value.ToString("0");

        electrofilterMat.SetColor(
            "_EmissionColor",
            new Color(0, 102, 191, 0) *
            Mathf.Lerp(0, 1, value / sliderMat.maxValue)
        );

        // Пересчитываем частицы, которые должны идти вправо
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

        // После изменения количества пыли
        // пересчитываем распределение
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

        // Если пыли не больше допустимого уровня,
        // все частицы идут вниз
        if (dust <= threshold)
            return;

        // Насколько количество пыли превышает напряжение
        float excessPercent = (dust - tension) / (float)dust;

        // Сколько частиц отправить вправо
        int rightCount = Mathf.RoundToInt(dust * excessPercent);

        rightCount = Mathf.Clamp(
            rightCount,
            0,
            spawnedPrefabs.Count
        );

        // Выбираем первые частицы для движения вправо
        for (int i = 0; i < rightCount; i++)
        {
            if (spawnedPrefabs[i] != null)
            {
                rightMovingPrefabs.Add(spawnedPrefabs[i]);
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

            GameObject prefabToRemove = spawnedPrefabs[lastIndex];

            spawnedPrefabs.RemoveAt(lastIndex);

            if (prefabToRemove != null)
            {
                rightMovingPrefabs.Remove(prefabToRemove);
                Destroy(prefabToRemove);
            }
        }
    }


    // =========================================================
    // ВЫБОР ЭЛЕКТРОФИЛЬТРА
    // =========================================================

    public void OnMouseSelect()
    {
        child.SetActive(true);

        // Случайное количество пыли
        int randomPrefabCount = Random.Range(1, 100);

        slider.value = randomPrefabCount;

        // Случайное напряжение
        float randomTensionValue = Random.Range(0, 100);

        sliderMat.value = randomTensionValue;

        // Запуск анимации через 3 секунды
        StartCoroutine(StartAnimationAfterDelay(3f));
    }


    // =========================================================
    // ВЫХОД ИЗ ЭЛЕКТРОФИЛЬТРА
    // =========================================================

    public void OnMouseDiselected()
    {
        sxemElectroAnim.Play("StopInSheme");

        foreach (var prefab in spawnedPrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }

        spawnedPrefabs.Clear();
        rightMovingPrefabs.Clear();

        movePrefabs = false;
        goDown = false;

        slider.value = 0;
        sliderMat.value = 0;

        previousSliderValue = 0;

        child.SetActive(false);
    }


    // =========================================================
    // ЗАПУСК АНИМАЦИИ
    // =========================================================

    private IEnumerator StartAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        sxemElectroAnim.Play("MainInSheme");
    }


    // =========================================================
    // УПРАВЛЕНИЕ ДВИЖЕНИЕМ
    // =========================================================

    public void boolOnMove()
    {
        movePrefabs = true;
    }


    public void boolOnDown()
    {
        goDown = true;
    }


    public void boolsOff()
    {
        movePrefabs = false;
        goDown = false;

        slider.value = 0;
        sliderMat.value = 0;
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

            prefabRectTransform.localPosition =
                new Vector3(
                    prefabRectTransform.localPosition.x,
                    prefabRectTransform.localPosition.y,
                    0
                );
            
            prefabRectTransform.localRotation = Quaternion.identity;
            
            spawnedPrefabs.Add(newPrefab);
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (movePrefabs)
        {
            MovePrefabsToStopPositions();
        }

        if (isOff)
        {
            isOff = false;
        }
    }


    // =========================================================
    // ДВИЖЕНИЕ ЧАСТИЦ
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


            // =====================================================
            // ДВИЖЕНИЕ ВПРАВО
            // =====================================================

            if (rightMovingPrefabs.Contains(currentPrefab))
            {
                float newX = prefabRectTransform.localPosition.x +
                             Time.deltaTime * 100f;

                prefabRectTransform.localPosition =
                    new Vector3(
                        newX,
                        prefabRectTransform.localPosition.y,
                        prefabRectTransform.localPosition.z
                    );

                continue;
            }


            // =====================================================
            // ДВИЖЕНИЕ ВНИЗ
            // =====================================================

            if (goDown)
            {
                float newY = Mathf.MoveTowards(
                    prefabRectTransform.localPosition.y,
                    -Screen.height,
                    Time.deltaTime * 100f
                );

                prefabRectTransform.localPosition =
                    new Vector3(
                        prefabRectTransform.localPosition.x,
                        newY,
                        prefabRectTransform.localPosition.z
                    );

                if (prefabRectTransform.localPosition.y <= -Screen.height)
                {
                    spawnedPrefabs[i] = null;
                }
            }


            // =====================================================
            // ДВИЖЕНИЕ К STOP POSITIONS
            // =====================================================

            else
            {
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
                        prefabRectTransform.localPosition.z
                    );

                if (
                    Vector3.Distance(
                        prefabRectTransform.localPosition,
                        stopPosition.localPosition
                    ) < 0.1f
                )
                {
                    spawnedPrefabs[i] = null;
                }
            }
        }
    }
}