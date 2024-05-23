using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000ED6 RID: 3798
public class SpellMoveToTargetAuto : Spell
{
	// Token: 0x060071E9 RID: 29161 RVA: 0x00217F38 File Offset: 0x00216138
	public override void SetSource(GameObject go)
	{
		GameObject source = base.GetSource();
		if (source != go)
		{
			this.m_sourceComputed = false;
		}
		base.SetSource(go);
	}

	// Token: 0x060071EA RID: 29162 RVA: 0x00217F66 File Offset: 0x00216166
	public override void RemoveSource()
	{
		base.RemoveSource();
		this.m_sourceComputed = false;
	}

	// Token: 0x060071EB RID: 29163 RVA: 0x00217F78 File Offset: 0x00216178
	public override void AddTarget(GameObject go)
	{
		GameObject target = base.GetTarget();
		if (target != go)
		{
			this.m_targetComputed = false;
		}
		base.AddTarget(go);
	}

	// Token: 0x060071EC RID: 29164 RVA: 0x00217FA8 File Offset: 0x002161A8
	public override bool RemoveTarget(GameObject go)
	{
		GameObject target = base.GetTarget();
		if (!base.RemoveTarget(go))
		{
			return false;
		}
		if (target == go)
		{
			this.m_targetComputed = false;
		}
		return true;
	}

	// Token: 0x060071ED RID: 29165 RVA: 0x00217FE0 File Offset: 0x002161E0
	public override void RemoveAllTargets()
	{
		bool flag = this.m_targets.Count > 0;
		base.RemoveAllTargets();
		if (flag)
		{
			this.m_targetComputed = false;
		}
	}

	// Token: 0x060071EE RID: 29166 RVA: 0x0021800F File Offset: 0x0021620F
	public override bool AddPowerTargets()
	{
		return base.CanAddPowerTargets() && base.AddSinglePowerTarget();
	}

