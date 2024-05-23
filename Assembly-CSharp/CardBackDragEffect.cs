using System;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class CardBackDragEffect : MonoBehaviour
{
	// Token: 0x06002B42 RID: 11074 RVA: 0x000D723B File Offset: 0x000D543B
	private void Awake()
	{
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x000D7240 File Offset: 0x000D5440
	private void Start()
	{
		if (!SceneMgr.Get() || SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			base.enabled = false;
			return;
		}
		this.m_LastPosition = base.transform.position;
		if (this.m_CardBackManager == null)
		{
			this.m_CardBackManager = CardBackManager.Get();
			if (this.m_CardBackManager == null)
			{
				Debug.LogError("Failed to get CardBackManager!");
				base.enabled = false;
			}
		}
		this.SetEffect();
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x000D72C9 File Offset: 0x000D54C9
	private void FixedUpdate()
	{
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x000D72CC File Offset: 0x000D54CC
	private void Update()
	{
		if (this.m_EffectsRoot != null)
		{
			if (!base.GetComponent<Renderer>().enabled)
			{
				this.ShowParticles(false);
				if (this.m_EffectsRoot.activeSelf)
				{
					this.m_EffectsRoot.SetActive(false);
				}
			}
			else
			{
				this.m_Speed = (base.transform.position - this.m_LastPosition).magnitude / Time.deltaTime * 3.6f;
				this.UpdateDragEffect();
				this.m_LastPosition = base.transform.position;
			}
		}
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x000D7369 File Offset: 0x000D5569
	private void OnDisable()
	{
		if (this.m_EffectsRoot != null)
		{
			this.ShowParticles(false);
		}
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x000D7383 File Offset: 0x000D5583
	private void OnDestroy()
	{
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000D7385 File Offset: 0x000D5585
	private void OnEnable()
	{
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x000D7388 File Offset: 0x000D5588
	public void SetEffect()
	{
		if (this.m_CardBackManager == null)
		{
			this.m_CardBackManager = CardBackManager.Get();
			if (this.m_CardBackManager == null)
			{
				Debug.LogError("Failed to get CardBackManager!");
				base.enabled = false;
				return;
			}
		}
		bool friendlySide = true;
		Entity entity = this.m_Actor.GetEntity();
		if (entity != null)
		{
			Player controller = entity.GetController();
			if (controller != null && controller.GetSide() == Player.Side.OPPOSING)
			{
				friendlySide = false;
			}
		}
		this.m_CardBackManager.UpdateDragEffect(base.gameObject, friendlySide);
		CardBack cardBack = this.m_CardBackManager.GetCardBack(this.m_Actor);
		if (cardBack != null)
		{
			this.m_Min = cardBack.m_EffectMinVelocity;
			this.m_Max = cardBack.m_EffectMaxVelocity;
		}
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x000D744C File Offset: 0x000D564C
	private void UpdateDragEffect()
	{
		if (this.m_Speed > this.m_Min && this.m_Speed < this.m_Max)
		{
			if (this.m_Active)
			{
				return;
			}
			this.m_Active = true;
			this.ShowParticles(true);
		}
		else
		{
			if (!this.m_Active)
			{
				return;
			}
			this.m_Active = false;
			this.ShowParticles(false);
		}
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x000D74B4 File Offset: 0x000D56B4
	private void ShowParticles(bool show)
	{
		if (show)
		{
			foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
			{
				if (!(particleSystem == null))
				{
					particleSystem.Play();
				}
			}
		}
		else
		{
			foreach (ParticleSystem particleSystem2 in base.GetComponentsInChildren<ParticleSystem>())
			{
				if (particleSystem2 == null)
				{
					return;
				}
				particleSystem2.Stop();
			}
		}
	}

	// Token: 0x04001A28 RID: 6696
	private const float MIN_VELOCITY = 2f;

	// Token: 0x04001A29 RID: 6697
	private const float MAX_VELOCITY = 30f;

	// Token: 0x04001A2A RID: 6698
	public Actor m_Actor;

	// Token: 0x04001A2B RID: 6699
	public GameObject m_EffectsRoot;

	// Token: 0x04001A2C RID: 6700
	private CardBackManager m_CardBackManager;

	// Token: 0x04001A2D RID: 6701
	private Vector3 m_LastPosition;

	// Token: 0x04001A2E RID: 6702
	private float m_Speed;

	// Token: 0x04001A2F RID: 6703
	private bool m_Active;

	// Token: 0x04001A30 RID: 6704
	private float m_Min = 2f;

	// Token: 0x04001A31 RID: 6705
	private float m_Max = 30f;
}
