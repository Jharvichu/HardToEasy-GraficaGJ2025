using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EmmiterController : MonoBehaviour {
    AudioSource m_audioSource;
    Sound m_sound;
    bool hasStartedPlaying = false;

    private void Awake() {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        if(AudioManager.Instance != null) {
            AudioManager.Instance.activeEmitters.Add(this);
        }
    }

    private void OnDestroy() {
        if(AudioManager.Instance != null) {
            AudioManager.Instance.activeEmitters.Remove(this);
        }
    }
    
    void Update(){
        if(m_sound == null) return;

        if (m_audioSource.isPlaying) {
            hasStartedPlaying = true;
        }

        if(hasStartedPlaying && !m_audioSource.isPlaying){
            Destroy(gameObject);
        }
    }

    public void SetupSound(Sound sound, float volume) {
        m_sound                 = sound;
        m_audioSource.clip      = m_sound.clip;
        m_audioSource.volume    = m_sound.volume * volume;
        m_audioSource.pitch     = m_sound.pitch;
        m_audioSource.loop      = m_sound.loop;
        m_audioSource.spatialBlend = 0;
    }

    public void SetupSound3D(Sound sound, float volume) {
        SetupSound(sound, volume);
        m_audioSource.spatialBlend = 1;
        m_audioSource.spatialize = true;
    }

    public void PlaySound() {
        m_audioSource.Play();
    }

    public void StopSound() {
        m_audioSource.Stop();
    }

    public void UpdateVolume(float generalVolumeCalc){
        if(m_sound != null)
            m_audioSource.volume = m_sound.volume * generalVolumeCalc;
    }
}