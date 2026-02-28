using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [SerializeField] private SoundMapScriptable soundMapScriptable;

    public void Awake(){
        if(Instance != null){
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void PlaySoundEffect(SoundEnum effectType){
        AudioClip audioClip = soundMapScriptable.soundMap.Find(x => x.soundEnum == effectType).audioClip;
        if(audioClip != null){
            sfxSource.PlayOneShot(audioClip);
        }
    }

    public void PlayBackgroundMusic(SoundEnum musicType){
        AudioClip audioClip = soundMapScriptable.soundMap.Find(x => x.soundEnum == musicType).audioClip;
        if(audioClip != null){
            musicSource.clip = audioClip;
            musicSource.Play();
            musicSource.loop = true;
        }
    }
}
