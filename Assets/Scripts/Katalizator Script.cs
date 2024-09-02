using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatalizatorScript : MonoBehaviour
{
    [SerializeField] private GameObject[] modules;
    [SerializeField] private GameObject triangleOrig;
    [SerializeField] private GameObject triangleSecond;
    [SerializeField] private ParticleSystem[] smokes1;
    [SerializeField] private ParticleSystem[] smokes2;
    [SerializeField] private MeshRenderer fourthObj;

    public Material realMat;
    public Material secondMat;

    public bool isSecond = false;

    public float x;
    public float y;
    public float z;

    public float delaySmoke;

    private void Start()
    {
        ChangePosition(0);
    }

    public void ChangePosition(int positionIndex)
    {
        Vector3[] positions = GetModulePositions(positionIndex);
        for (int i = 0; i < modules.Length; i++)
        {
            modules[i].transform.position = positions[i];
        }

        UpdateVisuals(positionIndex);
    }

    private Vector3[] GetModulePositions(int index)
    {
        switch (index)
        {
            case 1:
                return new Vector3[]
                {
                    new Vector3(0 - x, 0 - y, 0 - z),
                    new Vector3(0 - x, 0 - y, 0 - z),
                    new Vector3(147f - x, -0.315589905f - y, 0 - z),
                    new Vector3(-147f - x, 0 - y, 0 - z)
                };
            case 2:
                return new Vector3[]
                {
                    new Vector3(0 - x, 0 - y, 0 - z),
                    new Vector3(99.7f - x, 0 - y, 0 - z),
                    new Vector3(147f - x, -0.315589905f - y, 0 - z),
                    new Vector3(-194.05f - x, -0.1f - y, 0 - z)
                };
            case 0:
            default:
                return new Vector3[]
                {
                    new Vector3(0 - x, 0 - y, 0 - z),
                    new Vector3(0 - x, 0 - y, 0 - z),
                    new Vector3(0 - x, 0 - y, 0 - z),
                    new Vector3(0 - x, 0 - y, 0 - z)
                };
        }
    }

    private void UpdateVisuals(int positionIndex)
    {
        bool isPosition0 = positionIndex == 0;
        triangleOrig.SetActive(isPosition0);
        triangleSecond.SetActive(!isPosition0);
        fourthObj.material = isPosition0 ? realMat : secondMat;
        isSecond = !isPosition0;

        UpdateSmokeDelays(positionIndex);
    }

    private void UpdateSmokeDelays(int positionIndex)
    {
        float[] delays = GetSmokeDelays(positionIndex);
        for (int i = 0; i < smokes2.Length; i++)
        {
            smokes2[i].startDelay = delays[i];
        }
    }

    private float[] GetSmokeDelays(int index)
    {
        switch (index)
        {
            case 2:
                return new float[] { delaySmoke - 2, delaySmoke - 1, delaySmoke };
            case 1:
            default:
                return new float[] { delaySmoke, delaySmoke + 1, delaySmoke + 2 };
        }
    }
}
