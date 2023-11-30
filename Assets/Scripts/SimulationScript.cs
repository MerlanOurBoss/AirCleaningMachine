using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SimulationScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _myTextMeshPro;

    [SerializeField] private ParticleSystem[] _mySmokes;
    [SerializeField] private ParticleSystem _myBubble;

    [SerializeField] private PlayableDirector[] _myFluids;

    [SerializeField] private Button _simulationButton;
    [SerializeField] private TextMeshProUGUI _simulationText;

    public GameObject _errorText;

    private bool _startSimulation = false;
    private float _simulationTime = 20f;

    private void Update()
    {
        if (_simulationTime <= 0)
        {
            _startSimulation = false;
        }
        if (_startSimulation)
        {
            _simulationButton.interactable = false;
            _simulationText.text = "Èäåò ñèìóëÿöèÿ";
            _simulationTime -= 1 * Time.deltaTime;
            Debug.Log(_simulationTime.ToString("0"));
        }
        else
        {
            _simulationButton.interactable = true;
            _simulationText.text = "Ñèìóëèðîâàòü";
            _simulationTime = 20f;

            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Stop();
            }
            foreach (PlayableDirector fluid in _myFluids)
            {
                fluid.Stop();
            }
        }
    }

    [System.Obsolete]
    public void StartSimulation()
    {
        if (_myTextMeshPro.text == "")
        {
            Debug.Log("null");
            _errorText.SetActive(true);
        }
        else if (_myTextMeshPro.text == "0 °Ñ")
        {
            Debug.Log("0 °Ñ");
            _errorText.SetActive(false);
            _startSimulation = true;

            foreach(ParticleSystem smoke in _mySmokes)
            {
                smoke.Play();
                _myBubble.Play();
                smoke.startSpeed = smoke.startSpeed + 1;
                smoke.startLifetime = smoke.startLifetime - 1;
                smoke.startDelay = smoke.startDelay - 1;
            }
            foreach(PlayableDirector fluid in _myFluids)
            {
                fluid.Play();
            }
        }
        else if (_myTextMeshPro.text == "100 °Ñ")
        {
            Debug.Log("100 °Ñ");
            _errorText.SetActive(false);
            _startSimulation = true;

            foreach (ParticleSystem smoke in _mySmokes)
            {
                smoke.Play();
                _myBubble.Play();
                smoke.startSpeed = smoke.startSpeed + 2;
                smoke.startLifetime = smoke.startLifetime - 2;
                smoke.startDelay = smoke.startDelay - 2;
            }
            foreach (PlayableDirector fluid in _myFluids)
            {
                fluid.Play();
            }
        }
        else if (_myTextMeshPro.text == "300 °Ñ")
        {
            Debug.Log("300 °Ñ");
            _errorText.SetActive(false);
        }
        else if (_myTextMeshPro.text == "500 °Ñ")
        {
            Debug.Log("500 °Ñ");
            _errorText.SetActive(false);
        }
        else if (_myTextMeshPro.text == "600 °Ñ")
        {
            Debug.Log("600 °Ñ");
            _errorText.SetActive(false);
        }
        else if (_myTextMeshPro.text == "800 °Ñ")
        {
            Debug.Log("800 °Ñ");
            _errorText.SetActive(false);
        }
        else if (_myTextMeshPro.text == "1000 °Ñ")
        {
            Debug.Log("1000 °Ñ");
            _errorText.SetActive(false);
        }
    }
}
