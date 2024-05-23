using System;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class TGTArrow : MonoBehaviour
{
	// Token: 0x060045CC RID: 17868 RVA: 0x0014FD58 File Offset: 0x0014DF58
	private void onEnable()
	{
		this.m_ArrowRoot.transform.localEulerAngles = new Vector3(0f, 170f, 0f);
	}

	// Token: 0x060045CD RID: 17869 RVA: 0x0014FD8C File Offset: 0x0014DF8C
	public void FireArrow(bool randomRotation)
	{
		if (randomRotation)
		{
			Vector3 localEulerAngles = this.m_ArrowMesh.transform.localEulerAngles;
			this.m_ArrowMesh.transform.localEulerAngles = new Vector3(localEulerAngles.x + Random.Range(0f, 360f), localEulerAngles.y, localEulerAngles.z);
			this.m_ArrowRoot.transform.localEulerAngles = new Vector3(Random.Range(0f, 20f), Random.Range(160f, 180f), 0f);
		}
		this.ArrowAnimation();
	}

	// Token: 0x060045CE RID: 17870 RVA: 0x0014FE28 File Offset: 0x0014E028
	public void ArrowAnimation()
	{
		this.m_Trail.SetActive(true);
		this.m_Trail.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.15f, 0.15f, 0.15f, 0.15f));
		iTween.ColorTo(this.m_Trail, iTween.Hash(new object[]
		{
			"color",
			Color.clear,
			"time",
			0.1f,
			"oncomplete",
			"OnAnimationComplete"
		}));
		Vector3 localPosition = this.m_ArrowRoot.transform.localPosition;
		iTween.MoveFrom(this.m_ArrowRoot, iTween.Hash(new object[]
		{
			"position",
			new Vector3(localPosition.x, localPosition.y, localPosition.z + 0.4f),
			"islocal",
			true,
			"time",
			0.05f,
			"easetype",
			iTween.EaseType.easeOutQuart
		}));
	}

	// Token: 0x060045CF RID: 17871 RVA: 0x0014FF52 File Offset: 0x0014E152
	public void OnAnimationComplete()
	{
		this.m_Trail.SetActive(false);
	}

	// Token: 0x060045D0 RID: 17872 RVA: 0x0014FF60 File Offset: 0x0014E160
	public void Bullseye()
	{
		this.m_BullseyeParticles.Play();
	}

	// Token: 0x04002CD1 RID: 11473
	public GameObject m_ArrowRoot;

	// Token: 0x04002CD2 RID: 11474
	public GameObject m_ArrowMesh;

	// Token: 0x04002CD3 RID: 11475
	public GameObject m_Trail;

	// Token: 0x04002CD4 RID: 11476
	public ParticleSystem m_BullseyeParticles;
}
