using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000678 RID: 1656
public class TokyoDrift : MonoBehaviour
{
	// Token: 0x06004695 RID: 18069 RVA: 0x00153544 File Offset: 0x00151744
	private void Start()
	{
		this.m_originalPosition = base.transform.localPosition;
		this.m_newPosition = default(Vector3);
		this.m_posSeedX = (float)Random.Range(1, 10);
		this.m_posSeedY = (float)Random.Range(1, 10);
		this.m_posSeedZ = (float)Random.Range(1, 10);
		this.m_posOffsetX = Random.Range(0.6f, 0.99f);
		this.m_posOffsetY = Random.Range(0.6f, 0.99f);
		this.m_posOffsetZ = Random.Range(0.6f, 0.99f);
	}

	// Token: 0x06004696 RID: 18070 RVA: 0x001535DD File Offset: 0x001517DD
	private void OnDisable()
	{
		if (this.m_blend == 0f)
		{
			return;
		}
		if (this.m_blend > 1f)
		{
			this.m_blend = 1f;
		}
		base.StartCoroutine(this.BlendOut());
	}

	// Token: 0x06004697 RID: 18071 RVA: 0x00153618 File Offset: 0x00151818
	private void Update()
	{
		if (this.m_blendOut)
		{
			return;
		}
		float num = Time.timeSinceLevelLoad * this.m_DriftSpeed;
		float num2 = this.m_originalPosition.x;
		float num3 = this.m_originalPosition.y;
		float num4 = this.m_originalPosition.z;
		if (this.m_DriftPositionAxisX)
		{
			num2 = Mathf.Sin(num + this.m_posSeedX + Mathf.Cos(num * this.m_posOffsetX)) * this.m_DriftPositionAmount;
		}
		if (this.m_DriftPositionAxisY)
		{
			num3 = Mathf.Sin(num + this.m_posSeedY + Mathf.Cos(num * this.m_posOffsetY)) * this.m_DriftPositionAmount;
		}
		if (this.m_DriftPositionAxisZ)
		{
			num4 = Mathf.Sin(num + this.m_posSeedZ + Mathf.Cos(num * this.m_posOffsetZ)) * this.m_DriftPositionAmount;
		}
		this.m_newPosition.x = this.m_originalPosition.x + num2;
		this.m_newPosition.y = this.m_originalPosition.y + num3;
		this.m_newPosition.z = this.m_originalPosition.z + num4;
		if (this.m_blend < 1f)
		{
			base.transform.localPosition = Vector3.Lerp(this.m_originalPosition, this.m_newPosition, this.m_blend);
			this.m_blend += Time.deltaTime * this.m_DriftSpeed;
		}
		else
		{
			base.transform.localPosition = this.m_newPosition;
		}
	}

	// Token: 0x06004698 RID: 18072 RVA: 0x00153798 File Offset: 0x00151998
	private IEnumerator BlendOut()
	{
		this.m_blendOut = true;
		this.m_blend = 0f;
		while (this.m_blend < 1f)
		{
			base.transform.localPosition = Vector3.Lerp(this.m_newPosition, this.m_originalPosition, this.m_blend);
			this.m_blend += Time.deltaTime * this.m_DriftSpeed;
			yield return null;
		}
		this.m_blend = 0f;
		this.m_blendOut = false;
		yield break;
	}

	// Token: 0x04002DA6 RID: 11686
	public float m_DriftPositionAmount = 1f;

	// Token: 0x04002DA7 RID: 11687
	public bool m_DriftPositionAxisX = true;

	// Token: 0x04002DA8 RID: 11688
	public bool m_DriftPositionAxisY = true;

	// Token: 0x04002DA9 RID: 11689
	public bool m_DriftPositionAxisZ = true;

	// Token: 0x04002DAA RID: 11690
	public float m_DriftSpeed = 0.1f;

	// Token: 0x04002DAB RID: 11691
	private Vector3 m_originalPosition;

	// Token: 0x04002DAC RID: 11692
	private Vector3 m_newPosition;

	// Token: 0x04002DAD RID: 11693
	private float m_posSeedX;

	// Token: 0x04002DAE RID: 11694
	private float m_posSeedY;

	// Token: 0x04002DAF RID: 11695
	private float m_posSeedZ;

	// Token: 0x04002DB0 RID: 11696
	private float m_posOffsetX;

	// Token: 0x04002DB1 RID: 11697
	private float m_posOffsetY;

	// Token: 0x04002DB2 RID: 11698
	private float m_posOffsetZ;

	// Token: 0x04002DB3 RID: 11699
	private float m_blend;

	// Token: 0x04002DB4 RID: 11700
	private bool m_blendOut;
}
