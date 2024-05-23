using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000663 RID: 1635
[CustomEditClass]
public class TGTFood : MonoBehaviour
{
	// Token: 0x06004613 RID: 17939 RVA: 0x001513B0 File Offset: 0x0014F5B0
	private void Start()
	{
		this.m_CurrentFoodItem = this.m_Food[this.m_StartingFoodIndex];
		this.m_lastFoodIdx = this.m_StartingFoodIndex;
		this.m_CurrentFoodItem.m_FSM.gameObject.SetActive(true);
		this.m_CurrentFoodItem.m_FSM.SendEvent("Birth");
		this.m_Drink.m_FSM.gameObject.SetActive(true);
		this.m_Drink.m_FSM.SendEvent("Birth");
		if (this.m_Phone)
		{
			this.m_Triangle.SetActive(false);
		}
	}

	// Token: 0x06004614 RID: 17940 RVA: 0x00151450 File Offset: 0x0014F650
	private void Update()
	{
		this.HandleHits();
		if (!this.m_Phone || this.m_Triangle.activeSelf)
		{
			return;
		}
		if (Time.timeSinceLevelLoad < this.m_phoneNextCheckTime)
		{
			return;
		}
		this.m_phoneNextCheckTime = Time.timeSinceLevelLoad + 0.25f;
		bool value = this.m_CurrentFoodItem.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value;
		bool value2 = this.m_Drink.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value;
		if (value && value2 && !this.m_isAnimating)
		{
			this.m_Triangle.SetActive(true);
		}
		else if (this.m_Triangle.activeSelf)
		{
			this.m_Triangle.SetActive(false);
		}
	}

	// Token: 0x06004615 RID: 17941 RVA: 0x00151528 File Offset: 0x0014F728
	private void HandleHits()
	{
		if (UniversalInputManager.Get().GetMouseButtonUp(0) && this.IsOver(this.m_Triangle) && !this.m_isAnimating)
		{
			base.StartCoroutine(this.RingTheBell());
		}
	}

	// Token: 0x06004616 RID: 17942 RVA: 0x00151570 File Offset: 0x0014F770
	private IEnumerator RingTheBell()
	{
		if (this.m_Phone)
		{
			this.m_Triangle.SetActive(false);
		}
		this.m_isAnimating = true;
		bool foodEmpty = this.m_CurrentFoodItem.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value;
		bool drinkEmpty = this.m_Drink.m_FSM.FsmVariables.FindFsmBool("isEmpty").Value;
		this.BellAnimation();
		if (foodEmpty)
		{
			this.m_CurrentFoodItem.m_FSM.SendEvent("Death");
		}
		if (drinkEmpty)
		{
			this.m_Drink.m_FSM.SendEvent("Death");
		}
		yield return new WaitForSeconds(this.m_NewFoodDelay);
		if (this.m_Phone)
		{
			this.m_Triangle.SetActive(false);
		}
		if (foodEmpty)
		{
			int newFoodIdx = Random.Range(0, this.m_Food.Count);
			if (newFoodIdx == this.m_lastFoodIdx)
			{
				newFoodIdx = Random.Range(0, this.m_Food.Count);
				if (newFoodIdx == this.m_lastFoodIdx)
				{
					newFoodIdx = this.m_lastFoodIdx - 1;
					if (newFoodIdx < 0)
					{
						newFoodIdx = 0;
					}
				}
			}
			this.m_lastFoodIdx = newFoodIdx;
			this.m_CurrentFoodItem = this.m_Food[newFoodIdx];
			this.m_CurrentFoodItem.m_FSM.gameObject.SetActive(true);
			this.m_CurrentFoodItem.m_FSM.SendEvent("Birth");
		}
		if (drinkEmpty)
		{
			this.m_Drink.m_FSM.SendEvent("Birth");
		}
		yield return new WaitForSeconds(this.m_NewFoodDelay);
		this.m_isAnimating = false;
		yield break;
	}

	// Token: 0x06004617 RID: 17943 RVA: 0x0015158C File Offset: 0x0014F78C
	private void BellAnimation()
	{
		if (!this.m_Phone)
		{
			this.m_TriangleAnimator.SetTrigger("Clicked");
		}
		if (!string.IsNullOrEmpty(this.m_TriangleSoundPrefab))
		{
			string text = FileUtils.GameAssetPathToName(this.m_TriangleSoundPrefab);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, this.m_Triangle);
			}
		}
	}

	// Token: 0x06004618 RID: 17944 RVA: 0x001515EC File Offset: 0x0014F7EC
	private bool IsOver(GameObject go)
	{
		return go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
	}

	// Token: 0x04002D0C RID: 11532
	public bool m_Phone;

	// Token: 0x04002D0D RID: 11533
	public GameObject m_Triangle;

	// Token: 0x04002D0E RID: 11534
	public Animator m_TriangleAnimator;

	// Token: 0x04002D0F RID: 11535
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_TriangleSoundPrefab;

	// Token: 0x04002D10 RID: 11536
	public int m_StartingFoodIndex;

	// Token: 0x04002D11 RID: 11537
	public float m_NewFoodDelay = 1f;

	// Token: 0x04002D12 RID: 11538
	public List<TGTFood.FoodItem> m_Food;

	// Token: 0x04002D13 RID: 11539
	public TGTFood.FoodItem m_Drink;

	// Token: 0x04002D14 RID: 11540
	private bool m_isAnimating;

	// Token: 0x04002D15 RID: 11541
	private int m_lastFoodIdx;

	// Token: 0x04002D16 RID: 11542
	private TGTFood.FoodItem m_CurrentFoodItem;

	// Token: 0x04002D17 RID: 11543
	private float m_phoneNextCheckTime;

	// Token: 0x02000664 RID: 1636
	[Serializable]
	public class FoodItem
	{
		// Token: 0x04002D18 RID: 11544
		public PlayMakerFSM m_FSM;
	}
}
