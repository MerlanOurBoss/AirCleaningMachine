using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelShowButton : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;

    public Sprite rightSprite;
    public Sprite leftSprite;

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
}
