using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private Animator _myAnimation;

    [SerializeField] private Animation _textAnimations;

    private GameObject[] _rus;
    private GameObject[] _kaz;
    private GameObject[] _eng;

    private void Start()
    {
        _rus = GameObject.FindGameObjectsWithTag("Russian");
        _kaz = GameObject.FindGameObjectsWithTag("Kazakh");
        _eng = GameObject.FindGameObjectsWithTag("English");
        kazTextOn();
    }
    public void nextTutorial()
    {
        _myAnimation.SetBool("Next", true);
        _myAnimation.SetBool("Past", false);
    }
    public void priviousTutorial()
    {
        _myAnimation.SetBool("Next", false);
        _myAnimation.SetBool("Past", true);
    }

    public void stopTutorial()
    {
        _myAnimation.SetBool("Next", false);
        _myAnimation.SetBool("Past", false);
    }

    public void startTextAnim(string str)
    {
        _textAnimations.Play(str);
    }

    public void rusTextOn()
    {
        foreach (GameObject ru in _rus)
        {
            ru.SetActive(true);
        }
        foreach (GameObject kz in _kaz)
        {
            kz.SetActive(false);
        }
        foreach (GameObject en in _eng)
        {
            en.SetActive(false);
        }
    }
    public void kazTextOn()
    {
        foreach (GameObject ru in _rus)
        {
            ru.SetActive(false);
        }
        foreach (GameObject kz in _kaz)
        {
            kz.SetActive(true);
        }        
        foreach (GameObject en in _eng)
        {
            en.SetActive(false);
        }
    }
    public void engTextOn()
    {
        foreach (GameObject ru in _rus)
        {
            ru.SetActive(false);
        }
        foreach (GameObject kz in _kaz)
        {
            kz.SetActive(false);
        }
        foreach (GameObject en in _eng)
        {
            en.SetActive(true);
        }
    }
}
