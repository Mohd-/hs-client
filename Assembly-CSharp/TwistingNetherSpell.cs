using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000EA3 RID: 3747
public class TwistingNetherSpell : SuperSpell
{
	// Token: 0x06007102 RID: 28930 RVA: 0x00214D68 File Offset: 0x00212F68
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.TAG_CHANGE)
		{
			return null;
		}
		Network.HistTagChange histTagChange = power as Network.HistTagChange;
		if (Gameplay.Get() != null)
		{
			if (histTagChange.Tag != 360)
			{
				return null;
			}
			if (histTagChange.Value != 1)
			{
				return null;
			}
		}
		Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
		if (entity == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, histTagChange.Entity);
			Debug.LogWarning(text);
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x06007103 RID: 28931 RVA: 0x00214E00 File Offset: 0x00213000
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		if (base.IsFinished())
		{
			return;
		}
		this.Begin();
		if (this.CanFinish())
		{
			this.m_effectsPendingFinish--;
			base.FinishIfPossible();
		}
	}

	// Token: 0x06007104 RID: 28932 RVA: 0x00214E45 File Offset: 0x00213045
	protected override void CleanUp()
	{
		base.CleanUp();
		this.m_victims.Clear();
	}

	// Token: 0x06007105 RID: 28933 RVA: 0x00214E58 File Offset: 0x00213058
	private void Begin()
	{
		this.m_effectsPendingFinish++;
		Action<object> action = delegate(object amount)
		{
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			1f,
			"time",
			this.m_FinishTime,
			"onupdate",
			action,
			"oncomplete",
			"OnFinishTimeFinished",
			"oncompletetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args);
		this.Setup();
		this.Lift();
	}

	// Token: 0x06007106 RID: 28934 RVA: 0x00214F28 File Offset: 0x00213128
	private void Setup()
	{
		List<GameObject> targets = base.GetTargets();
		foreach (GameObject gameObject in targets)
		{
			Card component = gameObject.GetComponent<Card>();
			component.SetDoNotSort(true);
			TwistingNetherSpell.Victim victim = new TwistingNetherSpell.Victim();
			victim.m_state = TwistingNetherSpell.VictimState.NONE;
			victim.m_card = component;
			this.m_victims.Add(victim);
		}
	}

	// Token: 0x06007107 RID: 28935 RVA: 0x00214FB0 File Offset: 0x002131B0
	private void Lift()
	{
		foreach (TwistingNetherSpell.Victim victim in this.m_victims)
		{
			victim.m_state = TwistingNetherSpell.VictimState.LIFTING;
			Vector3 vector = TransformUtil.RandomVector3(this.m_LiftInfo.m_OffsetMin, this.m_LiftInfo.m_OffsetMax);
			Vector3 vector2 = victim.m_card.transform.position + vector;
			float num = Random.Range(this.m_LiftInfo.m_DelayMin, this.m_LiftInfo.m_DelayMax);
			float num2 = Random.Range(this.m_LiftInfo.m_DurationMin, this.m_LiftInfo.m_DurationMax);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				vector2,
				"delay",
				num,
				"time",
				num2,
				"easeType",
				this.m_LiftInfo.m_EaseType,
				"oncomplete",
				"OnLiftFinished",
				"oncompletetarget",
				base.gameObject,
				"oncompleteparams",
				victim
			});
			Vector3 vector3;
			vector3..ctor(Random.Range(this.m_LiftInfo.m_RotationMin, this.m_LiftInfo.m_RotationMax), Random.Range(this.m_LiftInfo.m_RotationMin, this.m_LiftInfo.m_RotationMax), Random.Range(this.m_LiftInfo.m_RotationMin, this.m_LiftInfo.m_RotationMax));
			float num3 = Random.Range(this.m_LiftInfo.m_RotDelayMin, this.m_LiftInfo.m_RotDelayMax);
			float num4 = Random.Range(this.m_LiftInfo.m_RotDurationMin, this.m_LiftInfo.m_RotDurationMax);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"rotation",
				vector3,
				"delay",
				num3,
				"time",
				num4,
				"easeType",
				this.m_LiftInfo.m_EaseType
			});
			iTween.MoveTo(victim.m_card.gameObject, args);
			iTween.RotateTo(victim.m_card.gameObject, args2);
		}
	}

	// Token: 0x06007108 RID: 28936 RVA: 0x00215228 File Offset: 0x00213428
	private void OnLiftFinished(TwistingNetherSpell.Victim victim)
	{
		this.Float(victim);
	}

	// Token: 0x06007109 RID: 28937 RVA: 0x00215234 File Offset: 0x00213434
	private void Float(TwistingNetherSpell.Victim victim)
	{
		victim.m_state = TwistingNetherSpell.VictimState.FLOATING;
		float num = Random.Range(this.m_FloatInfo.m_DurationMin, this.m_FloatInfo.m_DurationMax);
		Action<object> action = delegate(object amount)
		{
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			1f,
			"time",
			num,
			"onupdate",
			action,
			"oncomplete",
			"OnFloatFinished",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			victim
		});
		iTween.ValueTo(victim.m_card.gameObject, args);
	}

	// Token: 0x0600710A RID: 28938 RVA: 0x00215319 File Offset: 0x00213519
	private void OnFloatFinished(TwistingNetherSpell.Victim victim)
	{
		this.Drain(victim);
	}

	// Token: 0x0600710B RID: 28939 RVA: 0x00215324 File Offset: 0x00213524
	private void Drain(TwistingNetherSpell.Victim victim)
	{
		victim.m_state = TwistingNetherSpell.VictimState.LIFTING;
		float num = Random.Range(this.m_DrainInfo.m_DelayMin, this.m_DrainInfo.m_DelayMax);
		float num2 = Random.Range(this.m_DrainInfo.m_DurationMin, this.m_DrainInfo.m_DurationMax);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			base.transform.position,
			"delay",
			num,
			"time",
			num2,
			"easeType",
			this.m_DrainInfo.m_EaseType,
			"oncomplete",
			"OnDrainFinished",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			victim
		});
		iTween.MoveTo(victim.m_card.gameObject, args);
		float num3 = Random.Range(this.m_SqueezeInfo.m_DelayMin, this.m_SqueezeInfo.m_DelayMax);
		float num4 = Random.Range(this.m_SqueezeInfo.m_DurationMin, this.m_SqueezeInfo.m_DurationMax);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"scale",
			TwistingNetherSpell.DEAD_SCALE,
			"delay",
			num3,
			"time",
			num4,
			"easeType",
			this.m_SqueezeInfo.m_EaseType
		});
		iTween.ScaleTo(victim.m_card.gameObject, args2);
	}

	// Token: 0x0600710C RID: 28940 RVA: 0x002154C3 File Offset: 0x002136C3
	private void OnDrainFinished(TwistingNetherSpell.Victim victim)
	{
		this.CleanUpVictim(victim);
	}

	// Token: 0x0600710D RID: 28941 RVA: 0x002154CC File Offset: 0x002136CC
	private void OnFinishTimeFinished()
	{
		foreach (TwistingNetherSpell.Victim victim in this.m_victims)
		{
			this.CleanUpVictim(victim);
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x0600710E RID: 28942 RVA: 0x0021553C File Offset: 0x0021373C
	private void CleanUpVictim(TwistingNetherSpell.Victim victim)
	{
		if (victim.m_state != TwistingNetherSpell.VictimState.DEAD)
		{
			victim.m_state = TwistingNetherSpell.VictimState.DEAD;
			victim.m_card.SetDoNotSort(false);
		}
	}

	// Token: 0x0600710F RID: 28943 RVA: 0x00215560 File Offset: 0x00213760
	private bool CanFinish()
	{
		foreach (TwistingNetherSpell.Victim victim in this.m_victims)
		{
			if (victim.m_state != TwistingNetherSpell.VictimState.DEAD)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x04005A98 RID: 23192
	public float m_FinishTime;

	// Token: 0x04005A99 RID: 23193
	public TwistingNetherLiftInfo m_LiftInfo;

	// Token: 0x04005A9A RID: 23194
	public TwistingNetherFloatInfo m_FloatInfo;

	// Token: 0x04005A9B RID: 23195
	public TwistingNetherDrainInfo m_DrainInfo;

	// Token: 0x04005A9C RID: 23196
	public TwistingNetherSqueezeInfo m_SqueezeInfo;

	// Token: 0x04005A9D RID: 23197
	private static readonly Vector3 DEAD_SCALE = new Vector3(0.01f, 0.01f, 0.01f);

	// Token: 0x04005A9E RID: 23198
	private List<TwistingNetherSpell.Victim> m_victims = new List<TwistingNetherSpell.Victim>();

	// Token: 0x02000EA4 RID: 3748
	private enum VictimState
	{
		// Token: 0x04005AA2 RID: 23202
		NONE,
		// Token: 0x04005AA3 RID: 23203
		LIFTING,
		// Token: 0x04005AA4 RID: 23204
		FLOATING,
		// Token: 0x04005AA5 RID: 23205
		DRAINING,
		// Token: 0x04005AA6 RID: 23206
		DEAD
	}

	// Token: 0x02000EA5 RID: 3749
	private class Victim
	{
		// Token: 0x04005AA7 RID: 23207
		public TwistingNetherSpell.VictimState m_state;

		// Token: 0x04005AA8 RID: 23208
		public Card m_card;
	}
}
