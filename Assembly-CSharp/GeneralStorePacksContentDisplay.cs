using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AE0 RID: 2784
[CustomEditClass]
public class GeneralStorePacksContentDisplay : MonoBehaviour
{
	// Token: 0x06006003 RID: 24579 RVA: 0x001CBF7C File Offset: 0x001CA17C
	public void SetParent(GeneralStorePacksContent parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06006004 RID: 24580 RVA: 0x001CBF88 File Offset: 0x001CA188
	public int ShowPacks(int numVisiblePacks, float flyInTime, float flyOutTime, float flyInDelay, float flyOutDelay, bool forceImmediate = false)
	{
		if (this.m_lastVisiblePacks == numVisiblePacks)
		{
			return 0;
		}
		bool flag = this.m_parent.IsContentActive();
		AnimatedLowPolyPack[] currentPacks = this.GetCurrentPacks(this.m_parent.GetBoosterId(), numVisiblePacks);
		int num = 0;
		for (int i = currentPacks.Length - 1; i >= numVisiblePacks; i--)
		{
			AnimatedLowPolyPack animatedLowPolyPack = currentPacks[i];
			if (flag && !forceImmediate)
			{
				if (animatedLowPolyPack.FlyOut(flyOutTime, flyOutDelay * (float)num))
				{
					num++;
				}
			}
			else
			{
				animatedLowPolyPack.FlyOutImmediate();
			}
		}
		int num2 = 0;
		for (int j = 0; j < numVisiblePacks; j++)
		{
			AnimatedLowPolyPack animatedLowPolyPack2 = currentPacks[j];
			if (flag && !forceImmediate)
			{
				if (animatedLowPolyPack2.FlyIn(flyInTime, flyInDelay * (float)num2))
				{
					num2++;
				}
			}
			else
			{
				animatedLowPolyPack2.FlyInImmediate();
			}
		}
		this.FlyLeavingSoonBanner(num2, num, flyInTime, flyOutTime, flyInDelay, flyOutDelay, numVisiblePacks, flag && !forceImmediate);
		this.m_lastVisiblePacks = numVisiblePacks;
		if (num2 > num)
		{
			return num2;
		}
		return -num;
	}

	// Token: 0x06006005 RID: 24581 RVA: 0x001CC094 File Offset: 0x001CA294
	public void UpdatePackType(StorePackDef packDef)
	{
		this.ClearPacks();
		if (this.m_background != null && packDef != null)
		{
			this.m_background.material = packDef.m_background;
		}
	}

	// Token: 0x06006006 RID: 24582 RVA: 0x001CC0D8 File Offset: 0x001CA2D8
	public void ClearPacks()
	{
		foreach (AnimatedLowPolyPack animatedLowPolyPack in this.m_showingPacks)
		{
			Object.Destroy(animatedLowPolyPack.gameObject);
		}
		this.m_showingPacks.Clear();
		foreach (AnimatedLeavingSoonSign animatedLeavingSoonSign in this.m_showingLeavingSoonSigns)
		{
			Object.Destroy(animatedLeavingSoonSign.gameObject);
		}
		this.m_showingLeavingSoonSigns.Clear();
		this.m_lastVisiblePacks = 0;
	}

	// Token: 0x06006007 RID: 24583 RVA: 0x001CC1A0 File Offset: 0x001CA3A0
	private AnimatedLowPolyPack[] GetCurrentPacks(int id, int count)
	{
		if (count > this.m_showingPacks.Count)
		{
			AnimatedLowPolyPack animatedLowPolyPack = null;
			if (!GeneralStorePacksContentDisplay.s_packTemplates.TryGetValue(id, out animatedLowPolyPack))
			{
				StorePackDef storePackDef = this.m_parent.GetStorePackDef(id);
				GameObject gameObject = AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(storePackDef.m_lowPolyPrefab), true, false);
				animatedLowPolyPack = gameObject.GetComponent<AnimatedLowPolyPack>();
				GeneralStorePacksContentDisplay.s_packTemplates[id] = animatedLowPolyPack;
				animatedLowPolyPack.gameObject.SetActive(false);
			}
			for (int i = this.m_showingPacks.Count; i < count; i++)
			{
				AnimatedLowPolyPack animatedLowPolyPack2 = Object.Instantiate<AnimatedLowPolyPack>(animatedLowPolyPack);
				this.SetupLowPolyPack(animatedLowPolyPack2, i, false);
				this.m_showingPacks.Add(animatedLowPolyPack2);
			}
		}
		return this.m_showingPacks.ToArray();
	}

