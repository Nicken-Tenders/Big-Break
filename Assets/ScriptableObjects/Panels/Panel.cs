using UnityEngine;

[CreateAssetMenu(fileName = "New Panel", menuName = "Panel")]
public class Panel : ScriptableObject
{
    public new string name;
    public GameObject model;
    public GameObject particles;
    public int breakOffPieces;
    public int health;
    public float spawnDelay;
    public SoundEffects soundList;
    public AudioClip breakEffect;
}
