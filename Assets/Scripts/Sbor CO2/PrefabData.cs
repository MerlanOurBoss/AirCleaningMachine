using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabData : ScriptableObject
{
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
}
