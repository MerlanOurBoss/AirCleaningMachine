using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class User
{
    public string login;
    public string password;
}

[System.Serializable]
public class UsersData
{
    public List<User> users;
}

public class SignInSystem : MonoBehaviour
{
    public TMP_InputField loginField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI resultText;
    public Button showPasswordButton;
    private bool isPasswordVisible = false;

    public Sprite[] sprites;

    private UsersData usersData;

    private void Start()
    {
        passwordField.contentType = TMP_InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();  // ��������� ���������

        showPasswordButton.onClick.AddListener(TogglePasswordVisibility);
        showPasswordButton.image.sprite = sprites[0];
        LoadUsersData();
    }

    // ����� ��� �������� ������ �� JSON-�����
    void LoadUsersData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("loginData");  // ��������� ���� ��� ����������
        if (jsonFile != null)
        {
            string json = jsonFile.text;
            usersData = JsonUtility.FromJson<UsersData>(json);
        }
        else
        {
            resultText.text = "Error: Unable to load user data from Resources.";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Login();
        }
    }

    // ����� ��� ����� � �������
    public void Login()
    {
        string inputLogin = loginField.text;
        string inputPassword = passwordField.text;

        bool loginSuccess = false;

        foreach (User user in usersData.users)
        {
            if (user.login == inputLogin && user.password == inputPassword)
            {
                loginSuccess = true;
                break;
            }
        }

        if (loginSuccess)
        {
            resultText.text = "Login successful!";
            // ������ ��� �������� �����
            SceneManager.LoadScene(2);
        }
        else
        {
            resultText.text = "Error: Invalid login or password.";
        }
    }

    void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;  // ����������� ���������

        if (isPasswordVisible)
        {
            passwordField.contentType = TMP_InputField.ContentType.Standard;  // ���������� �����
            showPasswordButton.image.sprite = sprites[1];
        }
        else
        {
            passwordField.contentType = TMP_InputField.ContentType.Password;  // �������� �����
            showPasswordButton.image.sprite = sprites[0];
        }

        passwordField.ForceLabelUpdate();  // ��������� ����������� ������
    }

}
