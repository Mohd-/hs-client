using System;
using System.IO;
using UnityEngine;

// Token: 0x02000364 RID: 868
public class SoundUtils
{
	// Token: 0x06002C45 RID: 11333 RVA: 0x000DBF36 File Offset: 0x000DA136
	private static bool IsBackgroundMusicPlaying()
	{
		return false;
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x000DBF3C File Offset: 0x000DA13C
	public static Option GetCategoryEnabledOption(SoundCategory cat)
	{
		Option result = Option.INVALID;
		SoundDataTables.s_categoryEnabledOptionMap.TryGetValue(cat, out result);
		return result;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x000DBF5C File Offset: 0x000DA15C
	public static Option GetCategoryVolumeOption(SoundCategory cat)
	{
		Option result = Option.INVALID;
		SoundDataTables.s_categoryVolumeOptionMap.TryGetValue(cat, out result);
		return result;
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x000DBF7C File Offset: 0x000DA17C
	public static float GetCategoryVolume(SoundCategory cat)
	{
		float @float = Options.Get().GetFloat(Option.SOUND_VOLUME);
		Option categoryVolumeOption = SoundUtils.GetCategoryVolumeOption(cat);
		if (categoryVolumeOption == Option.INVALID)
		{
			return @float;
		}
		float float2 = Options.Get().GetFloat(categoryVolumeOption);
		return @float * float2;
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x000DBFB8 File Offset: 0x000DA1B8
	public static bool IsCategoryEnabled(SoundCategory cat)
	{
		if (SoundUtils.IsMusicCategory(cat) && SoundUtils.IsBackgroundMusicPlaying())
		{
			return false;
		}
		if (!Options.Get().GetBool(Option.SOUND))
		{
			return false;
		}
		Option categoryEnabledOption = SoundUtils.GetCategoryEnabledOption(cat);
		return categoryEnabledOption == Option.INVALID || Options.Get().GetBool(categoryEnabledOption);
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x000DC00A File Offset: 0x000DA20A
	public static bool IsFxEnabled()
	{
		return SoundUtils.IsCategoryEnabled(SoundCategory.FX);
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x000DC012 File Offset: 0x000DA212
	public static bool IsMusicEnabled()
	{
		return SoundUtils.IsCategoryEnabled(SoundCategory.MUSIC);
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x000DC01A File Offset: 0x000DA21A
	public static bool IsVoiceEnabled()
	{
		return SoundUtils.IsCategoryEnabled(SoundCategory.VO);
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x000DC022 File Offset: 0x000DA222
	public static bool IsCategoryAudible(SoundCategory cat)
	{
		return SoundUtils.GetCategoryVolume(cat) > Mathf.Epsilon && SoundUtils.IsCategoryEnabled(cat);
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x000DC03C File Offset: 0x000DA23C
	public static bool IsFxAudible()
	{
		return SoundUtils.IsCategoryAudible(SoundCategory.FX);
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x000DC044 File Offset: 0x000DA244
	public static bool IsMusicAudible()
	{
		return SoundUtils.IsCategoryAudible(SoundCategory.MUSIC);
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x000DC04C File Offset: 0x000DA24C
	public static bool IsVoiceAudible()
	{
		return SoundUtils.IsCategoryAudible(SoundCategory.VO);
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x000DC054 File Offset: 0x000DA254
	public static bool IsMusicCategory(SoundCategory cat)
	{
		return cat == SoundCategory.MUSIC || cat == SoundCategory.SPECIAL_MUSIC;
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x000DC069 File Offset: 0x000DA269
	public static bool IsVoiceCategory(SoundCategory cat)
	{
		return cat == SoundCategory.VO || cat == SoundCategory.SPECIAL_VO;
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x000DC080 File Offset: 0x000DA280
	public static SoundCategory GetCategoryFromSource(AudioSource source)
	{
		SoundDef component = source.GetComponent<SoundDef>();
		if (component == null)
		{
			return SoundCategory.NONE;
		}
		return component.m_Category;
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x000DC0A8 File Offset: 0x000DA2A8
	public static bool CanDetectVolume()
	{
		return SoundUtils.PlATFORM_CAN_DETECT_VOLUME;
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x000DC0B4 File Offset: 0x000DA2B4
	public static void SetVolumes(Component c, float volume, bool includeInactive = false)
	{
		if (!c)
		{
			return;
		}
		SoundUtils.SetVolumes(c.gameObject, volume, false);
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x000DC0D0 File Offset: 0x000DA2D0
	public static void SetVolumes(GameObject go, float volume, bool includeInactive = false)
	{
		if (!go)
		{
			return;
		}
		AudioSource[] componentsInChildren = go.GetComponentsInChildren<AudioSource>(includeInactive);
		foreach (AudioSource source in componentsInChildren)
		{
			SoundManager.Get().SetVolume(source, volume);
		}
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x000DC117 File Offset: 0x000DA317
	public static void SetSourceVolumes(Component c, float volume, bool includeInactive = false)
	{
		if (!c)
		{
			return;
		}
		SoundUtils.SetSourceVolumes(c.gameObject, volume, false);
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x000DC134 File Offset: 0x000DA334
	public static void SetSourceVolumes(GameObject go, float volume, bool includeInactive = false)
	{
		if (!go)
		{
			return;
		}
		AudioSource[] componentsInChildren = go.GetComponentsInChildren<AudioSource>(includeInactive);
		foreach (AudioSource audioSource in componentsInChildren)
		{
			audioSource.volume = volume;
		}
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x000DC178 File Offset: 0x000DA378
	public static AudioClip GetRandomClipFromDef(SoundDef def)
	{
		if (def == null)
		{
			return null;
		}
		if (def.m_RandomClips == null)
		{
			return null;
		}
		if (def.m_RandomClips.Count == 0)
		{
			return null;
		}
		float num = 0f;
		foreach (RandomAudioClip randomAudioClip in def.m_RandomClips)
		{
			num += randomAudioClip.m_Weight;
		}
		float num2 = Random.Range(0f, num);
		float num3 = 0f;
		int num4 = def.m_RandomClips.Count - 1;
		for (int i = 0; i < num4; i++)
		{
			RandomAudioClip randomAudioClip2 = def.m_RandomClips[i];
			num3 += randomAudioClip2.m_Weight;
			if (num2 <= num3)
			{
				return randomAudioClip2.m_Clip;
			}
		}
		return def.m_RandomClips[num4].m_Clip;
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x000DC280 File Offset: 0x000DA480
	public static float GetRandomVolumeFromDef(SoundDef def)
	{
		if (def == null)
		{
			return 1f;
		}
		return Random.Range(def.m_RandomVolumeMin, def.m_RandomVolumeMax);
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x000DC2A5 File Offset: 0x000DA4A5
	public static float GetRandomPitchFromDef(SoundDef def)
	{
		if (def == null)
		{
			return 1f;
		}
		return Random.Range(def.m_RandomPitchMin, def.m_RandomPitchMax);
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x000DC2CC File Offset: 0x000DA4CC
	public static void CopyDuckedCategoryDef(SoundDuckedCategoryDef src, SoundDuckedCategoryDef dst)
	{
		dst.m_Category = src.m_Category;
		dst.m_Volume = src.m_Volume;
		dst.m_BeginSec = src.m_BeginSec;
		dst.m_BeginEaseType = src.m_BeginEaseType;
		dst.m_RestoreSec = src.m_RestoreSec;
		dst.m_RestoreEaseType = src.m_RestoreEaseType;
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x000DC324 File Offset: 0x000DA524
	public static void CopyAudioSource(AudioSource src, AudioSource dst)
	{
		dst.clip = src.clip;
		dst.bypassEffects = src.bypassEffects;
		dst.loop = src.loop;
		dst.priority = src.priority;
		dst.volume = src.volume;
		dst.pitch = src.pitch;
		dst.panStereo = src.panStereo;
		dst.spatialBlend = src.spatialBlend;
		dst.reverbZoneMix = src.reverbZoneMix;
		dst.rolloffMode = src.rolloffMode;
		dst.dopplerLevel = src.dopplerLevel;
		dst.minDistance = src.minDistance;
		dst.maxDistance = src.maxDistance;
		dst.spread = src.spread;
		SoundDef component = src.GetComponent<SoundDef>();
		if (component == null)
		{
			SoundDef component2 = dst.GetComponent<SoundDef>();
			if (component2 != null)
			{
				Object.DestroyImmediate(component2);
			}
		}
		else
		{
			SoundDef soundDef = dst.GetComponent<SoundDef>();
			if (soundDef == null)
			{
				soundDef = dst.gameObject.AddComponent<SoundDef>();
			}
			SoundUtils.CopySoundDef(component, soundDef);
		}
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x000DC430 File Offset: 0x000DA630
	public static void CopySoundDef(SoundDef src, SoundDef dst)
	{
		dst.m_Category = src.m_Category;
		dst.m_RandomClips = null;
		if (src.m_RandomClips != null)
		{
			for (int i = 0; i < src.m_RandomClips.Count; i++)
			{
				dst.m_RandomClips.Add(src.m_RandomClips[i]);
			}
		}
		dst.m_RandomPitchMin = src.m_RandomPitchMin;
		dst.m_RandomPitchMax = src.m_RandomPitchMax;
		dst.m_RandomVolumeMin = src.m_RandomVolumeMin;
		dst.m_RandomVolumeMax = src.m_RandomVolumeMax;
		dst.m_IgnoreDucking = src.m_IgnoreDucking;
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000DC4CC File Offset: 0x000DA6CC
	public static bool ChangeAudioSourceSettings(AudioSource source, AudioSourceSettings settings)
	{
		bool result = false;
		if (source.bypassEffects != settings.m_bypassEffects)
		{
			source.bypassEffects = settings.m_bypassEffects;
			result = true;
		}
		if (source.loop != settings.m_loop)
		{
			source.loop = settings.m_loop;
			result = true;
		}
		if (source.priority != settings.m_priority)
		{
			source.priority = settings.m_priority;
			result = true;
		}
		if (!object.Equals(source.volume, settings.m_volume))
		{
			source.volume = settings.m_volume;
			result = true;
		}
		if (!object.Equals(source.pitch, settings.m_pitch))
		{
			source.pitch = settings.m_pitch;
			result = true;
		}
		if (!object.Equals(source.panStereo, settings.m_stereoPan))
		{
			source.panStereo = settings.m_stereoPan;
			result = true;
		}
		if (!object.Equals(source.spatialBlend, settings.m_spatialBlend))
		{
			source.spatialBlend = settings.m_spatialBlend;
			result = true;
		}
		if (!object.Equals(source.reverbZoneMix, settings.m_reverbZoneMix))
		{
			source.reverbZoneMix = settings.m_reverbZoneMix;
			result = true;
		}
		if (source.rolloffMode != settings.m_rolloffMode)
		{
			source.rolloffMode = settings.m_rolloffMode;
			result = true;
		}
		if (!object.Equals(source.dopplerLevel, settings.m_dopplerLevel))
		{
			source.dopplerLevel = settings.m_dopplerLevel;
			result = true;
		}
		if (!object.Equals(source.minDistance, settings.m_minDistance))
		{
			source.minDistance = settings.m_minDistance;
			result = true;
		}
		if (!object.Equals(source.maxDistance, settings.m_maxDistance))
		{
			source.maxDistance = settings.m_maxDistance;
			result = true;
		}
		if (!object.Equals(source.spread, settings.m_spread))
		{
			source.spread = settings.m_spread;
			result = true;
		}
		return result;
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x000DC6F8 File Offset: 0x000DA8F8
	public static bool AddAudioSourceComponents(GameObject go, AudioClip clip = null)
	{
		bool result = false;
		AudioSource audioSource = go.GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = go.AddComponent<AudioSource>();
			SoundUtils.ChangeAudioSourceSettings(audioSource, new AudioSourceSettings());
			result = true;
		}
		if (clip != null && clip != audioSource.clip)
		{
			audioSource.clip = clip;
			result = true;
		}
		if (audioSource.playOnAwake)
		{
			audioSource.playOnAwake = false;
			result = true;
		}
		if (go.GetComponent<SoundDef>() == null)
		{
			go.AddComponent<SoundDef>();
			result = true;
		}
		return result;
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x000DC784 File Offset: 0x000DA984
	public static bool IsVOFilePath(string path)
	{
		string fileName = Path.GetFileName(path);
		return fileName.StartsWith("VO_");
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x000DC7A3 File Offset: 0x000DA9A3
	public static bool IsVOFileName(string name)
	{
		return name.StartsWith("VO_");
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x000DC7B0 File Offset: 0x000DA9B0
	public static bool IsVOClip(AudioClip clip)
	{
		return !(clip == null) && SoundUtils.IsVOFileName(clip.name);
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x000DC7CB File Offset: 0x000DA9CB
	public static bool IsVOClip(RandomAudioClip randomClip)
	{
		return randomClip != null && SoundUtils.IsVOClip(randomClip.m_Clip);
	}

	// Token: 0x04001B6F RID: 7023
	public static PlatformDependentValue<bool> PlATFORM_CAN_DETECT_VOLUME = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		PC = true,
		Mac = true,
		iOS = false,
		Android = false
	};
}
