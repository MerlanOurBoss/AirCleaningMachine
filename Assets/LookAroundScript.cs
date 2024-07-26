using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LookAroundScript : MonoBehaviour
{
    public GameObject pipePrefab; // Assign your pipe prefab in the inspector
    public GameObject curvedPipePrefab; // Assign your curved pipe prefab in the inspector
    public GameObject curvedPipePrefabForStartsAndEnds; // Assign your curved pipe prefab in the inspector
    public string targetTag = "Untagged"; // Set the tag of objects that can be linked
    public float pipeOffset = 0.1f; // Set the offset between pipes
    public float verticalOffset = 0.1f; // Offset to place pipes above the start point
    private Transform startObject;
    private Transform endObject;
    private bool firstClick = true;

    public GameObject sphere;
    public GameObject[] curvedPipes;

    public GameObject littlePipePrefab;
    public GameObject littlecurvedPipePrefabForStartsAndEnds;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag(targetTag))
            {
                if (firstClick)
                {
                    startObject = hit.transform;
                    firstClick = false;
                }
                else
                {
                    endObject = hit.transform;

                    if (startObject.parent.tag == "Emul2" && endObject.parent.tag == "Emul")
                    {
                        GenerateForTwoEmuls(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Emul2")
                    {
                        GenerateForTwoEmuls(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Cooling")
                    {
                        GenerateForEmulAndCooling(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Cooling" && endObject.parent.tag == "Emul")
                    {
                        GenerateForEmulAndCooling(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Collector" && endObject.parent.tag == "Sbor")
                    {
                        GenerateForCollectorAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Sbor" && endObject.parent.tag == "Collector")
                    {
                        GenerateForCollectorAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Sbor" && endObject.parent.tag == "Emul2")
                    {
                        GenerateForSborAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul2" && endObject.parent.tag == "Sbor")
                    {
                        GenerateForSborAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Par" && endObject.parent.tag == "Sbor")
                    {
                        GenerateForParAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Sbor" && endObject.parent.tag == "Par")
                    {
                        GenerateForParAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "DryAir" && endObject.parent.tag == "Sbor")
                    {
                        GenerateForDryairAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Sbor" && endObject.parent.tag == "DryAir")
                    {
                        GenerateForDryairAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Oven" && endObject.parent.tag == "Electro")
                    {
                        GenerateForOvenAndElectro(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Electro" && endObject.parent.tag == "Oven")
                    {
                        GenerateForOvenAndElectro(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Oven" && endObject.parent.tag == "Kataz")
                    {
                        GenerateForKatazAndOven(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Kataz" && endObject.parent.tag == "Oven")
                    {
                        GenerateForKatazAndOven(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Oven" && endObject.parent.tag == "Sbor")
                    {
                        GenerateForOvenAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Sbor" && endObject.parent.tag == "Oven")
                    {
                        GenerateForOvenAndSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Oven" && endObject.parent.tag == "Emul")
                    {
                        GenerateForOvenAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Oven")
                    {
                        GenerateForOvenAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Electro")
                    {
                        GenerateForElectroAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Kataz")
                    {
                        GenerateForKatazAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Electro" && endObject.parent.tag == "Emul2")
                    {
                        GenerateForElectroAndEmul2(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul2" && endObject.parent.tag == "Electro")
                    {
                        GenerateForKatazAndEmul2(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul2" && endObject.parent.tag == "Kataz")
                    {
                        GenerateForKatazAndEmul2(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Electro" && endObject.parent.tag == "Kataz")
                    {
                        GenerateForElectroAndKataz(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Kataz" && endObject.parent.tag == "Electro")
                    {
                        GenerateForElectroAndKataz(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "NewSbor" && endObject.parent.tag == "Kataz")
                    {
                        //GenerateForKatazAndNewSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Kataz" && endObject.parent.tag == "NewSbor")
                    {
                        GenerateForKatazAndNewSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "LittleKataz" && endObject.parent.tag == "LittleSbor")
                    {
                        GenerateLittleForKatazAndNewSbor(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "LittleSbor" && endObject.parent.tag == "LittleKataz")
                    {
                        //GenerateForKatazAndNewSbor(startObject.position, endObject.position);
                    }
                    else
                    {
                        GeneratePipes(startObject.position, endObject.position);
                    }

                    firstClick = true;
                }
            }
        }
    }

    void GeneratePipes(Vector3 startPos, Vector3 endPos)
    {
        
        Vector3 direction = endPos - startPos;
        Vector3 midPoint;

        // Check if the X, Y, or Z coordinates are approximately equal within ±5 units
        bool xEqual = Mathf.Abs(startPos.x - endPos.x) <= 10f;
        bool yEqual = Mathf.Abs(startPos.y - endPos.y) <= 10f;
        bool zEqual = Mathf.Abs(startPos.z - endPos.z) <= 10f;

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        // Check if either object1 or object2 has localEulerAngles.z equal to 90 degrees
        bool object1ZRotated = Mathf.Approximately(startObject.localEulerAngles.z, 90f);
        bool object2ZRotated = Mathf.Approximately(endObject.localEulerAngles.z, 90f);

        if (object1ZRotated)
        {
            startPos.y += verticalOffset;
        }
        if (object2ZRotated)
        {
            endPos.y += verticalOffset;
        }

        // Одиночные трубы
        if (xEqual && yEqual)
        {
            // Create a straight pipe along the Z axis
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            CreatePipe(startPos, endPos, parent);
        }
        else if (xEqual && zEqual)
        {
            // Create a straight pipe along the Y axis
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            CreatePipe(startPos, endPos, parent);
        }
        else if (yEqual && zEqual)
        {
            // Create a straight pipe along the X axis
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            CreatePipe(startPos, endPos, parent);
        }

        // Двойнные трубы трубы
        else if (xEqual)
        {
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            if (startPos.y > endPos.y)
            {
                Vector3 startNew = startPos;
                startPos = endPos;
                endPos = startNew;
                Debug.Log("dknaoifubhnsuefg");
            }

            Vector3 intermediatePos = new Vector3(startPos.x, endPos.y, startPos.z);

            CreatePipeForStart(startPos, intermediatePos, parent);

            Vector3 directionx = intermediatePos - startPos;
            Vector3 normalizedDirection = directionx.normalized;

            AnalyzeNormalDirectionForX(normalizedDirection, intermediatePos, startPos, endPos, parent);

            CreatePipeForEnd(intermediatePos, endPos, parent);
        }

        else if (yEqual)
        {
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            if (startPos.x > endPos.x)
            {
                Vector3 startNew = startPos;
                startPos = endPos;
                endPos = startNew;
                Debug.Log("dknaoifubhnsuefg");
            }

            Vector3 intermediatePos = new Vector3(startPos.x, startPos.y, endPos.z);

            CreatePipeForStart(startPos, intermediatePos, parent);

            Vector3 directionx = intermediatePos - startPos;
            Vector3 normalizedDirection = directionx.normalized;

            AnalyzeNormalDirectionForY(normalizedDirection, intermediatePos, startPos, endPos, parent);

            CreatePipeForEnd(intermediatePos, endPos, parent);
            Debug.Log(normalizedDirection);

        }

        else if (zEqual)
        {
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            if (startPos.y > endPos.y)
            {
                Vector3 startNew = startPos;
                startPos = endPos;
                endPos = startNew;
                Debug.Log("dknaoifubhnsuefg");
            }

            Vector3 intermediatePos = new Vector3(endPos.x, startPos.y, startPos.z);

            CreatePipeForStart(startPos, intermediatePos, parent);

            Vector3 directionx = intermediatePos - startPos;
            Vector3 normalizedDirection = directionx.normalized;

            AnalyzeNormalDirectionForZ(normalizedDirection, intermediatePos, startPos, endPos, parent);


            CreatePipeForEnd(intermediatePos, endPos, parent);
            Debug.Log(normalizedDirection);

        }
        else
        {
            GameObject parent = new GameObject("parent");
            parent.transform.SetParent(endObject.transform);
            if (startPos.y < endPos.y)
            {
                Vector3 startNew = startPos;
                startPos = endPos;
                endPos = startNew;
                Debug.Log("dknaoifubhnsuefg");
            }

            Vector3 intermediatePosz = new Vector3(endPos.x, endPos.y, startPos.z);

            Vector3 intermediatePosx = new Vector3(endPos.x, startPos.y, startPos.z);

            CreatePipeForStart(startPos, intermediatePosx, parent);

            Vector3 directionx = intermediatePosx - startPos;
            Vector3 normalizedDirectionx = directionx.normalized;

            AnalyzeNormalDirectionForZ(normalizedDirectionx, intermediatePosx, startPos, endPos, parent);


            CreatePipeWithOffset(intermediatePosz, intermediatePosx, parent);            

            Vector3 directionz = intermediatePosz - startPos;
            Vector3 normalizedDirectionz = directionz.normalized;

            AnalyzeNormalDirectionForX(normalizedDirectionz, intermediatePosz, startPos, endPos, parent);

            CreatePipeForEnd(intermediatePosz, endPos, parent);

        }
    }

    void GenerateForOvenAndElectro(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);

        if (startPos.y > endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        startPos.y += 12;

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180f))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= verticalOffset;

                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0,  90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(-180, 0, -90), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 0, 90), parent);
            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= verticalOffset;

                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, 0, -90), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreateCurveforSpecial(startPos, endPos, parent);
                CreatePipeForEnd(startPos, endPos, parent);
            }
        }
        else
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x += verticalOffset;

                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(-180, 0, 90), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 0, -90), parent);
            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x += verticalOffset;

                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, 90, -90), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, -90, -90), parent);
            }
            else
            {
                CreateCurveforSpecial(startPos, endPos, parent);
                CreatePipeForEnd(startPos, endPos, parent);
            }
        }
        

    }

    void GenerateForKatazAndOven(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        startPos.y += 12;
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        CreateCurveforSpecial(startPos, endPos, parent);
        CreatePipeForEnd(startPos, endPos, parent);
    }

    void GenerateForCoolingAndKataz(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreatePipeForStart(startPos, midPointX, parent);
        CreateCurveforNotSpecial(midPointX, new Vector3(71, -180, -90), parent);
        CreateCurveforNotSpecial(midPointY, new Vector3(244, -90, -90), parent);
        CreatePipeForEnd(midPointY, midPointZ, parent);
    }

    void GenerateForEmulAndCooling(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");

        parent.transform.SetParent(endObject.transform);

        if (startPos.y > endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreatePipeForStart(startPos, midPointY, parent);
        CreateCurveforNotSpecial(midPointY, new Vector3(174.67f, -90, 90), parent);
        CreatePipeForEnd(midPointY, endPos, parent);
    }

    void GenerateForTwoEmuls(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }


        if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 180f) && Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180f))
        {
            startPos.y += verticalOffset;
            endPos.x += verticalOffset;

            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);

            CreateCurveforSpecial(startPos, midPointX, parent);
            CreatePipeWithOffset(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(90, -90, 0), parent);
            CreatePipeWithOffset(midPointX, endPos, parent);
            CreateCurveforNotSpecial(endPos, new Vector3(-90, 0, -90), parent);
        }
        else
        {
            startPos.y += verticalOffset;
            endPos.x -= verticalOffset;

            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);

            CreateCurveforSpecial(startPos, midPointX, parent);
            CreatePipeWithOffset(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
            CreatePipeWithOffset(midPointX, endPos, parent);
            CreateCurveforNotSpecial(endPos, new Vector3(-90, 0, 90), parent);
        }

    }
     
    void GenerateForCollectorAndSbor(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        startPos.y += verticalOffset;
        endPos.y += verticalOffset;

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 180f) && Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 90f))
        {
            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

            CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
            CreatePipeWithOffset(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(0, -270f, 0), parent);
            CreatePipeWithOffset(midPointX, midPointY, parent);
            CreateCurveforNotSpecial(new Vector3(endPos.x, endPos.y, startPos.z - 1), new Vector3(0, 0, 180), parent);
            CreatePipeWithOffset(midPointY, midPointZ, parent);
            CreateCurveforNotSpecial(endPos, new Vector3(0, 180, 0), parent);
        }
        else
        {
            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

            CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
            CreatePipeWithOffset(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
            CreatePipeWithOffset(midPointX, midPointY, parent);
            CreateCurveforNotSpecial(new Vector3(endPos.x, endPos.y, startPos.z + 1), new Vector3(180, 0, 0f), parent);
            CreatePipeWithOffset(midPointY, midPointZ, parent);
            CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 0), parent);
        }


    }

    void GenerateForSborAndEmul(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }
        startPos.y += verticalOffset;
        endPos.y += verticalOffset + 12;
        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);


        if ((Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 180f)  || Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 90f)) 
            && (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 90f) || Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180f)))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(-180, 0, 90), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, -0, 0), parent);
                CreatePipe(new Vector3(endPos.x, startPos.y - verticalOffset, endPos.z), new Vector3(endPos.x, endPos.y - verticalOffset * 2, endPos.z), parent);
            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, 0, 90), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 180, 0), parent);
                CreatePipe(new Vector3(endPos.x, startPos.y - verticalOffset, endPos.z), new Vector3(endPos.x, endPos.y - verticalOffset * 2, endPos.z), parent);
            }
            else
            {
                //endPos.x -= verticalOffset;

                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(90, 0, 90), parent);
                CreatePipe(new Vector3(endPos.x, startPos.y - verticalOffset, endPos.z), new Vector3(endPos.x, endPos.y - verticalOffset * 2, endPos.z), parent);

            }


        }
        else
        {
            CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
            CreatePipeWithOffset(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 90), parent);
            CreatePipeWithOffset(midPointY, midPointZ, parent);
            CreateCurveforNotSpecial(midPointZ, new Vector3(0, -180, 0), parent);
            CreatePipe(new Vector3(endPos.x, startPos.y - verticalOffset, endPos.z), new Vector3(endPos.x, endPos.y - verticalOffset * 2, endPos.z), parent);
        }
    }

    void GenerateForParAndSbor(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 270f) && Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 270f))
        {
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            CreatePipeForStart(startPos, midPointY, parent);
            CreateCurveforNotSpecial(midPointY, new Vector3(-4.2f, 90, -90), parent);
            CreatePipeForEnd(midPointY, endPos, parent);
        }
        else
        {
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            CreatePipeForStart(startPos, midPointY, parent);
            CreateCurveforNotSpecial(midPointY, new Vector3(-5.782f, -90, -90),parent);
            CreatePipeForEnd(midPointY, endPos, parent);
        }


    }

    void GenerateForDryairAndSbor(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.y > endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 180f) && Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 90f))
        {
            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);

            CreatePipeForStart(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(270, 0, -270), parent);
            CreatePipeForEnd(midPointX, endPos, parent);
        }
        else
        {
            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);

            CreatePipeForStart(startPos, midPointX, parent);
            CreateCurveforNotSpecial(midPointX, new Vector3(270, 0, -90), parent);
            CreatePipeForEnd(midPointX, endPos, parent);
        }


    }

    void GenerateForCoolingAndElectro(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreatePipeForStart(startPos, midPointX, parent);
        CreateCurveforNotSpecial(midPointX, new Vector3(71, -180, -90),parent);
        CreateCurveforNotSpecial(midPointY, new Vector3(244, -90, -90),parent);
        CreatePipeForEnd(midPointY, midPointZ, parent);
    }

    void GenerateForOvenAndSbor(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        startPos.y += verticalOffset;
        endPos.y += verticalOffset;

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 90))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(-90, 180, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 0), parent);
            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(-90, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, 0), parent);
                Debug.Log("fasfasfasfadfasfasfa");
            }
            else
            {
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreatePipeForEnd(midPointX, new Vector3(endPos.x, endPos.y - pipeOffset * 1.4f, endPos.z), parent);
            }
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 270f))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, -180, 180), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 0, 0), parent);
                Debug.Log("ghjnkmghplkjmghpojm");
            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, -180), parent);
                CreatePipeWithOffset(midPointY, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 180, 0), parent);
            }
            else
            {
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, 90, 0), parent);
                CreatePipeForEnd(midPointX, new Vector3(endPos.x, endPos.y - pipeOffset * 1.4f, endPos.z), parent);
            }
        }
        else
        {
            
            Debug.Log("asdasdasdadawdawdawdawvdvawd");
            Debug.Log(startObject.parent.parent.localEulerAngles.y);
            Debug.Log(endObject.parent.parent.localEulerAngles.y);
        }
    }

    void GenerateForElectroAndEmul(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.x > endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(90, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);
                
                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 0))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, -180, 180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
    }

    void GenerateForKatazAndEmul(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.x > endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(90, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 0))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, -180, 180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
    }

    void GenerateForOvenAndEmul(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        startPos.y += verticalOffset;
        endPos.y += verticalOffset;

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 180))
        {
            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

            CreateCurveforNotSpecial(startPos, new Vector3(0, -90, 0), parent);
            //CreatePipeWithOffset(startPos, midPointX, parent);
            CreatePipe(new Vector3(startPos.x - pipeOffset, startPos.y,startPos.z), new Vector3(endPos.x + pipeOffset * 2, startPos.y, endPos.z), parent);

            //CreatePipe(new Vector3(endPos.x, startPos.y - verticalOffset, endPos.z), new Vector3(endPos.x, endPos.y - verticalOffset * 2, endPos.z), parent);

            CreateCurveforNotSpecial(new Vector3(endPos.x + pipeOffset * 2, startPos.y, startPos.z), new Vector3(90, -90, 0), parent);
            CreateCurveforNotSpecial(midPointZ, new Vector3(270, -90, 0), parent);
            Debug.Log("asdasd");
        }
        else if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 0))
        {
            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

            CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
            //CreatePipeWithOffset(startPos, midPointX, parent);
            //CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
            //CreatePipeWithOffset(midPointX, midPointY, parent);
            //CreateCurveforNotSpecial(midPointY, new Vector3(90, -180, 0), parent);
            //CreatePipeWithOffset(midPointY, midPointZ, parent);
            //CreateCurveforNotSpecial(midPointZ, new Vector3(0, -180, 0), parent);
            Debug.Log("ghjnkmghplkjmghpojm");
        }
        else
        {

            Debug.Log("asdasdasdadawdawdawdawvdvawd");
            Debug.Log(startObject.parent.parent.localEulerAngles.y);
            Debug.Log(endObject.parent.parent.localEulerAngles.y);
        }
    }   /*End this sheet*/

    void GenerateForElectroAndEmul2(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);

        if (startPos.x > endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 180))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                startPos.y += pipeOffset;
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(-90, -90, -90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                startPos.y += pipeOffset;
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, -180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
                Debug.Log("dasdasdasdaWdwqegernbrstgnbrts");
            }
            else
            {
                startPos.y += pipeOffset;
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(0, 90, 0), parent);
                CreatePipeWithOffset(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreatePipeWithOffset(midPointX, midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, 90, 180), parent);
            }
        }
        else if (Mathf.Approximately(startObject.parent.parent.localEulerAngles.y, 0))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.y += pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, -180, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 0), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.y += pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreatePipeWithOffset(midPointX, midPointY, parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, 0), parent);
            }
            else
            {
                startPos.x += pipeOffset;
                endPos.y += pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreateCurveforNotSpecial(startPos, new Vector3(-90, -90, 0), parent);
                CreatePipeWithOffset(startPos, new Vector3(startPos.x, endPos.y, startPos.z), parent);
                CreateCurveforNotSpecial(new Vector3(startPos.x, endPos.y, startPos.z), new Vector3(-180, -90, -180), parent);
                CreatePipeWithOffset(new Vector3(startPos.x, endPos.y, startPos.z), midPointZ, parent);
                CreateCurveforNotSpecial(midPointZ, new Vector3(0, -90, 0), parent);
            }
        }
    }

    void GenerateForKatazAndEmul2(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);

        if (startPos.x > endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(90, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 0), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 0))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, -180, 180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 0), parent);
                CreateCurveforNotSpecial(midPointY, new Vector3(0, 0, 180), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
    }

    void GenerateForElectroAndKataz(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);

        if (startPos.x > endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, -90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 0))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, -90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(0, -90, 90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
    }

    void GenerateForKatazAndNewSbor(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (startPos.x < endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
            Debug.Log("dknaoifubhnsuefg");
        }

        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(-180, -90, -90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 0, 90), parent);
                Debug.Log("dasdasdadsasdawdawdawd1212");

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                endPos.x -= pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(startPos, midPointX, parent);
                CreateCurveforNotSpecial(midPointX, new Vector3(180, -90, 90), parent);
                CreatePipeWithOffset(midPointY, endPos, parent);
                CreateCurveforNotSpecial(endPos, new Vector3(0, 180, -90), parent);
                Debug.Log("dasdasdadsasdawdawdawd1212");
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 0))
        {
            if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z > endPos.z)
            {
                startPos.x += pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(endPos, new Vector3(startPos.x, startPos.y, endPos.z), parent);
                CreateCurveforNotSpecial(new Vector3(startPos.x, startPos.y, endPos.z), new Vector3(0, 0, -90), parent);
                CreatePipeWithOffset(new Vector3(startPos.x, startPos.y, endPos.z), startPos, parent);
                CreateCurveforNotSpecial(startPos, new Vector3(-180, 0, -90), parent);
                Debug.Log("dasdasdadsasdawdawdawd1212");

            }
            else if (Mathf.Abs(startPos.z - endPos.z) >= 20 && startPos.z < endPos.z)
            {
                startPos.x += pipeOffset;
                Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
                Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
                Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

                CreatePipeForStart(endPos, new Vector3(startPos.x, startPos.y, endPos.z), parent);
                CreateCurveforNotSpecial(new Vector3(startPos.x, startPos.y, endPos.z), new Vector3(0, -90, -90), parent);
                CreatePipeWithOffset(new Vector3(startPos.x, startPos.y, endPos.z), startPos, parent);
                CreateCurveforNotSpecial(startPos, new Vector3(0, 0, -90), parent);
                Debug.Log("dasdasdadsasdawdawdawd1212");
            }
            else
            {
                CreatePipe(startPos, endPos, parent);
            }
        }
    }

    void GenerateLittleForKatazAndNewSbor(Vector3 startPos, Vector3 endPos)
    {
        GameObject parent = new GameObject("parent");
        parent.transform.SetParent(endObject.transform);
        if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 180))
        {
            if (startPos.x > endPos.x)
            {
                Vector3 startNew = startPos;
                startPos = endPos;
                endPos = startNew;
                Debug.Log("dknaoifubhnsuefg");
            }

            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

            CreateLittleCurveforNotSpecial(endPos, new Vector3(0, 0, 0), parent);
            CreateLittlePipeWithOffset(midPointY, endPos, parent);
            CreateLittleCurveforNotSpecial(midPointY, new Vector3(0, 180, 0), parent);
            CreateLittlePipeWithOffset(midPointX, midPointY, parent);
            CreateLittleCurveforNotSpecial(midPointX, new Vector3(0, -90, 180), parent);
            CreateLittlePipeForStart(startPos, midPointX, parent);
        }
        else if (Mathf.Approximately(endObject.parent.parent.localEulerAngles.y, 0))
        {
            if (startPos.x < endPos.x)
            {
                Vector3 startNew = startPos;
                startPos = endPos;
                endPos = startNew;
                Debug.Log("dknaoifubhnsuefg");
            }

            Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

            CreateLittlePipeForStart(startPos, midPointX, parent);
            CreateLittleCurveforNotSpecial(midPointX, new Vector3(0, 90, 180), parent);
            CreateLittlePipeWithOffset(midPointX, midPointY, parent);
            CreateLittleCurveforNotSpecial(midPointY, new Vector3(0, 0, 0), parent);
            CreateLittlePipeWithOffset(midPointY, endPos, parent);
            CreateLittleCurveforNotSpecial(endPos, new Vector3(0, -180, 0), parent);
        }


    }

    void CreateCurveforNotSpecial(Vector3 position, Vector3 direction, GameObject parent)
    {
        GameObject curvedPipe = Instantiate(curvedPipePrefabForStartsAndEnds, position, Quaternion.identity);
        curvedPipe.transform.SetParent(parent.transform);
        curvedPipe.transform.position = position;
        curvedPipe.transform.Rotate(direction);
        curvedPipe.transform.SetParent(parent.transform);
    }

    void CreateLittleCurveforNotSpecial(Vector3 position, Vector3 direction, GameObject parent)
    {
        GameObject curvedPipe = Instantiate(littlecurvedPipePrefabForStartsAndEnds, position, Quaternion.identity);
        curvedPipe.transform.SetParent(parent.transform);
        curvedPipe.transform.position = position;
        curvedPipe.transform.Rotate(direction);
        curvedPipe.transform.SetParent(parent.transform);
    }

    void CreateCurveforSpecial(Vector3 position, Vector3 direction, GameObject parent)
    {
        GameObject curvedPipe = Instantiate(curvedPipePrefabForStartsAndEnds, position, Quaternion.identity);
        curvedPipe.transform.position = position;
        curvedPipe.transform.LookAt(direction);
        curvedPipe.transform.SetParent(parent.transform);
    }

    void CreatePipeWithOffset(Vector3 start, Vector3 end, GameObject parent)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 adjustedStart = start + direction * pipeOffset;
        Vector3 adjustedEnd = end - direction * pipeOffset;

        GameObject pipe = Instantiate(pipePrefab, adjustedStart, Quaternion.identity);
        direction = adjustedEnd - adjustedStart;
        float distance = direction.magnitude;
        pipe.transform.position = (adjustedStart + adjustedEnd) / 2; // Position the pipe in the middle of the two points
        pipe.transform.up = direction; // Rotate the pipe to face the second point
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z); // Scale the pipe to fit the distance
        pipe.transform.SetParent(parent.transform);
    }

    void CreateLittlePipeWithOffset(Vector3 start, Vector3 end, GameObject parent)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 adjustedStart = start + direction * pipeOffset/2.3f;
        Vector3 adjustedEnd = end - direction * pipeOffset/2.3f;

        GameObject pipe = Instantiate(littlePipePrefab, adjustedStart, Quaternion.identity);
        direction = adjustedEnd - adjustedStart;
        float distance = direction.magnitude;
        pipe.transform.position = (adjustedStart + adjustedEnd) / 2; // Position the pipe in the middle of the two points
        pipe.transform.up = direction; // Rotate the pipe to face the second point
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z); // Scale the pipe to fit the distance
        pipe.transform.SetParent(parent.transform);
    }

    void CreatePipe(Vector3 start, Vector3 end, GameObject parent)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 adjustedStart = start + direction;
        Vector3 adjustedEnd = end - direction;

        GameObject pipe = Instantiate(pipePrefab, adjustedStart, Quaternion.identity);
        direction = adjustedEnd - adjustedStart;
        float distance = direction.magnitude;
        pipe.transform.position = (adjustedStart + adjustedEnd) / 2; // Position the pipe in the middle of the two points
        pipe.transform.up = direction; // Rotate the pipe to face the second point
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z); // Scale the pipe to fit the distance
        pipe.transform.SetParent(parent.transform);
    }

    void CreatePipeForStart(Vector3 start, Vector3 end,GameObject parent)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 adjustedStart = start + direction ;
        Vector3 adjustedEnd = end - direction * pipeOffset;

        GameObject pipe = Instantiate(pipePrefab, adjustedStart, Quaternion.identity);
        direction = adjustedEnd - adjustedStart;
        float distance = direction.magnitude;
        pipe.transform.position = (adjustedStart + adjustedEnd) / 2; // Position the pipe in the middle of the two points
        pipe.transform.up = direction; // Rotate the pipe to face the second point
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z); // Scale the pipe to fit the distance
        pipe.transform.SetParent(parent.transform);
    }

    void CreateLittlePipeForStart(Vector3 start, Vector3 end, GameObject parent)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 adjustedStart = start + direction;
        Vector3 adjustedEnd = end - direction * pipeOffset/2.3f;

        GameObject pipe = Instantiate(littlePipePrefab, adjustedStart, Quaternion.identity);
        direction = adjustedEnd - adjustedStart;
        float distance = direction.magnitude;
        pipe.transform.position = (adjustedStart + adjustedEnd) / 2; // Position the pipe in the middle of the two points
        pipe.transform.up = direction; // Rotate the pipe to face the second point
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z); // Scale the pipe to fit the distance
        pipe.transform.SetParent(parent.transform);
    }

    void CreatePipeForEnd(Vector3 start, Vector3 end, GameObject parent)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 adjustedStart = start + direction * pipeOffset;
        Vector3 adjustedEnd = end - direction;

        GameObject pipe = Instantiate(pipePrefab, adjustedStart, Quaternion.identity);
        direction = adjustedEnd - adjustedStart;
        float distance = direction.magnitude;
        pipe.transform.position = (adjustedStart + adjustedEnd) / 2; // Position the pipe in the middle of the two points
        pipe.transform.up = direction; // Rotate the pipe to face the second point
        pipe.transform.localScale = new Vector3(pipe.transform.localScale.x, distance / 2, pipe.transform.localScale.z); // Scale the pipe to fit the distance
        pipe.transform.SetParent(parent.transform);
    }

    void CreateCurvedPipeX(Vector3 position, Vector3 startDirection, Vector3 endDirection, GameObject parent)
    {
        // Instantiate the curved pipe prefab at the specified position
        GameObject pipe = Instantiate(curvedPipePrefab, position, Quaternion.identity);

        // Calculate the rotation needed to align the curved pipe
        Vector3 direction = startDirection - endDirection;

        // Rotate the pipe to align with the average direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // Check the x rotation and adjust if needed

        // Apply the calculated rotation to the pipe
        pipe.transform.rotation = targetRotation;

        Debug.Log(pipe.transform.localRotation.x);
        Debug.Log(pipe.transform.eulerAngles.x);
        Debug.Log(pipe.transform.localEulerAngles.x);
        Debug.Log(pipe.transform.localEulerAngles.z);

        if (pipe.transform.localEulerAngles.x > 0)
        {
            if (pipe.transform.localEulerAngles.x > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(90f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }
        else if (pipe.transform.localEulerAngles.x == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(90f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }

        pipe.transform.SetParent(parent.transform);
    }

    void CreateCurvedPipeY(Vector3 position, Vector3 startDirection, Vector3 endDirection, GameObject parent)
    {
        // Instantiate the curved pipe prefab at the specified position
        GameObject pipe = Instantiate(curvedPipePrefab, position, Quaternion.identity);

        // Calculate the rotation needed to align the curved pipe
        Vector3 direction = startDirection - endDirection;

        // Rotate the pipe to align with the average direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // Check the x rotation and adjust if needed

        // Apply the calculated rotation to the pipe
        pipe.transform.rotation = targetRotation;

        Debug.Log(pipe.transform.localRotation.z);
        Debug.Log(pipe.transform.eulerAngles.z);
        Debug.Log(pipe.transform.localEulerAngles.z);

        if (pipe.transform.localEulerAngles.x > 0)
        {
            if (pipe.transform.localEulerAngles.x > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }
        else if (pipe.transform.localEulerAngles.x == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }

        if (pipe.transform.localEulerAngles.y > 0)
        {
            if (pipe.transform.localEulerAngles.y > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 0f, pipe.transform.localEulerAngles.z);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, -90f, pipe.transform.localEulerAngles.z);
        }
        else if (pipe.transform.localEulerAngles.y == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 90f, pipe.transform.localEulerAngles.z);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 0f, pipe.transform.localEulerAngles.z);
        }

        if (pipe.transform.localEulerAngles.z > 0)
        {
            if (pipe.transform.localEulerAngles.z > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 0f);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
        else if (pipe.transform.localEulerAngles.z == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
        pipe.transform.SetParent(parent.transform);
    }

    void CreateCurvedPipeZ(Vector3 position, Vector3 startDirection, Vector3 endDirection, GameObject parent)
    {
        // Instantiate the curved pipe prefab at the specified position
        GameObject pipe = Instantiate(curvedPipePrefab, position, Quaternion.identity);

        // Calculate the rotation needed to align the curved pipe
        Vector3 direction = startDirection - endDirection;

        // Rotate the pipe to align with the average direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // Check the x rotation and adjust if needed

        // Apply the calculated rotation to the pipe
        pipe.transform.rotation = targetRotation;

        Debug.Log(pipe.transform.localRotation.z);
        Debug.Log(pipe.transform.eulerAngles.z);
        Debug.Log(pipe.transform.localEulerAngles.z);

        if (pipe.transform.localEulerAngles.x > 0)
        {
            if (pipe.transform.localEulerAngles.x > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(90f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }
        else if (pipe.transform.localEulerAngles.x == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(0f, pipe.transform.localEulerAngles.y, pipe.transform.localEulerAngles.z);
        }

        if (pipe.transform.localEulerAngles.y > 0)
        {
            if (pipe.transform.localEulerAngles.y > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 0f, pipe.transform.localEulerAngles.z);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 90f, pipe.transform.localEulerAngles.z);
        }
        else if (pipe.transform.localEulerAngles.y == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 90f, pipe.transform.localEulerAngles.z);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 0f, pipe.transform.localEulerAngles.z);
        }

        if (pipe.transform.localEulerAngles.z > 0)
        {
            if (pipe.transform.localEulerAngles.z > 256f)
            {
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 0f);
            }
            else
                pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
        else if (pipe.transform.localEulerAngles.z == 0)
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 0f);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
        pipe.transform.SetParent(parent.transform);
    }

    void CreateCurvedPipeForEndAndStart(Vector3 position, Vector3 start, Vector3 end, GameObject parent)
    {
        GameObject curvedPipe = Instantiate(curvedPipePrefabForStartsAndEnds, position, Quaternion.identity);

        // Calculate direction from start to end
        Vector3 direction = end - start;

        // Rotate the curved pipe to face towards the end point
        curvedPipe.transform.LookAt(end);

        // Set the position of the curved pipe
        curvedPipe.transform.position = position;
        curvedPipe.transform.SetParent(parent.transform);
    }

    //void AnalyzeNormalDirection(Vector3 normalizedDirection, Vector3 startPos, Vector3 endPos, GameObject parent)
    //{

    //    if (Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y) && Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.z))
    //    {
    //        if (normalizedDirection.x > 0)
    //        {
    //            Debug.Log("Направление: вправо");
    //        }
    //        else
    //        {
    //            Debug.Log("Направление: влево");
    //        }
    //    }
    //    else if (Mathf.Abs(normalizedDirection.y) > Mathf.Abs(normalizedDirection.x) && Mathf.Abs(normalizedDirection.y) > Mathf.Abs(normalizedDirection.z))
    //    {
    //        if (normalizedDirection.y > 0)
    //        {
    //            if (startPos.x > endPos.x)
    //            {
    //                Debug.Log("Направление: вверх ");
    //                Instantiate(curvedPipes[4], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[4].transform.rotation);
    //            }
    //            else
    //            {
    //                Debug.Log("Направление: вверх ");
    //                Instantiate(curvedPipes[0], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[0].transform.rotation);
    //            }

    //        }
    //        else
    //        {
    //            if (startPos.x < endPos.x)
    //            {
    //                Instantiate(curvedPipes[7], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[7].transform.rotation);
    //                Debug.Log("Направление: вниз");
    //            }
    //            else
    //            {
    //                Instantiate(curvedPipes[3], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[3].transform.rotation);
    //                Debug.Log("Направление: вниз");
    //            }

    //        }
    //    }
    //    else if (Mathf.Abs(normalizedDirection.z) > Mathf.Abs(normalizedDirection.x) && Mathf.Abs(normalizedDirection.z) > Mathf.Abs(normalizedDirection.y))
    //    {
    //        if (normalizedDirection.z > 0)
    //        {
    //            if (startPos.z < endPos.z)
    //            {
    //                Instantiate(curvedPipes[2], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[2].transform.rotation);
    //                Debug.Log("Направление: вперед");
    //            }
    //            else
    //            {
    //                Instantiate(curvedPipes[6], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[6].transform.rotation);
    //                Debug.Log("Направление: вперед");
    //            }
    //        }
    //        else
    //        {
    //            if (startPos.z > endPos.z)
    //            {
    //                Instantiate(curvedPipes[1], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[1].transform.rotation);
    //                Debug.Log("Направление: назад");
    //            }
    //            else
    //            {
    //                Instantiate(curvedPipes[5], new Vector3(endPos.x, endPos.y, startPos.z), curvedPipes[5].transform.rotation);
    //                Debug.Log("Направление: назад");
    //            }


    //        }
    //    }
    //}


    //void AnalyzeNormalDirection(Vector3 normalizedDirection, Vector3 position, Vector3 startPos, Vector3 endPos, GameObject parent)
    //{

    //    if (Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.y) && Mathf.Abs(normalizedDirection.x) > Mathf.Abs(normalizedDirection.z))
    //    {
    //        Debug.Log(normalizedDirection);
    //        if (normalizedDirection.x > 0)
    //        {
    //            if (startPos.x < endPos.x)
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[8], position, curvedPipes[8].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: вправо");
    //            }
    //            else
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[9], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[9].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: вправо 2");
    //            }
    //        }
    //        else
    //        {
    //            if (startPos.x > endPos.x)
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[10], position, curvedPipes[10].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: влево");
    //            }
    //            else
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[11], position, curvedPipes[11].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: влево 2");
    //            }
    //        }
    //    }
    //    else if (Mathf.Abs(normalizedDirection.y) > Mathf.Abs(normalizedDirection.x) && Mathf.Abs(normalizedDirection.y) > Mathf.Abs(normalizedDirection.z))
    //    {
    //        if (normalizedDirection.y > 0)
    //        {
    //            if (startPos.y < endPos.y)
    //            {
    //                Debug.Log("Направление: вверх ");
    //                GameObject pipe = Instantiate(curvedPipes[4], position, curvedPipes[4].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //            }
    //            else
    //            {
    //                Debug.Log("Направление: вверх 2 ");
    //                GameObject pipe = Instantiate(curvedPipes[0], position, curvedPipes[0].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //            }
    //        }
    //        else
    //        {
    //            if (startPos.y > endPos.y)
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[7], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[7].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: вниз");
    //            }
    //            else
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[3], position, curvedPipes[3].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: вниз 2 ");
    //            }
    //        }
    //    }
    //    else if (Mathf.Abs(normalizedDirection.z) > Mathf.Abs(normalizedDirection.x) && Mathf.Abs(normalizedDirection.z) > Mathf.Abs(normalizedDirection.y))
    //    {
    //        if (normalizedDirection.z > 0)
    //        {
    //            if (startPos.z < endPos.z)
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[2], position, curvedPipes[2].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: вперед");
    //            }
    //            else
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[6], position, curvedPipes[6].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: вперед 2 ");
    //            }
    //        }
    //        else
    //        {
    //            if (startPos.z < endPos.z)
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[1], new Vector3(endPos.x, startPos.y, startPos.z), curvedPipes[1].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: назад");
    //            }
    //            else
    //            {
    //                GameObject pipe = Instantiate(curvedPipes[5], position, curvedPipes[5].transform.rotation);
    //                pipe.transform.SetParent(parent.transform);
    //                Debug.Log("Направление: назад 2 ");
    //            }
    //        }
    //    }

    //}

    void AnalyzeNormalDirectionForX(Vector3 normalizedDirection, Vector3 position, Vector3 startPos, Vector3 endPos, GameObject parent)
    {
        Debug.Log(normalizedDirection);
        if (startPos.z < endPos.z)
        {
            GameObject pipe = Instantiate(curvedPipes[2], position, curvedPipes[2].transform.rotation);
            pipe.transform.SetParent(parent.transform);
            Debug.Log("Направление: вперед");
        }
        else
        {
            GameObject pipe = Instantiate(curvedPipes[5], position, curvedPipes[5].transform.rotation);
            pipe.transform.SetParent(parent.transform);
            Debug.Log("Направление: назад ");
        }
    }

    void AnalyzeNormalDirectionForY(Vector3 normalizedDirection, Vector3 position, Vector3 startPos, Vector3 endPos, GameObject parent)
    {
        Debug.Log(normalizedDirection);
        if (startPos.z < endPos.z)
        {
            GameObject pipe = Instantiate(curvedPipes[9], position, curvedPipes[9].transform.rotation);
            pipe.transform.SetParent(parent.transform);
            Debug.Log("Направление: вправо");
        }
        else
        {
            GameObject pipe = Instantiate(curvedPipes[11], position, curvedPipes[11].transform.rotation);
            pipe.transform.SetParent(parent.transform);
            Debug.Log("Направление: лево");
        }
    }

    void AnalyzeNormalDirectionForZ(Vector3 normalizedDirection, Vector3 position, Vector3 startPos, Vector3 endPos, GameObject parent)
    {
        Debug.Log(normalizedDirection);
        if (startPos.x < endPos.x)
        {
            GameObject pipe = Instantiate(curvedPipes[0], position, curvedPipes[0].transform.rotation);
            pipe.transform.SetParent(parent.transform);
            Debug.Log("Направление: вправо");
        }
        else
        {
            GameObject pipe = Instantiate(curvedPipes[7], position, curvedPipes[7].transform.rotation);
            pipe.transform.SetParent(parent.transform);
            Debug.Log("Направление: вниз");
        }
    }
}
