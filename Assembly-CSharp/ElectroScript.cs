using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000D85 RID: 3461
public class ElectroScript : MonoBehaviour
{
	// Token: 0x06006C2C RID: 27692 RVA: 0x001FC734 File Offset: 0x001FA934
	private void Start()
	{
		this.srcTrgDist = 0f;
		this.srcDstDist = 0f;
		this.numVertices = 0;
		this.deltaV1 = this.prefabs.destination.position - this.prefabs.source.position;
		this.lastUpdate = 0f;
		this.branches = new Hashtable();
	}

	// Token: 0x06006C2D RID: 27693 RVA: 0x001FC7A0 File Offset: 0x001FA9A0
	private void Update()
	{
		this.srcTrgDist = Vector3.Distance(this.prefabs.source.position, this.prefabs.target.position);
		this.srcDstDist = Vector3.Distance(this.prefabs.source.position, this.prefabs.destination.position);
		if (this.branches.Count > 0)
		{
			Hashtable hashtable = (Hashtable)this.branches.Clone();
			ICollection keys = hashtable.Keys;
			foreach (object obj in keys)
			{
				int num = (int)obj;
				LineRenderer lineRenderer = (LineRenderer)this.branches[num];
				BranchScript component = lineRenderer.GetComponent<BranchScript>();
				if (component.timeSpawned + this.timers.branchLife < Time.time)
				{
					this.branches.Remove(num);
					Object.Destroy(lineRenderer.gameObject);
				}
			}
		}
		if (this.prefabs.target.localPosition != this.prefabs.destination.localPosition)
		{
			if (Vector3.Distance(Vector3.zero, this.deltaV1) * Time.deltaTime * (1f / this.timers.timeToPowerUp) > Vector3.Distance(this.prefabs.target.position, this.prefabs.destination.position))
			{
				this.prefabs.target.position = this.prefabs.destination.position;
			}
			else
			{
				this.prefabs.target.Translate(this.deltaV1 * Time.deltaTime * (1f / this.timers.timeToPowerUp));
			}
		}
		if (Time.time - this.lastUpdate < this.timers.timeToUpdate)
		{
			return;
		}
		this.lastUpdate = Time.time;
		this.AnimateArc();
		this.DrawArc();
		this.RayCast();
	}

