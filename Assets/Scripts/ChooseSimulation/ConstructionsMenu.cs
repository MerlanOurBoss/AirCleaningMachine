using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public static class ConstructionSelector
{
    /// <summary>
    /// ����� ����������� ������ ����������� �� MenuScene.
    /// ����� �������� ConstructionScene �� ������ ��� �������� �
    /// �������� ClearAndLoad(SelectedIndex).
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