using UnityEngine;

[System.Serializable]
public class Sound
{
    // Inspector Adjustable Values:
    [Header("---- Sound Settings ----\n")]
    public string name;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    [Range(-1f, 1f)]
    public float panning = 0f;
    public Vector2 randomPitchRange;

    // Initialise In Inspector:
    public AudioClip clip;

    [Header("(SFX Only)\n")]
    public bool looped = false;
}