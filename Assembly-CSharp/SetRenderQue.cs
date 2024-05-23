using System;
using UnityEngine;

// Token: 0x02000F3B RID: 3899
[RequireComponent(typeof(Renderer))]
public class SetRenderQue : MonoBehaviour
{
	// Token: 0x060073E8 RID: 29672 RVA: 0x00221EA8 File Offset: 0x002200A8
	private void Start()
	{
		if (this.includeChildren)
		{
			foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
			{
				if (!(renderer == null))
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							material.renderQueue += this.queue;
						}
					}
				}
			}
		}
		else
		{
			if (base.GetComponent<Renderer>() == null)
			{
				return;
			}
			base.GetComponent<Renderer>().material.renderQueue += this.queue;
		}
		if (this.queues == null)
		{
			return;
		}
		if (base.GetComponent<Renderer>() == null)
		{
			return;
		}
		Material[] materials2 = base.GetComponent<Renderer>().materials;
		if (materials2 == null)
		{
			return;
		}
		int num = materials2.Length;
		int num2 = 0;
		while (num2 < this.queues.Length && num2 < num)
		{
			Material material2 = base.GetComponent<Renderer>().materials[num2];
			if (!(material2 == null))
			{
				if (this.queues[num2] < 0)
				{
					Debug.LogWarning(string.Format("WARNING: Using negative renderQueue for {0}'s {1} (renderQueue = {2})", base.transform.root.name, base.gameObject.name, this.queues[num2]));
				}
				material2.renderQueue += this.queues[num2];
			}
			num2++;
		}
	}

	// Token: 0x04005E6E RID: 24174
	public int queue = 1;

	// Token: 0x04005E6F RID: 24175
	public bool includeChildren;

	// Token: 0x04005E70 RID: 24176
	public int[] queues;
}
