using InstantPipes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public PipeGenerator pipeGenerator;
    public PipeGenerator pipeGeneratorCO2;
    public GameObject pipeMeshStandardPrefab;
    private HashSet<GameObject> trackedFacilities = new HashSet<GameObject>();
    // Объекты, о которых уже предупредили (раньше LogWarning писался каждый кадр).
    private HashSet<GameObject> warnedFacilities = new HashSet<GameObject>();
    private bool warnedNoSborCO2;
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
        GameObject sborCO2 = null;
        if (transform.tag == "SborCO2")
        {
            sborCO2 = transform.gameObject;
        }
        else
        {
            sborCO2 = GameObject.FindGameObjectWithTag("SborCO2");
        }
        
        if (sborCO2 == null)
        {
            if (!warnedNoSborCO2) Debug.LogWarning("SborCO2 object not found!");
            warnedNoSborCO2 = true;
            return;
        }
        warnedNoSborCO2 = false;

        foreach (Transform child in sborCO2.transform.GetComponentsInChildren<Transform>())
        {
            GameObject obj = child.gameObject;

            if (!obj.tag.StartsWith("Facilities_") || trackedFacilities.Contains(obj) || warnedFacilities.Contains(obj))
                continue;

            Transform input = FindChildByTag(obj.transform, "ConnectionPointIN");
            Transform output = FindChildByTag(obj.transform, "ConnectionPointOUT");

            if (input != null || output != null)
            {

                var comp = new SystemComponent
                {
                    component = obj,
                    input = input,
                    output = output
                };
                components.Add(comp);
                trackedFacilities.Add(obj);
            }
            else
            {
                Debug.LogWarning($"Missing connection points on {obj.name}");
                warnedFacilities.Add(obj);
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
        // Убираем уничтоженные объекты (после сохранения/очистки в списке оставались пустые ссылки).
        components.RemoveAll(c => c == null || c.component == null);
        trackedFacilities.RemoveWhere(o => o == null);
        warnedFacilities.RemoveWhere(o => o == null);

        if (pipeGenerator == null)
        {
            Debug.LogError("PipeGenerator is not assigned!");
            return;
        }

        bool hasCO2Facilities = components.Exists(c =>
                c.component.CompareTag("Facilities_Kataz") || c.component.CompareTag("Facilities_NewCapsul"));

        // Основная цепочка: выход каждой установки -> вход следующей.
        for (int i = 0; i < components.Count - 1; i++)
        {
            SystemComponent fromComp = components[i];
            SystemComponent toComp = components[i + 1];

            if (fromComp.output != null && toComp.input != null)
            {
                CreatePipeConnection(fromComp.output, toComp.input, useCO2: false);
            }
            else
            {
                Debug.LogWarning($"Standard connection points missing between {fromComp.component.name} and {toComp.component.name}");
            }
        }

        // CO2-линия между катализатором и капсулой — один раз.
        // Раньше этот блок стоял внутри цикла и создавал N-1 одинаковых труб.
        if (hasCO2Facilities)
            CreateCO2Connections();

        pipeGenerator.UpdateMesh();
        if (hasCO2Facilities && pipeGeneratorCO2 != null)
            pipeGeneratorCO2.UpdateMesh();

        SaveGeneratedMeshToSborCO2(pipeGenerator, "PipeMesh_Standard");

        if (hasCO2Facilities && pipeGeneratorCO2 != null)
        {
            SaveGeneratedMeshToSborCO2(pipeGeneratorCO2, "PipeMesh_CO2");
        }

        ClearAllPipes();
    }

    private void CreateCO2Connections()
    {
        SystemComponent fromComp2 = components.Find(c => c.component.CompareTag("Facilities_Kataz"));
        SystemComponent toComp2 = components.Find(c => c.component.CompareTag("Facilities_NewCapsul"));

        if (fromComp2 == null || toComp2 == null)
        {
            Debug.LogWarning("Один из объектов Facilities_Kataz или Facilities_NewCapsul не найден в списке компонентов.");
            return;
        }

        Transform fromCO2 = FindChildByTag(fromComp2.component.transform, "ConnectionPointCO2OUT");
        Transform toCO22 = FindChildByTag(toComp2.component.transform, "ConnectionPointCO2OUT");

        Transform fromCO22 = FindChildByTag(fromComp2.component.transform, "ConnectionPointCO2IN");
        Transform toCO2 = FindChildByTag(toComp2.component.transform, "ConnectionPointCO2IN");

        if (fromCO2 != null && toCO22 != null)
            CreatePipeConnection(fromCO2, toCO22, useCO2: true);
        if (fromCO22 != null && toCO2 != null)
            CreatePipeConnection(fromCO22, toCO2, useCO2: true);

        if (fromCO2 == null || toCO2 == null || fromCO22 == null || toCO22 == null)
            Debug.LogWarning("Не найдены CO2-точки для Facilities_Kataz и Facilities_NewCapsul");
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
    public void ClearAllPipes()
    {
        if (pipeGenerator != null)
        {
            pipeGenerator.Clear();
            pipeGenerator.UpdateMesh();
        }

        // CO2-генератор назначен не во всех сценах — раньше здесь был NullReferenceException.
        if (pipeGeneratorCO2 != null)
        {
            pipeGeneratorCO2.Clear();
            pipeGeneratorCO2.UpdateMesh();
        }
    }

    public void ClearAllPipesAndFacilities()
    {
        if (pipeGenerator != null)
        {
            //pipeGenerator.Clear();
            pipeGenerator.UpdateMesh();
        }

        for (int i = components.Count - 1; i >= 0; i--)
        {
            var comp = components[i];

            if (comp.component != null)
            {
                if (Application.isPlaying)
                    DestroyImmediate(comp.component);
                else
                    DestroyImmediate(comp.component);
            }

            components.RemoveAt(i);
        }

        GameObject sborCO2 = GameObject.FindGameObjectWithTag("SborCO2");
        if (sborCO2 != null)
            DestroyImmediate(sborCO2);
        trackedFacilities.Clear();
        warnedFacilities.Clear();
    }

    private void SaveGeneratedMeshToSborCO2(PipeGenerator generator, string name)
    {
        GameObject sborCO2 = GameObject.FindGameObjectWithTag("SborCO2");
        if (sborCO2 == null)
        {
            Debug.LogWarning("SborCO2 object not found!");
            return;
        }

        Mesh meshCopy = Instantiate(generator.GetComponent<MeshFilter>().sharedMesh);
        meshCopy.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GameObject savedMeshObject = null;
        Scene activeScene = SceneManager.GetActiveScene();
        
        if (activeScene.name == "CHP")
        {
            savedMeshObject = Instantiate(pipeMeshStandardPrefab);
        }
        else
        {
            savedMeshObject = Instantiate(pipeMeshStandardPrefab, sborCO2.transform);
        }
        
        savedMeshObject.transform.localPosition = Vector3.zero;
        savedMeshObject.transform.localRotation = Quaternion.identity;
        savedMeshObject.transform.localScale = Vector3.one;

        var mf = savedMeshObject.GetComponent<MeshFilter>();
        if (mf == null) mf = savedMeshObject.AddComponent<MeshFilter>();
        mf.sharedMesh = meshCopy;

        var originalMR = generator.GetComponent<MeshRenderer>();
        var mr = savedMeshObject.GetComponent<MeshRenderer>();
        if (mr == null) mr = savedMeshObject.AddComponent<MeshRenderer>();
        if (originalMR != null) mr.sharedMaterials = originalMR.sharedMaterials;

        var mc = savedMeshObject.GetComponent<MeshCollider>();
        if (mc == null) mc = savedMeshObject.AddComponent<MeshCollider>();
        mc.sharedMesh = meshCopy;
    }
}