	// Token: 0x06006C2E RID: 27694 RVA: 0x001FC9E8 File Offset: 0x001FABE8
	private void DrawArc()
	{
		this.numVertices = Mathf.RoundToInt(this.srcTrgDist / this.lines.keyVertexDist) * (1 + this.lines.numInterpoles) + 1;
		this.deltaV2 = (this.prefabs.target.localPosition - this.prefabs.source.localPosition) / (float)this.numVertices;
		Vector3 vector = this.prefabs.source.localPosition;
		Vector3[] array = new Vector3[this.numVertices];
		this.prefabs.lightning.SetVertexCount(this.numVertices);
		for (int i = 0; i < this.numVertices; i++)
		{
			Vector3 vector2 = vector;
			vector2.y += (Random.value * 2f - 1f) * this.lines.keyVertexRange;
			vector2.z += (Random.value * 2f - 1f) * this.lines.keyVertexRange;
			this.prefabs.lightning.SetPosition(i, vector2);
			array[i] = vector2;
			if (!this.branches.ContainsKey(i))
			{
				float value = Random.value;
				if (value < this.dynamics.chanceToArc)
				{
					LineRenderer lineRenderer = (LineRenderer)Object.Instantiate(this.prefabs.branch, Vector3.zero, Quaternion.identity);
					lineRenderer.GetComponent<BranchScript>().timeSpawned = Time.time;
					lineRenderer.transform.parent = this.prefabs.lightning.transform;
					this.branches.Add(i, lineRenderer);
					lineRenderer.transform.position = this.prefabs.lightning.transform.TransformPoint(vector2);
					vector2.x = Random.value - 0.5f;
					vector2.y = (Random.value - 0.5f) * 2f;
					vector2.z = (Random.value - 0.5f) * 2f;
					lineRenderer.transform.LookAt(lineRenderer.transform.TransformPoint(vector2));
					lineRenderer.transform.Find("stop").localPosition = lineRenderer.transform.Find("start").localPosition + new Vector3(0f, 0f, Random.Range(this.lines.minBranchLength, this.lines.maxBranchLength));
					int num = Mathf.RoundToInt(Vector3.Distance(lineRenderer.transform.Find("start").position, lineRenderer.transform.Find("stop").position) / this.lines.keyVertexDist) * (1 + this.lines.numInterpoles) + 1;
					Vector3 vector3 = (lineRenderer.transform.Find("stop").localPosition - lineRenderer.transform.Find("start").localPosition) / (float)num;
					Vector3 vector4 = lineRenderer.transform.Find("start").localPosition;
					Vector3[] array2 = new Vector3[num];
					lineRenderer.SetVertexCount(num);
					for (int j = 0; j < num; j++)
					{
						Vector3 vector5 = vector4;
						vector5.x += (Random.value * 2f - 1f) * this.lines.keyVertexRange;
						vector5.y += (Random.value * 2f - 1f) * this.lines.keyVertexRange;
						lineRenderer.SetPosition(j, vector5);
						array2[j] = vector5;
						vector4 += vector3 * (float)(this.lines.numInterpoles + 1);
						j += this.lines.numInterpoles;
					}
					lineRenderer.SetPosition(0, lineRenderer.transform.Find("start").localPosition);
					lineRenderer.SetPosition(num - 1, lineRenderer.transform.Find("stop").localPosition);
					for (int k = 0; k < num; k++)
					{
						if (k % (this.lines.numInterpoles + 1) != 0)
						{
							Vector3 vector6 = array2[k - 1];
							Vector3 vector7 = array2[k + this.lines.numInterpoles];
							float num2 = Vector3.Distance(vector6, vector7) / (float)(this.lines.numInterpoles + 1) / Vector3.Distance(vector6, vector7) * 3.1415927f;
							for (int l = 0; l < this.lines.numInterpoles; l++)
							{
								Vector3 vector8;
								vector8.x = vector6.x + vector3.x * (float)(1 + l);
								vector8.y = vector6.y + (Mathf.Sin(num2 - 1.5707964f) / 2f + 0.5f) * (vector7.y - vector6.y);
								vector8.z = vector6.z + (Mathf.Sin(num2 - 1.5707964f) / 2f + 0.5f) * (vector7.z - vector6.z);
								lineRenderer.SetPosition(k + l, vector8);
								num2 += num2;
							}
							k += this.lines.numInterpoles;
						}
					}
				}
			}
			else
			{
				LineRenderer lineRenderer2 = (LineRenderer)this.branches[i];
				int num3 = Mathf.RoundToInt(Vector3.Distance(lineRenderer2.transform.Find("start").position, lineRenderer2.transform.Find("stop").position) / this.lines.keyVertexDist) * (1 + this.lines.numInterpoles) + 1;
				Vector3 vector9 = (lineRenderer2.transform.Find("stop").localPosition - lineRenderer2.transform.Find("start").localPosition) / (float)num3;
				Vector3 vector10 = lineRenderer2.transform.Find("start").localPosition;
				Vector3[] array3 = new Vector3[num3];
				lineRenderer2.SetVertexCount(num3);
				lineRenderer2.SetPosition(0, lineRenderer2.transform.Find("start").localPosition);
				for (int m = 0; m < num3; m++)
				{
					Vector3 vector11 = vector10;
					vector11.x += (Random.value * 2f - 1f) * this.lines.keyVertexRange;
					vector11.y += (Random.value * 2f - 1f) * this.lines.keyVertexRange;
					lineRenderer2.SetPosition(m, vector11);
					array3[m] = vector11;
					vector10 += vector9 * (float)(this.lines.numInterpoles + 1);
					m += this.lines.numInterpoles;
				}
				lineRenderer2.SetPosition(0, lineRenderer2.transform.Find("start").localPosition);
				lineRenderer2.SetPosition(num3 - 1, lineRenderer2.transform.Find("stop").localPosition);
				for (int n = 0; n < num3; n++)
				{
					if (n % (this.lines.numInterpoles + 1) != 0)
					{
						Vector3 vector12 = array3[n - 1];
						Vector3 vector13 = array3[n + this.lines.numInterpoles];
						float num4 = Vector3.Distance(vector12, vector13) / (float)(this.lines.numInterpoles + 1) / Vector3.Distance(vector12, vector13) * 3.1415927f;
						for (int num5 = 0; num5 < this.lines.numInterpoles; num5++)
						{
							Vector3 vector14;
							vector14.x = vector12.x + vector9.x * (float)(1 + num5);
							vector14.y = vector12.y + (Mathf.Sin(num4 - 1.5707964f) / 2f + 0.5f) * (vector13.y - vector12.y);
							vector14.z = vector12.z + (Mathf.Sin(num4 - 1.5707964f) / 2f + 0.5f) * (vector13.z - vector12.z);
							lineRenderer2.SetPosition(n + num5, vector14);
							num4 += num4;
						}
						n += this.lines.numInterpoles;
					}
				}
			}
			vector += this.deltaV2 * (float)(this.lines.numInterpoles + 1);
			i += this.lines.numInterpoles;
		}
		this.prefabs.lightning.SetPosition(0, this.prefabs.source.localPosition);
		this.prefabs.lightning.SetPosition(this.numVertices - 1, this.prefabs.target.localPosition);
		for (int num6 = 0; num6 < this.numVertices; num6++)
		{
			if (num6 % (this.lines.numInterpoles + 1) != 0)
			{
				Vector3 vector15 = array[num6 - 1];
				Vector3 vector16 = array[num6 + this.lines.numInterpoles];
				float num7 = Vector3.Distance(vector15, vector16) / (float)(this.lines.numInterpoles + 1) / Vector3.Distance(vector15, vector16) * 3.1415927f;
				for (int num8 = 0; num8 < this.lines.numInterpoles; num8++)
				{
					Vector3 vector17;
					vector17.x = vector15.x + this.deltaV2.x * (float)(1 + num8);
					vector17.y = vector15.y + (Mathf.Sin(num7 - 1.5707964f) / 2f + 0.5f) * (vector16.y - vector15.y);
					vector17.z = vector15.z + (Mathf.Sin(num7 - 1.5707964f) / 2f + 0.5f) * (vector16.z - vector15.z);
					this.prefabs.lightning.SetPosition(num6 + num8, vector17);
					num7 += num7;
				}
				num6 += this.lines.numInterpoles;
			}
		}
	}

