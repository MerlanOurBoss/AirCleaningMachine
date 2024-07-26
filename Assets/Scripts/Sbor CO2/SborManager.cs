using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SborManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
