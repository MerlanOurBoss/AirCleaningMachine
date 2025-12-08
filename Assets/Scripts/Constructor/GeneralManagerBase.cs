using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralManagerBase : MonoBehaviour
{
    /// <summary>
    /// Общий метод, который можно вызвать у всех менеджеров
    /// </summary>
    public void StartModule()
    {
        OnStartModule();
    }

    public void StopModule()
    {
        OnStopModule();
    }

    /// <summary>
    /// Конкретная логика запуска системы для наследников
    /// </summary>
    protected abstract void OnStartModule();
    
    protected abstract void OnStopModule();
}
