using InstantPipes;
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
        for (int i = 0; i < components.Count - 1; i++)
        {
            SystemComponent fromComp = components[i];
            SystemComponent toComp = components[i + 1];
            CreatePipeConnection(fromComp.output, toComp.input);
        }

        pipeGenerator.UpdateMesh();
    }

    private void CreatePipeConnection(Transform from, Transform to)
    {
        Vector3 start = from.position;
        Vector3 startNormal = from.forward;

        Vector3 end = to.position;
        Vector3 endNormal = to.forward; // вход обычно направлен в объект

        bool success = pipeGenerator.AddPipe(start, startNormal, end, endNormal);

        if (!success)
        {
            Debug.LogWarning($"Pipe between {from.name} and {to.name} failed to generate.");
        }
    }
}
