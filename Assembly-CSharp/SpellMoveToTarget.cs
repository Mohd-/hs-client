using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000ED1 RID: 3793
public class SpellMoveToTarget : Spell
{
	// Token: 0x060071C9 RID: 29129 RVA: 0x002176BC File Offset: 0x002158BC
	public override void SetSource(GameObject go)
	{
		GameObject source = base.GetSource();
		if (source != go)
		{
			this.m_sourceComputed = false;
		}
		base.SetSource(go);
	}

	// Token: 0x060071CA RID: 29130 RVA: 0x002176EA File Offset: 0x002158EA
	public override void RemoveSource()
	{
		base.RemoveSource();
		this.m_sourceComputed = false;
	}

	// Token: 0x060071CB RID: 29131 RVA: 0x002176FC File Offset: 0x002158FC
	public override void AddTarget(GameObject go)
	{
		GameObject target = base.GetTarget();
		if (target != go)
		{
			this.m_targetComputed = false;
		}
		base.AddTarget(go);
	}

	// Token: 0x060071CC RID: 29132 RVA: 0x0021772C File Offset: 0x0021592C
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

	// Token: 0x060071CD RID: 29133 RVA: 0x00217764 File Offset: 0x00215964
	public override void RemoveAllTargets()
	{
		bool flag = this.m_targets.Count > 0;
		base.RemoveAllTargets();
		if (flag)
		{
			this.m_targetComputed = false;
		}
	}

	// Token: 0x060071CE RID: 29134 RVA: 0x00217793 File Offset: 0x00215993
	public override bool AddPowerTargets()
	{
		return base.CanAddPowerTargets() && base.AddSinglePowerTarget();
	}

	// Token: 0x060071CF RID: 29135 RVA: 0x002177A8 File Offset: 0x002159A8
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

	// Token: 0x060071D0 RID: 29136 RVA: 0x00217828 File Offset: 0x00215A28
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
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

	// Token: 0x060071D1 RID: 29137 RVA: 0x002178E4 File Offset: 0x00215AE4
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

	// Token: 0x060071D2 RID: 29138 RVA: 0x002178FF File Offset: 0x00215AFF
	private void OnMoveToTargetComplete()
	{
		if (this.m_DisableContainerAfterAction)
		{
			base.ActivateObjectContainer(false);
		}
		this.ChangeState(SpellStateType.DEATH);
	}

	// Token: 0x060071D3 RID: 29139 RVA: 0x0021791A File Offset: 0x00215B1A
	private void StopWaitingToAct()
	{
		this.m_waitingToAct = false;
	}

	// Token: 0x060071D4 RID: 29140 RVA: 0x00217923 File Offset: 0x00215B23
	private void ResetPath()
	{
		this.m_spellPath = null;
		this.m_pathNodes = null;
		this.m_sourceComputed = false;
		this.m_targetComputed = false;
	}

	// Token: 0x060071D5 RID: 29141 RVA: 0x00217941 File Offset: 0x00215B41
	private void DoActionFallback(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		this.ChangeState(SpellStateType.DEATH);
	}

	// Token: 0x060071D6 RID: 29142 RVA: 0x00217954 File Offset: 0x00215B54
	private void SetStartPosition()
	{
		base.transform.position = this.m_pathNodes[0];
		if (this.m_OnlyMoveContainer)
		{
			this.m_ObjectContainer.transform.position = base.transform.position;
		}
	}

	// Token: 0x060071D7 RID: 29143 RVA: 0x002179A4 File Offset: 0x00215BA4
	private bool DeterminePath(Player sourcePlayer, Card sourceCard, Card targetCard)
	{
		if (this.m_pathNodes == null)
		{
			if (this.m_Paths == null || this.m_Paths.Count == 0)
			{
				Debug.LogError(string.Format("SpellMoveToTarget.DeterminePath() - no SpellPaths available", new object[0]));
				return false;
			}
			iTweenPath[] components = base.GetComponents<iTweenPath>();
			if (components == null || components.Length == 0)
			{
				Debug.LogError(string.Format("SpellMoveToTarget.DeterminePath() - no iTweenPaths available", new object[0]));
				return false;
			}
			iTweenPath iTweenPath;
			SpellPath spellPath;
			if (!this.FindBestPath(sourcePlayer, sourceCard, components, out iTweenPath, out spellPath) && !this.FindFallbackPath(components, out iTweenPath, out spellPath))
			{
				return false;
			}
			this.m_spellPath = spellPath;
			this.m_pathNodes = iTweenPath.nodes.ToArray();
		}
		this.FixupPathNodes(sourcePlayer, sourceCard, targetCard);
		this.SetStartPosition();
		return true;
	}

