using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeStartCreepyFace : MonoBehaviour
{
    [SerializeField] private GameObject smileImage;
    [SerializeField] private float waitBeforeSmileTime = 2.0f;
    void Start()
    {
        StartCoroutine(MakeCreepySmile());
    }

    private IEnumerator MakeCreepySmile()
    {
        yield return new WaitForSeconds(waitBeforeSmileTime);
        smileImage.SetActive(true);
    }
}
