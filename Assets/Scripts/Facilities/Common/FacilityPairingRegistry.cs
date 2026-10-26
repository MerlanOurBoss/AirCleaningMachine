using System;
using System.Collections.Generic;
using UnityEngine;

public class FacilityPairingRegistry : MonoBehaviour
{
    public static FacilityPairingRegistry Instance { get; private set; }

    [Serializable]
    public class Pair
    {
        [Tooltip("Только для читаемости в инспекторе, в логике не участвует.")]
        public string label;

        [Tooltip("Объект с GeneralManagerForXxx (любой конкретный наследник ModuleManagerBase<T>).")]
        public MonoBehaviour manager;

        [Tooltip("Объект с MathModuleForXxx, связанный с этим Manager'ом.")]
        public MonoBehaviour mathModule;
    }

    [SerializeField] private List<Pair> pairs = new List<Pair>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"На сцене больше одного FacilityPairingRegistry ('{Instance.name}' и '{name}') — " +
                              "используется первый найденный, второй игнорируется.");
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public TMathModule Resolve<TMathModule>(MonoBehaviour managerInstance) where TMathModule : MonoBehaviour
    {
        foreach (var pair in pairs)
        {
            if (pair.manager == managerInstance)
                return pair.mathModule as TMathModule;
        }
        return null;
    }
}