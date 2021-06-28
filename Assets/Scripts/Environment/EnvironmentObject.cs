using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    public ObjectSpriteHandler spriteHandler;

    public Vector2Int coordinate;

    private void Awake()
    {
        if (!this.spriteHandler)
        {
            if (!(this.spriteHandler = this.GetComponent<ObjectSpriteHandler>()))
            {
                if (!(this.spriteHandler = this.GetComponentInChildren<ObjectSpriteHandler>()))
                {
                    Debug.Log("No se ha encontrado el sprite handler del objeto " + this.gameObject.name);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
