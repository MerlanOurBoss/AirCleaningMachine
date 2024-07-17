using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDirection : MonoBehaviour
{
    public Transform firstPoint;
    public Transform lastPoint;

    public GameObject sphere;

    public GameObject[] curvedPipes;

    private Vector3 startPos;
    private Vector3 endPos;
    private void Start()
    {
        startPos = firstPoint.position;
        endPos = lastPoint.position;

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        Vector3 midPointX = new Vector3(startPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        Vector3 direction = midPointY - midPointX;
        Vector3 normalizedDirection = direction.normalized;
        AnalyzeNormalDirection(normalizedDirection);

        Vector3 directionZ = midPointZ - midPointY;
        Vector3 normalizedDirectionZ = directionZ.normalized;
        AnalyzeNormalDirection(normalizedDirectionZ);


    }

    void AnalyzeNormalDirection(Vector3 normalizedDirection)
    {
        if (Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y) && Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.z))
        {
            if (normalizedDirection.x > 0)
            {
                Debug.Log("Направление: вправо");
            }
            else
            {
                Debug.Log("Направление: влево");
            }
        }
        else if (Mathf.Abs(normalizedDirection.y) > Mathf.Abs(normalizedDirection.x) && Mathf.Abs(normalizedDirection.y) > Mathf.Abs(normalizedDirection.z))
        {
            if (normalizedDirection.y > 0)
            {
                if (startPos.x > endPos.x)
                {
                    Debug.Log("Направление: вверх ");
                    Instantiate(curvedPipes[4], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[4].transform.rotation);
                }
                else
                {
                    Debug.Log("Направление: вверх ");
                    Instantiate(curvedPipes[0], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[0].transform.rotation);
                }
                
            }
            else
            {
                if (startPos.x > endPos.x)
                {
                    Instantiate(curvedPipes[7], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[7].transform.rotation);
                    Debug.Log("Направление: вниз");
                }
                else
                {
                    Instantiate(curvedPipes[3], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[3].transform.rotation);
                    Debug.Log("Направление: вниз");
                }
                    
            }
        }
        else if (Mathf.Abs(normalizedDirection.z) > Mathf.Abs(normalizedDirection.x) && Mathf.Abs(normalizedDirection.z) > Mathf.Abs(normalizedDirection.y))
        {
            if (normalizedDirection.z > 0)
            {
                if (startPos.z > endPos.z)
                {
                    Instantiate(curvedPipes[2], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[2].transform.rotation);
                    Debug.Log("Направление: вперед");
                }
                else
                {
                    Instantiate(curvedPipes[6], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[6].transform.rotation);
                    Debug.Log("Направление: вперед");
                }                
            }
            else
            {
                if (startPos.z < endPos.z)
                {
                    Instantiate(curvedPipes[1], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[1].transform.rotation);
                    Debug.Log("Направление: назад");
                }
                else
                {
                    Instantiate(curvedPipes[5], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[5].transform.rotation);
                    Debug.Log("Направление: назад");
                }
                
                
            }
        }
    }
}
