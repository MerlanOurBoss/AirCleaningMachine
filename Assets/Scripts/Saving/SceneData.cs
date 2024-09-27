using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectData
{
    public string objectName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[Serializable]
[CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObjects/SceneData", order = 1)]
public class SceneData : ScriptableObject
{
    public ObjectData[] objectsData;
    public string screenshotPath;
}