	// Token: 0x06006008 RID: 24584 RVA: 0x001CC25C File Offset: 0x001CA45C
	private void SetupLowPolyPack(AnimatedLowPolyPack pack, int i, bool useVisiblePacksOnly)
	{
		pack.gameObject.SetActive(true);
		int num = this.DeterminePackColumn(i);
		GameUtils.SetParent(pack, this.m_packStacks[num], true);
		pack.transform.localScale = GeneralStorePacksContentDisplay.PACK_SCALE;
		pack.Init(num, this.DeterminePackLocalPos(num, this.m_showingPacks, useVisiblePacksOnly), new Vector3(0f, 3.5f, -0.1f), true, true);
		SceneUtils.SetLayer(pack, this.m_packStacks[num].layer);
		float num2 = Random.Range(-this.m_parent.m_PackYDegreeVariationMag, this.m_parent.m_PackYDegreeVariationMag);
		Vector3 flyInLocalAngles;
		flyInLocalAngles..ctor(0f, num2, 0f);
		float num3 = Random.Range(-GeneralStorePacksContentDisplay.PACK_FLY_OUT_X_DEG_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_FLY_OUT_X_DEG_VARIATION_MAG);
		float num4 = Random.Range(-GeneralStorePacksContentDisplay.PACK_FLY_OUT_Z_DEG_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_FLY_OUT_Z_DEG_VARIATION_MAG);
		Vector3 flyOutLocalAngles;
		flyOutLocalAngles..ctor(num3, 0f, num4);
		pack.SetFlyingLocalRotations(flyInLocalAngles, flyOutLocalAngles);
	}

