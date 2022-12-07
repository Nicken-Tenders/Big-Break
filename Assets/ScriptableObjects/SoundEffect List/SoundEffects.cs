using UnityEngine;

[CreateAssetMenu(fileName = "New SoundEffect List", menuName = "Sound List")]
public class SoundEffects : ScriptableObject
{
    public new string name;
    public AudioClip[] soundEffects;
}
