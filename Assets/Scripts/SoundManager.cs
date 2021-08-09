using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoopingAudioSource
    {
        public AudioSource AudioSource { get; private set; }
        public float TargetVolume { get; set; }
        public float OriginalTargetVolume { get; private set; }
        public bool Stopping { get; private set; }
        public bool Persist { get; private set; }
        public int Tag { get; set; }

        private float startVolume;
        private float startMultiplier;
        private float stopMultiplier;
        private float currentMultiplier;
        private float timestamp;
        private bool paused;
        
        public LoopingAudioSource(AudioSource audioSource, float startMultiplier, float stopMultiplier, bool persist)
        {
            AudioSource = audioSource;
            if (audioSource != null)
            {
                AudioSource.loop = true;
                AudioSource.volume = 0.0f;
                AudioSource.Stop();
            }

            this.startMultiplier = currentMultiplier = startMultiplier;
            this.stopMultiplier = stopMultiplier;
            Persist = persist;
        }
        
        public void Play(bool isMusic)
        {
            Play(1.0f, isMusic);
        }

        public bool Play(float targetVolume, bool isMusic)
        {
            if (AudioSource != null)
            {
                AudioSource.volume = startVolume = (AudioSource.isPlaying ? AudioSource.volume : 0.0f);
                AudioSource.loop = true;
                currentMultiplier = startMultiplier;
                OriginalTargetVolume = targetVolume;
                TargetVolume = targetVolume;
                Stopping = false;
                timestamp = 0.0f;
                if (!AudioSource.isPlaying)
                {
                    AudioSource.Play();
                    return true;
                }
            }
            return false;
        }
        
        public void Stop()
        {
            if (AudioSource != null && AudioSource.isPlaying && !Stopping)
            {
                startVolume = AudioSource.volume;
                TargetVolume = 0.0f;
                currentMultiplier = stopMultiplier;
                Stopping = true;
                timestamp = 0.0f;
            }
        }

        public void Pause()
        {
            if (AudioSource != null && !paused && AudioSource.isPlaying)
            {
                paused = true;
                AudioSource.Pause();
            }
        }
        
        public void Resume()
        {
            if (AudioSource != null && paused)
            {
                paused = false;
                AudioSource.UnPause();
            }
        }
        
        public bool Update()
        {
            if (AudioSource != null && AudioSource.isPlaying)
            {
                if ((AudioSource.volume = Mathf.Lerp(startVolume, TargetVolume, (timestamp += Time.deltaTime) / currentMultiplier)) == 0.0f && Stopping)
                {
                    AudioSource.Stop();
                    Stopping = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return !paused;
        }
    }

    public static class SoundManagerExtensions
    {
        public static void PlayOneShotSoundManaged(this AudioSource source, AudioClip clip)
        {
            SoundManager.PlayOneShotSound(source, clip, 1.0f);
        }
        
        public static void PlayOneShotSoundManaged(this AudioSource source, AudioClip clip, float volumeScale)
        {
            SoundManager.PlayOneShotSound(source, clip, volumeScale);
        }
        
        public static void PlayOneShotMusicManaged(this AudioSource source, AudioClip clip)
        {
            SoundManager.PlayOneShotMusic(source, clip, 1.0f);
        }
        
        public static void PlayOneShotMusicManaged(this AudioSource source, AudioClip clip, float volumeScale)
        {
            SoundManager.PlayOneShotMusic(source, clip, volumeScale);
        }
        
        public static void PlayLoopingSoundManaged(this AudioSource source)
        {
            SoundManager.PlayLoopingSound(source, 1.0f, 1.0f);
        }
        
        public static void PlayLoopingSoundManaged(this AudioSource source, float volumeScale, float fadeSeconds)
        {
            SoundManager.PlayLoopingSound(source, volumeScale, fadeSeconds);
        }
        
        public static void PlayLoopingMusicManaged(this AudioSource source)
        {
            SoundManager.PlayLoopingMusic(source, 1.0f, 1.0f, false);
        }

        public static void PlayLoopingMusicManaged(this AudioSource source, float volumeScale, float fadeSeconds, bool persist)
        {
            SoundManager.PlayLoopingMusic(source, volumeScale, fadeSeconds, persist);
        }

        public static void StopLoopingSoundManaged(this AudioSource source)
        {
            SoundManager.StopLoopingSound(source);
        }
        
        public static void StopLoopingMusicManaged(this AudioSource source)
        {
            SoundManager.StopLoopingMusic(source);
        }
    }

    public class SoundManager : MonoBehaviour
    {
        private static int persistTag = 0;
        private static bool needsInitialize = true;
        private static GameObject root;
        private static SoundManager instance;
        private static readonly List<LoopingAudioSource> music = new List<LoopingAudioSource>();
        private static readonly List<AudioSource> musicOneShot = new List<AudioSource>();
        private static readonly List<LoopingAudioSource> sounds = new List<LoopingAudioSource>();
        private static readonly HashSet<LoopingAudioSource> persistedSounds = new HashSet<LoopingAudioSource>();
        private static readonly Dictionary<AudioClip, List<float>> soundsOneShot = new Dictionary<AudioClip, List<float>>();
        private static float soundVolume = 1.0f;
        private static float musicVolume = 1.0f;
        private static bool updated;
        private static bool pauseSoundsOnApplicationPause = true;
        public static int MaxDuplicateAudioClips = 4;
        public static bool StopSoundsOnLevelLoad = true;

        private static void EnsureCreated()
        {
            if (needsInitialize)
            {
                needsInitialize = false;
                root = new GameObject();
                root.name = "DigitalRubySoundManager";
                root.hideFlags = HideFlags.HideAndDontSave;
                instance = root.AddComponent<SoundManager>();
                GameObject.DontDestroyOnLoad(root);
            }
        }

        private void StopLoopingListOnLevelLoad(IList<LoopingAudioSource> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].Persist || !list[i].AudioSource.isPlaying)
                {
                    list.RemoveAt(i);
                }
            }
        }

        private void ClearPersistedSounds()
        {
            foreach (LoopingAudioSource s in persistedSounds)
            {
                if (!s.AudioSource.isPlaying)
                {
                    GameObject.Destroy(s.AudioSource.gameObject);
                }
            }
            persistedSounds.Clear();
        }

        private void SceneManagerSceneLoaded(UnityEngine.SceneManagement.Scene s, UnityEngine.SceneManagement.LoadSceneMode m)
        {
            if (updated && StopSoundsOnLevelLoad)
            {
                persistTag++;

                updated = false;

                Debug.LogWarningFormat("Reloaded level, new sound manager persist tag: {0}", persistTag);

                StopNonLoopingSounds();
                StopLoopingListOnLevelLoad(sounds);
                StopLoopingListOnLevelLoad(music);
                soundsOneShot.Clear();
                ClearPersistedSounds();
            }
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManagerSceneLoaded;
        }

        private void Update()
        {
            updated = true;

            for (int i = sounds.Count - 1; i >= 0; i--)
            {
                if (sounds[i].Update())
                {
                    sounds.RemoveAt(i);
                }
            }
            for (int i = music.Count - 1; i >= 0; i--)
            {
                bool nullMusic = (music[i] == null || music[i].AudioSource == null);
                if (nullMusic || music[i].Update())
                {
                    if (!nullMusic && music[i].Tag != persistTag)
                    {
                        Debug.LogWarning("Destroying persisted audio from previous scene: " + music[i].AudioSource.gameObject.name);
                        
                        GameObject.Destroy(music[i].AudioSource.gameObject);
                    }
                    music.RemoveAt(i);
                }
            }
            for (int i = musicOneShot.Count - 1; i >= 0; i--)
            {
                if (!musicOneShot[i].isPlaying)
                {
                    musicOneShot.RemoveAt(i);
                }
            }
        }

        private void OnApplicationFocus(bool paused)
        {
            if (SoundManager.PauseSoundsOnApplicationPause)
            {
                if (paused)
                {
                    SoundManager.ResumeAll();
                }
                else
                {
                    SoundManager.PauseAll();
                }
            }
        }

        private static void UpdateSounds()
        {
            foreach (LoopingAudioSource s in sounds)
            {
                s.TargetVolume = s.OriginalTargetVolume * soundVolume;
            }
        }

        private static void UpdateMusic()
        {
            foreach (LoopingAudioSource s in music)
            {
                if (!s.Stopping)
                {
                    s.TargetVolume = s.OriginalTargetVolume * musicVolume;
                }
            }
            foreach (AudioSource s in musicOneShot)
            {
                s.volume = musicVolume;
            }
        }

        private static IEnumerator RemoveVolumeFromClip(AudioClip clip, float volume)
        {
            yield return new WaitForSeconds(clip.length);

            List<float> volumes;
            if (soundsOneShot.TryGetValue(clip, out volumes))
            {
                volumes.Remove(volume);
            }
        }

        private static void PlayLooping(AudioSource source, List<LoopingAudioSource> sources, float volumeScale, float fadeSeconds, bool persist, bool stopAll)
        {
            EnsureCreated();

            for (int i = sources.Count - 1; i >= 0; i--)
            {
                LoopingAudioSource s = sources[i];
                if (s.AudioSource == source)
                {
                    sources.RemoveAt(i);
                }
                if (stopAll)
                {
                    s.Stop();
                }
            }
            {
                source.gameObject.SetActive(true);
                LoopingAudioSource s = new LoopingAudioSource(source, fadeSeconds, fadeSeconds, persist);
                s.Play(volumeScale, true);
                s.Tag = persistTag;
                sources.Add(s);

                if (persist)
                {
                    if (!source.gameObject.name.StartsWith("PersistedBySoundManager-"))
                    {
                        source.gameObject.name = "PersistedBySoundManager-" + source.gameObject.name + "-" + source.gameObject.GetInstanceID();
                    }
                    source.gameObject.transform.parent = null;
                    GameObject.DontDestroyOnLoad(source.gameObject);
                    persistedSounds.Add(s);
                }
            }
        }

        private static void StopLooping(AudioSource source, List<LoopingAudioSource> sources)
        {
            foreach (LoopingAudioSource s in sources)
            {
                if (s.AudioSource == source)
                {
                    s.Stop();
                    source = null;
                    break;
                }
            }
            if (source != null)
            {
                source.Stop();
            }
        }
        
        public static void PlayOneShotSound(AudioSource source, AudioClip clip)
        {
            PlayOneShotSound(source, clip, 1.0f);
        }
        
        public static void PlayOneShotSound(AudioSource source, AudioClip clip, float volumeScale)
        {
            EnsureCreated();

            List<float> volumes;
            if (!soundsOneShot.TryGetValue(clip, out volumes))
            {
                volumes = new List<float>();
                soundsOneShot[clip] = volumes;
            }
            else if (volumes.Count == MaxDuplicateAudioClips)
            {
                return;
            }

            float minVolume = float.MaxValue;
            float maxVolume = float.MinValue;
            foreach (float volume in volumes)
            {
                minVolume = Mathf.Min(minVolume, volume);
                maxVolume = Mathf.Max(maxVolume, volume);
            }

            float requestedVolume = (volumeScale * soundVolume);
            if (maxVolume > 0.5f)
            {
                requestedVolume = (minVolume + maxVolume) / (float)(volumes.Count + 2);
            }

            volumes.Add(requestedVolume);
            source.PlayOneShot(clip, requestedVolume);
            instance.StartCoroutine(RemoveVolumeFromClip(clip, requestedVolume));
        }
        
        public static void PlayLoopingSound(AudioSource source)
        {
            PlayLoopingSound(source, 1.0f, 1.0f);
        }
        
        public static void PlayLoopingSound(AudioSource source, float volumeScale, float fadeSeconds)
        {
            PlayLooping(source, sounds, volumeScale, fadeSeconds, false, false);
            UpdateSounds();
        }
        
        public static void PlayOneShotMusic(AudioSource source, AudioClip clip)
        {
            PlayOneShotMusic(source, clip, 1.0f);
        }
        
        public static void PlayOneShotMusic(AudioSource source, AudioClip clip, float volumeScale)
        {
            EnsureCreated();

            int index = musicOneShot.IndexOf(source);
            if (index >= 0)
            {
                musicOneShot.RemoveAt(index);
            }
            source.PlayOneShot(clip, volumeScale * musicVolume);
            musicOneShot.Add(source);
        }
        
        public static void PlayLoopingMusic(AudioSource source)
        {
            PlayLoopingMusic(source, 1.0f, 1.0f, false);
        }

        public static void PlayLoopingMusic(AudioSource source, float volumeScale, float fadeSeconds, bool persist)
        {
            PlayLooping(source, music, volumeScale, fadeSeconds, persist, true);
            UpdateMusic();
        }

        public static void StopLoopingSound(AudioSource source)
        {
            StopLooping(source, sounds);
        }

        public static void StopLoopingMusic(AudioSource source)
        {
            StopLooping(source, music);
        }

        public static void StopAll()
        {
            StopAllLoopingSounds();
            StopNonLoopingSounds();
        }

        public static void StopAllLoopingSounds()
        {
            foreach (LoopingAudioSource s in sounds)
            {
                s.Stop();
            }
            foreach (LoopingAudioSource s in music)
            {
                s.Stop();
            }
        }

        public static void StopNonLoopingSounds()
        {
            foreach (AudioSource s in musicOneShot)
            {
                s.Stop();
            }
        }

        public static void PauseAll()
        {
            foreach (LoopingAudioSource s in sounds)
            {
                s.Pause();
            }
            foreach (LoopingAudioSource s in music)
            {
                s.Pause();
            }
        }

        public static void ResumeAll()
        {
            foreach (LoopingAudioSource s in sounds)
            {
                s.Resume();
            }
            foreach (LoopingAudioSource s in music)
            {
                s.Resume();
            }
        }

        public static float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                if (value != musicVolume)
                {
                    musicVolume = value;
                    UpdateMusic();
                }
            }
        }

        public static float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                if (value != soundVolume)
                {
                    soundVolume = value;
                    UpdateSounds();
                }
            }
        }

        public static bool PauseSoundsOnApplicationPause
        {
            get { return pauseSoundsOnApplicationPause; }
            set { pauseSoundsOnApplicationPause = value; }
        }
    }