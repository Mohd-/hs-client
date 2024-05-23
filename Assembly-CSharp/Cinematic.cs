using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000526 RID: 1318
public class Cinematic : MonoBehaviour
{
	// Token: 0x06003D37 RID: 15671 RVA: 0x001273F4 File Offset: 0x001255F4
	private void Awake()
	{
		this.m_soundDucker = base.gameObject.AddComponent<SoundDucker>();
		this.m_soundDucker.m_GlobalDuckDef = new SoundDuckedCategoryDef();
		this.m_soundDucker.m_GlobalDuckDef.m_Volume = 0f;
		this.m_soundDucker.m_GlobalDuckDef.m_RestoreSec = 0f;
		this.m_soundDucker.m_GlobalDuckDef.m_BeginSec = 0f;
	}

	// Token: 0x06003D38 RID: 15672 RVA: 0x00127464 File Offset: 0x00125664
	public void Play(Cinematic.MovieCallback callback)
	{
		this.options = new Cinematic.PlayOptions();
		this.options.callback = callback;
		string text = string.Empty;
		text = "PlayPC";
		if (text == string.Empty)
		{
			return;
		}
		base.StartCoroutine(text, this.options);
	}

	// Token: 0x06003D39 RID: 15673 RVA: 0x001274B3 File Offset: 0x001256B3
	public bool isPlaying()
	{
		return this.m_isPlaying;
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x001274BB File Offset: 0x001256BB
	private static void PlayCinematic(string gameObjectName, string localeName)
	{
	}

	// Token: 0x06003D3B RID: 15675 RVA: 0x001274BD File Offset: 0x001256BD
	private void OnGUI()
	{
		if (!this.m_isPlaying)
		{
			return;
		}
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.m_MovieTexture, 2, false, 0f);
	}

	// Token: 0x06003D3C RID: 15676 RVA: 0x001274F8 File Offset: 0x001256F8
	private IEnumerator PlayPC(Cinematic.PlayOptions options)
	{
		AssetLoader.Get().LoadMovie("Cinematic", new AssetLoader.ObjectCallback(this.MovieLoaded), null, false);
		float timeOut = Time.time + 10f;
		while (!this.m_isMovieLoaded && Time.time < timeOut)
		{
			yield return null;
		}
		if (this.m_MovieTexture == null)
		{
			Debug.LogWarning("m_MovieTexture is null!");
			yield break;
		}
		yield return null;
		while (!this.m_MovieTexture.isReadyToPlay)
		{
			yield return null;
		}
		Options.Get().SetBool(Option.HAS_SEEN_CINEMATIC, true);
		BnetBar.Get().gameObject.SetActive(false);
		AudioClip movieAudioClip = this.m_MovieTexture.audioClip;
		Locale locale = Localization.GetLocale();
		if (locale != Locale.enUS && locale != Locale.enGB && locale != Locale.zhTW && locale != Locale.zhCN)
		{
			AssetLoader.Get().LoadSound("CinematicAudio", new AssetLoader.GameObjectCallback(this.AudioLoaded), null, false, null);
			while (!this.m_isMovieAudioLoaded && Time.time < timeOut)
			{
				yield return null;
			}
			if (this.m_MovieAudio == null)
			{
				Debug.LogWarning("m_MovieAudio is null!");
				yield break;
			}
			movieAudioClip = this.m_MovieAudio;
		}
		this.m_isPlaying = true;
		this.m_MovieTexture.filterMode = 1;
		this.m_MovieTexture.loop = false;
		PegCursor.Get().Hide();
		this.m_MovieTexture.Play();
		this.m_soundDucker.StartDucking();
		SoundPlayClipArgs args = new SoundPlayClipArgs();
		args.m_clip = movieAudioClip;
		args.m_volume = new float?(1f);
		args.m_pitch = new float?(1f);
		args.m_category = new SoundCategory?(SoundCategory.FX);
		args.m_parentObject = base.gameObject;
		AudioSource movieSound = SoundManager.Get().PlayClip(args);
		SoundManager.Get().Set3d(movieSound, false);
		SoundManager.Get().SetIgnoreDucking(movieSound, true);
		Camera CinematicCamera = new GameObject
		{
			transform = 
			{
				position = new Vector3(-9997.9f, -9998.9f, -9999.9f)
			}
		}.AddComponent<Camera>();
		CinematicCamera.name = "Cinematic Background Camera";
		CinematicCamera.clearFlags = 2;
		CinematicCamera.backgroundColor = Color.black;
		CinematicCamera.depth = 1000f;
		CinematicCamera.nearClipPlane = 0.01f;
		CinematicCamera.farClipPlane = 0.02f;
		while (this.m_MovieTexture.isPlaying && !Input.anyKey)
		{
			yield return null;
		}
		if (this.m_MovieTexture.isPlaying)
		{
			SoundManager.Get().Stop(movieSound);
		}
		this.m_MovieTexture.Stop();
		this.m_soundDucker.StopDucking();
		PegCursor.Get().Show();
		BnetBar.Get().gameObject.SetActive(true);
		SocialToastMgr.Get().Reset();
		Object.Destroy(CinematicCamera);
		this.m_isPlaying = false;
		options.callback();
		yield break;
	}

	// Token: 0x06003D3D RID: 15677 RVA: 0x00127524 File Offset: 0x00125724
	private void MovieLoaded(string name, Object obj, object callbackData)
	{
		if (obj == null)
		{
			Debug.LogError("Failed to load Cinematic movie!");
			return;
		}
		this.m_isMovieLoaded = true;
		this.m_MovieTexture = (obj as MovieTexture);
	}

	// Token: 0x06003D3E RID: 15678 RVA: 0x0012755B File Offset: 0x0012575B
	private void AudioLoaded(string name, GameObject obj, object callbackData)
	{
		if (obj == null)
		{
			Debug.LogError("Failed to load Cinematic Audio Track!");
			return;
		}
		this.m_isMovieAudioLoaded = true;
		this.m_MovieAudio = obj.GetComponent<AudioSource>().clip;
	}

	// Token: 0x040026DE RID: 9950
	private const string CINEMATIC_FILE_NAME = "Cinematic";

	// Token: 0x040026DF RID: 9951
	private const float MOVIE_LOAD_TIMEOUT = 10f;

	// Token: 0x040026E0 RID: 9952
	private bool m_isPlaying;

	// Token: 0x040026E1 RID: 9953
	private bool m_isMovieLoaded;

	// Token: 0x040026E2 RID: 9954
	private bool m_isMovieAudioLoaded;

	// Token: 0x040026E3 RID: 9955
	private Cinematic.PlayOptions options;

	// Token: 0x040026E4 RID: 9956
	private MovieTexture m_MovieTexture;

	// Token: 0x040026E5 RID: 9957
	private AudioClip m_MovieAudio;

	// Token: 0x040026E6 RID: 9958
	private SoundDucker m_soundDucker;

	// Token: 0x02000527 RID: 1319
	// (Invoke) Token: 0x06003D40 RID: 15680
	public delegate void MovieCallback();

	// Token: 0x0200097E RID: 2430
	private class PlayOptions
	{
		// Token: 0x04003F3C RID: 16188
		public Cinematic.MovieCallback callback;
	}
}
