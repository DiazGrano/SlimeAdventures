using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersGroup : MonoBehaviour
{
    public List<CharController> group = new List<CharController>();

    // Start is called before the first frame update
    private void Awake()
    {
        SetGroup();
    }
    public void SetGroup()
    {
        this.group.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<CharController>())
            {
                if (this.transform.GetChild(i).GetComponent<CharController>())
                {
                    this.transform.GetChild(i).GetComponent<CharController>().currentGroup = this;
                    this.group.Add(this.transform.GetChild(i).gameObject.GetComponent<CharController>());
                }
            }
        }
    }

}
