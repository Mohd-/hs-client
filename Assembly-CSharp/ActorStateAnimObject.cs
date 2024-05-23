using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000ECC RID: 3788
[Serializable]
public class ActorStateAnimObject
{
	// Token: 0x060071AA RID: 29098 RVA: 0x00216AB4 File Offset: 0x00214CB4
	public void Init()
	{
		if (this.m_GameObject == null)
		{
			return;
		}
		if (this.m_AnimClip == null)
		{
			return;
		}
		string name = this.m_AnimClip.name;
		Animation animation;
		if (this.m_GameObject.GetComponent<Animation>() == null)
		{
			animation = this.m_GameObject.AddComponent<Animation>();
		}
		else
		{
			animation = this.m_GameObject.GetComponent<Animation>();
		}
		animation.playAutomatically = false;
		if (animation[name] == null)
		{
			animation.AddClip(this.m_AnimClip, name);
		}
		animation[name].layer = this.m_AnimLayer;
	}

	// Token: 0x060071AB RID: 29099 RVA: 0x00216B5C File Offset: 0x00214D5C
	public void Play()
	{
		if (!this.m_Enabled)
		{
			return;
		}
		if (this.m_GameObject == null)
		{
			return;
		}
		if (this.m_AnimClip != null)
		{
			string name = this.m_AnimClip.name;
			this.m_GameObject.GetComponent<Animation>()[name].enabled = true;
			if (object.Equals(this.m_CrossFadeSec, 0f))
			{
				if (!this.m_GameObject.GetComponent<Animation>().Play(name))
				{
					Debug.LogWarning(string.Format("ActorStateAnimObject.PlayNow() - FAILED to play clip {0} on {1}", name, this.m_GameObject));
				}
			}
			else
			{
				this.m_GameObject.GetComponent<Animation>().CrossFade(name, this.m_CrossFadeSec);
			}
		}
		if (this.m_ControlParticles && this.m_GameObject.GetComponent<ParticleEmitter>() != null)
		{
			this.m_GameObject.GetComponent<ParticleEmitter>().emit = this.m_EmitParticles;
		}
	}

	// Token: 0x060071AC RID: 29100 RVA: 0x00216C5C File Offset: 0x00214E5C
	public void Stop()
	{
		if (!this.m_Enabled)
		{
			return;
		}
		if (this.m_GameObject == null)
		{
			return;
		}
		if (this.m_AnimClip != null)
		{
			this.m_GameObject.GetComponent<Animation>()[this.m_AnimClip.name].time = 0f;
			this.m_GameObject.GetComponent<Animation>().Sample();
			this.m_GameObject.GetComponent<Animation>()[this.m_AnimClip.name].enabled = false;
		}
		if (this.m_ControlParticles && this.m_GameObject.GetComponent<ParticleEmitter>() != null)
		{
			this.m_GameObject.GetComponent<ParticleEmitter>().emit = this.m_EmitParticles;
		}
	}

	// Token: 0x060071AD RID: 29101 RVA: 0x00216D28 File Offset: 0x00214F28
	public void Stop(List<ActorState> nextStateList)
	{
		if (!this.m_Enabled)
		{
			return;
		}
		if (this.m_GameObject == null)
		{
			return;
		}
		if (this.m_AnimClip != null)
		{
			bool flag = false;
			int num = 0;
			while (!flag && num < nextStateList.Count)
			{
				ActorState actorState = nextStateList[num];
				for (int i = 0; i < actorState.m_ExternalAnimatedObjects.Count; i++)
				{
					ActorStateAnimObject actorStateAnimObject = actorState.m_ExternalAnimatedObjects[i];
					if (this.m_GameObject == actorStateAnimObject.m_GameObject && this.m_AnimLayer == actorStateAnimObject.m_AnimLayer)
					{
						flag = true;
						break;
					}
				}
				num++;
			}
			if (!flag)
			{
				this.m_GameObject.GetComponent<Animation>().Stop(this.m_AnimClip.name);
			}
		}
		if (this.m_ControlParticles && this.m_GameObject.GetComponent<ParticleEmitter>() != null)
		{
			this.m_GameObject.GetComponent<ParticleEmitter>().emit = this.m_EmitParticles;
		}
	}

	// Token: 0x04005BE2 RID: 23522
	public bool m_Enabled = true;

	// Token: 0x04005BE3 RID: 23523
	public GameObject m_GameObject;

	// Token: 0x04005BE4 RID: 23524
	public AnimationClip m_AnimClip;

	// Token: 0x04005BE5 RID: 23525
	public int m_AnimLayer;

	// Token: 0x04005BE6 RID: 23526
	public float m_CrossFadeSec;

	// Token: 0x04005BE7 RID: 23527
	public bool m_ControlParticles;

	// Token: 0x04005BE8 RID: 23528
	public bool m_EmitParticles;

	// Token: 0x04005BE9 RID: 23529
	public string m_Comment;

	// Token: 0x04005BEA RID: 23530
	private bool m_prevParticleEmitValue;
}
