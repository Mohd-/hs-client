using System;
using System.Collections.Generic;
using System.Reflection;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000D80 RID: 3456
public class PlayMakerAnimatorStateSynchronization : MonoBehaviour
{
	// Token: 0x06006C1A RID: 27674 RVA: 0x001FC19C File Offset: 0x001FA39C
	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		if (this.Fsm != null)
		{
			string layerName = this.animator.GetLayerName(this.LayerIndex);
			this.fsmStateLUT = new Dictionary<int, FsmState>();
			foreach (FsmState fsmState in this.Fsm.Fsm.States)
			{
				string name = fsmState.Name;
				this.RegisterHash(fsmState.Name, fsmState);
				if (!name.StartsWith(layerName + "."))
				{
					this.RegisterHash(layerName + "." + fsmState.Name, fsmState);
				}
			}
		}
	}

	// Token: 0x06006C1B RID: 27675 RVA: 0x001FC250 File Offset: 0x001FA450
	private void RegisterHash(string key, FsmState state)
	{
		int num = Animator.StringToHash(key);
		this.fsmStateLUT.Add(num, state);
		if (this.debug)
		{
			Debug.Log(string.Concat(new object[]
			{
				"registered ",
				key,
				" ->",
				num
			}));
		}
	}

	// Token: 0x06006C1C RID: 27676 RVA: 0x001FC2A9 File Offset: 0x001FA4A9
	private void Update()
	{
		if (this.EveryFrame)
		{
			this.Synchronize();
		}
	}

	// Token: 0x06006C1D RID: 27677 RVA: 0x001FC2BC File Offset: 0x001FA4BC
	public void Synchronize()
	{
		if (this.animator == null || this.Fsm == null)
		{
			return;
		}
		bool flag = false;
		if (this.animator.IsInTransition(this.LayerIndex))
		{
			int fullPathHash = this.animator.GetAnimatorTransitionInfo(this.LayerIndex).fullPathHash;
			int userNameHash = this.animator.GetAnimatorTransitionInfo(this.LayerIndex).userNameHash;
			if (this.lastTransition != fullPathHash)
			{
				if (this.debug)
				{
					Debug.Log("is in transition");
				}
				if (this.fsmStateLUT.ContainsKey(userNameHash))
				{
					FsmState fsmState = this.fsmStateLUT[userNameHash];
					if (this.Fsm.Fsm.ActiveState != fsmState)
					{
						this.SwitchState(this.Fsm.Fsm, fsmState);
						flag = true;
					}
				}
				if (!flag && this.fsmStateLUT.ContainsKey(fullPathHash))
				{
					FsmState fsmState2 = this.fsmStateLUT[fullPathHash];
					if (this.Fsm.Fsm.ActiveState != fsmState2)
					{
						this.SwitchState(this.Fsm.Fsm, fsmState2);
						flag = true;
					}
				}
				if (!flag && this.debug)
				{
					Debug.LogWarning("Fsm is missing animator transition name or username for hash:" + fullPathHash);
				}
				this.lastTransition = fullPathHash;
			}
		}
		if (!flag)
		{
			int fullPathHash2 = this.animator.GetCurrentAnimatorStateInfo(this.LayerIndex).fullPathHash;
			if (this.lastState != fullPathHash2)
			{
				if (this.debug)
				{
					Debug.Log("Net state switch");
				}
				if (this.fsmStateLUT.ContainsKey(fullPathHash2))
				{
					FsmState fsmState3 = this.fsmStateLUT[fullPathHash2];
					if (this.Fsm.Fsm.ActiveState != fsmState3)
					{
						this.SwitchState(this.Fsm.Fsm, fsmState3);
					}
				}
				else if (this.debug)
				{
					Debug.LogWarning("Fsm is missing animator state hash:" + fullPathHash2);
				}
				this.lastState = fullPathHash2;
			}
		}
	}

	// Token: 0x06006C1E RID: 27678 RVA: 0x001FC4E0 File Offset: 0x001FA6E0
	private void SwitchState(Fsm fsm, FsmState state)
	{
		MethodInfo method = fsm.GetType().GetMethod("SwitchState", 36);
		if (method != null)
		{
			method.Invoke(fsm, new object[]
			{
				state
			});
		}
	}

	// Token: 0x0400549D RID: 21661
	public int LayerIndex;

	// Token: 0x0400549E RID: 21662
	public PlayMakerFSM Fsm;

	// Token: 0x0400549F RID: 21663
	public bool EveryFrame = true;

	// Token: 0x040054A0 RID: 21664
	public bool debug;

	// Token: 0x040054A1 RID: 21665
	private Animator animator;

	// Token: 0x040054A2 RID: 21666
	private int lastState;

	// Token: 0x040054A3 RID: 21667
	private int lastTransition;

	// Token: 0x040054A4 RID: 21668
	private Dictionary<int, FsmState> fsmStateLUT;
}
