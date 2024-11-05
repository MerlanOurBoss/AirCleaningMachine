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

    public GameObject prefab; // ������, ������� ����� ��������
    public Transform target;  // ������� ������ ��� ������
    public Slider slider;     // �������
    public bool movePrefabs = false; // ���� ��� �������� ��������
    public bool goDown = false;
    public Transform[] stopPositions; // ������ ������� ���������

    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private int previousSliderValue = 0;


    private bool isOff = false;

    private bool isActivated = false;

    private bool isPaused = false;

    public void Start()
    {
        pauseText.text = "Pause";
        slider.onValueChanged.AddListener(OnSliderValueChanged);
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

    public void boolOn(bool a)
    {
        a = true;
    }

    public void boolsOff()
    {
        movePrefabs = false; // ���� ��� �������� ��������
        goDown = false;
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

    private void Update()
    {
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
    public void SxemaStart()
    {
        sxemElectroAnim.Play("Main");
    }


    public void PauseSxema()
    {
        if (!isPaused)
        {
            pauseText.text = "Resume";
            sxemElectroAnim.speed = 0;
            isPaused = true;
        }
        else
        {
            pauseText.text = "Pause";
            sxemElectroAnim.speed = 1;
            isPaused = false;
        }
    }
    public void SxemasStop()
    {
        sxemElectroAnim.Play("Stop");
    }
}

public class PrefabMover : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool moveForward;

    public void SetTargetPosition(Vector3 position, bool shouldMove)
    {
        targetPosition = position;
        moveForward = shouldMove;
    }

    void Update()
    {
        if (moveForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);
            if (transform.position == targetPosition)
            {
                moveForward = false;
            }
        }
    }
}