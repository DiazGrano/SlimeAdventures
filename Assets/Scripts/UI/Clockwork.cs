using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clockwork : MonoBehaviour
{
    public float rotationSpeed;
    public bool gear = true;
    public float direction = 1f;
    float rotation;

    private void Start()
    {
        if (gear)
        {
            this.direction *= Mathf.Sign(Random.Range(-1f, 1f));
        }
        else
        {
            StartCoroutine(Needle());
        }
    }

    IEnumerator Needle()
    {
        while (true)
        {

            rotation += rotationSpeed * Time.deltaTime * this.direction;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation);
            yield return new WaitForFixedUpdate();

            if (Random.Range(-10f, 1f) > 0)
            {
                this.direction *= -1f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (gear)
        {
            rotation += rotationSpeed * this.direction * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }
    }
}
