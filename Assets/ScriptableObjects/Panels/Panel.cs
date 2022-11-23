using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Panel", menuName = "Panel")]
public class Panel : ScriptableObject
{
    public new string name;
    public GameObject model;
    public int health;
    public SoundEffects soundList;
    public AudioClip breakEffect;
}
