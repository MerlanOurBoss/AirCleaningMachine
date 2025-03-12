using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddingMesh : MonoBehaviour
{
    private Dictionary<ParticleSystem, (Color startColor, bool colorOverLifetime, bool colorBySpeed, bool playOnAwake)> particleStates = new();
    private Dictionary<ParticleSystem, GameObject> particleParents = new();

    public void HidingMeshAndParticle(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target GameObject is null!");
            return;
        }

        // ��������� ��� MeshRenderer
        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }

        // ��������� ��������� ParticleSystem � �������� ���������
        ParticleSystem[] particleSystems = target.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            var colorOverLifetime = ps.colorOverLifetime;
            var colorBySpeed = ps.colorBySpeed;

            // ��������� ���������
            particleStates[ps] = (main.startColor.color, colorOverLifetime.enabled, colorBySpeed.enabled, main.playOnAwake);
            particleParents[ps] = ps.gameObject;

            // ��������� ���������
            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 0);
            colorOverLifetime.enabled = false;
            colorBySpeed.enabled = false;
            main.playOnAwake = true;

            // ��������� GameObject � ParticleSystem
            ps.gameObject.SetActive(false);
        }

        // ������������� ����� ��������
        GameObject[] dropObjects = GameObject.FindGameObjectsWithTag("DropCreatings");
        foreach (var obj in dropObjects)
        {
            DropSpawner spawner = obj.GetComponent<DropSpawner>();
            if (spawner != null)
            {
                spawner.PauseSpawning();
            }
        }

        NewSborScript SborCO2 = target.GetComponent<NewSborScript>();
        if (SborCO2 != null)
        {
            SborCO2.PauseProcess();
        }
    }

    public void ShowMeshAndParticle(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target GameObject is null!");
            return;
        }

        // �������� ��� MeshRenderer
        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }

        // �������� GameObject � ParticleSystem � ��������������� ���������
        ParticleSystem[] particleSystems = target.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particleSystems)
        {
            if (particleParents.TryGetValue(ps, out var parent))
            {
                parent.SetActive(true);
            }
        }

        foreach (var ps in particleSystems)
        {
            if (particleStates.TryGetValue(ps, out var state))
            {
                var main = ps.main;
                main.startColor = state.startColor;
                main.playOnAwake = false;

                var colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.enabled = state.colorOverLifetime;

                var colorBySpeed = ps.colorBySpeed;
                colorBySpeed.enabled = state.colorBySpeed;
            }
        }

        // ������������ ����� ��������
        GameObject[] dropObjects = GameObject.FindGameObjectsWithTag("DropCreatings");
        foreach (var obj in dropObjects)
        {
            DropSpawner spawner = obj.GetComponent<DropSpawner>();
            if (spawner != null)
            {
                spawner.ResumeSpawning();
            }
        }
        NewSborScript SborCO2 = target.GetComponent<NewSborScript>();
        if (SborCO2 != null)
        {
            SborCO2.ResumeProcess();
        }

    }
}
