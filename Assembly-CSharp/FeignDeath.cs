using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E65 RID: 3685
public class FeignDeath : SuperSpell
{
	// Token: 0x06006FC2 RID: 28610 RVA: 0x0020D035 File Offset: 0x0020B235
	protected override void Awake()
	{
		base.Awake();
		this.m_RootObject.SetActive(false);
	}

	// Token: 0x06006FC3 RID: 28611 RVA: 0x0020D04C File Offset: 0x0020B24C
	protected override void OnAction(SpellStateType prevStateType)
	{
		if (!this.m_taskList.IsStartOfBlock())
		{
			base.OnAction(prevStateType);
			return;
		}
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		this.m_targets.Clear();
		for (PowerTaskList powerTaskList = this.m_taskList; powerTaskList != null; powerTaskList = powerTaskList.GetNext())
		{
			foreach (PowerTask powerTask in powerTaskList.GetTaskList())
			{
				Network.PowerHistory power = powerTask.GetPower();
				Network.HistMetaData histMetaData = power as Network.HistMetaData;
				if (histMetaData != null)
				{
					if (histMetaData.MetaType == null)
					{
						foreach (int id in histMetaData.Info)
						{
							Entity entity = GameState.Get().GetEntity(id);
							Card card = entity.GetCard();
							this.m_targets.Add(card.gameObject);
						}
					}
				}
			}
		}
		base.StartCoroutine(this.ActionVisual());
	}

	// Token: 0x06006FC4 RID: 28612 RVA: 0x0020D198 File Offset: 0x0020B398
	private IEnumerator ActionVisual()
	{
		List<GameObject> fxObjects = new List<GameObject>();
		foreach (GameObject target in this.m_targets)
		{
			GameObject fx = Object.Instantiate<GameObject>(this.m_RootObject);
			fx.SetActive(true);
			fxObjects.Add(fx);
			fx.transform.position = target.transform.position;
			fx.transform.position = new Vector3(fx.transform.position.x, fx.transform.position.y + this.m_Height, fx.transform.position.z);
			foreach (ParticleSystem ps in fx.GetComponentsInChildren<ParticleSystem>())
			{
				ps.Play();
			}
		}
		yield return new WaitForSeconds(1f);
		foreach (GameObject fxObj in fxObjects)
		{
			Object.Destroy(fxObj);
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		yield break;
	}

	// Token: 0x040058E9 RID: 22761
	public GameObject m_RootObject;

	// Token: 0x040058EA RID: 22762
	public GameObject m_Glow;

	// Token: 0x040058EB RID: 22763
	public float m_Height = 1f;
}
