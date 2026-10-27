using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingAndCreatingMolec : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private GameObject spawnInObj;

    private static bool s_isQuitting;
    private static int s_suppressSpawnFrame = -1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        s_isQuitting = false;
        s_suppressSpawnFrame = -1;
        Application.quitting -= OnQuitting;
        Application.quitting += OnQuitting;
    }

    private static void OnQuitting() => s_isQuitting = true;

    /// <summary>
    /// Не создавать продукты реакции для молекул, уничтоженных в этом кадре
    /// (используется при «Удалить все молекулы»).
    /// </summary>
    public static void SuppressSpawnThisFrame() => s_suppressSpawnFrame = Time.frameCount;

    private void Start()
    {
        spawnInObj = GameObject.FindWithTag(Tags.SpawnInObj);

    }

    void OnDestroy()
    {
        // Раньше Instantiate вызывался и при выгрузке сцены/выходе из игры —
        // объекты создавались во время уничтожения сцены или летел NullReference.
        if (s_isQuitting || !gameObject.scene.isLoaded) return;
        if (Time.frameCount == s_suppressSpawnFrame) return;
        if (spawnPrefab == null || spawnInObj == null) return;

        Instantiate(spawnPrefab, transform.position, gameObject.transform.rotation, spawnInObj.transform);
    }
}
