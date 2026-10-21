using System;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class NasosScript : MonoBehaviour
{
    public Slider mySliderNasos;
    public Slider mySliderWater;
    public GameObject[] fluids;
    public MeshRenderer[] fluidsWater;
    public PlayableDirector[] fliudDirector;
    public GameObject nasoso;
    public GameObject water;
    private int count = 1;
    public int countWater = 1;
    public ChamgingEmul chamgingEmul;
    private bool isEnableNasos = false;
    private bool isEnableWater = false;

    private void Start()
    {
        countWater = chamgingEmul.countWater;
    }

    private void Update()
    {
        mySliderNasos.value = count;
        // Тип воды хранится в ChamgingEmul (его же используют расчёты); здесь только показываем.
        if (chamgingEmul != null) countWater = chamgingEmul.countWater > 3 ? 1 : chamgingEmul.countWater;
        mySliderWater.value = countWater;
        if (count == 1 || count == 2)
        {
            foreach (GameObject item in fluids)
            {
                item.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            foreach (PlayableDirector item in fliudDirector)
            {
                if (item == null) 
                    continue;

                if (!item.playableGraph.IsValid())
                    continue;

                var graph = item.playableGraph;

                int rootCount = graph.GetRootPlayableCount();
                for (int i = 0; i < rootCount; i++)
                {
                    var root = graph.GetRootPlayable(i);
                    if (root.IsValid())
                        root.SetSpeed(1f);
                }
            }
        }
        if (count == 3)
        {
            foreach (GameObject item in fluids)
            {
                item.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

            }
            foreach (PlayableDirector item in fliudDirector)
            {
                if (item == null) 
                    continue;

                if (!item.playableGraph.IsValid())
                    continue;

                var graph = item.playableGraph;

                int rootCount = graph.GetRootPlayableCount();
                for (int i = 0; i < rootCount; i++)
                {
                    var root = graph.GetRootPlayable(i);
                    if (root.IsValid())
                        root.SetSpeed(1.5f);
                }
            }
        }
        if (count == 4)
        {
            foreach (GameObject item in fluids)
            {
                item.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
            foreach (PlayableDirector item in fliudDirector)
            {
                if (item == null) 
                    continue;

                if (!item.playableGraph.IsValid())
                    continue;

                var graph = item.playableGraph;

                int rootCount = graph.GetRootPlayableCount();
                for (int i = 0; i < rootCount; i++)
                {
                    var root = graph.GetRootPlayable(i);
                    if (root.IsValid())
                        root.SetSpeed(1.7f);
                }
            }
        }
        if (count > 4)
        {
            count = 1;
        }


        // Цвет жидкости красит ChamgingEmul при смене типа. Раньше оба скрипта перекрашивали
        // одни и те же рендереры каждый кадр, каждый по своему countWater, и перебивали друг друга.
        if (chamgingEmul == null)
        {
            if (countWater > 3) countWater = 1;
            Color color = countWater == 2 ? new Color(0.7f, 1, 0, 0.6f)
                        : countWater == 3 ? new Color(0, 0, 0, 0.6f)
                        : new Color(1, 1, 1, 0.6f);
            foreach (MeshRenderer item in fluidsWater)
            {
                if (item.sharedMaterial != null && item.material.color != color) item.material.color = color;
            }
        }
    }


    public void PlusCount()
    {
        count++;
    }

    public void PlusCountWater()
    {
        if (chamgingEmul != null) chamgingEmul.ChangingE();
        else countWater++;
    }


    public void OnorOffObjectNasos()
    {
        if (isEnableNasos == false) { nasoso.SetActive(true); isEnableNasos = true;
            water.SetActive(false); isEnableWater = false;

        }
        else { nasoso.SetActive(false); isEnableNasos = false; isEnableWater = false;

        }
    }

    public void OnorOffObjectWater()
    {
        if (isEnableWater == false) { water.SetActive(true); isEnableWater = true;
            nasoso.SetActive(false); isEnableNasos = false;
        }
        else { water.SetActive(false); isEnableWater = false; isEnableNasos = false;
        }
    }
}
