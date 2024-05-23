using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000ECE RID: 3790
[Serializable]
public class SpellStateAnimObject
{
	// Token: 0x060071B5 RID: 29109 RVA: 0x00216EEC File Offset: 0x002150EC
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
		this.SetupAnimation();
	}

	// Token: 0x060071B6 RID: 29110 RVA: 0x00216F24 File Offset: 0x00215124
	private void SetupAnimation()
	{
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

	// Token: 0x060071B7 RID: 29111 RVA: 0x00216FA8 File Offset: 0x002151A8
	public void OnLoad(SpellState state)
	{
		if (this.m_Target == SpellStateAnimObject.Target.AS_SPECIFIED)
		{
			if (this.m_GameObject == null)
			{
				Debug.LogError("Error: spell state anim target has a null game object after load");
			}
			return;
		}
		if (this.m_Target == SpellStateAnimObject.Target.ACTOR)
		{
			Actor actor = SceneUtils.FindComponentInParents<Actor>(state.transform);
			if (actor == null || actor.gameObject == null)
			{
				Debug.LogError("Error: spell state anim target has a null game object after load");
				return;
			}
			this.m_GameObject = actor.gameObject;
			this.SetupAnimation();
		}
		else if (this.m_Target == SpellStateAnimObject.Target.ROOT_OBJECT)
		{
			Actor actor2 = SceneUtils.FindComponentInParents<Actor>(state.transform);
			if (actor2 == null || actor2.gameObject == null)
			{
				Debug.LogError("Error: spell state anim target has a null game object after load");
				return;
			}
			this.m_GameObject = actor2.GetRootObject();
			this.SetupAnimation();
		}
		else
		{
			Debug.LogWarning("Error: unimplemented spell anim target");
		}
	}

	// Token: 0x060071B8 RID: 29112 RVA: 0x00217094 File Offset: 0x00215294
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
			Animation component = this.m_GameObject.GetComponent<Animation>();
			string name = this.m_AnimClip.name;
			AnimationState animationState = component[name];
			animationState.enabled = true;
			animationState.speed = this.m_AnimSpeed;
			if (object.Equals(this.m_CrossFadeSec, 0f))
			{
				if (!component.Play(name))
				{
					Debug.LogWarning(string.Format("SpellStateAnimObject.PlayNow() - FAILED to play clip {0} on {1}", name, this.m_GameObject));
				}
			}
			else
			{
				component.CrossFade(name, this.m_CrossFadeSec);
			}
		}
		if (this.m_ControlParticles)
		{
			if (this.m_GameObject.GetComponent<ParticleEmitter>() != null)
			{
				this.m_prevParticleEmitValue = this.m_GameObject.GetComponent<ParticleEmitter>().emit;
				this.m_GameObject.GetComponent<ParticleEmitter>().emit = this.m_EmitParticles;
			}
			ParticleSystem component2 = this.m_GameObject.GetComponent<ParticleSystem>();
			if (component2 != null)
			{
				component2.Play();
			}
		}
	}

	// Token: 0x060071B9 RID: 29113 RVA: 0x002171C4 File Offset: 0x002153C4
	public void Stop()
	{
		if (this.m_GameObject == null)
		{
			return;
		}
		if (this.m_AnimClip != null)
		{
			this.m_GameObject.GetComponent<Animation>().Stop(this.m_AnimClip.name);
		}
		if (this.m_ControlParticles)
		{
			if (this.m_GameObject.GetComponent<ParticleEmitter>() != null)
			{
				this.m_GameObject.GetComponent<ParticleEmitter>().emit = this.m_prevParticleEmitValue;
			}
			ParticleSystem component = this.m_GameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Stop();
			}
		}
	}

	// Token: 0x060071BA RID: 29114 RVA: 0x00217264 File Offset: 0x00215464
	public void Stop(List<SpellState> nextStateList)
	{
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
				SpellState spellState = nextStateList[num];
				for (int i = 0; i < spellState.m_ExternalAnimatedObjects.Count; i++)
				{
					SpellStateAnimObject spellStateAnimObject = spellState.m_ExternalAnimatedObjects[i];
					if (spellStateAnimObject.m_Enabled)
					{
						if (this.m_GameObject == spellStateAnimObject.m_GameObject && this.m_AnimLayer == spellStateAnimObject.m_AnimLayer)
						{
							flag = true;
							break;
						}
					}
				}
				num++;
			}
			if (!flag)
			{
				this.m_GameObject.GetComponent<Animation>().Stop(this.m_AnimClip.name);
			}
		}
		if (this.m_ControlParticles)
		{
			if (this.m_GameObject.GetComponent<ParticleEmitter>() != null)
			{
				this.m_GameObject.GetComponent<ParticleEmitter>().emit = this.m_prevParticleEmitValue;
			}
			ParticleSystem component = this.m_GameObject.GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Stop();
			}
		}
	}

	// Token: 0x060071BB RID: 29115 RVA: 0x0021739F File Offset: 0x0021559F
	public void Show()
	{
		if (this.m_GameObject == null)
		{
			return;
		}
		this.m_GameObject.SetActive(true);
	}

	// Token: 0x060071BC RID: 29116 RVA: 0x002173BF File Offset: 0x002155BF
	public void Hide()
	{
		if (this.m_GameObject == null)
		{
			return;
		}
		this.m_GameObject.SetActive(false);
	}

	// Token: 0x04005BEE RID: 23534
	public GameObject m_GameObject;

	// Token: 0x04005BEF RID: 23535
	public SpellStateAnimObject.Target m_Target;

	// Token: 0x04005BF0 RID: 23536
	public AnimationClip m_AnimClip;

	// Token: 0x04005BF1 RID: 23537
	public int m_AnimLayer;

	// Token: 0x04005BF2 RID: 23538
	public float m_AnimSpeed = 1f;

	// Token: 0x04005BF3 RID: 23539
	public float m_CrossFadeSec;

	// Token: 0x04005BF4 RID: 23540
	public bool m_ControlParticles;

	// Token: 0x04005BF5 RID: 23541
	public bool m_EmitParticles;

	// Token: 0x04005BF6 RID: 23542
	public string m_Comment;

	// Token: 0x04005BF7 RID: 23543
	public bool m_Enabled = true;

	// Token: 0x04005BF8 RID: 23544
	private bool m_prevParticleEmitValue;

	// Token: 0x02000ED8 RID: 3800
	public enum Target
	{
		// Token: 0x04005C33 RID: 23603
		AS_SPECIFIED,
		// Token: 0x04005C34 RID: 23604
		ACTOR,
		// Token: 0x04005C35 RID: 23605
		ROOT_OBJECT
	}
}
