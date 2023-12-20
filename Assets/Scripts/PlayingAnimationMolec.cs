using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayingAnimationMolec : MonoBehaviour
{
    //public GameObject _CO;
    //public GameObject _CH2H2O2;
    //public GameObject _S2O4;
    //public GameObject _N2O2;

    //public Animator[] _animatorCO;
    //public Animator[] _animatorCH2H2O2;
    //public Animator[] _animatorS2O4;
    //public Animator[] _animatorN2O2;


    [SerializeField] private Animation _molecAnimation;
    [SerializeField] private TMP_InputField _myTextMeshProContent;


    public void checkContent()
    {
        //if (_myTextMeshProContent.text == "CO")
        //{
        //    _CO.SetActive(true);
        //    _CH2H2O2.SetActive(false); 
        //    _S2O4.SetActive(false);
        //    _N2O2.SetActive(false);
        //}
        //else if (_myTextMeshProContent.text == "CH4O2")
        //{
        //    _CO.SetActive(false);
        //    _CH2H2O2.SetActive(true);
        //    _S2O4.SetActive(false);
        //    _N2O2.SetActive(false);
        //}
        //else if (_myTextMeshProContent.text == "S2O4")
        //{
        //    _CO.SetActive(false);
        //    _CH2H2O2.SetActive(false);
        //    _S2O4.SetActive(true);
        //    _N2O2.SetActive(false);
        //}
        //else if (_myTextMeshProContent.text == "N2O2")
        //{
        //    _CO.SetActive(false);
        //    _CH2H2O2.SetActive(false);
        //    _S2O4.SetActive(false);
        //    _N2O2.SetActive(true);
        //}
    }
}
