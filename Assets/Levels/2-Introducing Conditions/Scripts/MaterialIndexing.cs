using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Materials", menuName = "ScriptableObjects/MaterialIndexing", order = 1)]
public class MaterialIndexing : ScriptableObject
{
    public Material[] materials;
}