	// Token: 0x060071EF RID: 29167 RVA: 0x00218024 File Offset: 0x00216224
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.OnBirth(prevStateType);
		this.ResetPath();
		this.m_waitingToAct = true;
		Card sourceCard = base.GetSourceCard();
		if (sourceCard == null)
		{
			Debug.LogError(string.Format("{0}.OnBirth() - sourceCard is null", this));
			base.OnBirth(prevStateType);
			return;
		}
		Player controller = sourceCard.GetEntity().GetController();
		if (!this.DeterminePath(controller, sourceCard, null))
		{
			Debug.LogError(string.Format("{0}.OnBirth() - no paths available", this));
			base.OnBirth(prevStateType);
			return;
		}
	}

	// Token: 0x060071F0 RID: 29168 RVA: 0x002180A4 File Offset: 0x002162A4
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		if (this.m_pathNodes == null)
		{
			this.ResetPath();
		}
		Card sourceCard = base.GetSourceCard();
		if (sourceCard == null)
		{
			Debug.LogError(string.Format("SpellMoveToTarget.OnAction() - no source card", new object[0]));
			this.DoActionFallback(prevStateType);
			return;
		}
		Card targetCard = base.GetTargetCard();
		if (targetCard == null)
		{
			Debug.LogError(string.Format("SpellMoveToTarget.OnAction() - no target card", new object[0]));
			this.DoActionFallback(prevStateType);
			return;
		}
		Player controller = sourceCard.GetEntity().GetController();
		if (!this.DeterminePath(controller, sourceCard, targetCard))
		{
			Debug.LogError(string.Format("SpellMoveToTarget.DoAction() - no paths available, going to DEATH state", new object[0]));
			this.DoActionFallback(prevStateType);
			return;
		}
		base.StartCoroutine(this.WaitThenDoAction(prevStateType));
	}

	// Token: 0x060071F1 RID: 29169 RVA: 0x00218170 File Offset: 0x00216370
	protected IEnumerator WaitThenDoAction(SpellStateType prevStateType)
	{
		while (this.m_waitingToAct)
		{
			yield return null;
		}
		Hashtable argTable = iTween.Hash(new object[]
		{
			"path",
			this.m_pathNodes,
			"time",
			this.m_MovementDurationSec,
			"easetype",
			this.m_EaseType,
			"oncomplete",
			"OnMoveToTargetComplete",
			"oncompletetarget",
			base.gameObject,
			"orienttopath",
			this.m_OrientToPath
		});
		GameObject go = (!this.m_OnlyMoveContainer) ? base.gameObject : this.m_ObjectContainer;
		iTween.MoveTo(go, argTable);
		yield break;
	}

	// Token: 0x060071F2 RID: 29170 RVA: 0x0021818B File Offset: 0x0021638B
	private void OnMoveToTargetComplete()
	{
		if (this.m_DisableContainerAfterAction)
		{
			base.ActivateObjectContainer(false);
		}
		this.ChangeState(SpellStateType.DEATH);
	}

	// Token: 0x060071F3 RID: 29171 RVA: 0x002181A6 File Offset: 0x002163A6
	private void StopWaitingToAct()
	{
		this.m_waitingToAct = false;
	}

	// Token: 0x060071F4 RID: 29172 RVA: 0x002181B0 File Offset: 0x002163B0
	private void ResetPath()
	{
		this.m_pathNodes = new Vector3[]
		{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};
		this.m_sourceComputed = false;
		this.m_targetComputed = false;
	}

	// Token: 0x060071F5 RID: 29173 RVA: 0x0021820A File Offset: 0x0021640A
	private void DoActionFallback(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		this.ChangeState(SpellStateType.DEATH);
	}

	// Token: 0x060071F6 RID: 29174 RVA: 0x0021821C File Offset: 0x0021641C
	private void SetStartPosition()
	{
		base.transform.position = this.m_pathNodes[0];
		if (this.m_OnlyMoveContainer)
		{
			this.m_ObjectContainer.transform.position = base.transform.position;
		}
	}

	// Token: 0x060071F7 RID: 29175 RVA: 0x0021826B File Offset: 0x0021646B
	private bool DeterminePath(Player sourcePlayer, Card sourceCard, Card targetCard)
	{
		this.FixupPathNodes(sourcePlayer, sourceCard, targetCard);
		this.SetStartPosition();
		return true;
	}

	// Token: 0x060071F8 RID: 29176 RVA: 0x00218280 File Offset: 0x00216480
	private void FixupPathNodes(Player sourcePlayer, Card sourceCard, Card targetCard)
	{
		if (!this.m_sourceComputed)
		{
			this.m_pathNodes[0] = base.transform.position;
			this.m_sourceComputed = true;
		}
		if (!this.m_targetComputed && targetCard != null)
		{
			this.m_pathNodes[this.m_pathNodes.Length - 1] = targetCard.transform.position;
			float num = targetCard.transform.position.x - base.transform.position.x;
			float num2 = num / Mathf.Abs(num);
			for (int i = 1; i < this.m_pathNodes.Length - 1; i++)
			{
				Vector3 vector = this.m_pathNodes[i];
				float num3 = vector.x - base.transform.position.x;
				float num4 = num3 / Mathf.Sqrt(num3 * num3);
				if (object.Equals(num2, num4))
				{
					this.m_pathNodes[i].x = base.transform.position.x - num3;
				}
			}
			this.m_targetComputed = true;
		}
		this.MoveCenterPoint();
	}

	// Token: 0x060071F9 RID: 29177 RVA: 0x002183D0 File Offset: 0x002165D0
	private void MoveCenterPoint()
	{
		if (this.m_pathNodes.Length < 3)
		{
			return;
		}
		Vector3 vector = this.m_pathNodes[0];
		Vector3 vector2 = this.m_pathNodes[this.m_pathNodes.Length - 1];
		float num = Vector3.Distance(vector, vector2);
		Vector3 vector3 = vector2 - vector;
		vector3 /= num;
		Vector3 vector4 = vector + vector3 * (num * (this.CenterOffsetPercent * 0.01f));
		float num2 = num / this.DistanceScaleFactor;
		if (this.CenterPointHeightMin == this.CenterPointHeightMax)
		{
			ref Vector3 ptr = ref vector4;
			int num4;
			int num3 = num4 = 1;
			float num5 = ptr[num4];
			vector4[num3] = num5 + this.CenterPointHeightMax * num2;
		}
		else
		{
			ref Vector3 ptr2 = ref vector4;
			int num4;
			int num6 = num4 = 1;
			float num5 = ptr2[num4];
			vector4[num6] = num5 + Random.Range(this.CenterPointHeightMin * num2, this.CenterPointHeightMax * num2);
		}
		float num7 = 1f;
		if (vector[2] > vector2[2])
		{
			num7 = -1f;
		}
		bool flag = false;
		if (Random.value > 0.5f)
		{
			flag = true;
		}
		if (this.RightMin == 0f && this.RightMax == 0f)
		{
			flag = false;
		}
		if (this.LeftMin == 0f && this.LeftMax == 0f)
		{
			flag = true;
		}
		if (flag)
		{
			if (this.RightMin == this.RightMax || this.DebugForceMax)
			{
				ref Vector3 ptr3 = ref vector4;
				int num4;
				int num8 = num4 = 0;
				float num5 = ptr3[num4];
				vector4[num8] = num5 + this.RightMax * num2 * num7;
			}
			else
			{
				ref Vector3 ptr4 = ref vector4;
				int num4;
				int num9 = num4 = 0;
				float num5 = ptr4[num4];
				vector4[num9] = num5 + Random.Range(this.RightMin * num2, this.RightMax * num2) * num7;
			}
		}
		else if (this.LeftMin == this.LeftMax || this.DebugForceMax)
		{
			ref Vector3 ptr5 = ref vector4;
			int num4;
			int num10 = num4 = 0;
			float num5 = ptr5[num4];
			vector4[num10] = num5 - this.LeftMax * num2 * num7;
		}
		else
		{
			ref Vector3 ptr6 = ref vector4;
			int num4;
			int num11 = num4 = 0;
			float num5 = ptr6[num4];
			vector4[num11] = num5 - Random.Range(this.LeftMin * num2, this.LeftMax * num2) * num7;
		}
		this.m_pathNodes[1] = vector4;
	}

	// Token: 0x04005C1B RID: 23579
	public float m_MovementDurationSec = 0.5f;

	// Token: 0x04005C1C RID: 23580
	public iTween.EaseType m_EaseType = iTween.EaseType.linear;

	// Token: 0x04005C1D RID: 23581
	public bool m_DisableContainerAfterAction;

	// Token: 0x04005C1E RID: 23582
	public bool m_OnlyMoveContainer;

	// Token: 0x04005C1F RID: 23583
	public bool m_OrientToPath;

	// Token: 0x04005C20 RID: 23584
	public float CenterOffsetPercent = 50f;

	// Token: 0x04005C21 RID: 23585
	public float CenterPointHeightMin;

	// Token: 0x04005C22 RID: 23586
	public float CenterPointHeightMax;

	// Token: 0x04005C23 RID: 23587
	public float RightMin;

	// Token: 0x04005C24 RID: 23588
	public float RightMax;

	// Token: 0x04005C25 RID: 23589
	public float LeftMin;

	// Token: 0x04005C26 RID: 23590
	public float LeftMax;

	// Token: 0x04005C27 RID: 23591
	public bool DebugForceMax;

	// Token: 0x04005C28 RID: 23592
	public float DistanceScaleFactor = 8f;

	// Token: 0x04005C29 RID: 23593
	private bool m_waitingToAct = true;

	// Token: 0x04005C2A RID: 23594
	private Vector3[] m_pathNodes;

	// Token: 0x04005C2B RID: 23595
	private bool m_sourceComputed;

	// Token: 0x04005C2C RID: 23596
	private bool m_targetComputed;
}
