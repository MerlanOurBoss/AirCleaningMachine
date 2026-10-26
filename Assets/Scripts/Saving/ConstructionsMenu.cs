using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public static class ConstructionSelector
{
    public static int SelectedIndex = -1;
}
public class ConstructionsMenu : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
}