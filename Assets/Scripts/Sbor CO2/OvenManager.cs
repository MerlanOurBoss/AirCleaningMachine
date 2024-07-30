using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvenManager : MonoBehaviour
{
    public Toggle[] toggles;
    public ParticleSystem smoke;

    public bool isElectrofilterOn = false;
    public bool isKatalizatorOn = false;
    public bool isWaterEmulgator = false;
    public bool isReagentEmulgator = false;
    public bool isSheloshEmulgator = false;
    public bool isSborCO2 = false;


    public void Update()
    {
        isElectrofilterOn = toggles[0].isOn;
        isKatalizatorOn = toggles[1].isOn;
        isWaterEmulgator = toggles[2].isOn;
        isReagentEmulgator = toggles[3].isOn;
        isSheloshEmulgator = toggles[4].isOn;
        isSborCO2 = toggles[5].isOn;


        if (!isElectrofilterOn && !isKatalizatorOn && !isWaterEmulgator && !isReagentEmulgator && !isSheloshEmulgator && !isSborCO2)
        {
            smoke.startColor = new Color(0.6075471f, 0.6075471f, 0.6075471f);
        }
        else if (isElectrofilterOn)
        {
            smoke.startColor = new Color(0.6235294f, 0.4130386f, 0);
        }
        else if(isKatalizatorOn)
        {
            smoke.startColor = new Color(0.7450981f, 0.8078431f, 7176471f);
        }
        else if(isWaterEmulgator)
        {
            smoke.startColor = new Color(0.5686275f, 0.7529412f, 0.8078431f);
        }
        else if(isReagentEmulgator)
        {
            smoke.startColor = new Color(0.5686275f, 0.7529412f, 0.8078431f);
        }
        else if(isSheloshEmulgator)
        {
            smoke.startColor = new Color(0.5686275f, 0.7529412f, 0.8078431f);
        }
        else if(isSborCO2)
        {
            smoke.startColor = new Color(0.7764706f, 0.9215686f, 1f);
        }
    }
}
