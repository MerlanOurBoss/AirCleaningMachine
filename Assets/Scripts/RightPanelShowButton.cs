using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelShowButton : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private Button[] _typesB;

    public Sprite rightSprite;
    public Sprite leftSprite;

    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private bool isOpened = false;

    void Start()
    {
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.Log("nety Animatora");
        }

        if (_button != null)
        {
            _button.onClick.AddListener(TogglePanel);
        }
        else
        {
            Debug.Log("Button не назначена");
        }

        foreach (Button btn in _typesB)
        {
            btn.onClick.AddListener(() => OnTypeButtonClicked(btn));
        }

        if (_typesB.Length > 0)
        {
            OnTypeButtonClicked(_typesB[0]);
        }
    }
    private void TogglePanel()
    {
        if (isOpened)
        {
            _image.sprite = leftSprite;
            _anim.Play("Close");
        }
        else
        {
            _image.sprite = rightSprite;
            _anim.Play("Open");
        }

        isOpened = !isOpened;
    }

    private void OnTypeButtonClicked(Button clickedButton)
    {
        foreach (Button btn in _typesB)
        {
            Image img = btn.GetComponent<Image>();
            Transform detail = btn.transform.Find("Подробно");

            if (btn == clickedButton)
            {
                if (img != null)
                    img.color = selectedColor;

                if (detail != null)
                    detail.gameObject.SetActive(true);
            }
            else
            {
                if (img != null)
                    img.color = normalColor;

                if (detail != null)
                    detail.gameObject.SetActive(false);
            }
        }
    }
}
