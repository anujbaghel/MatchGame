using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "SoundMap", menuName = "Scriptable Objects/SoundMap")]
public class SoundMapScriptable : ScriptableObject
{
    public List<SoundPair> soundMap;
}