	// Token: 0x06006C2F RID: 27695 RVA: 0x001FD4B8 File Offset: 0x001FB6B8
	private void AnimateArc()
	{
		Vector2 mainTextureOffset = this.prefabs.lightning.GetComponent<Renderer>().material.mainTextureOffset;
		Vector2 mainTextureScale = this.prefabs.lightning.GetComponent<Renderer>().material.mainTextureScale;
		mainTextureOffset.x += Time.deltaTime * this.tex.animateSpeed;
		mainTextureOffset.y = this.tex.offsetY;
		mainTextureScale.x = this.srcTrgDist / this.srcDstDist * this.tex.scaleX;
		mainTextureScale.y = this.tex.scaleY;
		this.prefabs.lightning.GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset;
		this.prefabs.lightning.GetComponent<Renderer>().material.mainTextureScale = mainTextureScale;
	}

	// Token: 0x06006C30 RID: 27696 RVA: 0x001FD594 File Offset: 0x001FB794
	private void RayCast()
	{
		RaycastHit[] array = Physics.RaycastAll(this.prefabs.source.position, this.prefabs.target.position - this.prefabs.source.position, Vector3.Distance(this.prefabs.source.position, this.prefabs.target.position));
		foreach (RaycastHit raycastHit in array)
		{
			Object.Instantiate(this.prefabs.sparks, raycastHit.point, Quaternion.identity);
		}
		if (this.branches.Count > 0)
		{
			Hashtable hashtable = (Hashtable)this.branches.Clone();
			ICollection keys = hashtable.Keys;
			foreach (object obj in keys)
			{
				int num = (int)obj;
				LineRenderer lineRenderer = (LineRenderer)this.branches[num];
				array = Physics.RaycastAll(lineRenderer.transform.Find("start").position, lineRenderer.transform.Find("stop").position - lineRenderer.transform.Find("start").position, Vector3.Distance(lineRenderer.transform.Find("start").position, lineRenderer.transform.Find("stop").position));
				foreach (RaycastHit raycastHit2 in array)
				{
					Object.Instantiate(this.prefabs.sparks, raycastHit2.point, Quaternion.identity);
				}
			}
		}
	}

