using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string name;
    public string desc;
    public Mesh mesh;
    public Material mat;

    public InventoryItem(string iname, Mesh imesh, Material paint){
        name = iname;
        mesh = imesh;
        mat = paint;
    }
}
