using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200065B RID: 1627
[CustomEditClass]
public class TGTArcheryTarget : MonoBehaviour
{
	// Token: 0x060045BB RID: 17851 RVA: 0x0014F35C File Offset: 0x0014D55C
	private void Start()
	{
		this.m_arrows = new GameObject[this.m_MaxArrows];
		for (int i = 0; i < this.m_MaxArrows; i++)
		{
			this.m_arrows[i] = Object.Instantiate<GameObject>(this.m_Arrow);
			this.m_arrows[i].transform.position = new Vector3(-15f, -15f, -15f);
			this.m_arrows[i].transform.parent = this.m_TargetRoot.transform;
			this.m_arrows[i].SetActive(false);
		}
		this.m_arrows[0].SetActive(true);
		this.m_arrows[0].transform.position = this.m_ArrowBone01.transform.position;
		this.m_arrows[0].transform.rotation = this.m_ArrowBone01.transform.rotation;
		this.m_arrows[1].SetActive(true);
		this.m_arrows[1].transform.position = this.m_ArrowBone02.transform.position;
		this.m_arrows[1].transform.rotation = this.m_ArrowBone02.transform.rotation;
		this.m_lastArrow = 2;
		this.m_targetRadius = Vector3.Distance(this.m_CenterBone.position, this.m_OuterRadiusBone.position);
		this.m_bullseyeRadius = Vector3.Distance(this.m_BullseyeCenterBone.position, this.m_BullseyeRadiusBone.position);
		this.m_AvailableTargetDummyArrows = new List<int>();
		for (int j = 0; j < this.m_TargetDummyArrows.Count; j++)
		{
			this.m_AvailableTargetDummyArrows.Add(j);
		}
		this.m_SplitArrow.SetActive(false);
	}

	// Token: 0x060045BC RID: 17852 RVA: 0x0014F521 File Offset: 0x0014D721
	private void Update()
	{
		this.HandleHits();
	}

	// Token: 0x060045BD RID: 17853 RVA: 0x0014F529 File Offset: 0x0014D729
	private void HandleHits()
	{
		if (UniversalInputManager.Get().GetMouseButtonDown(0) && this.IsOver(this.m_Collider01))
		{
			this.HnadleFireArrow();
		}
	}

