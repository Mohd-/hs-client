using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000714 RID: 1812
public class ActorState : MonoBehaviour
{
	// Token: 0x06004A1A RID: 18970 RVA: 0x00162424 File Offset: 0x00160624
	private void Start()
	{
		this.m_stateMgr = SceneUtils.FindComponentInParents<ActorStateMgr>(base.gameObject);
		foreach (ActorStateAnimObject actorStateAnimObject in this.m_ExternalAnimatedObjects)
		{
			actorStateAnimObject.Init();
		}
		this.m_initialized = true;
		if (this.m_playing)
		{
			base.gameObject.SetActive(true);
			this.PlayNow();
		}
	}

	// Token: 0x06004A1B RID: 18971 RVA: 0x001624B4 File Offset: 0x001606B4
	public void Play()
	{
		if (this.m_playing)
		{
			return;
		}
		this.m_playing = true;
		if (!this.m_initialized)
		{
			return;
		}
		base.gameObject.SetActive(true);
		this.PlayNow();
	}

	// Token: 0x06004A1C RID: 18972 RVA: 0x001624E8 File Offset: 0x001606E8
	public void Stop(List<ActorState> nextStateList)
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
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().Stop();
		}
		if (base.GetComponent<ParticleEmitter>() != null)
		{
			base.GetComponent<ParticleEmitter>().emit = false;
		}
		if (nextStateList == null)
		{
			foreach (ActorStateAnimObject actorStateAnimObject in this.m_ExternalAnimatedObjects)
			{
				actorStateAnimObject.Stop();
			}
		}
		else
		{
			foreach (ActorStateAnimObject actorStateAnimObject2 in this.m_ExternalAnimatedObjects)
			{
				actorStateAnimObject2.Stop(nextStateList);
			}
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004A1D RID: 18973 RVA: 0x001625FC File Offset: 0x001607FC
	public float GetAnimationDuration()
	{
		float num = 0f;
		for (int i = 0; i < this.m_ExternalAnimatedObjects.Count; i++)
		{
			if (this.m_ExternalAnimatedObjects[i].m_GameObject != null)
			{
				num = Mathf.Max(this.m_ExternalAnimatedObjects[i].m_AnimClip.length, num);
			}
		}
		return num;
	}

	// Token: 0x06004A1E RID: 18974 RVA: 0x00162665 File Offset: 0x00160865
	public void ShowState()
	{
		base.gameObject.SetActive(true);
		this.Play();
	}

	// Token: 0x06004A1F RID: 18975 RVA: 0x00162679 File Offset: 0x00160879
	public void HideState()
	{
		this.Stop(null);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004A20 RID: 18976 RVA: 0x0016268E File Offset: 0x0016088E
	private void OnChangeState(ActorStateType stateType)
	{
		this.m_stateMgr.ChangeState(stateType);
	}

	// Token: 0x06004A21 RID: 18977 RVA: 0x001626A0 File Offset: 0x001608A0
	private void PlayNow()
	{
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().Play();
		}
		if (base.GetComponent<ParticleEmitter>() != null)
		{
			base.GetComponent<ParticleEmitter>().emit = true;
		}
		foreach (ActorStateAnimObject actorStateAnimObject in this.m_ExternalAnimatedObjects)
		{
			actorStateAnimObject.Play();
		}
	}

	// Token: 0x04003126 RID: 12582
	public ActorStateType m_StateType;

	// Token: 0x04003127 RID: 12583
	public List<ActorStateAnimObject> m_ExternalAnimatedObjects;

	// Token: 0x04003128 RID: 12584
	private ActorStateMgr m_stateMgr;

	// Token: 0x04003129 RID: 12585
	private bool m_playing;

	// Token: 0x0400312A RID: 12586
	private bool m_initialized;
}
