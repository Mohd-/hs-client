using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000857 RID: 2135
public class DragCardSoundEffects : MonoBehaviour
{
	// Token: 0x06005236 RID: 21046 RVA: 0x00188AC8 File Offset: 0x00186CC8
	private void Awake()
	{
		this.m_PreviousPosition = base.transform.position;
	}

	// Token: 0x06005237 RID: 21047 RVA: 0x00188ADC File Offset: 0x00186CDC
	private void Update()
	{
		if (this.m_Disabled || !base.enabled)
		{
			return;
		}
		if (this.m_AirSoundLoading || this.m_MagicalSoundLoading)
		{
			return;
		}
		if (this.m_AirSoundLoop == null)
		{
			this.LoadAirSound();
			return;
		}
		if (this.m_MagicalSoundLoop == null)
		{
			this.LoadMagicalSound();
			return;
		}
		if (this.m_AirSoundLoop == null || this.m_MagicalSoundLoop == null)
		{
			return;
		}
		if (this.m_Card == null)
		{
			this.m_Card = base.GetComponent<Card>();
		}
		if (this.m_Card == null)
		{
			this.Disable();
			return;
		}
		if (this.m_Actor == null)
		{
			this.m_Actor = this.m_Card.GetActor();
		}
		if (this.m_Actor == null)
		{
			this.Disable();
			return;
		}
		if (!this.m_Actor.IsShown())
		{
			Log.Kyle.Print(string.Format("Something went wrong in DragCardSoundEffects on {0} and we are killing a stuck sound!", this.m_Card.gameObject.name), new object[0]);
			this.Disable();
			return;
		}
		this.m_MagicalSoundLoop.transform.position = base.transform.position;
		this.m_AirSoundLoop.transform.position = base.transform.position;
		if (this.m_MagicalVolume < 0.15f)
		{
			this.m_MagicalVolume = Mathf.SmoothDamp(this.m_MagicalVolume, 0.15f, ref this.m_MagicalVelocity, 0.5f);
			SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
		}
		else if (this.m_MagicalVolume > 0.15f)
		{
			this.m_MagicalVolume = 0.15f;
			SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
		}
		Vector3 position = base.transform.position;
		Vector3 vector = position - this.m_PreviousPosition;
		this.m_AirVolume = Mathf.SmoothDamp(this.m_AirVolume, Mathf.Log(vector.magnitude * 0.5f + 0.92f), ref this.m_AirVelocity, 0.040000003f, 1f);
		SoundManager.Get().SetVolume(this.m_AirSoundLoop, Mathf.Clamp(this.m_AirVolume, 0f, 0.5f));
		SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
		this.m_PreviousPosition = position;
	}

	// Token: 0x06005238 RID: 21048 RVA: 0x00188D61 File Offset: 0x00186F61
	public void Restart()
	{
		base.enabled = true;
		this.m_Disabled = false;
	}

	// Token: 0x06005239 RID: 21049 RVA: 0x00188D71 File Offset: 0x00186F71
	public void Disable()
	{
		this.m_Disabled = true;
		if (base.enabled && !this.m_FadingOut)
		{
			base.StartCoroutine("FadeOutSound");
		}
	}

	// Token: 0x0600523A RID: 21050 RVA: 0x00188D9C File Offset: 0x00186F9C
	private void OnDisable()
	{
		this.StopSound();
	}

	// Token: 0x0600523B RID: 21051 RVA: 0x00188DA4 File Offset: 0x00186FA4
	private void OnDestroy()
	{
		base.StopCoroutine("FadeOutSound");
		this.StopSound();
	}

	// Token: 0x0600523C RID: 21052 RVA: 0x00188DB8 File Offset: 0x00186FB8
	private void StopSound()
	{
		this.m_FadingOut = false;
		this.m_MagicalVolume = 0f;
		this.m_AirVolume = 0f;
		this.m_AirVelocity = 0f;
		if (this.m_AirSoundLoop != null)
		{
			SoundManager.Get().Stop(this.m_AirSoundLoop);
		}
		if (this.m_MagicalSoundLoop != null)
		{
			SoundManager.Get().Stop(this.m_MagicalSoundLoop);
		}
	}

