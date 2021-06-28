using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject resourceCostOptionsObject;

    public ResourceCostOptions resourceCostOptions;
    public ResourceCostOptions resourceCostOptions2;
    // Start is called before the first frame update

    private void Awake()
    {
        resourceCostOptions = FindObjectOfType(typeof(ResourceCostOptions)) as ResourceCostOptions;
        resourceCostOptions2 = resourceCostOptionsObject.GetComponentInChildren<ResourceCostOptions>();
    }
    void Start()
    {
        //resourceCostOptions = FindObjectOfType(typeof(ResourceCostOptions)) as ResourceCostOptions;
        //resourceCostOptions2 = resourceCostOptionsObject.GetComponentInChildren<ResourceCostOptions>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
