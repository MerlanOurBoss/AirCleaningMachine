using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAnimation : MonoBehaviour
{
    private Animator _loadAnim;

    private void Awake()
    {
        _loadAnim = GetComponent<Animator>();
        _loadAnim.Play("Load"); 
    }
}
