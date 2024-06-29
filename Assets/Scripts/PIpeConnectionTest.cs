using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PipeConnectionTest : MonoBehaviour
{
    // Assign the pipe prefabs in the inspector
    public GameObject straightPipePrefab;
    public GameObject curvedPipePrefab;
    public GameObject starterCurvedPipePrefab;

    // Assign the two objects between which the pipe will be placed
    public Transform object1;
    public Transform object2;

    public float pipeOffset = 0.1f; // Set the offset between pipes
    public float verticalOffset = 0.1f; // Offset to place pipes above the 


    void Start()
    {
        GeneratePipes(object1.position, object2.position);
    }

    void GeneratePipes(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = endPos - startPos;
        Vector3 midPoint;

        // Check if the X, Y, or Z coordinates are approximately equal within ±5 units
        bool xEqual = Mathf.Abs(startPos.x - endPos.x) <= 10f; 
        bool yEqual = Mathf.Abs(startPos.y - endPos.y) <= 10f; 
        bool zEqual = Mathf.Abs(startPos.z - endPos.z) <= 10f; 

        // Check if either object1 or object2 has localEulerAngles.z equal to 90 degrees
        bool object1ZRotated = Mathf.Approximately(object1.localEulerAngles.z, 90f);
        bool object2ZRotated = Mathf.Approximately(object2.localEulerAngles.z, 90f);

        if (xEqual && yEqual)
        {
            // Create a straight pipe along the Z axis
            midPoint = (startPos + endPos) / 2f;
            CreateStraightPipe(midPoint, direction, direction.magnitude);
        }
        else if (xEqual && zEqual)
        {
            // Create a straight pipe along the Y axis
            midPoint = (startPos + endPos) / 2f;
            CreateStraightPipe(midPoint, direction, direction.magnitude);
        }
        else if (yEqual && zEqual && object2ZRotated) //////////////////////////////////////////////////////////// offset addded
        {
            // Create a straight pipe along the X axis
            Vector3 midPointZ = new Vector3(endPos.x, endPos.y, endPos.z);
            midPoint = (startPos + endPos) / 2f;

            midPoint.y += verticalOffset;

            CreateStraightPipe(midPoint, direction, direction.magnitude);
            CreateStarterCurvedPipe(midPointZ, midPointZ, endPos);
        }
        else if (yEqual && zEqual)
        {
            // Create a straight pipe along the X axis
            midPoint = (startPos + endPos) / 2f;
            CreateStraightPipe(midPoint, direction, direction.magnitude);
        }
        else if (xEqual)
        {
            // Create a pipe along the X axis first, then a curved pipe
            Vector3 intermediatePos = new Vector3(startPos.x, endPos.y, startPos.z);
            CreateStraightPipe((startPos + intermediatePos) / 2f, intermediatePos - startPos, (intermediatePos - startPos).magnitude);
            CreateCurvedPipe(intermediatePos, intermediatePos - startPos, endPos - intermediatePos);
            CreateStraightPipe((intermediatePos + endPos) / 2f, endPos - intermediatePos, (endPos - intermediatePos).magnitude);
        }
        else if (yEqual)
        {
            // Create a pipe along the Y axis first, then a curved pipe
            Vector3 intermediatePos = new Vector3(startPos.x, startPos.y, endPos.z);
            CreateStraightPipe((startPos + intermediatePos) / 2f, intermediatePos - startPos, (intermediatePos - startPos).magnitude);
            CreateCurvedPipeY(intermediatePos, intermediatePos - startPos, endPos - intermediatePos);
            CreateStraightPipe((intermediatePos + endPos) / 2f, endPos - intermediatePos, (endPos - intermediatePos).magnitude);
            
        }
        else if (zEqual)
        {
            // Create a pipe along the Z axis first, then a curved pipe
            Vector3 intermediatePos = new Vector3(endPos.x, startPos.y, startPos.z);
            CreateStraightPipe((startPos + intermediatePos) / 2f, intermediatePos - startPos, (intermediatePos - startPos).magnitude);
            CreateCurvedPipe(intermediatePos, intermediatePos - startPos, endPos - intermediatePos);
            CreateStraightPipe((intermediatePos + endPos) / 2f, endPos - intermediatePos, (endPos - intermediatePos).magnitude);
            
        }
        else if ((!zEqual && !xEqual && !yEqual) && object1ZRotated && object2ZRotated)
        {
            // No coordinates are equal within ±5 units, create pipes in two steps with curved connections
            Vector3 intermediatePos1 = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 intermediatePos2 = new Vector3(endPos.x, endPos.y, startPos.z);
            CreateStarterCurvedPipe(startPos, startPos, intermediatePos1);

            // Create straight pipe segments with curved connections
            CreateStraightPipe((startPos + intermediatePos1) / 2f, intermediatePos1 - startPos, (intermediatePos1 - startPos).magnitude);
            CreateCurvedPipe(intermediatePos1, intermediatePos1 - startPos, intermediatePos2 - intermediatePos1);
            CreateStraightPipe((intermediatePos1 + intermediatePos2) / 2f, intermediatePos2 - intermediatePos1, (intermediatePos2 - intermediatePos1).magnitude);
            CreateCurvedPipe(intermediatePos2, intermediatePos2 - intermediatePos1, endPos - intermediatePos2);
            CreateStraightPipe((intermediatePos2 + endPos) / 2f, endPos - intermediatePos2, (endPos - intermediatePos2).magnitude);

            // Create final curved pipe
            CreateStarterCurvedPipe(endPos, intermediatePos2, endPos);
        }
        else
        {
            Vector3 intermediatePos1 = new Vector3(endPos.x, startPos.y, startPos.z);
            Vector3 intermediatePos2 = new Vector3(endPos.x, endPos.y, startPos.z);

            float verticalOffset = 10f; // Change this value as needed
            float horizontalOffset = 10f; // Change this value as needed

            CreateStraightPipe((startPos + intermediatePos1) / 2f, intermediatePos1 - startPos, (intermediatePos1 - startPos).magnitude);
            CreateCurvedPipe(intermediatePos1 + Vector3.right * horizontalOffset, intermediatePos1 - startPos, intermediatePos2 - intermediatePos1);
            CreateStraightPipe((intermediatePos1 + intermediatePos2) / 2f, intermediatePos2 - intermediatePos1, (intermediatePos2 - intermediatePos1).magnitude);
            CreateCurvedPipe(intermediatePos2 + Vector3.right * horizontalOffset, intermediatePos2 - intermediatePos1, endPos - intermediatePos2);
            CreateStraightPipe((intermediatePos2 + endPos) / 2f, endPos - intermediatePos2, (endPos - intermediatePos2).magnitude);
            Debug.Log("dasdasdasda");
        }

    }

    void CreateStraightPipe(Vector3 position, Vector3 direction, float distance)
    {
        // Instantiate the straight pipe prefab at the specified position
        GameObject pipe = Instantiate(straightPipePrefab, position, Quaternion.identity);

        // Rotate the pipe to face the direction
        pipe.transform.up = direction.normalized;

        // Adjust the scale of the pipe to fit the distance
        Vector3 pipeScale = pipe.transform.localScale;
        pipeScale.y = distance / 2f; // Assuming the pipe's length is aligned with its Y axis
        pipe.transform.localScale = pipeScale;
    }

    void CreateCurvedPipe(Vector3 position, Vector3 startDirection, Vector3 endDirection)
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


        //if (pipe.transform.localEulerAngles.y > 0)
        //{
        //    if (pipe.transform.localEulerAngles.y > 256f)
        //    {
        //        pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 0f, pipe.transform.localEulerAngles.z);
        //    }
        //    else
        //        pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 90f, pipe.transform.localEulerAngles.z);
        //}
        //else if (pipe.transform.localEulerAngles.y == 0)
        //{
        //    pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 90f, pipe.transform.localEulerAngles.z);
        //}
        //else
        //{
        //    pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, 0f, pipe.transform.localEulerAngles.z);
        //}


        //if (pipe.transform.localEulerAngles.z > 0)
        //{
        //    if (pipe.transform.localEulerAngles.z > 256f)
        //    {
        //        pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 0f);
        //    }
        //    else
        //        pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        //}
        //else if (pipe.transform.localEulerAngles.z == 0)
        //{
        //    pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        //}
        //else
        //{
        //    pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        //}
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
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
        else
        {
            pipe.transform.localRotation = Quaternion.Euler(pipe.transform.localEulerAngles.x, pipe.transform.localEulerAngles.y, 90f);
        }
    }

    void CreateStarterCurvedPipe(Vector3 position, Vector3 start, Vector3 end)
    {
        GameObject curvedPipe = Instantiate(starterCurvedPipePrefab, position, Quaternion.identity);

        curvedPipe.transform.LookAt(end);

        curvedPipe.transform.position = position;
    }
}