	// Token: 0x060071D8 RID: 29144 RVA: 0x00217A6C File Offset: 0x00215C6C
	private bool FindBestPath(Player sourcePlayer, Card sourceCard, iTweenPath[] pathComponents, out iTweenPath tweenPath, out SpellPath spellPath)
	{
		tweenPath = null;
		spellPath = null;
		if (sourcePlayer == null)
		{
			return false;
		}
		if (sourcePlayer.GetSide() == Player.Side.FRIENDLY)
		{
			Predicate<SpellPath> match = (SpellPath currSpellPath) => currSpellPath.m_Type == SpellPathType.FRIENDLY;
			return this.FindPath(pathComponents, out tweenPath, out spellPath, match);
		}
		if (sourcePlayer.GetSide() == Player.Side.OPPOSING)
		{
			Predicate<SpellPath> match2 = (SpellPath currSpellPath) => currSpellPath.m_Type == SpellPathType.OPPOSING;
			return this.FindPath(pathComponents, out tweenPath, out spellPath, match2);
		}
		return false;
	}

	// Token: 0x060071D9 RID: 29145 RVA: 0x00217AF8 File Offset: 0x00215CF8
	private bool FindFallbackPath(iTweenPath[] pathComponents, out iTweenPath tweenPath, out SpellPath spellPath)
	{
		Predicate<SpellPath> match = (SpellPath currSpellPath) => currSpellPath != null;
		return this.FindPath(pathComponents, out tweenPath, out spellPath, match);
	}

	// Token: 0x060071DA RID: 29146 RVA: 0x00217B30 File Offset: 0x00215D30
	private bool FindPath(iTweenPath[] pathComponents, out iTweenPath tweenPath, out SpellPath spellPath, Predicate<SpellPath> match)
	{
		tweenPath = null;
		spellPath = null;
		SpellPath spellPath2 = this.m_Paths.Find(match);
		if (spellPath2 == null)
		{
			return false;
		}
		string desiredSpellPathName = spellPath2.m_PathName.ToLower().Trim();
		iTweenPath iTweenPath = Array.Find<iTweenPath>(pathComponents, delegate(iTweenPath currTweenPath)
		{
			string text = currTweenPath.pathName.ToLower().Trim();
			return text == desiredSpellPathName;
		});
		if (iTweenPath == null)
		{
			return false;
		}
		if (iTweenPath.nodes == null || iTweenPath.nodes.Count == 0)
		{
			return false;
		}
		tweenPath = iTweenPath;
		spellPath = spellPath2;
		return true;
	}

	// Token: 0x060071DB RID: 29147 RVA: 0x00217BBC File Offset: 0x00215DBC
	private void FixupPathNodes(Player sourcePlayer, Card sourceCard, Card targetCard)
	{
		if (!this.m_sourceComputed)
		{
			this.m_pathNodes[0] = base.transform.position + this.m_spellPath.m_FirstNodeOffset;
			this.m_sourceComputed = true;
		}
		if (!this.m_targetComputed && targetCard != null)
		{
			this.m_pathNodes[this.m_pathNodes.Length - 1] = targetCard.transform.position + this.m_spellPath.m_LastNodeOffset;
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
	}

	// Token: 0x04005C00 RID: 23552
	public float m_MovementDurationSec = 1f;

	// Token: 0x04005C01 RID: 23553
	public iTween.EaseType m_EaseType = iTween.EaseType.easeInOutSine;

	// Token: 0x04005C02 RID: 23554
	public bool m_DisableContainerAfterAction;

	// Token: 0x04005C03 RID: 23555
	public bool m_OnlyMoveContainer;

	// Token: 0x04005C04 RID: 23556
	public bool m_OrientToPath;

	// Token: 0x04005C05 RID: 23557
	public List<SpellPath> m_Paths;

	// Token: 0x04005C06 RID: 23558
	private bool m_waitingToAct = true;

	// Token: 0x04005C07 RID: 23559
	private SpellPath m_spellPath;

	// Token: 0x04005C08 RID: 23560
	private Vector3[] m_pathNodes;

	// Token: 0x04005C09 RID: 23561
	private bool m_sourceComputed;

	// Token: 0x04005C0A RID: 23562
	private bool m_targetComputed;
}
