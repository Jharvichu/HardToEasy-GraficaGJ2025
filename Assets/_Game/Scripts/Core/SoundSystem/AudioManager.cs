using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] GameObject m_soundEmitterPrefab;

    public Sound[] m_MusicList;
	public Sound[] m_SoundList;

	private float m_generalVolumen = 1.0f;

	AudioSource m_backgroundAudioSource;
	Sound m_currentBackground;

	Dictionary<string, Sound> m_musicDict;
	Dictionary<string, Sound> m_soundDict;

	public List<EmmiterController> activeEmitters = new List<EmmiterController>();    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupClips();
			m_backgroundAudioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
		//m_backgroundAudioSource = GetComponent<AudioSource>();
	}

    private void SetupClips() {
		m_musicDict = m_MusicList.ToDictionary(s => s.name, s => s);
		m_soundDict = m_SoundList.ToDictionary(s => s.name, s => s);
	}


    #region Sound Effects
	public void Play(string name) {
		if(!m_soundDict.ContainsKey(name)){
			Debug.LogError("Sound with name " + name + " not found!");
			return;
		}
		CreateSound(name, Vector3.zero, false);
	}

	public void Play3D(string name, Vector3 position) {
		if(!m_soundDict.ContainsKey(name)){
			Debug.LogError("Sound with name " + name + " not found!");
			return;
		}
		CreateSound(name, position, true);
	}

	private void CreateSound(string name, Vector3 position, bool is3D) {
		var sound = m_soundDict[name];
		
		GameObject newSoundObj = Instantiate(m_soundEmitterPrefab, position, Quaternion.identity);
		EmmiterController emmiter = newSoundObj.GetComponent<EmmiterController>();

		if (is3D) {
			emmiter.SetupSound3D(sound, m_generalVolumen);
		} else {
			emmiter.SetupSound(sound, m_generalVolumen);
		}

		emmiter.PlaySound();
	}

	public void StopAllSounds() {
		for (int i = activeEmitters.Count - 1; i >= 0; i--) {
			if(activeEmitters[i] != null) {
				activeEmitters[i].StopSound();
			}
		}
	}
	#endregion

    #region Background Music
	public void PlayBGM(string name, float time = 0) {
		if(!m_musicDict.ContainsKey(name)){
			Debug.LogError("Music with name " + name + " not found!");
			return;
		}
		var music = m_musicDict[name];
		m_currentBackground = music;
		m_backgroundAudioSource.clip 	= music.clip;
		m_backgroundAudioSource.volume 	= music.volume * m_generalVolumen;
		m_backgroundAudioSource.pitch 	= music.pitch;
		m_backgroundAudioSource.loop 	= music.loop;
		m_backgroundAudioSource.time 	= time;
		m_backgroundAudioSource.Play();
	}

	public void UpdateBGMusic(string name, float time = 0) {
		if(m_currentBackground != null && m_currentBackground.name == name) return;
		m_backgroundAudioSource.Stop();
		PlayBGM(name, time);
	}

	public void UpdateBGMusicInTime(string name){
		if(m_currentBackground != null && m_currentBackground.name == name) return;
		float time = m_backgroundAudioSource.time;
		UpdateBGMusic(name, time);
	}

	public void StopBGM() {
		m_backgroundAudioSource.Stop();
	}
	
	public void ResumeBGM() {
		m_backgroundAudioSource.Play();
	}
	#endregion

    #region Volume
	public void UpdateGeneralVolume(float volume){
		m_generalVolumen = Mathf.Clamp01(volume);

		if(m_currentBackground != null) {
			m_backgroundAudioSource.volume = m_currentBackground.volume * m_generalVolumen;
		}

		foreach(var emmiter in activeEmitters) {
			if(emmiter != null) 
				emmiter.UpdateVolume(m_generalVolumen);
		}
	}

	public float GetGeneralVolume() { return m_generalVolumen; }
	#endregion

}
