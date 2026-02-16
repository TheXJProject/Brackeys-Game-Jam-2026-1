using UnityEngine;

[System.Serializable]
public class SoundSource
{
    // Constant values:
    public const string defaultName  = "Empty";

    // Initialise In Inspector:
    [Header("---- Source Settings ----\n")]
    public AudioSource audioSource;

    // Runtime Variables:
    public string soundName = defaultName;
    public bool soundIsSelected = false;
}
