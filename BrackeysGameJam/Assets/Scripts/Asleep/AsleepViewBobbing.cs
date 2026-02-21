using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsleepViewBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float verticleAmplitude = 0.04f;
    public float frequency = 8.0f;

    [Header("References")]
    public Rigidbody playersAss;

    private Vector3 startPosition = Vector3.zero;
    private float timer = 0.0f;
 
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersAss.velocity.magnitude > 0.1f)
        {
            timer += Time.deltaTime * frequency;
            float bobY = Mathf.Sin(timer) * verticleAmplitude;

            transform.localPosition = startPosition + new Vector3(0.0f, bobY, 0.0f);
        }
        else
        {
            timer = 0.0f;
        }
    }
}