	// Token: 0x06006009 RID: 24585 RVA: 0x001CC34C File Offset: 0x001CA54C
	private Vector3 DeterminePackLocalPos(int column, List<AnimatedLowPolyPack> packs, bool useVisiblePacksOnly)
	{
		List<AnimatedLowPolyPack> list = packs.FindAll((AnimatedLowPolyPack obj) => obj.Column == column && (!useVisiblePacksOnly || obj.GetState() == AnimatedLowPolyPack.State.FLOWN_IN || obj.GetState() == AnimatedLowPolyPack.State.FLYING_IN));
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-GeneralStorePacksContentDisplay.PACK_X_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_X_VARIATION_MAG);
		zero.y = GeneralStorePacksContentDisplay.PACK_Y_OFFSET * (float)list.Count;
		zero.z = Random.Range(-GeneralStorePacksContentDisplay.PACK_Z_VARIATION_MAG, GeneralStorePacksContentDisplay.PACK_Z_VARIATION_MAG);
		if (column % 2 == 0)
		{
			zero.y += 0.03f;
		}
		return zero;
	}

	// Token: 0x0600600A RID: 24586 RVA: 0x001CC3EC File Offset: 0x001CA5EC
	private int DeterminePackColumn(int packNumber)
	{
		Random random = new Random(GeneralStorePacksContentDisplay.PACK_STACK_SEED + packNumber);
		double num = random.NextDouble();
		double num2 = 0.0;
		float num3 = 1f / (float)this.m_packStacks.Count;
		int i;
		for (i = 0; i < this.m_packStacks.Count - 1; i++)
		{
			num2 += (double)num3;
			if (num <= num2)
			{
				break;
			}
		}
		return i;
	}

	// Token: 0x0600600B RID: 24587 RVA: 0x001CC45C File Offset: 0x001CA65C
	private void FlyLeavingSoonBanner(int numPacksFlyingIn, int numPacksFlyingOut, float flyInTime, float flyOutTime, float flyInDelay, float flyOutDelay, int numVisiblePacks, bool animated)
	{
		foreach (AnimatedLeavingSoonSign animatedLeavingSoonSign in this.m_showingLeavingSoonSigns)
		{
			if (animated)
			{
				animatedLeavingSoonSign.FlyOut(flyOutTime, 0f);
			}
			else
			{
				animatedLeavingSoonSign.FlyOutImmediate();
			}
		}
		List<AnimatedLeavingSoonSign> list = this.m_showingLeavingSoonSigns.FindAll((AnimatedLeavingSoonSign l) => l.GetState() == AnimatedLowPolyPack.State.HIDDEN);
		foreach (AnimatedLeavingSoonSign animatedLeavingSoonSign2 in list)
		{
			Object.Destroy(animatedLeavingSoonSign2.gameObject);
		}
		this.m_showingLeavingSoonSigns.RemoveAll((AnimatedLeavingSoonSign l) => l.GetState() == AnimatedLowPolyPack.State.HIDDEN);
		if (string.IsNullOrEmpty(this.m_leavingSoonBannerPrefab))
		{
			return;
		}
		BoosterDbfRecord boosterRecord = GameDbf.Booster.GetRecord(this.m_parent.GetBoosterId());
		if (boosterRecord == null)
		{
			return;
		}
		if (!boosterRecord.LeavingSoon)
		{
			return;
		}
		AnimatedLeavingSoonSign animatedLeavingSoonSign3 = GameUtils.LoadGameObjectWithComponent<AnimatedLeavingSoonSign>(FileUtils.GameAssetPathToName(this.m_leavingSoonBannerPrefab));
		if (animatedLeavingSoonSign3 == null)
		{
			return;
		}
		if (animatedLeavingSoonSign3.m_leavingSoonButton != null)
		{
			animatedLeavingSoonSign3.m_leavingSoonButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OnLeavingSoonButtonClicked(boosterRecord.LeavingSoonText);
			});
		}
		this.SetupLowPolyPack(animatedLeavingSoonSign3, numVisiblePacks, true);
		this.m_showingLeavingSoonSigns.Add(animatedLeavingSoonSign3);
		if (animated)
		{
			animatedLeavingSoonSign3.FlyIn(flyInTime, flyInDelay * (float)numPacksFlyingIn);
		}
		else
		{
			animatedLeavingSoonSign3.FlyInImmediate();
		}
	}

	// Token: 0x0600600C RID: 24588 RVA: 0x001CC650 File Offset: 0x001CA850
	private void OnLeavingSoonButtonClicked(string leavingSoonText)
	{
		DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo
		{
			m_headerText = GameStrings.Get("GLUE_STORE_EXPANSION_LEAVING_SOON"),
			m_text = leavingSoonText,
			m_showAlertIcon = true,
			m_responseDisplay = AlertPopup.ResponseDisplay.OK
		});
	}

	// Token: 0x0400477F RID: 18303
	public MeshRenderer m_background;

	// Token: 0x04004780 RID: 18304
	public List<GameObject> m_packStacks = new List<GameObject>();

	// Token: 0x04004781 RID: 18305
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_leavingSoonBannerPrefab;

	// Token: 0x04004782 RID: 18306
	private GeneralStorePacksContent m_parent;

	// Token: 0x04004783 RID: 18307
	private List<AnimatedLowPolyPack> m_showingPacks = new List<AnimatedLowPolyPack>();

	// Token: 0x04004784 RID: 18308
	private List<AnimatedLeavingSoonSign> m_showingLeavingSoonSigns = new List<AnimatedLeavingSoonSign>();

	// Token: 0x04004785 RID: 18309
	private static readonly Vector3 PACK_SCALE = new Vector3(0.06f, 0.03f, 0.06f);

	// Token: 0x04004786 RID: 18310
	private static readonly float PACK_X_VARIATION_MAG = 0.015f;

	// Token: 0x04004787 RID: 18311
	private static readonly float PACK_Y_OFFSET = 0.02f;

	// Token: 0x04004788 RID: 18312
	private static readonly float PACK_Z_VARIATION_MAG = 0.01f;

	// Token: 0x04004789 RID: 18313
	private static readonly float PACK_FLY_OUT_X_DEG_VARIATION_MAG = 10f;

	// Token: 0x0400478A RID: 18314
	private static readonly float PACK_FLY_OUT_Z_DEG_VARIATION_MAG = 10f;

	// Token: 0x0400478B RID: 18315
	private static readonly int PACK_STACK_SEED = 2;

	// Token: 0x0400478C RID: 18316
	private int m_lastVisiblePacks;

	// Token: 0x0400478D RID: 18317
	private static Map<int, AnimatedLowPolyPack> s_packTemplates = new Map<int, AnimatedLowPolyPack>();
}
