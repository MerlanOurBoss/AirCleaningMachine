﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class TutoriolSimulatio : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private PlayableDirector[] _myFluidsWater;
    [SerializeField] private PlayableDirector[] _myFluidsReact;
    [SerializeField] private GameObject[] _dropCreating;
    [SerializeField] private DropSpawner[] _dropSpawns;

    [SerializeField] private Button _simulationButton;

    [SerializeField] private Animator _electroFilter;
    [SerializeField] private Animator _lightBulb;
    [SerializeField] private NewCollectors _myCollector;
    [SerializeField] private TemperatureCatalizator _myCatalizator;

    [SerializeField] private GameObject ComponentsCameras;


    private bool _startSimulationTemp = false;
    private bool _startSimulationContent = false;
    private float _simulationTime = 1400f;
    private float _fluidDelayWater = 15f;
    private float _fluidDelayReact = 32f;

    private int max = 130;
    private void Start()
    {
        _startSimulationTemp = true;
        _startSimulationContent = true;
        StartSmokesAndFluids();
    }

    private void Update()
    {
        if (_simulationTime <= 0)
        {
            _startSimulationTemp = false;
            _startSimulationContent = false;
        }
        if (_startSimulationTemp && _startSimulationContent)
        {
            _simulationTime -= 1 * Time.deltaTime;
            _fluidDelayWater -= 1 * Time.deltaTime;
            _fluidDelayReact -= 1 * Time.deltaTime;

            if (_fluidDelayWater < 0)
            {
                foreach (PlayableDirector fluid in _myFluidsWater)
                {
                    fluid.Play();
                }
            }
            if (_fluidDelayReact < 0)
            {
                foreach (PlayableDirector fluid in _myFluidsReact)
                {
                    fluid.Play();
                }
            }
        }
        else
        {
            _simulationTime = 140f;
            ComponentsCameras.SetActive(false);
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Stop();
            }
            _electroFilter.Play("NewColecAnimStop");
            _myCollector.StopColumnProcess();
            _myCatalizator.StopSimulation();
            _lightBulb.Play("LightsAnimationStops");
            foreach (PlayableDirector fluid in _myFluidsWater)
            {
                fluid.Stop();
            }
            foreach (PlayableDirector fluid in _myFluidsReact)
            {
                fluid.Stop();
            }
            foreach (GameObject drop in _dropCreating)
            {
                drop.SetActive(false);
            }
        }
    }

    public void StartSmokesAndFluids()
    {
        if (_startSimulationTemp && _startSimulationContent)
        {
            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Play();
            }
            _electroFilter.Play("NewColecAnim");
            _myCollector.StartColumnProcess3();
            _myCatalizator.StartSimulation();
            _lightBulb.Play("LightsAnimation");
            foreach (GameObject drop in _dropCreating)
            {
                drop.SetActive(true);
            }
        }
    }

    public void StartSimulation()
    {
        StartSmokesAndFluids();
    }
}