using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class CycleCredits : MonoBehaviour
{
    public float fadeInAndOutTime;
    public float fadeIntime;
    public float fadeOutTime;
    public string[] credits;
    float creditTimer = 0;
    int creditIndex = 0;
    public TextMeshProUGUI textMeshPro;
    UnityEngine.Color color;

    private void Awake()
    {
        creditTimer = 0f;
        creditIndex = credits.Length - 1;
        color = textMeshPro.color;
    }

    // Update is called once per frame
    void Update()
    {
        creditTimer += Time.deltaTime;

        if (creditTimer > fadeInAndOutTime)
        {
            creditTimer = 0f;
            color.a = 0;
            NextCredit();
        }

        if (creditTimer < fadeIntime)
        {
            color.a = creditTimer / fadeIntime;
        }
        else if (creditTimer > (fadeInAndOutTime - fadeOutTime))
        {
            color.a = (fadeInAndOutTime - creditTimer) / fadeOutTime;
        }
        else
        {
            color.a = 1;
        }
        textMeshPro.color = color;
    }

    void NextCredit()
    {
        if (++creditIndex == credits.Length)
        { 
            creditIndex = 0;
        }
        textMeshPro.text = credits[creditIndex];
    }
}
