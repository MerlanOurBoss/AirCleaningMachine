using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadRightPanel : MonoBehaviour
{
    [SerializeField] private GameObject parentComponent;

    [SerializeField] private GameObject rootToComponent;
    
    [Header("Main Gas Flow")]
    [SerializeField] private TMP_InputField _gasFlow;

    [Header("Electrofilter")]
    [SerializeField] private TextMeshProUGUI _temperatureElectro;
    [SerializeField] private TextMeshProUGUI _solidParticleElectro;
    [SerializeField] private Animator _collec;
    
    
    [SerializeField] private GameObject[] prefabs;

    private List<Button> spawnedButtons = new List<Button>();
    private List<GameObject> spawnedPrefabs = new List<GameObject>();
    private Button currentlySelectedButton;
    private GameObject currentlySelectedPrefab;

    private Dictionary<GameObject, Vector3> originalDetailsPositions = new Dictionary<GameObject, Vector3>();
    private void Start()
    {
        if (parentComponent == null || rootToComponent == null || prefabs == null)
        {

            return;
        }

        SpawnPrefabs();
        SetupButtonLogic();
    }

    private void SpawnPrefabs()
    {
        Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null && !prefabDictionary.ContainsKey(prefab.name))
            {
                prefabDictionary.Add(prefab.name, prefab);
            }
        }

        foreach (Transform child in parentComponent.transform)
        {
            string childName = child.gameObject.name;

            if (childName.Contains("(Clone)"))
            {
                childName = childName.Replace("(Clone)", "").Trim();
            }

            if (prefabDictionary.TryGetValue(childName, out GameObject matchingPrefab))
            {
                GameObject spawnedObject = Instantiate(matchingPrefab, rootToComponent.transform);
                spawnedPrefabs.Add(spawnedObject);

                Button button = spawnedObject.GetComponent<Button>();
                if (button != null)
                {
                    spawnedButtons.Add(button);
                }
                
                SaveOriginalDetailsPosition(spawnedObject);
                SetDetailsActive(spawnedObject, false);
                
                if (spawnedObject.name == "ElectroFilter(Clone)")
                {
                    var script = spawnedObject.GetComponent<MathModuleForElectro>();
                    script._gasFlow = _gasFlow;
                    script._temperature = _temperatureElectro;
                    script._solidParticle = _solidParticleElectro;
                    
                    script._electroFilterAnimator = _collec;
                }
                else if(spawnedObject.name == "New Katalizator(Clone)")
                {
                    var script = spawnedObject.GetComponent<MathModulForKataz>();
                    script._gasFlowMain = _gasFlow;
                }
                else if(spawnedObject.name == "Emul(Clone)")
                {
                    var script = spawnedObject.GetComponent<MathModuleForEmul>();
                    script._gasFlowMain = _gasFlow;
                }
                else if(spawnedObject.name == "New Sbor(Clone)")
                {
                    var script = spawnedObject.GetComponent<MathModulForSborCO2>();
                    script._gasFlowMain = _gasFlow;
                }
            }
        }

        StartCoroutine(ActivateFirstItemDelayed());
    }

    private void SetupButtonLogic()
    {
        foreach (Button button in spawnedButtons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }
    private IEnumerator ActivateFirstItemDelayed()
    {
        yield return new WaitForEndOfFrame();

        if (spawnedButtons.Count > 0 && spawnedPrefabs.Count > 0)
        {
            Button firstButton = spawnedButtons[0];
            GameObject firstPrefab = spawnedPrefabs[0];

            SetDetailsActive(firstPrefab, true);
            MoveDetailsToTop(firstPrefab);

            currentlySelectedButton = firstButton;
            currentlySelectedPrefab = firstPrefab;
        }
    }
    private void OnButtonClick(Button clickedButton)
    {
        if (currentlySelectedButton == clickedButton)
        {
            ResetSelection();
            return;
        }

        ResetAllDetails();

        GameObject clickedPrefab = clickedButton.gameObject;
        SetDetailsActive(clickedPrefab, true);
        MoveDetailsToTop(clickedPrefab);

        currentlySelectedButton = clickedButton;
        currentlySelectedPrefab = clickedPrefab;
    }

    private void SaveOriginalDetailsPosition(GameObject prefabObject)
    {
        Transform detailsTransform = FindChildRecursive(prefabObject.transform, "Details");
        if (detailsTransform != null)
        {
            originalDetailsPositions[detailsTransform.gameObject] = detailsTransform.localPosition;
        }
    }

    private void SetDetailsActive(GameObject prefabObject, bool active)
    {
        Transform detailsTransform = FindChildRecursive(prefabObject.transform, "Details");
        if (detailsTransform != null)
        {
            detailsTransform.gameObject.SetActive(active);
        }
        else
        {
            Debug.LogWarning($"Object 'Details' dont found in {prefabObject.name}");
        }
    }

    private void MoveDetailsToTop(GameObject prefabObject)
    {
        Transform detailsTransform = FindChildRecursive(prefabObject.transform, "Details");
        if (detailsTransform != null)
    {
        RectTransform detailsRect = detailsTransform.GetComponent<RectTransform>();
            if (detailsRect != null)
            {
                RectTransform rootRect = prefabObject.GetComponent<RectTransform>();
                float parentY = rootRect.anchoredPosition.y;
                Vector2 newPosition = detailsRect.anchoredPosition;
                newPosition.y = newPosition.y + (parentY*-1) - 100f;

                detailsRect.anchoredPosition = newPosition;
            }
        }
    }

    private void RestoreDetailsPosition(GameObject prefabObject)
    {
        Transform detailsTransform = FindChildRecursive(prefabObject.transform, "Details");
        if (detailsTransform != null && originalDetailsPositions.ContainsKey(detailsTransform.gameObject))
        {
            detailsTransform.localPosition = originalDetailsPositions[detailsTransform.gameObject];
        }
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindChildRecursive(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }

    private void ResetAllDetails()
    {
        foreach (GameObject prefab in spawnedPrefabs)
        {
            RestoreDetailsPosition(prefab);
            SetDetailsActive(prefab, false);
        }

        currentlySelectedButton = null;
        currentlySelectedPrefab = null;
    }

    public void ResetSelection()
    {
        ResetAllDetails();
    }

    public Button GetSelectedButton()
    {
        return currentlySelectedButton;
    }

    public GameObject GetSelectedPrefab()
    {
        return currentlySelectedPrefab;
    }

    public string GetSelectedPrefabName()
    {
        return currentlySelectedPrefab != null ? currentlySelectedPrefab.name.Replace("(Clone)", "").Trim() : "";
    }

    public bool IsDetailsActive(GameObject prefabObject)
    {
        Transform detailsTransform = FindChildRecursive(prefabObject.transform, "Details");
        return detailsTransform != null && detailsTransform.gameObject.activeSelf;
    }
}
