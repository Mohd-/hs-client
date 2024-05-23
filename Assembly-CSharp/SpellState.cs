using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E2 RID: 994
public class SpellState : MonoBehaviour
{
	// Token: 0x060032FA RID: 13050 RVA: 0x000FDAF0 File Offset: 0x000FBCF0
	private void Start()
	{
		this.m_spell = SceneUtils.FindComponentInParents<Spell>(base.gameObject);
		for (int i = 0; i < this.m_ExternalAnimatedObjects.Count; i++)
		{
			SpellStateAnimObject spellStateAnimObject = this.m_ExternalAnimatedObjects[i];
			spellStateAnimObject.Init();
		}
		for (int j = 0; j < this.m_AudioSources.Count; j++)
		{
			SpellStateAudioSource spellStateAudioSource = this.m_AudioSources[j];
			spellStateAudioSource.Init();
		}
		this.m_initialized = true;
		if (this.m_shown && this.m_playing)
		{
			this.PlayImpl();
		}
		else
		{
			this.StopImpl(null);
		}
	}

	// Token: 0x060032FB RID: 13051 RVA: 0x000FDB9B File Offset: 0x000FBD9B
	public void Play()
	{
		if (this.m_playing)
		{
			return;
		}
		if (!this.m_shown)
		{
			return;
		}
		this.m_playing = true;
		if (!this.m_initialized)
		{
			return;
		}
		this.PlayImpl();
	}

	// Token: 0x060032FC RID: 13052 RVA: 0x000FDBD0 File Offset: 0x000FBDD0
	public void Stop(List<SpellState> nextStateList)
	{
		if (!this.m_playing)
		{
			return;
		}
		this.m_playing = false;
		if (!this.m_initialized)
		{
			return;
		}
		this.StopImpl(nextStateList);
	}

	// Token: 0x060032FD RID: 13053 RVA: 0x000FDC03 File Offset: 0x000FBE03
	public void ShowState()
	{
		if (this.m_shown)
		{
			return;
		}
		this.m_shown = true;
		if (!this.m_initialized)
		{
			return;
		}
		if (!this.m_playing)
		{
			return;
		}
		this.PlayImpl();
	}

	// Token: 0x060032FE RID: 13054 RVA: 0x000FDC38 File Offset: 0x000FBE38
	public void HideState()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		if (!this.m_initialized)
		{
			return;
		}
		if (!this.m_playing)
		{
			return;
		}
		this.StopImpl(null);
	}

	// Token: 0x060032FF RID: 13055 RVA: 0x000FDC78 File Offset: 0x000FBE78
	public void OnLoad()
	{
		base.gameObject.SetActive(true);
		foreach (SpellStateAnimObject spellStateAnimObject in this.m_ExternalAnimatedObjects)
		{
			spellStateAnimObject.OnLoad(this);
		}
	}

	// Token: 0x06003300 RID: 13056 RVA: 0x000FDCE0 File Offset: 0x000FBEE0
	private void OnStateFinished()
	{
		this.m_spell.OnStateFinished();
	}

	// Token: 0x06003301 RID: 13057 RVA: 0x000FDCED File Offset: 0x000FBEED
	private void OnSpellFinished()
	{
		this.m_spell.OnSpellFinished();
	}

	// Token: 0x06003302 RID: 13058 RVA: 0x000FDCFA File Offset: 0x000FBEFA
	private void OnChangeState(SpellStateType stateType)
	{
		this.m_spell.ChangeState(stateType);
	}

	// Token: 0x06003303 RID: 13059 RVA: 0x000FDD08 File Offset: 0x000FBF08
	private IEnumerator DelayedPlay()
	{
		yield return new WaitForSeconds(this.m_StartDelaySec);
		this.PlayNow();
		yield break;
	}

	// Token: 0x06003304 RID: 13060 RVA: 0x000FDD24 File Offset: 0x000FBF24
	private void PlayImpl()
	{
		base.gameObject.SetActive(true);
		if (object.Equals(this.m_StartDelaySec, 0f))
		{
			this.PlayNow();
		}
		else
		{
			base.StartCoroutine(this.DelayedPlay());
		}
	}

	// Token: 0x06003305 RID: 13061 RVA: 0x000FDD74 File Offset: 0x000FBF74
	private void StopImpl(List<SpellState> nextStateList)
	{
		if (nextStateList == null)
		{
			foreach (SpellStateAnimObject spellStateAnimObject in this.m_ExternalAnimatedObjects)
			{
				spellStateAnimObject.Stop();
			}
		}
		else
		{
			foreach (SpellStateAnimObject spellStateAnimObject2 in this.m_ExternalAnimatedObjects)
			{
				spellStateAnimObject2.Stop(nextStateList);
			}
		}
		foreach (SpellStateAudioSource spellStateAudioSource in this.m_AudioSources)
		{
			spellStateAudioSource.Stop();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x000FDE7C File Offset: 0x000FC07C
	private void PlayNow()
	{
		foreach (SpellStateAnimObject spellStateAnimObject in this.m_ExternalAnimatedObjects)
		{
			spellStateAnimObject.Play();
		}
		foreach (SpellStateAudioSource spellStateAudioSource in this.m_AudioSources)
		{
			spellStateAudioSource.Play(this);
		}
	}

	// Token: 0x04001FF2 RID: 8178
	public SpellStateType m_StateType;

	// Token: 0x04001FF3 RID: 8179
	public float m_StartDelaySec;

	// Token: 0x04001FF4 RID: 8180
	public List<SpellStateAnimObject> m_ExternalAnimatedObjects;

	// Token: 0x04001FF5 RID: 8181
	public List<SpellStateAudioSource> m_AudioSources;

	// Token: 0x04001FF6 RID: 8182
	private Spell m_spell;

	// Token: 0x04001FF7 RID: 8183
	private bool m_playing;

	// Token: 0x04001FF8 RID: 8184
	private bool m_initialized;

	// Token: 0x04001FF9 RID: 8185
	private bool m_shown = true;
}
