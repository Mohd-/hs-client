using System;
using System.Collections;

// Token: 0x02000AB1 RID: 2737
public class ActorAttackSpell : Spell
{
	// Token: 0x06005EC6 RID: 24262 RVA: 0x001C610F File Offset: 0x001C430F
	protected override void Start()
	{
		base.Start();
	}

	// Token: 0x06005EC7 RID: 24263 RVA: 0x001C6117 File Offset: 0x001C4317
	protected override void OnBirth(SpellStateType prevStateType)
	{
		this.m_waitingToAct = true;
		base.OnBirth(prevStateType);
	}

	// Token: 0x06005EC8 RID: 24264 RVA: 0x001C6127 File Offset: 0x001C4327
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.WaitThenDoAction(prevStateType));
	}

	// Token: 0x06005EC9 RID: 24265 RVA: 0x001C6137 File Offset: 0x001C4337
	private void StopWaitingToAct()
	{
		this.m_waitingToAct = false;
	}

	// Token: 0x06005ECA RID: 24266 RVA: 0x001C6140 File Offset: 0x001C4340
	protected IEnumerator WaitThenDoAction(SpellStateType prevStateType)
	{
		while (this.m_waitingToAct)
		{
			yield return null;
		}
		base.OnAction(prevStateType);
		yield break;
	}

	// Token: 0x0400463A RID: 17978
	private bool m_waitingToAct = true;
}
