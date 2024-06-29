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

                    if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Emul")
                    {
                        GenerateForTwoEmuls(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Cooling"  && endObject.parent.tag == "Kataz")
                    {
                        GenerateForCoolingAndKataz(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Kataz" && endObject.parent.tag == "Cooling")
                    {
                        GenerateForCoolingAndKataz(startObject.position, endObject.position);
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
                    else if (startObject.parent.tag == "Sbor" && endObject.parent.tag == "Emul")
                    {
                        GenerateForSborAndEmul(startObject.position, endObject.position);
                    }
                    else if (startObject.parent.tag == "Emul" && endObject.parent.tag == "Sbor")
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
                    else
                    {
                        GeneratePipes(startObject.position, endObject.position);
                    }
                    


                    firstClick = true;
                }
            }
        }
    }

    void GenerateForTwoEmuls(Vector3 startPos, Vector3 endPos)
    {
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }
        
        startPos.y += verticalOffset;
        endPos.x -= verticalOffset;

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreateCurveforSpecial(startPos, new Vector3(0, 90, 0));
        CreatePipeWithOffset(startPos, midPointX);
        CreateCurveforSpecial(midPointX, new Vector3(0,-90,0));
        CreatePipeWithOffset(midPointX, endPos);
        CreateCurveforSpecial(endPos, new Vector3(-90, 90, 0));
    }

    void GenerateForCoolingAndKataz(Vector3 startPos, Vector3 endPos)
    {
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreatePipeForStart(startPos, midPointX);
        CreateCurveforSpecial(midPointX, new Vector3(71, -180, -90));
        CreateCurveforSpecial(midPointY, new Vector3(244, -90, -90));
        CreatePipeForEnd(midPointY, midPointZ);
    }

    void GenerateForEmulAndCooling(Vector3 startPos, Vector3 endPos)
    {
        if (startPos.y > endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreatePipeForStart(startPos, midPointY);
        CreateCurveforSpecial(midPointY, new Vector3(174.67f, -90,90));
        CreatePipeForEnd(midPointY, endPos);
    }

    void GenerateForCollectorAndSbor(Vector3 startPos, Vector3 endPos)
    {
        startPos.y += verticalOffset;
        endPos.y += verticalOffset;

        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreateCurveforSpecial(startPos, new Vector3(0, 90, 0));
        CreatePipeWithOffset(startPos, midPointX);
        CreateCurveforSpecial(midPointX, new Vector3(0, -90, -9.2f));
        CreateCurveforSpecial(midPointY, new Vector3(180, 0, -5.75f));
        CreatePipeWithOffset(midPointY, midPointZ);
        CreateCurveforSpecial(endPos, new Vector3(0, 0, 0));
    }

    void GenerateForSborAndEmul(Vector3 startPos, Vector3 endPos)
    {
        startPos.y += verticalOffset;
        endPos.y += verticalOffset+12;
        if (startPos.x > endPos.x)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);
        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);

        CreateCurveforSpecial(startPos, new Vector3(0, 90, 0));
        CreatePipeWithOffset(startPos, midPointX);
        CreateCurveforSpecial(midPointX, new Vector3(0, -90, 90));
        CreatePipeWithOffset(midPointY, midPointZ);
        CreateCurveforSpecial(midPointZ, new Vector3(0, -180, 0));
        CreatePipe(new Vector3(endPos.x, startPos.y - verticalOffset, endPos.z), new Vector3(endPos.x, endPos.y - verticalOffset*2 , endPos.z));
    }

    void GenerateForParAndSbor(Vector3 startPos, Vector3 endPos)
    {
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointY = new Vector3(endPos.x, endPos.y, startPos.z);
        Debug.Log("fasfa");
        CreatePipeForStart(startPos, midPointY);
        CreateCurveforSpecial(midPointY, new Vector3(-4.2f, 90,-90));
        CreatePipeForEnd(midPointY, endPos);
    }

    void GenerateForDryairAndSbor(Vector3 startPos, Vector3 endPos)
    {
        if (startPos.y > endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }

        Vector3 midPointX = new Vector3(endPos.x, startPos.y, startPos.z);

        CreatePipeForStart(startPos, midPointX);
        CreateCurveforSpecial(midPointX, new Vector3(270,0,-90));
        CreatePipeForEnd(midPointX, endPos);
    }

    void GenerateForOvenAndElectro(Vector3 startPos, Vector3 endPos)
    {
        startPos.y += 12;
        if (startPos.y < endPos.y)
        {
            Vector3 startNew = startPos;
            startPos = endPos;
            endPos = startNew;
        }
        
        CreateCurveforSpecial(startPos, new Vector3(90, 0, -90));
        CreatePipeForEnd(startPos, endPos);
    }



    void CreateCurveforSpecial(Vector3 position, Vector3 direction)
    {
        GameObject curvedPipe = Instantiate(curvedPipePrefabForStartsAndEnds, position, Quaternion.identity);
        curvedPipe.transform.position = position;
        curvedPipe.transform.Rotate(direction);
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
            CreatePipe(startPos, endPos);
        }
        else if (xEqual && zEqual)
        {
            // Create a straight pipe along the Y axis
            CreatePipe(startPos, endPos);
        }
        else if (yEqual && zEqual)
        {
            // Create a straight pipe along the X axis
            CreatePipe(startPos, endPos);
        }

        // Двойнные трубы трубы
        else if (xEqual)
        {
            Vector3 intermediatePos = new Vector3(startPos.x, endPos.y, startPos.z);
            CreatePipeForStart(startPos, intermediatePos);
            CreateCurvedPipeX(intermediatePos, intermediatePos - startPos, endPos - intermediatePos);
            CreatePipeForEnd(intermediatePos, endPos);
        }

        else if (yEqual)
        {
            Vector3 intermediatePos = new Vector3(startPos.x, startPos.y, endPos.z);
            CreatePipeForStart(startPos, midPointY);
            CreateCurvedPipeY(midPointY, intermediatePos - startPos, endPos - intermediatePos);
            CreatePipeForEnd(midPointY, endPos);
            Debug.Log("ADadasd");
        }

        else if (zEqual)
        {
            Vector3 intermediatePos = new Vector3(endPos.x, startPos.y, startPos.z);

            CreateCurvedPipeForEndAndStart(startPos, startPos, intermediatePos);
            CreatePipeWithOffset(startPos, intermediatePos);
            CreateCurvedPipeX(intermediatePos, intermediatePos - startPos, endPos - intermediatePos);
            CreatePipeWithOffset(intermediatePos, endPos);
            CreateCurvedPipeZ(endPos, intermediatePos , endPos );
            
        }
        else
        {
            CreatePipeForStart(startPos, midPointX);
            CreateCurvedPipeY(midPointX, midPointX - startPos, endPos - midPointX);
            CreatePipeWithOffset(midPointX, midPointY);
            CreateCurvedPipeZ(midPointY, midPointY - startPos, endPos - midPointY);
            CreatePipeForEnd(midPointY, midPointZ);

        }
    }

    void CreatePipeWithOffset(Vector3 start, Vector3 end)
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
    }

    void CreatePipe(Vector3 start, Vector3 end)
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
    }

    void CreatePipeForStart(Vector3 start, Vector3 end)
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
    }

    void CreatePipeForEnd(Vector3 start, Vector3 end)
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
    }

    void CreateCurvedPipeX(Vector3 position, Vector3 startDirection, Vector3 endDirection)
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
    }

    void CreateCurvedPipeY(Vector3 position, Vector3 startDirection, Vector3 endDirection)
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
    }

    void CreateCurvedPipeZ(Vector3 position, Vector3 startDirection, Vector3 endDirection)
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
    }

    void CreateCurvedPipeForEndAndStart(Vector3 position, Vector3 start, Vector3 end)
    {
        GameObject curvedPipe = Instantiate(curvedPipePrefabForStartsAndEnds, position, Quaternion.identity);

        // Calculate direction from start to end
        Vector3 direction = end - start;

        // Rotate the curved pipe to face towards the end point
        curvedPipe.transform.LookAt(end);

        // Set the position of the curved pipe
        curvedPipe.transform.position = position;
    }

}
