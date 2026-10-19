#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class FacilityIdLinkTool
{
    private const string FacilityIdFieldName = "facilityId";

    [MenuItem("GameObject/Facility ID/Link Selected As New Pair", false, 0)]
    private static void LinkSelectedAsPair()
    {
        var selected = Selection.gameObjects;

        if (selected.Length != 2)
        {
            EditorUtility.DisplayDialog("Facility ID",
                "Выделите ровно два объекта: один с GeneralManagerForXxx, " +
                "другой с MathModuleForXxx.", "OK");
            return;
        }

        var componentA = FindComponentWithFacilityId(selected[0]);
        var componentB = FindComponentWithFacilityId(selected[1]);

        if (componentA == null || componentB == null)
        {
            var missing = componentA == null ? selected[0].name : selected[1].name;
            EditorUtility.DisplayDialog("Facility ID",
                $"На объекте '{missing}' не найден компонент с полем facilityId " +
                "(GeneralManagerForXxx или MathModuleForXxx).", "OK");
            return;
        }

        string newId = GenerateShortId();

        SetFacilityId(componentA, newId);
        SetFacilityId(componentB, newId);

        Debug.Log($"Facility ID: '{selected[0].name}' ({componentA.GetType().Name}) и " +
                  $"'{selected[1].name}' ({componentB.GetType().Name}) связаны новым ID '{newId}'.");
    }

    [MenuItem("GameObject/Facility ID/Link Selected As New Pair", true)]
    private static bool ValidateLinkSelectedAsPair() => Selection.gameObjects.Length == 2;

    private static string GenerateShortId() => System.Guid.NewGuid().ToString("N").Substring(0, 8);

    private static Component FindComponentWithFacilityId(GameObject go)
    {
        foreach (var component in go.GetComponents<Component>())
        {
            if (component == null) continue;
            var so = new SerializedObject(component);
            if (so.FindProperty(FacilityIdFieldName) != null)
                return component;
        }
        return null;
    }

    private static void SetFacilityId(Component component, string value)
    {
        var so = new SerializedObject(component);
        var prop = so.FindProperty(FacilityIdFieldName);
        prop.stringValue = value;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(component);
    }
}
#endif