	// Token: 0x040054AB RID: 21675
	public ElectroScript.Prefabs prefabs;

	// Token: 0x040054AC RID: 21676
	public ElectroScript.Timers timers;

	// Token: 0x040054AD RID: 21677
	public ElectroScript.Dynamics dynamics;

	// Token: 0x040054AE RID: 21678
	public ElectroScript.LineSettings lines;

	// Token: 0x040054AF RID: 21679
	public ElectroScript.TextureSettings tex;

	// Token: 0x040054B0 RID: 21680
	private int numVertices;

	// Token: 0x040054B1 RID: 21681
	private Vector3 deltaV1;

	// Token: 0x040054B2 RID: 21682
	private Vector3 deltaV2;

	// Token: 0x040054B3 RID: 21683
	private float srcTrgDist;

	// Token: 0x040054B4 RID: 21684
	private float srcDstDist;

	// Token: 0x040054B5 RID: 21685
	private float lastUpdate;

	// Token: 0x040054B6 RID: 21686
	private Hashtable branches;

	// Token: 0x02000D86 RID: 3462
	[Serializable]
	public class Prefabs
	{
		// Token: 0x040054B7 RID: 21687
		public LineRenderer lightning;

		// Token: 0x040054B8 RID: 21688
		public LineRenderer branch;

		// Token: 0x040054B9 RID: 21689
		public Transform sparks;

		// Token: 0x040054BA RID: 21690
		public Transform source;

		// Token: 0x040054BB RID: 21691
		public Transform destination;

		// Token: 0x040054BC RID: 21692
		public Transform target;
	}

	// Token: 0x02000D87 RID: 3463
	[Serializable]
	public class Timers
	{
		// Token: 0x040054BD RID: 21693
		public float timeToUpdate = 0.05f;

		// Token: 0x040054BE RID: 21694
		public float timeToPowerUp = 0.5f;

		// Token: 0x040054BF RID: 21695
		public float branchLife = 0.1f;
	}

	// Token: 0x02000D88 RID: 3464
	[Serializable]
	public class Dynamics
	{
		// Token: 0x040054C0 RID: 21696
		public float chanceToArc = 0.2f;
	}

	// Token: 0x02000D89 RID: 3465
	[Serializable]
	public class LineSettings
	{
		// Token: 0x040054C1 RID: 21697
		public float keyVertexDist = 3f;

		// Token: 0x040054C2 RID: 21698
		public float keyVertexRange = 4f;

		// Token: 0x040054C3 RID: 21699
		public int numInterpoles = 5;

		// Token: 0x040054C4 RID: 21700
		public float minBranchLength = 11f;

		// Token: 0x040054C5 RID: 21701
		public float maxBranchLength = 16f;
	}

	// Token: 0x02000D8A RID: 3466
	[Serializable]
	public class TextureSettings
	{
		// Token: 0x040054C6 RID: 21702
		public float scaleX;

		// Token: 0x040054C7 RID: 21703
		public float scaleY;

		// Token: 0x040054C8 RID: 21704
		public float animateSpeed;

		// Token: 0x040054C9 RID: 21705
		public float offsetY;
	}
}
