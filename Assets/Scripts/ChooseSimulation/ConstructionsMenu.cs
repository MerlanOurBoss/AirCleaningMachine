using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public static class ConstructionSelector
{
    /// <summary>
    /// Здесь сохраняется индекс конструкции из MenuScene.
    /// После загрузки ConstructionScene мы читаем это значение и
    /// вызываем ClearAndLoad(SelectedIndex).
    /// </summary>
    public static int SelectedIndex = -1;
}
public class ConstructionsMenu : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
}