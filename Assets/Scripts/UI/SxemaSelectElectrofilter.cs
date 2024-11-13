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

    public GameObject prefab; // ������, ������� ����� ��������
    public Transform target;  // ������� ������ ��� ������
    public Slider slider;     // �������
    public Slider sliderMat;     
    public bool movePrefabs = false; // ���� ��� �������� ��������
    public bool goDown = false;
    public Transform[] stopPositions; // ������ ������� ���������

    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private int previousSliderValue = 0;

    private bool isOff = false;

    public void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        sliderMat.onValueChanged.AddListener(OnSliderValueChangedTension);
    }
    void OnSliderValueChangedTension(float value)
    {
        // Update UI text
        TensionText.text = value.ToString("0");

        // Update material emission color based on slider value ranges
        if (value <= 14)
        {
            electrofilterMat.SetColor("_EmissionColor", new Color(0, 102, 191, 0) * 0f);
        }
        else if (value >= 14 && value <= 28)
        {
            electrofilterMat.SetColor("_EmissionColor", new Color(0, 102, 191, 0) * 0.01f);
        }
        else if (value >= 28 && value <= 42)
        {
            electrofilterMat.SetColor("_EmissionColor", new Color(0, 102, 191, 0) * 0.05f);
        }
        else if (value >= 42 && value <= 70)
        {
            electrofilterMat.SetColor("_EmissionColor", new Color(0, 102, 191, 0) * 0.1f);
        }
        else if (value >= 70 && value <= 98)
        {
            electrofilterMat.SetColor("_EmissionColor", new Color(0, 102, 191, 0) * 0.5f);
        }
        else if (value >= 98 && value <= 100)
        {
            electrofilterMat.SetColor("_EmissionColor", new Color(0, 102, 191, 0) * 1f);
        }
    }
    void OnSliderValueChanged(float value)
    {
        int newValue = Mathf.FloorToInt(value);
        dustCountText.text = newValue.ToString();
        // ���� �������� �������� ������ �����������, �� ������� ����������� �������
        if (newValue > previousSliderValue)
        {
            int prefabsToSpawn = newValue - previousSliderValue;
            for (int i = 0; i < prefabsToSpawn; i++)
            {
                SpawnPrefab();
            }
            
        }
        // ���� �������� �������� ������, �� ������� ������ ������� (�����������)
        else if (newValue < previousSliderValue)
        {
            int prefabsToRemove = previousSliderValue - newValue;
            for (int i = 0; i < prefabsToRemove; i++)
            {
                RemovePrefab();
            }
        }

        previousSliderValue = newValue;
    }
    private void RemovePrefab()
    {
        if (spawnedPrefabs.Count > 0)
        {
            // ������� ��������� ���������� ������
            GameObject prefabToRemove = spawnedPrefabs[spawnedPrefabs.Count - 1];
            spawnedPrefabs.RemoveAt(spawnedPrefabs.Count - 1);
            Destroy(prefabToRemove);
        }
    }

    public void OnMouseSelect()
    {
        // Generate random number of prefabs to spawn
        int randomPrefabCount = Random.Range(1, 100); // Adjust range as needed
        slider.value = randomPrefabCount;
        for (int i = 0; i < randomPrefabCount; i++)
        {
            SpawnPrefab();
        }

        // Set a random tension value
        float randomTensionValue = Random.Range(0, 100); // Adjust range based on slider min/max
        sliderMat.value = randomTensionValue;

        // Start the coroutine to delay animation start
        StartCoroutine(StartAnimationAfterDelay(3f)); // 3-second delay
    }

    public void OnMouseDiselected()
    {
        // Destroy all spawned prefabs and clear the list
        foreach (var prefab in spawnedPrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }
        spawnedPrefabs.Clear();

        // Reset other properties if needed
        movePrefabs = false;
        goDown = false;
        slider.value = 0;
        sliderMat.value = 0;

        // Optionally stop any animations or reset UI
        sxemElectroAnim.Play("Stop"); // Adjust to your needs
    }

    private IEnumerator StartAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        sxemElectroAnim.Play("Main");
    }

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
        movePrefabs = false; // ���� ��� �������� ��������
        goDown = false;
        slider.value = 0;
        sliderMat.value = 0;
    }
    void SpawnPrefab()
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            // ���������� ��������� ���������� �� X � Y ������ ������ target
            float randomX = Random.Range(-width / 2, width / 2);
            float randomY = Random.Range(-height / 2, height / 2);

            // ������ ������� ������ ������������ target
            Vector3 spawnPosition = target.position + new Vector3(randomX / 1000, randomY / 1000,0);
            // ������� ������ �� ��������������� �������
            GameObject newPrefab = Instantiate(prefab, spawnPosition, new Quaternion(0,0,0,0), target);
            RectTransform prefabRectTransform = newPrefab.GetComponent<RectTransform>();

            // ������������� ������� �� Z � 0 ��� UI
            prefabRectTransform.localPosition = new Vector3(prefabRectTransform.localPosition.x, prefabRectTransform.localPosition.y, 0);
            spawnedPrefabs.Add(newPrefab);
        }
    }


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

    void MovePrefabsToStopPositions()
    {
        for (int i = 0; i < spawnedPrefabs.Count; i++)
        {
            GameObject prefab = spawnedPrefabs[i];
            if (prefab != null)
            {
                RectTransform prefabRectTransform = prefab.GetComponent<RectTransform>();

                if (goDown)
                {
                    // ������� ������ ���� �� ��� Y
                    float newY = Mathf.MoveTowards(prefabRectTransform.localPosition.y, -Screen.height, Time.deltaTime * 100f); // -Screen.height ��� ����������� ���� �� �����
                    prefabRectTransform.localPosition = new Vector3(prefabRectTransform.localPosition.x, newY, prefabRectTransform.localPosition.z);

                    // ������������� ��������, ���� ������ ������ ������ ������� ������
                    if (prefabRectTransform.localPosition.y <= -Screen.height)
                    {
                        spawnedPrefabs[i] = null; // ���������� �������� ��� ������������ �������
                    }
                }
                else
                {
                    Transform stopPosition = stopPositions[i % stopPositions.Length];
                    // ���������� localPosition ��� UI-��������
                    float newX = Mathf.MoveTowards(prefabRectTransform.localPosition.x, stopPosition.localPosition.x, Time.deltaTime * 100f);
                    prefabRectTransform.localPosition = new Vector3(newX, prefabRectTransform.localPosition.y, prefabRectTransform.localPosition.z); ;
                    // �������� ���������� ����� ������� �������� � �������� ���������
                    if (Vector3.Distance(prefabRectTransform.localPosition, stopPosition.localPosition) < 0.1f)
                    {
                        spawnedPrefabs[i] = null; // ���������� �������� ��� ������������ �������
                    }
                }
            }
        }
    }
}
