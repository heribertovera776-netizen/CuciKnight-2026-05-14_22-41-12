using UnityEngine;

namespace SoulKnight.Audio
{
    /// <summary>
    /// Centralized audio manager. Handles BGM crossfade and pooled SFX playback.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip dungeonMusic;
        [SerializeField] private AudioClip bossMusic;

        [Header("SFX")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.6f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyVolumes();
        }

        private void Start()
        {
            if (Systems.GameManager.Instance != null)
                Systems.GameManager.Instance.OnGameStateChanged += HandleStateMusic;
        }

        private void HandleStateMusic(Systems.GameState state)
        {
            switch (state)
            {
                case Systems.GameState.MainMenu: PlayMusic(mainMenuMusic); break;
                case Systems.GameState.Playing:  PlayMusic(dungeonMusic);  break;
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlayBossMusic() => PlayMusic(bossMusic);

        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeScale);
        }

        public void SetMasterVolume(float v) { masterVolume = v; ApplyVolumes(); }
        public void SetMusicVolume(float v)  { musicVolume = v;  ApplyVolumes(); }
        public void SetSFXVolume(float v)    { sfxVolume = v;    ApplyVolumes(); }

        private void ApplyVolumes()
        {
            if (musicSource) musicSource.volume = musicVolume * masterVolume;
        }
    }
}
