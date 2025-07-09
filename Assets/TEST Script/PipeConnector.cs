using InstantPipes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnector : MonoBehaviour
{
    [System.Serializable]
    public class SystemComponent
    {
        public GameObject component;
        public Transform input;
        public Transform output;
    }

    public List<SystemComponent> components = new List<SystemComponent>();
    public PipeGenerator pipeGenerator; // Ссылка на PipeGenerator в сцене
    public PipeGenerator pipeGeneratorCO2; // Ссылка на PipeGeneratorCO2 в сцене

    private HashSet<GameObject> trackedFacilities = new HashSet<GameObject>();
    private void Start()
    {
        if (pipeGenerator == null)
        {
            Debug.LogError("PipeGenerator is not assigned!");
            return;
        }
    }

    private void Update()
    {
        bool anyNewFound = false;

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Untagged") || trackedFacilities.Contains(obj))
                continue;

            if (obj.tag.StartsWith("Facilities_"))
            {
                Transform input = FindChildByTag(obj.transform, "ConnectionPointIN");
                Transform output = FindChildByTag(obj.transform, "ConnectionPointOUT");

                if (input != null || output != null)
                {
                    SystemComponent component = new SystemComponent
                    {
                        component = obj,
                        input = input,
                        output = output
                    };

                    components.Add(component);
                    trackedFacilities.Add(obj);
                    anyNewFound = true;
                    
                }
                else
                {
                    Debug.LogWarning($"Missing connection points on {obj.name}");
                }
            }
        }
    }

    private Transform FindChildByTag(Transform parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag(tag))
                return child;
        }
        return null;
    }

    public void ConnectComponents()
    {
        bool hasCO2Facilities = components.Exists(c =>
                c.component.tag == "Facilities_Kataz" || c.component.tag == "Facilities_NewCapsul");

        for (int i = 0; i < components.Count - 1; i++)
        {
            SystemComponent fromComp = components[i];
            SystemComponent toComp = components[i + 1];

            // Всегда стандартные соединения
            if (fromComp.output != null && toComp.input != null)
            {
                CreatePipeConnection(fromComp.output, toComp.input, useCO2: false);
            }
            else
            {
                Debug.LogWarning($"Standard connection points missing between {fromComp.component.name} and {toComp.component.name}");
            }

            // Дополнительные CO2 соединения, если есть CO2-объекты
            if (hasCO2Facilities)
            {
                SystemComponent fromComp2 = components.Find(c => c.component.tag == "Facilities_Kataz");
                SystemComponent toComp2 = components.Find(c => c.component.tag == "Facilities_NewCapsul");
                Debug.Log(fromComp2 + " b " + toComp2);
                if (fromComp2 != null && toComp2 != null)
                {
                    Transform fromCO2 = FindChildByTag(fromComp2.component.transform, "ConnectionPointCO2OUT");
                    Transform toCO22 = FindChildByTag(toComp2.component.transform, "ConnectionPointCO2OUT");

                    Transform fromCO22 = FindChildByTag(fromComp2.component.transform, "ConnectionPointCO2IN");
                    Transform toCO2 = FindChildByTag(toComp2.component.transform, "ConnectionPointCO2IN");

                    if (fromCO2 != null && toCO2 != null)
                    {
                        CreatePipeConnection(fromCO2, toCO22, useCO2: true);
                        CreatePipeConnection(fromCO22, toCO2, useCO2: true);
                    }
                    else
                    {
                        Debug.LogWarning("Не найдены CO2-точки для Facilities_Kataz и Facilities_NewCapsul");
                    }
                }
                else
                {
                    Debug.LogWarning("Один из объектов Facilities_Kataz или Facilities_NewCapsul не найден в списке компонентов.");
                }
            }
        }

        pipeGenerator.UpdateMesh();
        if (hasCO2Facilities && pipeGeneratorCO2 != null)
        {
            pipeGeneratorCO2.UpdateMesh();
        }
    }

    private void CreatePipeConnection(Transform from, Transform to, bool useCO2)
    {
        Vector3 start = from.position;
        Vector3 startNormal = from.forward;

        Vector3 end = to.position;
        Vector3 endNormal = to.forward;

        PipeGenerator generator = useCO2 ? pipeGeneratorCO2 : pipeGenerator;

        if (generator == null)
        {
            Debug.LogError("PipeGenerator is missing for " + (useCO2 ? "CO2" : "standard") + " connection.");
            return;
        }

        bool success = generator.AddPipe(start, startNormal, end, endNormal);

        if (!success)
        {
            Debug.LogWarning($"Pipe between {from.name} and {to.name} failed to generate.");
        }
    }

    public void ClearAllPipesAndFacilities()
    {
        if (pipeGenerator != null)
        {
            pipeGenerator.Clear();
            pipeGenerator.UpdateMesh();
        }

        for (int i = components.Count - 1; i >= 0; i--)
        {
            var comp = components[i];

            if (comp.component != null)
            {
                if (Application.isPlaying)
                    DestroyImmediate(comp.component); // гарантированное немедленное удаление
                else
                    DestroyImmediate(comp.component);
            }

            components.RemoveAt(i); // работает 100% потому что объект уже удалён
        }

        trackedFacilities.Clear();
    }
}
