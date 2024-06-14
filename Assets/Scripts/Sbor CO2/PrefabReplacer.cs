using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReplacer : MonoBehaviour
{
    public GameObject currentPrefab; // Текущий префаб
    public GameObject newPrefab; // Новый префаб, на который заменим

    public void ReplacePrefab()
    {
        // Создаём экземпляр нового префаба на позиции текущего
        GameObject newInstance = Instantiate(newPrefab, currentPrefab.transform.position, currentPrefab.transform.rotation);

        // Уничтожаем текущий префаб
        Destroy(currentPrefab);

        // Обновляем ссылку на текущий префаб
        currentPrefab = newInstance;
        newPrefab.GetComponent<MoveObjectWithMouse>().isSelected = true;
    }
}
