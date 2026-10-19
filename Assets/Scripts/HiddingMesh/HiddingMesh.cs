using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddingMesh : MonoBehaviour
{
    private Dictionary<ParticleSystem, (Color startColor, bool colorOverLifetime, bool colorBySpeed, bool playOnAwake, bool wasPlaying)> particleStates = new();
    private Dictionary<ParticleSystem, GameObject> particleParents = new();

    public void HidingMeshAndParticle(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target GameObject is null!");
            return;
        }

        // Отключаем все MeshRenderer
        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }

        // Сохраняем состояние ParticleSystem и изменяем параметры
        ParticleSystem[] particleSystems = target.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            var colorOverLifetime = ps.colorOverLifetime;
            var colorBySpeed = ps.colorBySpeed;

            // Сохраняем состояние
            // Запоминаем и то, играла ли система: при показе запускаем только те, что играли.
            particleStates[ps] = (main.startColor.color, colorOverLifetime.enabled, colorBySpeed.enabled, main.playOnAwake, ps.isPlaying);
            particleParents[ps] = ps.gameObject;

            // Отключаем видимость
            main.startColor = new Color(main.startColor.color.r, main.startColor.color.g, main.startColor.color.b, 0);
            colorOverLifetime.enabled = false;
            colorBySpeed.enabled = false;
            // Раньше здесь ставилось playOnAwake = true, и при показе дым запускался,
            // даже если симуляция была остановлена.
            main.playOnAwake = false;

            // Отключаем GameObject с ParticleSystem
            ps.gameObject.SetActive(false);
        }

        // Останавливаем спавн объектов
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

        // Новый контроллер сбора CO2 (раньше ставился на паузу только старый NewSborScript).
        var sborController = target.GetComponentInChildren<SborCO2Controller>(true);
        if (sborController != null) sborController.PauseProcess();
    }

    public void ShowMeshAndParticle(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("Target GameObject is null!");
            return;
        }

        // Включаем все MeshRenderer
        MeshRenderer[] meshRenderers = target.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }

        // Включаем GameObject с ParticleSystem и восстанавливаем состояние
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
                main.playOnAwake = state.playOnAwake;

                var colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.enabled = state.colorOverLifetime;

                var colorBySpeed = ps.colorBySpeed;
                colorBySpeed.enabled = state.colorBySpeed;

                if (state.wasPlaying) ps.Play();
            }
        }

        // Возобновляем спавн объектов
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

        var sborController = target.GetComponentInChildren<SborCO2Controller>(true);
        if (sborController != null) sborController.ResumeProcess();

    }
}
