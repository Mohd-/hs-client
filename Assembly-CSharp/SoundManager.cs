using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class SoundManager : MonoBehaviour
{
	// Token: 0x06001414 RID: 5140 RVA: 0x00058A9F File Offset: 0x00056C9F
	private void Awake()
	{
		SoundManager.s_instance = this;
		this.InitializeOptions();
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x00058AAD File Offset: 0x00056CAD
	private void OnDestroy()
	{
		SoundManager.s_instance = null;
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x00058AB8 File Offset: 0x00056CB8
	private void Start()
	{
		this.UpdateAppMute();
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().AddFocusChangedListener(new ApplicationMgr.FocusChangedCallback(this.OnAppFocusChanged));
		}
		if (SceneMgr.Get() == null)
		{
			GameObject gameObject = new GameObject("SoundConfig");
			this.m_config = gameObject.AddComponent<SoundConfig>();
		}
		else
		{
			SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
	}

	// Token: 0x06001417 RID: 5143 RVA: 0x00058B34 File Offset: 0x00056D34
	private void Update()
	{
		this.m_frame = (this.m_frame + 1U & uint.MaxValue);
		this.UpdateMusicAndSources();
	}

	// Token: 0x06001418 RID: 5144 RVA: 0x00058B4C File Offset: 0x00056D4C
	public static SoundManager Get()
	{
		return SoundManager.s_instance;
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x00058B53 File Offset: 0x00056D53
	public SoundConfig GetConfig()
	{
		return this.m_config;
	}

	// Token: 0x0600141A RID: 5146 RVA: 0x00058B5B File Offset: 0x00056D5B
	public void SetConfig(SoundConfig config)
	{
		this.m_config = config;
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x00058B64 File Offset: 0x00056D64
	public bool IsInitialized()
	{
		return this.m_config != null;
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x00058B74 File Offset: 0x00056D74
	public GameObject GetPlaceholderSound()
	{
		AudioSource placeholderSource = this.GetPlaceholderSource();
		if (placeholderSource == null)
		{
			return null;
		}
		return placeholderSource.gameObject;
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x00058B9C File Offset: 0x00056D9C
	public AudioSource GetPlaceholderSource()
	{
		if (this.m_config == null)
		{
			return null;
		}
		if (ApplicationMgr.IsInternal())
		{
			return this.m_config.m_PlaceholderSound;
		}
		return null;
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x00058BD3 File Offset: 0x00056DD3
	public bool Play(AudioSource source)
	{
		return this.PlayImpl(source, null);
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x00058BE4 File Offset: 0x00056DE4
	public bool PlayOneShot(AudioSource source, AudioClip clip, float volume = 1f)
	{
		if (!this.PlayImpl(source, clip))
		{
			return false;
		}
		if (this.IsActive(source))
		{
			this.SetVolume(source, volume);
		}
		return true;
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x00058C1A File Offset: 0x00056E1A
	public bool IsPlaying(AudioSource source)
	{
		return !(source == null) && source.isPlaying;
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x00058C30 File Offset: 0x00056E30
	public bool Pause(AudioSource source)
	{
		if (source == null)
		{
			return false;
		}
		if (this.IsPaused(source))
		{
			return false;
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
		if (sourceExtension == null)
		{
			return false;
		}
		sourceExtension.m_paused = true;
		this.UpdateSource(source, sourceExtension);
		source.Pause();
		return true;
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x00058C84 File Offset: 0x00056E84
	public bool IsPaused(AudioSource source)
	{
		if (source == null)
		{
			return false;
		}
		SoundManager.SourceExtension extension = this.GetExtension(source);
		return extension != null && extension.m_paused;
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x00058CB8 File Offset: 0x00056EB8
	public bool Stop(AudioSource source)
	{
		if (source == null)
		{
			return false;
		}
		if (!this.IsActive(source))
		{
			return false;
		}
		source.Stop();
		this.FinishSource(source);
		return true;
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x00058CEF File Offset: 0x00056EEF
	public void Destroy(AudioSource source)
	{
		if (source == null)
		{
			return;
		}
		this.FinishSource(source);
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x00058D08 File Offset: 0x00056F08
	public bool IsActive(AudioSource source)
	{
		return !(source == null) && (this.IsPlaying(source) || this.IsPaused(source));
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x00058D40 File Offset: 0x00056F40
	public float GetVolume(AudioSource source)
	{
		if (source == null)
		{
			return 1f;
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
		if (sourceExtension == null)
		{
			return 1f;
		}
		return sourceExtension.m_codeVolume;
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x00058D7C File Offset: 0x00056F7C
	public void SetVolume(AudioSource source, float volume)
	{
		if (source == null)
		{
			return;
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
		if (sourceExtension == null)
		{
			return;
		}
		sourceExtension.m_codeVolume = volume;
		this.UpdateVolume(source, sourceExtension);
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x00058DB8 File Offset: 0x00056FB8
	public float GetPitch(AudioSource source)
	{
		if (source == null)
		{
			return 1f;
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
		if (sourceExtension == null)
		{
			return 1f;
		}
		return sourceExtension.m_codePitch;
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x00058DF4 File Offset: 0x00056FF4
	public void SetPitch(AudioSource source, float pitch)
	{
		if (source == null)
		{
			return;
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
		if (sourceExtension == null)
		{
			return;
		}
		sourceExtension.m_codePitch = pitch;
		this.UpdatePitch(source, sourceExtension);
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x00058E30 File Offset: 0x00057030
	public SoundCategory GetCategory(AudioSource source)
	{
		if (source == null)
		{
			return SoundCategory.NONE;
		}
		SoundDef defFromSource = this.GetDefFromSource(source);
		return defFromSource.m_Category;
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x00058E5C File Offset: 0x0005705C
	public void SetCategory(AudioSource source, SoundCategory cat)
	{
		if (source == null)
		{
			return;
		}
		SoundDef soundDef = source.GetComponent<SoundDef>();
		if (soundDef != null)
		{
			if (soundDef.m_Category == cat)
			{
				return;
			}
		}
		else
		{
			soundDef = source.gameObject.AddComponent<SoundDef>();
		}
		soundDef.m_Category = cat;
		this.UpdateSource(source);
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x00058EB5 File Offset: 0x000570B5
	public bool Is3d(AudioSource source)
	{
		return !(source == null) && source.spatialBlend >= 1f;
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x00058ED8 File Offset: 0x000570D8
	public void Set3d(AudioSource source, bool enable)
	{
		if (source == null)
		{
			return;
		}
		source.spatialBlend = ((!enable) ? 0f : 1f);
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x00058F0D File Offset: 0x0005710D
	public AudioSource GetCurrentMusicTrack()
	{
		return this.m_currentMusicTrack;
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x00058F15 File Offset: 0x00057115
	public AudioSource GetCurrentAmbienceTrack()
	{
		return this.m_currentAmbienceTrack;
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x00058F1D File Offset: 0x0005711D
	public bool Load(string soundName)
	{
		return AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnLoadSoundLoaded), null, true, null);
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x00058F39 File Offset: 0x00057139
	public void LoadAndPlay(string soundName)
	{
		this.LoadAndPlay(soundName, null, 1f, null, null);
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x00058F4A File Offset: 0x0005714A
	public void LoadAndPlay(string soundName, float volume)
	{
		this.LoadAndPlay(soundName, null, volume, null, null);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x00058F57 File Offset: 0x00057157
	public void LoadAndPlay(string soundName, GameObject parent)
	{
		this.LoadAndPlay(soundName, parent, 1f, null, null);
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x00058F68 File Offset: 0x00057168
	public void LoadAndPlay(string soundName, GameObject parent, float volume)
	{
		this.LoadAndPlay(soundName, parent, volume, null, null);
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x00058F75 File Offset: 0x00057175
	public void LoadAndPlay(string soundName, GameObject parent, float volume, SoundManager.LoadedCallback callback)
	{
		this.LoadAndPlay(soundName, parent, volume, callback, null);
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x00058F84 File Offset: 0x00057184
	public void LoadAndPlay(string soundName, GameObject parent, float volume, SoundManager.LoadedCallback callback, object userData)
	{
		SoundManager.SoundLoadContext soundLoadContext = new SoundManager.SoundLoadContext();
		soundLoadContext.Init(parent, volume, callback, userData);
		AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnLoadAndPlaySoundLoaded), soundLoadContext, true, this.GetPlaceholderSound());
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x00058FC3 File Offset: 0x000571C3
	public void LoadAndPlayTemplate(AudioSource template, AudioClip clip)
	{
		this.LoadAndPlayTemplate(template, clip, 1f, null, null);
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x00058FD4 File Offset: 0x000571D4
	public void LoadAndPlayTemplate(AudioSource template, AudioClip clip, float volume)
	{
		this.LoadAndPlayTemplate(template, clip, volume, null, null);
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x00058FE1 File Offset: 0x000571E1
	public void LoadAndPlayTemplate(AudioSource template, AudioClip clip, float volume, SoundManager.LoadedCallback callback)
	{
		this.LoadAndPlayTemplate(template, clip, volume, callback, null);
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x00058FF0 File Offset: 0x000571F0
	public void LoadAndPlayTemplate(AudioSource template, AudioClip clip, float volume, SoundManager.LoadedCallback callback, object userData)
	{
		if (template == null)
		{
			Error.AddDevFatal("SoundManager.LoadAndPlayTemplate() - template is null", new object[0]);
			return;
		}
		if (clip == null)
		{
			Error.AddDevFatal("SoundManager.LoadAndPlayTemplate() - Attempted to play template {0} with a null clip. Top-level parent is {1}.", new object[]
			{
				template,
				SceneUtils.FindTopParent(template)
			});
			return;
		}
		SoundManager.SoundLoadContext soundLoadContext = new SoundManager.SoundLoadContext();
		soundLoadContext.m_template = template;
		soundLoadContext.Init(template.gameObject, volume, callback, userData);
		AssetLoader.Get().LoadSound(clip.name, new AssetLoader.GameObjectCallback(this.OnLoadAndPlaySoundLoaded), soundLoadContext, true, this.GetPlaceholderSound());
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x00059087 File Offset: 0x00057287
	public void PlayPreloaded(AudioSource source)
	{
		this.PlayPreloaded(source, null);
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x00059091 File Offset: 0x00057291
	public void PlayPreloaded(AudioSource source, float volume)
	{
		this.PlayPreloaded(source, null, volume);
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0005909C File Offset: 0x0005729C
	public void PlayPreloaded(AudioSource source, GameObject parentObject)
	{
		this.PlayPreloaded(source, parentObject, 1f);
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x000590AC File Offset: 0x000572AC
	public void PlayPreloaded(AudioSource source, GameObject parentObject, float volume)
	{
		if (source == null)
		{
			Debug.LogError("Preloaded audio source is null! Cannot play!");
			return;
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
		sourceExtension.m_codeVolume = volume;
		this.InitSourceTransform(source, parentObject);
		this.m_generatedSources.Add(source);
		this.Play(source);
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x00059100 File Offset: 0x00057300
	public AudioSource PlayClip(SoundPlayClipArgs args)
	{
		if (args == null || args.m_clip == null)
		{
			return this.PlayImpl(null, null);
		}
		AudioSource audioSource = this.GenerateAudioSource(args.m_templateSource, args.m_clip);
		audioSource.clip = args.m_clip;
		float? volume = args.m_volume;
		if (volume != null)
		{
			audioSource.volume = args.m_volume.Value;
		}
		float? pitch = args.m_pitch;
		if (pitch != null)
		{
			audioSource.pitch = args.m_pitch.Value;
		}
		float? spatialBlend = args.m_spatialBlend;
		if (spatialBlend != null)
		{
			audioSource.spatialBlend = args.m_spatialBlend.Value;
		}
		SoundCategory? category = args.m_category;
		if (category != null)
		{
			SoundDef component = audioSource.GetComponent<SoundDef>();
			component.m_Category = args.m_category.Value;
		}
		this.InitSourceTransform(audioSource, args.m_parentObject);
		if (this.Play(audioSource))
		{
			return audioSource;
		}
		this.FinishGeneratedSource(audioSource);
		return null;
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x00059208 File Offset: 0x00057408
	private void OnLoadSoundLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SoundManager.OnLoadSoundLoaded() - ERROR \"{0}\" failed to load", name));
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		if (component == null)
		{
			Object.DestroyImmediate(go);
			Debug.LogError(string.Format("SoundManager.OnLoadSoundLoaded() - ERROR \"{0}\" has no AudioSource", name));
			return;
		}
		this.RegisterSourceBundle(name, component);
		component.volume = 0f;
		component.Play();
		component.Stop();
		this.UnregisterSourceBundle(name, component);
		Object.DestroyImmediate(component.gameObject);
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x00059290 File Offset: 0x00057490
	private void OnLoadAndPlaySoundLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SoundManager.OnLoadAndPlaySoundLoaded() - ERROR \"{0}\" failed to load", name));
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		if (component == null)
		{
			Object.DestroyImmediate(go);
			Debug.LogError(string.Format("SoundManager.OnLoadAndPlaySoundLoaded() - ERROR \"{0}\" has no AudioSource", name));
			return;
		}
		SoundManager.SoundLoadContext soundLoadContext = (SoundManager.SoundLoadContext)callbackData;
		if (soundLoadContext.m_sceneMode != SceneMgr.Mode.FATAL_ERROR && SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
		{
			Object.DestroyImmediate(go);
			return;
		}
		this.RegisterSourceBundle(name, component);
		if (soundLoadContext.m_haveCallback && !GeneralUtils.IsCallbackValid(soundLoadContext.m_callback))
		{
			Object.DestroyImmediate(go);
			this.UnregisterSourceBundle(name, component);
			return;
		}
		this.m_generatedSources.Add(component);
		if (soundLoadContext.m_template != null)
		{
			SoundUtils.CopyAudioSource(soundLoadContext.m_template, component);
		}
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(component, false, null);
		sourceExtension.m_codeVolume = soundLoadContext.m_volume;
		this.InitSourceTransform(component, soundLoadContext.m_parent);
		this.Play(component);
		if (soundLoadContext.m_callback != null)
		{
			soundLoadContext.m_callback(component, soundLoadContext.m_userData);
		}
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x000593B4 File Offset: 0x000575B4
	public void AddMusicTracks(List<MusicTrack> tracks)
	{
		this.AddTracks(tracks, this.m_musicTracks);
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x000593C3 File Offset: 0x000575C3
	public void AddAmbienceTracks(List<MusicTrack> tracks)
	{
		this.AddTracks(tracks, this.m_ambienceTracks);
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x000593D2 File Offset: 0x000575D2
	public List<MusicTrack> GetCurrentMusicTracks()
	{
		return this.m_musicTracks;
	}

	// Token: 0x06001445 RID: 5189 RVA: 0x000593DA File Offset: 0x000575DA
	public List<MusicTrack> GetCurrentAmbienceTracks()
	{
		return this.m_ambienceTracks;
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x000593E4 File Offset: 0x000575E4
	public void StopCurrentMusicTrack()
	{
		if (this.m_currentMusicTrack != null)
		{
			this.FadeTrackOut(this.m_currentMusicTrack);
			this.ChangeCurrentMusicTrack(null);
		}
	}

	// Token: 0x06001447 RID: 5191 RVA: 0x00059418 File Offset: 0x00057618
	public void StopCurrentAmbienceTrack()
	{
		if (this.m_currentAmbienceTrack != null)
		{
			this.FadeTrackOut(this.m_currentAmbienceTrack);
			this.ChangeCurrentAmbienceTrack(null);
		}
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0005944C File Offset: 0x0005764C
	public void NukeMusicAndAmbiencePlaylists()
	{
		this.m_musicTracks.Clear();
		this.m_ambienceTracks.Clear();
		this.m_musicTrackIndex = 0;
		this.m_ambienceTrackIndex = 0;
	}

	// Token: 0x06001449 RID: 5193 RVA: 0x0005947D File Offset: 0x0005767D
	public void NukePlaylistsAndStopPlayingCurrentTracks()
	{
		this.NukeMusicAndAmbiencePlaylists();
		this.StopCurrentMusicTrack();
		this.StopCurrentAmbienceTrack();
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x00059491 File Offset: 0x00057691
	public void NukeMusicAndStopPlayingCurrentTrack()
	{
		this.m_musicTracks.Clear();
		this.m_musicTrackIndex = 0;
		this.StopCurrentMusicTrack();
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x000594AB File Offset: 0x000576AB
	public void NukeAmbienceAndStopPlayingCurrentTrack()
	{
		this.m_ambienceTracks.Clear();
		this.m_ambienceTrackIndex = 0;
		this.StopCurrentAmbienceTrack();
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x000594C8 File Offset: 0x000576C8
	public void ImmediatelyKillMusicAndAmbience()
	{
		this.NukeMusicAndAmbiencePlaylists();
		AudioSource[] array = this.m_fadingTracks.ToArray();
		foreach (AudioSource source in array)
		{
			this.FinishSource(source);
		}
		if (this.m_currentMusicTrack != null)
		{
			this.FinishSource(this.m_currentMusicTrack);
			this.ChangeCurrentMusicTrack(null);
		}
		if (this.m_currentAmbienceTrack != null)
		{
			this.FinishSource(this.m_currentAmbienceTrack);
			this.ChangeCurrentAmbienceTrack(null);
		}
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x00059550 File Offset: 0x00057750
	private void AddTracks(List<MusicTrack> sourceTracks, List<MusicTrack> destTracks)
	{
		foreach (MusicTrack musicTrack in sourceTracks)
		{
			destTracks.Add(musicTrack);
		}
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x000595A8 File Offset: 0x000577A8
	private void OnMusicLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SoundManager.OnMusicLoaded() - ERROR \"{0}\" failed to load", name));
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		if (component == null)
		{
			Debug.LogError(string.Format("SoundManager.OnMusicLoaded() - ERROR \"{0}\" has no AudioSource", name));
			return;
		}
		this.RegisterSourceBundle(name, component);
		MusicTrack musicTrack = (MusicTrack)callbackData;
		if (!this.m_musicTracks.Contains(musicTrack))
		{
			this.UnregisterSourceBundle(name, component);
			Object.DestroyImmediate(go);
		}
		else
		{
			this.m_generatedSources.Add(component);
			component.transform.parent = base.transform;
			component.volume *= musicTrack.m_volume;
			this.ChangeCurrentMusicTrack(component);
			this.Play(component);
		}
		this.m_musicIsAboutToPlay = false;
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x00059670 File Offset: 0x00057870
	private void OnAmbienceLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("SoundManager.OnAmbienceLoaded() - ERROR \"{0}\" failed to load", name));
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		if (component == null)
		{
			Debug.LogError(string.Format("SoundManager.OnAmbienceLoaded() - ERROR \"{0}\" has no AudioSource", name));
			return;
		}
		this.RegisterSourceBundle(name, component);
		MusicTrack musicTrack = (MusicTrack)callbackData;
		if (!this.m_ambienceTracks.Contains(musicTrack))
		{
			this.UnregisterSourceBundle(name, component);
			Object.DestroyImmediate(go);
		}
		else
		{
			this.m_generatedSources.Add(component);
			component.transform.parent = base.transform;
			component.volume *= musicTrack.m_volume;
			this.ChangeCurrentAmbienceTrack(component);
			this.m_fadingTracksIn.Add(base.StartCoroutine(this.FadeTrackIn(component)));
			this.Play(component);
		}
		this.m_ambienceIsAboutToPlay = false;
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x00059750 File Offset: 0x00057950
	private void ChangeCurrentMusicTrack(AudioSource source)
	{
		this.m_currentMusicTrack = source;
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x00059759 File Offset: 0x00057959
	private void ChangeCurrentAmbienceTrack(AudioSource source)
	{
		this.m_currentAmbienceTrack = source;
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x00059764 File Offset: 0x00057964
	private void UpdateMusicAndAmbience()
	{
		if (!SoundUtils.IsMusicEnabled())
		{
			return;
		}
		if (!this.m_musicIsAboutToPlay)
		{
			if (this.m_currentMusicTrack != null)
			{
				if (!this.IsPlaying(this.m_currentMusicTrack))
				{
					base.StartCoroutine(this.PlayMusicInSeconds(this.m_config.m_SecondsBetweenMusicTracks));
				}
			}
			else
			{
				this.m_musicIsAboutToPlay = this.PlayNextMusic();
			}
		}
		if (!this.m_ambienceIsAboutToPlay)
		{
			if (this.m_currentAmbienceTrack != null)
			{
				if (!this.IsPlaying(this.m_currentAmbienceTrack))
				{
					base.StartCoroutine(this.PlayAmbienceInSeconds(0f));
				}
			}
			else
			{
				this.m_ambienceIsAboutToPlay = this.PlayNextAmbience();
			}
		}
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x00059824 File Offset: 0x00057A24
	private IEnumerator PlayMusicInSeconds(float seconds)
	{
		this.m_musicIsAboutToPlay = true;
		yield return new WaitForSeconds(seconds);
		this.m_musicIsAboutToPlay = this.PlayNextMusic();
		yield break;
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x00059850 File Offset: 0x00057A50
	private bool PlayNextMusic()
	{
		if (!SoundUtils.IsMusicEnabled())
		{
			return false;
		}
		if (this.m_musicTracks.Count <= 0)
		{
			return false;
		}
		MusicTrack musicTrack = this.m_musicTracks[this.m_musicTrackIndex];
		this.m_musicTrackIndex = (this.m_musicTrackIndex + 1) % this.m_musicTracks.Count;
		if (musicTrack == null)
		{
			return false;
		}
		if (this.m_currentMusicTrack != null)
		{
			this.FadeTrackOut(this.m_currentMusicTrack);
			this.ChangeCurrentMusicTrack(null);
		}
		string soundName = FileUtils.GameAssetPathToName(musicTrack.m_name);
		return AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnMusicLoaded), musicTrack, true, this.GetPlaceholderSound());
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x00059900 File Offset: 0x00057B00
	private IEnumerator PlayAmbienceInSeconds(float seconds)
	{
		this.m_ambienceIsAboutToPlay = true;
		yield return new WaitForSeconds(seconds);
		this.m_ambienceIsAboutToPlay = this.PlayNextAmbience();
		yield break;
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x0005992C File Offset: 0x00057B2C
	private bool PlayNextAmbience()
	{
		if (!SoundUtils.IsMusicEnabled())
		{
			return false;
		}
		if (this.m_ambienceTracks.Count <= 0)
		{
			return false;
		}
		MusicTrack musicTrack = this.m_ambienceTracks[this.m_ambienceTrackIndex];
		this.m_ambienceTrackIndex = (this.m_ambienceTrackIndex + 1) % this.m_ambienceTracks.Count;
		if (musicTrack == null)
		{
			return false;
		}
		string soundName = FileUtils.GameAssetPathToName(musicTrack.m_name);
		foreach (Coroutine coroutine in this.m_fadingTracksIn)
		{
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
			}
		}
		this.m_fadingTracksIn.Clear();
		return AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnAmbienceLoaded), musicTrack, true, this.GetPlaceholderSound());
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x00059A18 File Offset: 0x00057C18
	private void FadeTrackOut(AudioSource source)
	{
		if (!this.IsActive(source))
		{
			this.FinishSource(source);
			return;
		}
		base.StartCoroutine(this.FadeTrack(source, 0f));
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x00059A4C File Offset: 0x00057C4C
	private IEnumerator FadeTrackIn(AudioSource source)
	{
		SoundManager.SourceExtension ext = this.GetExtension(source);
		float targetVolume = this.GetVolume(source);
		float currTime = 0f;
		float targetVolumeTime = 1f;
		ext.m_codeVolume = 0f;
		this.UpdateVolume(source, ext);
		while (ext.m_codeVolume < targetVolume)
		{
			currTime += Time.deltaTime;
			ext.m_codeVolume = Mathf.Lerp(0f, targetVolume, Mathf.Clamp01(currTime / targetVolumeTime));
			this.UpdateVolume(source, ext);
			yield return null;
			if (source == null)
			{
				yield break;
			}
			if (!this.IsActive(source))
			{
				yield break;
			}
		}
		yield break;
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x00059A78 File Offset: 0x00057C78
	private IEnumerator FadeTrack(AudioSource source, float targetVolume)
	{
		this.m_fadingTracks.Add(source);
		SoundManager.SourceExtension ext = this.GetExtension(source);
		while (ext.m_codeVolume > 0.0001f)
		{
			ext.m_codeVolume = Mathf.Lerp(ext.m_codeVolume, targetVolume, Time.deltaTime);
			this.UpdateVolume(source, ext);
			yield return null;
			if (source == null)
			{
				yield break;
			}
			if (!this.IsActive(source))
			{
				yield break;
			}
		}
		this.FinishSource(source);
		yield break;
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x00059AB0 File Offset: 0x00057CB0
	private SoundManager.SourceExtension RegisterExtension(AudioSource source, bool aboutToPlay = false, AudioClip oneShotClip = null)
	{
		SoundDef defFromSource = this.GetDefFromSource(source);
		SoundManager.SourceExtension sourceExtension = this.GetExtension(source);
		if (sourceExtension == null)
		{
			AudioClip audioClip = this.DetermineClipForPlayback(source, defFromSource, oneShotClip);
			if (audioClip == null)
			{
				return null;
			}
			if (aboutToPlay && this.ProcessClipLimits(audioClip))
			{
				return null;
			}
			sourceExtension = new SoundManager.SourceExtension();
			sourceExtension.m_sourceVolume = source.volume;
			sourceExtension.m_sourcePitch = source.pitch;
			sourceExtension.m_sourceClip = source.clip;
			sourceExtension.m_id = this.GetNextSourceId();
			this.AddExtensionMapping(source, sourceExtension);
			this.RegisterSourceByCategory(source, defFromSource.m_Category);
			this.InitNewClipOnSource(source, defFromSource, sourceExtension, audioClip);
		}
		else if (aboutToPlay)
		{
			AudioClip audioClip2 = this.DetermineClipForPlayback(source, defFromSource, oneShotClip);
			if (!this.CanPlayClipOnExistingSource(source, audioClip2))
			{
				if (this.IsActive(source))
				{
					this.Stop(source);
				}
				else
				{
					this.FinishSource(source);
				}
				return null;
			}
			if (source.clip != audioClip2)
			{
				if (source.clip != null)
				{
					this.UnregisterSourceByClip(source);
				}
				this.InitNewClipOnSource(source, defFromSource, sourceExtension, audioClip2);
			}
		}
		return sourceExtension;
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x00059BCC File Offset: 0x00057DCC
	private AudioClip DetermineClipForPlayback(AudioSource source, SoundDef def, AudioClip oneShotClip)
	{
		AudioClip audioClip = oneShotClip;
		if (audioClip == null)
		{
			audioClip = SoundUtils.GetRandomClipFromDef(def);
			if (audioClip == null)
			{
				audioClip = source.clip;
				if (audioClip == null)
				{
					string text = string.Empty;
					if (ApplicationMgr.IsInternal())
					{
						text = " " + DebugUtils.GetHierarchyPathAndType(source, '.');
					}
					Error.AddDevFatal("{0} has no AudioClip. Top-level parent is {1}{2}.", new object[]
					{
						source,
						SceneUtils.FindTopParent(source),
						text
					});
					return null;
				}
			}
		}
		AudioClip audioClip2 = AssetLoader.Get().LoadAudioClip(audioClip.name, false) as AudioClip;
		if (audioClip2 != null)
		{
			audioClip = audioClip2;
		}
		else
		{
			Debug.LogError("DetermineClipForPlayback: failed to load localized audio clip for " + base.name);
		}
		return audioClip;
	}

	// Token: 0x0600145C RID: 5212 RVA: 0x00059C94 File Offset: 0x00057E94
	private bool CanPlayClipOnExistingSource(AudioSource source, AudioClip clip)
	{
		return !(clip == null) && ((this.IsActive(source) && !(source.clip != clip)) || !this.ProcessClipLimits(clip));
	}

	// Token: 0x0600145D RID: 5213 RVA: 0x00059CDB File Offset: 0x00057EDB
	private void InitNewClipOnSource(AudioSource source, SoundDef def, SoundManager.SourceExtension ext, AudioClip clip)
	{
		ext.m_defVolume = SoundUtils.GetRandomVolumeFromDef(def);
		ext.m_defPitch = SoundUtils.GetRandomPitchFromDef(def);
		source.clip = clip;
		this.RegisterSourceByClip(source, clip);
	}

	// Token: 0x0600145E RID: 5214 RVA: 0x00059D08 File Offset: 0x00057F08
	private void UnregisterExtension(AudioSource source, SoundManager.SourceExtension ext)
	{
		source.volume = ext.m_sourceVolume;
		source.pitch = ext.m_sourcePitch;
		source.clip = ext.m_sourceClip;
		this.RemoveExtensionMapping(source);
	}

	// Token: 0x0600145F RID: 5215 RVA: 0x00059D40 File Offset: 0x00057F40
	private void UpdateSource(AudioSource source)
	{
		SoundManager.SourceExtension extension = this.GetExtension(source);
		this.UpdateSource(source, extension);
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x00059D5D File Offset: 0x00057F5D
	private void UpdateSource(AudioSource source, SoundManager.SourceExtension ext)
	{
		this.UpdateMute(source);
		this.UpdateVolume(source, ext);
		this.UpdatePitch(source, ext);
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x00059D78 File Offset: 0x00057F78
	private void UpdateMute(AudioSource source)
	{
		bool categoryEnabled = this.IsCategoryEnabled(source);
		this.UpdateMute(source, categoryEnabled);
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x00059D95 File Offset: 0x00057F95
	private void UpdateMute(AudioSource source, bool categoryEnabled)
	{
		source.mute = (this.m_mute || !categoryEnabled);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x00059DB0 File Offset: 0x00057FB0
	private void UpdateCategoryMute(SoundCategory cat)
	{
		List<AudioSource> list;
		if (!this.m_sourcesByCategory.TryGetValue(cat, out list))
		{
			return;
		}
		bool categoryEnabled = SoundUtils.IsCategoryEnabled(cat);
		for (int i = 0; i < list.Count; i++)
		{
			AudioSource source = list[i];
			this.UpdateMute(source, categoryEnabled);
		}
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x00059E00 File Offset: 0x00058000
	private void UpdateAllMutes()
	{
		foreach (SoundManager.ExtensionMapping extensionMapping in this.m_extensionMappings)
		{
			this.UpdateMute(extensionMapping.Source);
		}
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x00059E60 File Offset: 0x00058060
	private void UpdateVolume(AudioSource source, SoundManager.SourceExtension ext)
	{
		float categoryVolume = this.GetCategoryVolume(source);
		float duckingVolume = this.GetDuckingVolume(source);
		this.UpdateVolume(source, ext, categoryVolume, duckingVolume);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x00059E87 File Offset: 0x00058087
	private void UpdateVolume(AudioSource source, SoundManager.SourceExtension ext, float categoryVolume, float duckingVolume)
	{
		source.volume = ext.m_codeVolume * ext.m_sourceVolume * ext.m_defVolume * categoryVolume * duckingVolume;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x00059EA8 File Offset: 0x000580A8
	private void UpdateCategoryVolume(SoundCategory cat)
	{
		List<AudioSource> list;
		if (!this.m_sourcesByCategory.TryGetValue(cat, out list))
		{
			return;
		}
		float categoryVolume = SoundUtils.GetCategoryVolume(cat);
		for (int i = 0; i < list.Count; i++)
		{
			AudioSource audioSource = list[i];
			if (!(audioSource == null))
			{
				SoundManager.SourceExtension extension = this.GetExtension(audioSource);
				float duckingVolume = this.GetDuckingVolume(audioSource);
				this.UpdateVolume(audioSource, extension, categoryVolume, duckingVolume);
			}
		}
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x00059F20 File Offset: 0x00058120
	private void UpdateAllCategoryVolumes()
	{
		foreach (SoundCategory cat in this.m_sourcesByCategory.Keys)
		{
			this.UpdateCategoryVolume(cat);
		}
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x00059F80 File Offset: 0x00058180
	private void UpdatePitch(AudioSource source, SoundManager.SourceExtension ext)
	{
		source.pitch = ext.m_codePitch * ext.m_sourcePitch * ext.m_defPitch;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x00059F9C File Offset: 0x0005819C
	private void InitializeOptions()
	{
		Options.Get().RegisterChangedListener(Option.SOUND, new Options.ChangedCallback(this.OnMasterEnabledOptionChanged));
		Options.Get().RegisterChangedListener(Option.SOUND_VOLUME, new Options.ChangedCallback(this.OnMasterVolumeOptionChanged));
		Options.Get().RegisterChangedListener(Option.MUSIC, new Options.ChangedCallback(this.OnEnabledOptionChanged));
		Options.Get().RegisterChangedListener(Option.MUSIC_VOLUME, new Options.ChangedCallback(this.OnVolumeOptionChanged));
		Options.Get().RegisterChangedListener(Option.BACKGROUND_SOUND, new Options.ChangedCallback(this.OnBackgroundSoundOptionChanged));
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0005A022 File Offset: 0x00058222
	private void OnMasterEnabledOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		this.UpdateAllMutes();
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0005A02A File Offset: 0x0005822A
	private void OnMasterVolumeOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		this.UpdateAllCategoryVolumes();
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x0005A034 File Offset: 0x00058234
	private void OnEnabledOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		foreach (KeyValuePair<SoundCategory, Option> keyValuePair in SoundDataTables.s_categoryEnabledOptionMap)
		{
			SoundCategory key = keyValuePair.Key;
			Option value = keyValuePair.Value;
			if (value == option)
			{
				this.UpdateCategoryMute(key);
			}
		}
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0005A0A4 File Offset: 0x000582A4
	private void OnVolumeOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		foreach (KeyValuePair<SoundCategory, Option> keyValuePair in SoundDataTables.s_categoryVolumeOptionMap)
		{
			SoundCategory key = keyValuePair.Key;
			Option value = keyValuePair.Value;
			if (value == option)
			{
				this.UpdateCategoryVolume(key);
			}
		}
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0005A114 File Offset: 0x00058314
	private void OnBackgroundSoundOptionChanged(Option option, object prevValue, bool existed, object userData)
	{
		this.UpdateAppMute();
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x0005A11C File Offset: 0x0005831C
	private void RegisterSourceByCategory(AudioSource source, SoundCategory cat)
	{
		List<AudioSource> list;
		if (!this.m_sourcesByCategory.TryGetValue(cat, out list))
		{
			list = new List<AudioSource>();
			this.m_sourcesByCategory.Add(cat, list);
			list.Add(source);
		}
		else if (!list.Contains(source))
		{
			list.Add(source);
		}
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x0005A170 File Offset: 0x00058370
	private void UnregisterSourceByCategory(AudioSource source)
	{
		SoundCategory category = this.GetCategory(source);
		List<AudioSource> list;
		if (!this.m_sourcesByCategory.TryGetValue(category, out list))
		{
			Debug.LogError(string.Format("SoundManager.UnregisterSourceByCategory() - {0} is untracked. category={1}", this.GetSourceId(source), category));
			return;
		}
		if (list.Remove(source))
		{
		}
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x0005A1CC File Offset: 0x000583CC
	private bool IsCategoryEnabled(AudioSource source)
	{
		SoundDef component = source.GetComponent<SoundDef>();
		return SoundUtils.IsCategoryEnabled(component.m_Category);
	}

	// Token: 0x06001473 RID: 5235 RVA: 0x0005A1EC File Offset: 0x000583EC
	private float GetCategoryVolume(AudioSource source)
	{
		SoundDef component = source.GetComponent<SoundDef>();
		return SoundUtils.GetCategoryVolume(component.m_Category);
	}

	// Token: 0x06001474 RID: 5236 RVA: 0x0005A20C File Offset: 0x0005840C
	private void RegisterSourceByClip(AudioSource source, AudioClip clip)
	{
		List<AudioSource> list;
		if (!this.m_sourcesByClipName.TryGetValue(clip.name, out list))
		{
			list = new List<AudioSource>();
			this.m_sourcesByClipName.Add(clip.name, list);
			list.Add(source);
		}
		else if (!list.Contains(source))
		{
			list.Add(source);
		}
	}

	// Token: 0x06001475 RID: 5237 RVA: 0x0005A268 File Offset: 0x00058468
	private void UnregisterSourceByClip(AudioSource source)
	{
		AudioClip clip = source.clip;
		if (clip == null)
		{
			Debug.LogError(string.Format("SoundManager.UnregisterSourceByClip() - id {0} (source {1}) is untracked", this.GetSourceId(source), source));
			return;
		}
		List<AudioSource> list;
		if (!this.m_sourcesByClipName.TryGetValue(clip.name, out list))
		{
			Debug.LogError(string.Format("SoundManager.UnregisterSourceByClip() - id {0} (source {1}) is untracked. clip={2}", this.GetSourceId(source), source, clip));
			return;
		}
		list.Remove(source);
		if (list.Count == 0)
		{
			this.m_sourcesByClipName.Remove(clip.name);
		}
	}

	// Token: 0x06001476 RID: 5238 RVA: 0x0005A300 File Offset: 0x00058500
	private bool ProcessClipLimits(AudioClip clip)
	{
		if (this.m_config == null || this.m_config.m_PlaybackLimitDefs == null)
		{
			return false;
		}
		string name = clip.name;
		bool flag = false;
		AudioSource audioSource = null;
		foreach (SoundPlaybackLimitDef soundPlaybackLimitDef in this.m_config.m_PlaybackLimitDefs)
		{
			SoundPlaybackLimitClipDef soundPlaybackLimitClipDef = this.FindClipDefInPlaybackDef(name, soundPlaybackLimitDef);
			if (soundPlaybackLimitClipDef != null)
			{
				int num = soundPlaybackLimitClipDef.m_Priority;
				float num2 = 2f;
				int num3 = 0;
				foreach (SoundPlaybackLimitClipDef soundPlaybackLimitClipDef2 in soundPlaybackLimitDef.m_ClipDefs)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(soundPlaybackLimitClipDef2.m_Path);
					List<AudioSource> list;
					if (this.m_sourcesByClipName.TryGetValue(fileNameWithoutExtension, out list))
					{
						int priority = soundPlaybackLimitClipDef2.m_Priority;
						foreach (AudioSource audioSource2 in list)
						{
							if (this.IsPlaying(audioSource2))
							{
								float num4 = audioSource2.time / audioSource2.clip.length;
								if (num4 <= soundPlaybackLimitClipDef2.m_ExclusivePlaybackThreshold)
								{
									num3++;
									if (priority < num && num4 < num2)
									{
										audioSource = audioSource2;
										num = priority;
										num2 = num4;
									}
								}
							}
						}
					}
				}
				if (num3 >= soundPlaybackLimitDef.m_Limit)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		if (audioSource == null)
		{
			return true;
		}
		this.Stop(audioSource);
		return false;
	}

	// Token: 0x06001477 RID: 5239 RVA: 0x0005A518 File Offset: 0x00058718
	private SoundPlaybackLimitClipDef FindClipDefInPlaybackDef(string clipName, SoundPlaybackLimitDef def)
	{
		if (def.m_ClipDefs == null)
		{
			return null;
		}
		foreach (SoundPlaybackLimitClipDef soundPlaybackLimitClipDef in def.m_ClipDefs)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(soundPlaybackLimitClipDef.m_Path);
			if (clipName == fileNameWithoutExtension)
			{
				return soundPlaybackLimitClipDef;
			}
		}
		return null;
	}

	// Token: 0x06001478 RID: 5240 RVA: 0x0005A59C File Offset: 0x0005879C
	public bool StartDucking(SoundDucker ducker)
	{
		if (ducker == null)
		{
			return false;
		}
		if (ducker.m_DuckedCategoryDefs == null)
		{
			return false;
		}
		if (ducker.m_DuckedCategoryDefs.Count == 0)
		{
			return false;
		}
		this.RegisterForDucking(ducker, ducker.GetDuckedCategoryDefs());
		return true;
	}

	// Token: 0x06001479 RID: 5241 RVA: 0x0005A5E4 File Offset: 0x000587E4
	public void StopDucking(SoundDucker ducker)
	{
		if (ducker == null)
		{
			return;
		}
		if (ducker.m_DuckedCategoryDefs == null)
		{
			return;
		}
		if (ducker.m_DuckedCategoryDefs.Count == 0)
		{
			return;
		}
		this.UnregisterForDucking(ducker, ducker.GetDuckedCategoryDefs());
	}

	// Token: 0x0600147A RID: 5242 RVA: 0x0005A628 File Offset: 0x00058828
	public bool IsIgnoringDucking(AudioSource source)
	{
		if (source == null)
		{
			return true;
		}
		SoundDef component = source.GetComponent<SoundDef>();
		return component == null || component.m_IgnoreDucking;
	}

	// Token: 0x0600147B RID: 5243 RVA: 0x0005A660 File Offset: 0x00058860
	public void SetIgnoreDucking(AudioSource source, bool enable)
	{
		if (source == null)
		{
			return;
		}
		SoundDef component = source.GetComponent<SoundDef>();
		if (component == null)
		{
			return;
		}
		component.m_IgnoreDucking = enable;
	}

	// Token: 0x0600147C RID: 5244 RVA: 0x0005A698 File Offset: 0x00058898
	private void RegisterSourceForDucking(AudioSource source, SoundManager.SourceExtension ext)
	{
		SoundDuckingDef soundDuckingDef = this.FindDuckingDefForSource(source);
		if (soundDuckingDef == null)
		{
			return;
		}
		this.RegisterForDucking(source, soundDuckingDef.m_DuckedCategoryDefs);
		ext.m_ducking = true;
	}

	// Token: 0x0600147D RID: 5245 RVA: 0x0005A6C8 File Offset: 0x000588C8
	private void RegisterForDucking(object trigger, List<SoundDuckedCategoryDef> defs)
	{
		foreach (SoundDuckedCategoryDef duckedCatDef in defs)
		{
			SoundManager.DuckState state = this.RegisterDuckState(trigger, duckedCatDef);
			this.ChangeDuckState(state, SoundManager.DuckMode.BEGINNING);
		}
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x0005A728 File Offset: 0x00058928
	private SoundManager.DuckState RegisterDuckState(object trigger, SoundDuckedCategoryDef duckedCatDef)
	{
		SoundCategory category = duckedCatDef.m_Category;
		List<SoundManager.DuckState> list;
		SoundManager.DuckState duckState;
		if (this.m_duckStates.TryGetValue(category, out list))
		{
			duckState = list.Find((SoundManager.DuckState currState) => currState.IsTrigger(trigger));
			if (duckState != null)
			{
				return duckState;
			}
		}
		else
		{
			list = new List<SoundManager.DuckState>();
			this.m_duckStates.Add(category, list);
		}
		duckState = new SoundManager.DuckState();
		list.Add(duckState);
		duckState.SetTrigger(trigger);
		duckState.SetDuckedDef(duckedCatDef);
		return duckState;
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x0005A7B0 File Offset: 0x000589B0
	private void UnregisterSourceForDucking(AudioSource source, SoundManager.SourceExtension ext)
	{
		if (!ext.m_ducking)
		{
			return;
		}
		SoundDuckingDef soundDuckingDef = this.FindDuckingDefForSource(source);
		if (soundDuckingDef == null)
		{
			return;
		}
		this.UnregisterForDucking(source, soundDuckingDef.m_DuckedCategoryDefs);
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x0005A7E8 File Offset: 0x000589E8
	private void UnregisterForDucking(object trigger, List<SoundDuckedCategoryDef> defs)
	{
		foreach (SoundDuckedCategoryDef soundDuckedCategoryDef in defs)
		{
			SoundCategory category = soundDuckedCategoryDef.m_Category;
			List<SoundManager.DuckState> list;
			if (!this.m_duckStates.TryGetValue(category, out list))
			{
				Debug.LogError(string.Format("SoundManager.UnregisterForDucking() - {0} ducks {1}, but no DuckStates were found for {1}", trigger, category));
			}
			else
			{
				SoundManager.DuckState duckState = list.Find((SoundManager.DuckState currState) => currState.IsTrigger(trigger));
				if (duckState != null)
				{
					this.ChangeDuckState(duckState, SoundManager.DuckMode.RESTORING);
				}
			}
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x0005A8AC File Offset: 0x00058AAC
	private uint GetNextDuckStateTweenId()
	{
		this.m_nextDuckStateTweenId = (this.m_nextDuckStateTweenId + 1U & uint.MaxValue);
		return this.m_nextDuckStateTweenId;
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x0005A8C4 File Offset: 0x00058AC4
	private void ChangeDuckState(SoundManager.DuckState state, SoundManager.DuckMode mode)
	{
		string tweenName = state.GetTweenName();
		if (tweenName != null)
		{
			iTween.StopByName(base.gameObject, tweenName);
		}
		state.SetMode(mode);
		state.SetTweenName(null);
		switch (mode)
		{
		case SoundManager.DuckMode.BEGINNING:
			this.AnimateBeginningDuckState(state);
			break;
		case SoundManager.DuckMode.RESTORING:
			this.AnimateRestoringDuckState(state);
			break;
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x0005A92C File Offset: 0x00058B2C
	private void AnimateBeginningDuckState(SoundManager.DuckState state)
	{
		string text = string.Format("DuckState Begin id={0}", this.GetNextDuckStateTweenId());
		state.SetTweenName(text);
		SoundDuckedCategoryDef duckedDef = state.GetDuckedDef();
		Action<object> action = delegate(object amount)
		{
			float volume = (float)amount;
			state.SetVolume(volume);
			this.UpdateCategoryVolume(duckedDef.m_Category);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"name",
			text,
			"time",
			duckedDef.m_BeginSec,
			"easeType",
			duckedDef.m_BeginEaseType,
			"from",
			state.GetVolume(),
			"to",
			duckedDef.m_Volume,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"OnDuckStateBeginningComplete",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			state
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x0005AA78 File Offset: 0x00058C78
	private void OnDuckStateBeginningComplete(SoundManager.DuckState state)
	{
		state.SetMode(SoundManager.DuckMode.HOLD);
		state.SetTweenName(null);
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x0005AA88 File Offset: 0x00058C88
	private void AnimateRestoringDuckState(SoundManager.DuckState state)
	{
		string text = string.Format("DuckState Finish id={0}", this.GetNextDuckStateTweenId());
		state.SetTweenName(text);
		SoundDuckedCategoryDef duckedDef = state.GetDuckedDef();
		Action<object> action = delegate(object amount)
		{
			float volume = (float)amount;
			state.SetVolume(volume);
			this.UpdateCategoryVolume(duckedDef.m_Category);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"name",
			text,
			"time",
			duckedDef.m_RestoreSec,
			"easeType",
			duckedDef.m_RestoreEaseType,
			"from",
			state.GetVolume(),
			"to",
			1f,
			"onupdate",
			action,
			"onupdatetarget",
			base.gameObject,
			"oncomplete",
			"OnDuckStateRestoringComplete",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			state
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x0005ABD0 File Offset: 0x00058DD0
	private void OnDuckStateRestoringComplete(SoundManager.DuckState state)
	{
		SoundDuckedCategoryDef duckedDef = state.GetDuckedDef();
		SoundCategory category = duckedDef.m_Category;
		List<SoundManager.DuckState> list = this.m_duckStates[category];
		for (int i = 0; i < list.Count; i++)
		{
			SoundManager.DuckState duckState = list[i];
			if (duckState == state)
			{
				list.RemoveAt(i);
				if (list.Count == 0)
				{
					this.m_duckStates.Remove(category);
				}
				break;
			}
		}
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x0005AC44 File Offset: 0x00058E44
	private SoundDuckingDef FindDuckingDefForSource(AudioSource source)
	{
		SoundCategory category = this.GetCategory(source);
		return this.FindDuckingDefForCategory(category);
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x0005AC60 File Offset: 0x00058E60
	private SoundDuckingDef FindDuckingDefForCategory(SoundCategory cat)
	{
		if (this.m_config == null || this.m_config.m_DuckingDefs == null)
		{
			return null;
		}
		foreach (SoundDuckingDef soundDuckingDef in this.m_config.m_DuckingDefs)
		{
			if (cat == soundDuckingDef.m_TriggerCategory)
			{
				return soundDuckingDef;
			}
		}
		return null;
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x0005ACF4 File Offset: 0x00058EF4
	private float GetDuckingVolume(AudioSource source)
	{
		if (source == null)
		{
			return 1f;
		}
		SoundDef component = source.GetComponent<SoundDef>();
		if (component.m_IgnoreDucking)
		{
			return 1f;
		}
		return this.GetDuckingVolume(component.m_Category);
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x0005AD38 File Offset: 0x00058F38
	private float GetDuckingVolume(SoundCategory cat)
	{
		List<SoundManager.DuckState> list;
		if (!this.m_duckStates.TryGetValue(cat, out list))
		{
			return 1f;
		}
		float num = 1f;
		foreach (SoundManager.DuckState duckState in list)
		{
			SoundCategory triggerCategory = duckState.GetTriggerCategory();
			if (triggerCategory == SoundCategory.NONE || SoundUtils.IsCategoryAudible(triggerCategory))
			{
				float volume = duckState.GetVolume();
				if (num > volume)
				{
					num = volume;
				}
			}
		}
		return num;
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x0005ADDC File Offset: 0x00058FDC
	private int GetNextSourceId()
	{
		int nextSourceId = this.m_nextSourceId;
		this.m_nextSourceId = ((this.m_nextSourceId != int.MaxValue) ? (this.m_nextSourceId + 1) : 1);
		return nextSourceId;
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x0005AE18 File Offset: 0x00059018
	private int GetSourceId(AudioSource source)
	{
		SoundManager.SourceExtension extension = this.GetExtension(source);
		if (extension == null)
		{
			return 0;
		}
		return extension.m_id;
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x0005AE3C File Offset: 0x0005903C
	private AudioSource PlayImpl(AudioSource source, AudioClip oneShotClip = null)
	{
		if (source == null)
		{
			AudioSource placeholderSource = this.GetPlaceholderSource();
			if (placeholderSource == null)
			{
				Error.AddDevFatal("SoundManager.Play() - source is null and fallback is null", new object[0]);
				return null;
			}
			source = Object.Instantiate<AudioSource>(placeholderSource);
			this.m_generatedSources.Add(source);
		}
		bool flag = this.IsActive(source);
		SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, true, oneShotClip);
		if (sourceExtension == null)
		{
			return null;
		}
		if (!flag)
		{
			this.RegisterSourceForDucking(source, sourceExtension);
		}
		this.UpdateSource(source, sourceExtension);
		source.Play();
		return source;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0005AEC8 File Offset: 0x000590C8
	private SoundDef GetDefFromSource(AudioSource source)
	{
		SoundDef soundDef = source.GetComponent<SoundDef>();
		if (soundDef == null)
		{
			Log.Sound.Print("SoundUtils.GetDefFromSource() - source={0} has no def. adding new def.", new object[]
			{
				source
			});
			soundDef = source.gameObject.AddComponent<SoundDef>();
		}
		return soundDef;
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x0005AF0E File Offset: 0x0005910E
	private void OnAppFocusChanged(bool focus, object userData)
	{
		this.UpdateAppMute();
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x0005AF18 File Offset: 0x00059118
	private void UpdateAppMute()
	{
		this.UpdateMusicAndSources();
		if (ApplicationMgr.Get() != null)
		{
			this.m_mute = (!ApplicationMgr.Get().HasFocus() && !Options.Get().GetBool(Option.BACKGROUND_SOUND));
		}
		this.UpdateAllMutes();
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x0005AF68 File Offset: 0x00059168
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		this.GarbageCollectBundles();
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x0005AF70 File Offset: 0x00059170
	private AudioSource GenerateAudioSource(AudioSource templateSource, AudioClip clip)
	{
		string text = string.Format("Audio Object - {0}", clip.name);
		AudioSource component;
		if (templateSource)
		{
			GameObject gameObject = new GameObject(text);
			SoundUtils.AddAudioSourceComponents(gameObject, null);
			component = gameObject.GetComponent<AudioSource>();
			SoundUtils.CopyAudioSource(templateSource, component);
		}
		else if (this.m_config.m_PlayClipTemplate)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.m_config.m_PlayClipTemplate.gameObject);
			gameObject.name = text;
			component = gameObject.GetComponent<AudioSource>();
		}
		else
		{
			GameObject gameObject = new GameObject(text);
			SoundUtils.AddAudioSourceComponents(gameObject, null);
			component = gameObject.GetComponent<AudioSource>();
		}
		this.m_generatedSources.Add(component);
		return component;
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x0005B01C File Offset: 0x0005921C
	private void InitSourceTransform(AudioSource source, GameObject parentObject)
	{
		source.transform.parent = base.transform;
		if (parentObject == null)
		{
			source.transform.position = Vector3.zero;
		}
		else
		{
			source.transform.position = parentObject.transform.position;
		}
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x0005B074 File Offset: 0x00059274
	private void FinishSource(AudioSource source)
	{
		if (this.m_currentMusicTrack == source)
		{
			this.ChangeCurrentMusicTrack(null);
		}
		else if (this.m_currentAmbienceTrack == source)
		{
			this.ChangeCurrentAmbienceTrack(null);
		}
		for (int i = 0; i < this.m_fadingTracks.Count; i++)
		{
			AudioSource audioSource = this.m_fadingTracks[i];
			if (audioSource == source)
			{
				this.m_fadingTracks.RemoveAt(i);
				break;
			}
		}
		this.UnregisterSourceByCategory(source);
		this.UnregisterSourceByClip(source);
		SoundManager.SourceExtension extension = this.GetExtension(source);
		if (extension != null)
		{
			this.UnregisterSourceForDucking(source, extension);
			this.UnregisterSourceBundle(source, extension);
			this.UnregisterExtension(source, extension);
		}
		this.FinishGeneratedSource(source);
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x0005B138 File Offset: 0x00059338
	private void FinishGeneratedSource(AudioSource source)
	{
		for (int i = 0; i < this.m_generatedSources.Count; i++)
		{
			AudioSource audioSource = this.m_generatedSources[i];
			if (audioSource == source)
			{
				Object.DestroyImmediate(source.gameObject);
				this.m_generatedSources.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x0005B194 File Offset: 0x00059394
	private SoundManager.BundleInfo RegisterSourceBundle(string name, AudioSource source)
	{
		SoundManager.BundleInfo bundleInfo;
		if (!this.m_bundleInfos.TryGetValue(name, out bundleInfo))
		{
			bundleInfo = new SoundManager.BundleInfo();
			bundleInfo.SetName(name);
			this.m_bundleInfos.Add(name, bundleInfo);
		}
		if (source != null)
		{
			bundleInfo.AddRef(source);
			SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source, false, null);
			sourceExtension.m_bundleName = name;
		}
		return bundleInfo;
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x0005B1F3 File Offset: 0x000593F3
	private void UnregisterSourceBundle(AudioSource source, SoundManager.SourceExtension ext)
	{
		if (ext.m_bundleName == null)
		{
			return;
		}
		this.UnregisterSourceBundle(ext.m_bundleName, source);
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x0005B210 File Offset: 0x00059410
	private void UnregisterSourceBundle(string name, AudioSource source)
	{
		SoundManager.BundleInfo bundleInfo;
		if (!this.m_bundleInfos.TryGetValue(name, out bundleInfo))
		{
			return;
		}
		if (!bundleInfo.RemoveRef(source))
		{
			return;
		}
		if (bundleInfo.CanGarbageCollect())
		{
			this.m_bundleInfos.Remove(name);
			this.UnloadSoundBundle(name);
		}
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x0005B262 File Offset: 0x00059462
	private void UnloadSoundBundle(string name)
	{
		AssetCache.ClearSound(name);
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x0005B26C File Offset: 0x0005946C
	private void GarbageCollectBundles()
	{
		Map<string, SoundManager.BundleInfo> map = new Map<string, SoundManager.BundleInfo>();
		foreach (KeyValuePair<string, SoundManager.BundleInfo> keyValuePair in this.m_bundleInfos)
		{
			string key = keyValuePair.Key;
			SoundManager.BundleInfo value = keyValuePair.Value;
			value.EnableGarbageCollect(true);
			if (value.CanGarbageCollect())
			{
				this.UnloadSoundBundle(key);
			}
			else
			{
				map.Add(key, value);
			}
		}
		this.m_bundleInfos = map;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x0005B308 File Offset: 0x00059508
	private void UpdateMusicAndSources()
	{
		this.UpdateMusicAndAmbience();
		this.UpdateSources();
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x0005B318 File Offset: 0x00059518
	private void UpdateSources()
	{
		this.UpdateSourceExtensionMappings();
		this.UpdateSourcesByCategory();
		this.UpdateSourcesByClipName();
		this.UpdateSourceBundles();
		this.UpdateGeneratedSources();
		this.UpdateDuckStates();
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x0005B34C File Offset: 0x0005954C
	private void UpdateSourceExtensionMappings()
	{
		int i = 0;
		while (i < this.m_extensionMappings.Count)
		{
			SoundManager.ExtensionMapping extensionMapping = this.m_extensionMappings[i];
			AudioSource source = extensionMapping.Source;
			if (source == null)
			{
				this.m_extensionMappings.RemoveAt(i);
			}
			else
			{
				if (!this.IsActive(source))
				{
					this.m_inactiveSources.Add(source);
				}
				i++;
			}
		}
		this.CleanInactiveSources();
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x0005B3C4 File Offset: 0x000595C4
	private void CleanUpSourceList(List<AudioSource> sources)
	{
		if (sources == null)
		{
			return;
		}
		int i = 0;
		while (i < sources.Count)
		{
			AudioSource audioSource = sources[i];
			if (audioSource == null)
			{
				sources.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0005B410 File Offset: 0x00059610
	private void UpdateSourcesByCategory()
	{
		foreach (List<AudioSource> sources in this.m_sourcesByCategory.Values)
		{
			this.CleanUpSourceList(sources);
		}
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x0005B470 File Offset: 0x00059670
	private void UpdateSourcesByClipName()
	{
		foreach (List<AudioSource> sources in this.m_sourcesByClipName.Values)
		{
			this.CleanUpSourceList(sources);
		}
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x0005B4D0 File Offset: 0x000596D0
	private void UpdateSourceBundles()
	{
		foreach (SoundManager.BundleInfo bundleInfo in this.m_bundleInfos.Values)
		{
			List<AudioSource> refs = bundleInfo.GetRefs();
			int i = 0;
			bool flag = false;
			while (i < refs.Count)
			{
				AudioSource audioSource = refs[i];
				if (audioSource == null)
				{
					flag = true;
					refs.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			if (flag)
			{
				string name = bundleInfo.GetName();
				if (bundleInfo.CanGarbageCollect())
				{
					this.m_bundleInfos.Remove(name);
					this.UnloadSoundBundle(name);
				}
			}
		}
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x0005B5A4 File Offset: 0x000597A4
	private void UpdateGeneratedSources()
	{
		this.CleanUpSourceList(this.m_generatedSources);
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x0005B5B4 File Offset: 0x000597B4
	private void FinishDeadGeneratedSource(AudioSource source)
	{
		for (int i = 0; i < this.m_generatedSources.Count; i++)
		{
			AudioSource audioSource = this.m_generatedSources[i];
			if (audioSource == source)
			{
				this.m_generatedSources.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x0005B604 File Offset: 0x00059804
	private void UpdateDuckStates()
	{
		foreach (List<SoundManager.DuckState> list in this.m_duckStates.Values)
		{
			foreach (SoundManager.DuckState duckState in list)
			{
				if (!duckState.IsTriggerAlive())
				{
					SoundManager.DuckMode mode = duckState.GetMode();
					if (mode != SoundManager.DuckMode.RESTORING)
					{
						this.ChangeDuckState(duckState, SoundManager.DuckMode.RESTORING);
					}
				}
			}
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0005B6C8 File Offset: 0x000598C8
	private void CleanInactiveSources()
	{
		foreach (AudioSource source in this.m_inactiveSources)
		{
			this.FinishSource(source);
		}
		this.m_inactiveSources.Clear();
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0005B730 File Offset: 0x00059930
	[Conditional("SOUND_SOURCE_DEBUG")]
	private void SourcePrint(string format, params object[] args)
	{
		Log.Sound.Print(format, args);
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x0005B73E File Offset: 0x0005993E
	[Conditional("SOUND_SOURCE_DEBUG")]
	private void SourceScreenPrint(string format, params object[] args)
	{
		Log.Sound.ScreenPrint(format, args);
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x0005B74C File Offset: 0x0005994C
	[Conditional("SOUND_TRACK_DEBUG")]
	private void TrackPrint(string format, params object[] args)
	{
		Log.Sound.Print(format, args);
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x0005B75A File Offset: 0x0005995A
	[Conditional("SOUND_TRACK_DEBUG")]
	private void TrackScreenPrint(string format, params object[] args)
	{
		Log.Sound.ScreenPrint(format, args);
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x0005B768 File Offset: 0x00059968
	[Conditional("SOUND_CATEGORY_DEBUG")]
	private void CategoryPrint(string format, params object[] args)
	{
		Log.Sound.Print(format, args);
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x0005B776 File Offset: 0x00059976
	[Conditional("SOUND_CATEGORY_DEBUG")]
	private void CategoryScreenPrint(string format, params object[] args)
	{
		Log.Sound.ScreenPrint(format, args);
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x0005B784 File Offset: 0x00059984
	[Conditional("SOUND_CATEGORY_DEBUG")]
	private void PrintAllCategorySources()
	{
		Log.Sound.Print("SoundManager.PrintAllCategorySources()", new object[0]);
		foreach (KeyValuePair<SoundCategory, List<AudioSource>> keyValuePair in this.m_sourcesByCategory)
		{
			SoundCategory key = keyValuePair.Key;
			List<AudioSource> value = keyValuePair.Value;
			Log.Sound.Print("Category {0}:", new object[]
			{
				key
			});
			for (int i = 0; i < value.Count; i++)
			{
				Log.Sound.Print("    {0} = {1}", new object[]
				{
					i,
					value[i]
				});
			}
		}
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x0005B860 File Offset: 0x00059A60
	[Conditional("SOUND_BUNDLE_DEBUG")]
	private void BundlePrint(string format, params object[] args)
	{
		Log.Sound.Print(format, args);
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x0005B86E File Offset: 0x00059A6E
	[Conditional("SOUND_BUNDLE_DEBUG")]
	private void BundleScreenPrint(string format, params object[] args)
	{
		Log.Sound.ScreenPrint(format, args);
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x0005B87C File Offset: 0x00059A7C
	[Conditional("SOUND_DUCKING_DEBUG")]
	private void DuckingPrint(string format, params object[] args)
	{
		Log.Sound.Print(format, args);
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x0005B88A File Offset: 0x00059A8A
	[Conditional("SOUND_DUCKING_DEBUG")]
	private void DuckingScreenPrint(string format, params object[] args)
	{
		Log.Sound.ScreenPrint(format, args);
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x0005B898 File Offset: 0x00059A98
	private void AddExtensionMapping(AudioSource source, SoundManager.SourceExtension extension)
	{
		if (source == null || extension == null)
		{
			return;
		}
		SoundManager.ExtensionMapping extensionMapping = new SoundManager.ExtensionMapping();
		extensionMapping.Source = source;
		extensionMapping.Extension = extension;
		this.m_extensionMappings.Add(extensionMapping);
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x0005B8D8 File Offset: 0x00059AD8
	private void RemoveExtensionMapping(AudioSource source)
	{
		for (int i = 0; i < this.m_extensionMappings.Count; i++)
		{
			SoundManager.ExtensionMapping extensionMapping = this.m_extensionMappings[i];
			if (extensionMapping.Source == source)
			{
				this.m_extensionMappings.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x0005B92C File Offset: 0x00059B2C
	private SoundManager.SourceExtension GetExtension(AudioSource source)
	{
		for (int i = 0; i < this.m_extensionMappings.Count; i++)
		{
			SoundManager.ExtensionMapping extensionMapping = this.m_extensionMappings[i];
			if (extensionMapping.Source == source)
			{
				return extensionMapping.Extension;
			}
		}
		return null;
	}

	// Token: 0x04000A52 RID: 2642
	private static SoundManager s_instance;

	// Token: 0x04000A53 RID: 2643
	private SoundConfig m_config;

	// Token: 0x04000A54 RID: 2644
	private List<AudioSource> m_generatedSources = new List<AudioSource>();

	// Token: 0x04000A55 RID: 2645
	private List<SoundManager.ExtensionMapping> m_extensionMappings = new List<SoundManager.ExtensionMapping>();

	// Token: 0x04000A56 RID: 2646
	private Map<SoundCategory, List<AudioSource>> m_sourcesByCategory = new Map<SoundCategory, List<AudioSource>>();

	// Token: 0x04000A57 RID: 2647
	private Map<string, List<AudioSource>> m_sourcesByClipName = new Map<string, List<AudioSource>>();

	// Token: 0x04000A58 RID: 2648
	private Map<string, SoundManager.BundleInfo> m_bundleInfos = new Map<string, SoundManager.BundleInfo>();

	// Token: 0x04000A59 RID: 2649
	private Map<SoundCategory, List<SoundManager.DuckState>> m_duckStates = new Map<SoundCategory, List<SoundManager.DuckState>>();

	// Token: 0x04000A5A RID: 2650
	private uint m_nextDuckStateTweenId;

	// Token: 0x04000A5B RID: 2651
	private List<AudioSource> m_inactiveSources = new List<AudioSource>();

	// Token: 0x04000A5C RID: 2652
	private List<MusicTrack> m_musicTracks = new List<MusicTrack>();

	// Token: 0x04000A5D RID: 2653
	private List<MusicTrack> m_ambienceTracks = new List<MusicTrack>();

	// Token: 0x04000A5E RID: 2654
	private bool m_musicIsAboutToPlay;

	// Token: 0x04000A5F RID: 2655
	private bool m_ambienceIsAboutToPlay;

	// Token: 0x04000A60 RID: 2656
	private AudioSource m_currentMusicTrack;

	// Token: 0x04000A61 RID: 2657
	private AudioSource m_currentAmbienceTrack;

	// Token: 0x04000A62 RID: 2658
	private List<AudioSource> m_fadingTracks = new List<AudioSource>();

	// Token: 0x04000A63 RID: 2659
	private int m_musicTrackIndex;

	// Token: 0x04000A64 RID: 2660
	private int m_ambienceTrackIndex;

	// Token: 0x04000A65 RID: 2661
	private bool m_mute;

	// Token: 0x04000A66 RID: 2662
	private int m_nextSourceId = 1;

	// Token: 0x04000A67 RID: 2663
	private uint m_frame;

	// Token: 0x04000A68 RID: 2664
	private List<Coroutine> m_fadingTracksIn = new List<Coroutine>();

	// Token: 0x02000350 RID: 848
	// (Invoke) Token: 0x06002BFE RID: 11262
	public delegate void LoadedCallback(AudioSource source, object userData);

	// Token: 0x02000351 RID: 849
	private class ExtensionMapping
	{
		// Token: 0x04001B07 RID: 6919
		public AudioSource Source;

		// Token: 0x04001B08 RID: 6920
		public SoundManager.SourceExtension Extension;
	}

	// Token: 0x02000352 RID: 850
	private class SourceExtension
	{
		// Token: 0x04001B09 RID: 6921
		public int m_id;

		// Token: 0x04001B0A RID: 6922
		public float m_codeVolume = 1f;

		// Token: 0x04001B0B RID: 6923
		public float m_sourceVolume = 1f;

		// Token: 0x04001B0C RID: 6924
		public float m_defVolume = 1f;

		// Token: 0x04001B0D RID: 6925
		public float m_codePitch = 1f;

		// Token: 0x04001B0E RID: 6926
		public float m_sourcePitch = 1f;

		// Token: 0x04001B0F RID: 6927
		public float m_defPitch = 1f;

		// Token: 0x04001B10 RID: 6928
		public AudioClip m_sourceClip;

		// Token: 0x04001B11 RID: 6929
		public bool m_paused;

		// Token: 0x04001B12 RID: 6930
		public bool m_ducking;

		// Token: 0x04001B13 RID: 6931
		public string m_bundleName;
	}

	// Token: 0x02000353 RID: 851
	private class SoundLoadContext
	{
		// Token: 0x06002C04 RID: 11268 RVA: 0x000DB7DD File Offset: 0x000D99DD
		public void Init(GameObject parent, float volume, SoundManager.LoadedCallback callback, object userData)
		{
			this.m_parent = parent;
			this.m_volume = volume;
			this.Init(callback, userData);
		}

		// Token: 0x06002C05 RID: 11269 RVA: 0x000DB7F8 File Offset: 0x000D99F8
		public void Init(SoundManager.LoadedCallback callback, object userData)
		{
			this.m_sceneMode = ((!(SceneMgr.Get() == null)) ? SceneMgr.Get().GetMode() : SceneMgr.Mode.INVALID);
			this.m_haveCallback = (callback != null);
			this.m_callback = callback;
			this.m_userData = userData;
		}

		// Token: 0x04001B14 RID: 6932
		public AudioSource m_template;

		// Token: 0x04001B15 RID: 6933
		public GameObject m_parent;

		// Token: 0x04001B16 RID: 6934
		public float m_volume;

		// Token: 0x04001B17 RID: 6935
		public SceneMgr.Mode m_sceneMode;

		// Token: 0x04001B18 RID: 6936
		public bool m_haveCallback;

		// Token: 0x04001B19 RID: 6937
		public SoundManager.LoadedCallback m_callback;

		// Token: 0x04001B1A RID: 6938
		public object m_userData;
	}

	// Token: 0x02000354 RID: 852
	private class BundleInfo
	{
		// Token: 0x06002C07 RID: 11271 RVA: 0x000DB859 File Offset: 0x000D9A59
		public string GetName()
		{
			return this.m_name;
		}

		// Token: 0x06002C08 RID: 11272 RVA: 0x000DB861 File Offset: 0x000D9A61
		public void SetName(string name)
		{
			this.m_name = name;
		}

		// Token: 0x06002C09 RID: 11273 RVA: 0x000DB86A File Offset: 0x000D9A6A
		public int GetRefCount()
		{
			return this.m_refs.Count;
		}

		// Token: 0x06002C0A RID: 11274 RVA: 0x000DB877 File Offset: 0x000D9A77
		public List<AudioSource> GetRefs()
		{
			return this.m_refs;
		}

		// Token: 0x06002C0B RID: 11275 RVA: 0x000DB87F File Offset: 0x000D9A7F
		public void AddRef(AudioSource instance)
		{
			this.m_garbageCollect = false;
			this.m_refs.Add(instance);
		}

		// Token: 0x06002C0C RID: 11276 RVA: 0x000DB894 File Offset: 0x000D9A94
		public bool RemoveRef(AudioSource instance)
		{
			return this.m_refs.Remove(instance);
		}

		// Token: 0x06002C0D RID: 11277 RVA: 0x000DB8A2 File Offset: 0x000D9AA2
		public bool CanGarbageCollect()
		{
			return this.m_garbageCollect && this.m_refs.Count <= 0 && !AssetCache.IsLoading(this.m_name);
		}

		// Token: 0x06002C0E RID: 11278 RVA: 0x000DB8D7 File Offset: 0x000D9AD7
		public bool IsGarbageCollectEnabled()
		{
			return this.m_garbageCollect;
		}

		// Token: 0x06002C0F RID: 11279 RVA: 0x000DB8DF File Offset: 0x000D9ADF
		public void EnableGarbageCollect(bool enable)
		{
			this.m_garbageCollect = enable;
		}

		// Token: 0x04001B1B RID: 6939
		private string m_name;

		// Token: 0x04001B1C RID: 6940
		private List<AudioSource> m_refs = new List<AudioSource>();

		// Token: 0x04001B1D RID: 6941
		private bool m_garbageCollect;
	}

	// Token: 0x02000355 RID: 853
	private enum DuckMode
	{
		// Token: 0x04001B1F RID: 6943
		IDLE,
		// Token: 0x04001B20 RID: 6944
		BEGINNING,
		// Token: 0x04001B21 RID: 6945
		HOLD,
		// Token: 0x04001B22 RID: 6946
		RESTORING
	}

	// Token: 0x02000356 RID: 854
	private class DuckState
	{
		// Token: 0x06002C11 RID: 11281 RVA: 0x000DB8FB File Offset: 0x000D9AFB
		public object GetTrigger()
		{
			return this.m_trigger;
		}

		// Token: 0x06002C12 RID: 11282 RVA: 0x000DB904 File Offset: 0x000D9B04
		public void SetTrigger(object trigger)
		{
			this.m_trigger = trigger;
			AudioSource audioSource = trigger as AudioSource;
			if (audioSource != null)
			{
				this.m_triggerCategory = SoundManager.Get().GetCategory(audioSource);
			}
		}

		// Token: 0x06002C13 RID: 11283 RVA: 0x000DB93C File Offset: 0x000D9B3C
		public bool IsTrigger(object trigger)
		{
			return this.m_trigger == trigger;
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x000DB947 File Offset: 0x000D9B47
		public bool IsTriggerAlive()
		{
			return GeneralUtils.IsObjectAlive(this.m_trigger);
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x000DB954 File Offset: 0x000D9B54
		public SoundCategory GetTriggerCategory()
		{
			return this.m_triggerCategory;
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x000DB95C File Offset: 0x000D9B5C
		public SoundDuckedCategoryDef GetDuckedDef()
		{
			return this.m_duckedDef;
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x000DB964 File Offset: 0x000D9B64
		public void SetDuckedDef(SoundDuckedCategoryDef def)
		{
			this.m_duckedDef = def;
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x000DB96D File Offset: 0x000D9B6D
		public SoundManager.DuckMode GetMode()
		{
			return this.m_mode;
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x000DB975 File Offset: 0x000D9B75
		public void SetMode(SoundManager.DuckMode mode)
		{
			this.m_mode = mode;
		}

		// Token: 0x06002C1A RID: 11290 RVA: 0x000DB97E File Offset: 0x000D9B7E
		public string GetTweenName()
		{
			return this.m_tweenName;
		}

		// Token: 0x06002C1B RID: 11291 RVA: 0x000DB986 File Offset: 0x000D9B86
		public void SetTweenName(string name)
		{
			this.m_tweenName = name;
		}

		// Token: 0x06002C1C RID: 11292 RVA: 0x000DB98F File Offset: 0x000D9B8F
		public float GetVolume()
		{
			return this.m_volume;
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x000DB997 File Offset: 0x000D9B97
		public void SetVolume(float volume)
		{
			this.m_volume = volume;
		}

		// Token: 0x04001B23 RID: 6947
		private object m_trigger;

		// Token: 0x04001B24 RID: 6948
		private SoundCategory m_triggerCategory;

		// Token: 0x04001B25 RID: 6949
		private SoundDuckedCategoryDef m_duckedDef;

		// Token: 0x04001B26 RID: 6950
		private SoundManager.DuckMode m_mode;

		// Token: 0x04001B27 RID: 6951
		private string m_tweenName;

		// Token: 0x04001B28 RID: 6952
		private float m_volume = 1f;
	}
}
