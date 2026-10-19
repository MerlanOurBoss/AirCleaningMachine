#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FacilityIdSceneValidator
{
    private const string FacilityIdFieldName = "facilityId";

    [MenuItem("Tools/Facility ID/Validate Scene")]
    private static void ValidateScene()
    {
        var allWithId = new List<(Component component, string id)>();

        foreach (var component in Object.FindObjectsOfType<MonoBehaviour>(true))
        {
            var so = new SerializedObject(component);
            var prop = so.FindProperty(FacilityIdFieldName);
            if (prop == null) continue;

            allWithId.Add((component, prop.stringValue));
        }

        int problems = 0;

        var byTypeAndId = allWithId
            .Where(x => !string.IsNullOrWhiteSpace(x.id))
            .GroupBy(x => (x.component.GetType(), x.id));

        foreach (var group in byTypeAndId)
        {
            if (group.Count() <= 1) continue;

            problems++;
            string names = string.Join(", ", group.Select(x => x.component.gameObject.name));
            Debug.LogError($"[Facility ID] Коллизия: {group.Count()} объектов типа " +
                            $"{group.Key.Item1.Name} с одинаковым facilityId='{group.Key.Item2}': {names}");
        }

        foreach (var (component, id) in allWithId)
        {
            if (string.IsNullOrWhiteSpace(id))
                Debug.LogWarning($"[Facility ID] '{component.gameObject.name}' ({component.GetType().Name}): facilityId пуст.");
        }

        if (problems == 0)
            Debug.Log($"[Facility ID] Проверено {allWithId.Count} объектов с полем facilityId — коллизий не найдено.");
        else
            Debug.LogError($"[Facility ID] Найдено проблем: {problems}. См. сообщения выше.");
    }
}
#endif