	// Token: 0x060045BE RID: 17854 RVA: 0x0014F554 File Offset: 0x0014D754
	private void HnadleFireArrow()
	{
		if (this.m_clearingArrows)
		{
			return;
		}
		this.m_ArrowCount++;
		if (this.m_ArrowCount > this.m_Levelup)
		{
			this.m_ArrowCount = 0;
			this.m_MaxRandomOffset *= 0.95f;
			this.m_BullseyePercent += 4;
		}
		if (Random.Range(0, 100) < this.m_TargetDummyPercent && this.m_AvailableTargetDummyArrows.Count > 0)
		{
			this.HitTargetDummy();
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		bool bullseye = false;
		RaycastHit raycastHit;
		if (this.m_BoxColliderBullseye.Raycast(ray, ref raycastHit, 100f))
		{
			bullseye = true;
		}
		if (!this.m_BoxCollider02.Raycast(ray, ref raycastHit, 100f))
		{
			return;
		}
		this.m_lastArrow++;
		if (this.m_lastArrow >= this.m_MaxArrows)
		{
			this.m_lastArrow = 0;
			base.StartCoroutine(this.ClearArrows());
			return;
		}
		GameObject gameObject = this.m_arrows[this.m_lastArrow];
		TGTArrow component = gameObject.GetComponent<TGTArrow>();
		this.FireArrow(component, raycastHit.point, bullseye);
		gameObject.transform.eulerAngles = raycastHit.normal;
		this.ImpactTarget();
	}

	// Token: 0x060045BF RID: 17855 RVA: 0x0014F698 File Offset: 0x0014D898
	private IEnumerator ClearArrows()
	{
		this.m_clearingArrows = true;
		foreach (GameObject arrowGO in this.m_arrows)
		{
			if (arrowGO.activeSelf)
			{
				arrowGO.SetActive(false);
				this.m_TargetPhysics.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(this.m_HitIntensity * -0.25f, this.m_HitIntensity * -0.5f), 0f, 0f);
				this.PlaySound(this.m_RemoveArrowSoundPrefab);
				yield return new WaitForSeconds(0.2f);
			}
		}
		yield return new WaitForSeconds(0.2f);
		if (this.m_SplitArrow.activeSelf)
		{
			this.m_SplitArrow.SetActive(false);
			this.m_TargetPhysics.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(this.m_HitIntensity * -0.25f, this.m_HitIntensity * -0.5f), 0f, 0f);
			this.PlaySound(this.m_RemoveArrowSoundPrefab);
		}
		this.m_lastArrowWasBullseye = false;
		this.m_lastBullseyeArrow = null;
		this.m_clearingArrows = false;
		yield break;
	}

	// Token: 0x060045C0 RID: 17856 RVA: 0x0014F6B4 File Offset: 0x0014D8B4
	private void FireArrow(TGTArrow arrow, Vector3 hitPosition, bool bullseye)
	{
		arrow.transform.position = hitPosition;
		bool flag = false;
		if (Time.timeSinceLevelLoad > this.m_lastClickTime + 0.8f)
		{
			flag = true;
		}
		this.m_lastClickTime = Time.timeSinceLevelLoad;
		int num = this.m_BullseyePercent;
		if (flag)
		{
			num *= 2;
		}
		if (num > 80)
		{
			num = 80;
		}
		if (bullseye && Random.Range(0, 100) < num)
		{
			int num2 = 2;
			if (flag)
			{
				num2 = 8;
			}
			if (this.m_lastArrowWasBullseye && !this.m_SplitArrow.activeSelf && bullseye && Random.Range(0, 100) < num2)
			{
				this.m_SplitArrow.transform.position = this.m_lastBullseyeArrow.transform.position;
				this.m_SplitArrow.transform.rotation = this.m_lastBullseyeArrow.transform.rotation;
				TGTArrow component = this.m_SplitArrow.GetComponent<TGTArrow>();
				TGTArrow component2 = this.m_lastBullseyeArrow.GetComponent<TGTArrow>();
				this.m_SplitArrow.SetActive(true);
				component.FireArrow(false);
				component.Bullseye();
				this.PlaySound(this.m_SplitArrowSoundPrefab);
				component.m_ArrowRoot.transform.position = component2.m_ArrowRoot.transform.position;
				component.m_ArrowRoot.transform.rotation = component2.m_ArrowRoot.transform.rotation;
				this.m_lastBullseyeArrow.SetActive(false);
				this.m_lastArrowWasBullseye = false;
				this.m_lastBullseyeArrow = null;
				return;
			}
			arrow.gameObject.SetActive(true);
			arrow.Bullseye();
			this.PlaySound(this.m_HitBullseyeSoundPrefab);
			arrow.m_ArrowRoot.transform.localPosition = Vector3.zero;
			this.m_lastBullseyeArrow = arrow.gameObject;
			this.m_lastArrowWasBullseye = true;
			return;
		}
		else
		{
			this.m_lastArrowWasBullseye = false;
			this.m_lastBullseyeArrow = null;
			arrow.gameObject.SetActive(true);
			if (bullseye)
			{
				Vector2 vector = Random.insideUnitCircle.normalized * this.m_bullseyeRadius * 2f;
				arrow.m_ArrowRoot.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
				arrow.FireArrow(true);
				this.PlaySound(this.m_HitTargetSoundPrefab);
				return;
			}
			Vector2 vector2 = Random.insideUnitCircle * Random.Range(0f, this.m_MaxRandomOffset);
			arrow.m_ArrowRoot.transform.localPosition = new Vector3(vector2.x, vector2.y, 0f);
			if (Vector3.Distance(arrow.m_ArrowRoot.transform.position, this.m_CenterBone.position) > this.m_targetRadius)
			{
				arrow.m_ArrowRoot.transform.localPosition = Vector3.zero;
			}
			if (Vector3.Distance(arrow.m_ArrowRoot.transform.position, this.m_BullseyeCenterBone.position) < this.m_bullseyeRadius)
			{
				arrow.m_ArrowRoot.transform.localPosition = Vector3.zero;
			}
			arrow.FireArrow(true);
			this.PlaySound(this.m_HitTargetSoundPrefab);
			return;
		}
	}

	// Token: 0x060045C1 RID: 17857 RVA: 0x0014F9D8 File Offset: 0x0014DBD8
	private void HitTargetDummy()
	{
		int num = 0;
		if (this.m_AvailableTargetDummyArrows.Count > 1)
		{
			num = Random.Range(0, this.m_AvailableTargetDummyArrows.Count);
		}
		TGTArrow tgtarrow = this.m_TargetDummyArrows[this.m_AvailableTargetDummyArrows[num]];
		tgtarrow.gameObject.SetActive(true);
		tgtarrow.FireArrow(false);
		TGTTargetDummy.Get().ArrowHit();
		this.PlaySound(this.m_HitTargetDummySoundPrefab);
		if (this.m_AvailableTargetDummyArrows.Count > 1)
		{
			this.m_AvailableTargetDummyArrows.RemoveAt(num);
		}
		else
		{
			this.m_AvailableTargetDummyArrows.Clear();
		}
	}

	// Token: 0x060045C2 RID: 17858 RVA: 0x0014FA78 File Offset: 0x0014DC78
	private void ImpactTarget()
	{
		this.m_TargetPhysics.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(this.m_HitIntensity * 0.25f, this.m_HitIntensity), 0f, 0f);
	}

	// Token: 0x060045C3 RID: 17859 RVA: 0x0014FAB0 File Offset: 0x0014DCB0
	private void PlaySound(string soundPrefab)
	{
		if (!string.IsNullOrEmpty(soundPrefab))
		{
			string text = FileUtils.GameAssetPathToName(soundPrefab);
			if (!string.IsNullOrEmpty(text))
			{
				SoundManager.Get().LoadAndPlay(text, base.gameObject);
			}
		}
	}

	// Token: 0x060045C4 RID: 17860 RVA: 0x0014FAEC File Offset: 0x0014DCEC
	private bool IsOver(GameObject go)
	{
		return go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
	}

	// Token: 0x04002CA7 RID: 11431
	public int m_BullseyePercent = 5;

	// Token: 0x04002CA8 RID: 11432
	public int m_TargetDummyPercent = 1;

	// Token: 0x04002CA9 RID: 11433
	public float m_MaxRandomOffset = 0.3f;

	// Token: 0x04002CAA RID: 11434
	public int m_Levelup = 50;

	// Token: 0x04002CAB RID: 11435
	public GameObject m_Collider01;

	// Token: 0x04002CAC RID: 11436
	public GameObject m_TargetPhysics;

	// Token: 0x04002CAD RID: 11437
	public GameObject m_TargetRoot;

	// Token: 0x04002CAE RID: 11438
	public GameObject m_Arrow;

	// Token: 0x04002CAF RID: 11439
	public GameObject m_SplitArrow;

	// Token: 0x04002CB0 RID: 11440
	public float m_HitIntensity;

	// Token: 0x04002CB1 RID: 11441
	public int m_MaxArrows;

	// Token: 0x04002CB2 RID: 11442
	public List<TGTArrow> m_TargetDummyArrows;

	// Token: 0x04002CB3 RID: 11443
	public GameObject m_ArrowBone01;

	// Token: 0x04002CB4 RID: 11444
	public GameObject m_ArrowBone02;

	// Token: 0x04002CB5 RID: 11445
	public BoxCollider m_BoxCollider01;

	// Token: 0x04002CB6 RID: 11446
	public BoxCollider m_BoxCollider02;

	// Token: 0x04002CB7 RID: 11447
	public BoxCollider m_BoxColliderBullseye;

	// Token: 0x04002CB8 RID: 11448
	public Transform m_CenterBone;

	// Token: 0x04002CB9 RID: 11449
	public Transform m_OuterRadiusBone;

	// Token: 0x04002CBA RID: 11450
	public Transform m_BullseyeCenterBone;

	// Token: 0x04002CBB RID: 11451
	public Transform m_BullseyeRadiusBone;

	// Token: 0x04002CBC RID: 11452
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitTargetSoundPrefab;

	// Token: 0x04002CBD RID: 11453
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitBullseyeSoundPrefab;

	// Token: 0x04002CBE RID: 11454
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HitTargetDummySoundPrefab;

	// Token: 0x04002CBF RID: 11455
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_SplitArrowSoundPrefab;

	// Token: 0x04002CC0 RID: 11456
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_RemoveArrowSoundPrefab;

	// Token: 0x04002CC1 RID: 11457
	private GameObject[] m_arrows;

	// Token: 0x04002CC2 RID: 11458
	private int m_lastArrow = 1;

	// Token: 0x04002CC3 RID: 11459
	private float m_targetRadius;

	// Token: 0x04002CC4 RID: 11460
	private float m_bullseyeRadius;

	// Token: 0x04002CC5 RID: 11461
	private int m_ArrowCount;

	// Token: 0x04002CC6 RID: 11462
	private List<int> m_AvailableTargetDummyArrows;

	// Token: 0x04002CC7 RID: 11463
	private GameObject m_lastBullseyeArrow;

	// Token: 0x04002CC8 RID: 11464
	private bool m_lastArrowWasBullseye;

	// Token: 0x04002CC9 RID: 11465
	private bool m_clearingArrows;

	// Token: 0x04002CCA RID: 11466
	private float m_lastClickTime;
}