	// Token: 0x0600523D RID: 21053 RVA: 0x00188E34 File Offset: 0x00187034
	private IEnumerator FadeOutSound()
	{
		if (this.m_AirSoundLoop == null || this.m_MagicalSoundLoop == null)
		{
			this.StopSound();
			yield break;
		}
		this.m_FadingOut = true;
		while (this.m_AirVolume > 0f && this.m_MagicalVolume > 0f)
		{
			this.m_AirVolume = Mathf.SmoothDamp(this.m_AirVolume, 0f, ref this.m_AirVelocity, 0.2f);
			this.m_MagicalVolume = Mathf.SmoothDamp(this.m_MagicalVolume, 0f, ref this.m_MagicalVelocity, 0.2f);
			SoundManager.Get().SetVolume(this.m_AirSoundLoop, Mathf.Clamp(this.m_AirVolume, 0f, 1f));
			SoundManager.Get().SetVolume(this.m_MagicalSoundLoop, this.m_MagicalVolume);
			yield return null;
		}
		this.m_FadingOut = false;
		this.StopSound();
		yield break;
	}

	// Token: 0x0600523E RID: 21054 RVA: 0x00188E50 File Offset: 0x00187050
	private void LoadAirSound()
	{
		SoundManager.LoadedCallback callback = delegate(AudioSource source, object userData)
		{
			if (source == null)
			{
				return;
			}
			this.m_AirSoundLoading = false;
			this.m_AirSoundLoop = source;
			if (this.m_Disabled || !base.enabled)
			{
				SoundManager.Get().Stop(this.m_AirSoundLoop);
			}
		};
		SoundManager.Get().LoadAndPlay("card_motion_loop_air", base.gameObject, 0f, callback);
	}

	// Token: 0x0600523F RID: 21055 RVA: 0x00188E88 File Offset: 0x00187088
	private void LoadMagicalSound()
	{
		SoundManager.LoadedCallback callback = delegate(AudioSource source, object userData)
		{
			if (source == null)
			{
				return;
			}
			this.m_MagicalSoundLoading = false;
			if (this.m_MagicalSoundLoop != null)
			{
				SoundManager.Get().Stop(this.m_MagicalSoundLoop);
			}
			this.m_MagicalSoundLoop = source;
			if (this.m_Disabled || !base.enabled)
			{
				SoundManager.Get().Stop(this.m_MagicalSoundLoop);
			}
		};
		SoundManager.Get().LoadAndPlay("card_motion_loop_magical", base.gameObject, 0f, callback);
	}

	// Token: 0x04003874 RID: 14452
	private const string CARD_MOTION_LOOP_AIR_SOUND = "card_motion_loop_air";

	// Token: 0x04003875 RID: 14453
	private const string CARD_MOTION_LOOP_MAGICAL_SOUND = "card_motion_loop_magical";

	// Token: 0x04003876 RID: 14454
	private const float MAGICAL_SOUND_VOLUME = 0.15f;

	// Token: 0x04003877 RID: 14455
	private const float MAGICAL_SOUND_FADE_IN_TIME = 0.5f;

	// Token: 0x04003878 RID: 14456
	private const float AIR_SOUND_MAX_VOLUME = 0.5f;

	// Token: 0x04003879 RID: 14457
	private const float AIR_SOUND_MOVEMENT_THRESHOLD = 0.92f;

	// Token: 0x0400387A RID: 14458
	private const float AIR_SOUND_VOLUME_SPEED = 0.4f;

	// Token: 0x0400387B RID: 14459
	private const float AIR_SOUND_VOLUME_VELOCITY_SCALE = 0.5f;

	// Token: 0x0400387C RID: 14460
	private const float DISABLE_VOLUME_FADE_OUT_TIME = 0.2f;

	// Token: 0x0400387D RID: 14461
	private bool m_Disabled;

	// Token: 0x0400387E RID: 14462
	private bool m_FadingOut;

	// Token: 0x0400387F RID: 14463
	private Vector3 m_PreviousPosition;

	// Token: 0x04003880 RID: 14464
	private AudioSource m_AirSoundLoop;

	// Token: 0x04003881 RID: 14465
	private bool m_AirSoundLoading;

	// Token: 0x04003882 RID: 14466
	private AudioSource m_MagicalSoundLoop;

	// Token: 0x04003883 RID: 14467
	private bool m_MagicalSoundLoading;

	// Token: 0x04003884 RID: 14468
	private float m_MagicalVolume;

	// Token: 0x04003885 RID: 14469
	private float m_MagicalVelocity;

	// Token: 0x04003886 RID: 14470
	private float m_AirVolume;

	// Token: 0x04003887 RID: 14471
	private float m_AirVelocity;

	// Token: 0x04003888 RID: 14472
	private Actor m_Actor;

	// Token: 0x04003889 RID: 14473
	private Card m_Card;
}
