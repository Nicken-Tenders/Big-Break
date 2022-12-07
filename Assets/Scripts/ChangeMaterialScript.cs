using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialScript : MonoBehaviour
{
    public Material[] panelMaterial;
    Renderer panelRenderer;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetMaterialOnInstantiate()
    {
        panelRenderer = GetComponent<Renderer>();
        panelRenderer.enabled = true;
        panelRenderer.sharedMaterial = panelMaterial[0];
    }

    public void ChangeMaterial()
    {
        panelRenderer.sharedMaterial = panelMaterial[1];
    }

}
