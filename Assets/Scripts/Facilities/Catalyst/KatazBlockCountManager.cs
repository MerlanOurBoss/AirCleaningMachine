using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatazBlockCountManager : MonoBehaviour
{
    [SerializeField] private int count;
    [SerializeField] private GameObject[] blocks;

    private void Start()
    {
        count = 4;
    }

    public void ChangeBlocks(int n)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (i < n)
            {
                blocks[i].SetActive(true);
            }
            else if (i >= n)
            { 
                blocks[i].SetActive(false); 
            }
        }
    }
}
