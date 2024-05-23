using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F2C RID: 3884
public class RandomSpawnTimed : MonoBehaviour
{
	// Token: 0x060073A6 RID: 29606 RVA: 0x00220FE3 File Offset: 0x0021F1E3
	private void Start()
	{
		this.listOfObjs = new List<GameObject>();
		base.StartCoroutine(this.RespawnLoop());
	}

	// Token: 0x060073A7 RID: 29607 RVA: 0x00221000 File Offset: 0x0021F200
	private IEnumerator RespawnLoop()
	{
		for (;;)
		{
			float timeToWait = Random.Range(this.minWaitTime, this.maxWaitTime);
			yield return new WaitForSeconds(timeToWait);
			this.listOfObjs.Add((GameObject)Object.Instantiate(this.objPrefab, base.transform.position, Random.rotation));
		}
		yield break;
	}

	// Token: 0x060073A8 RID: 29608 RVA: 0x0022101C File Offset: 0x0021F21C
	private void Update()
	{
		for (int i = 0; i < this.listOfObjs.Count; i++)
		{
			if (Mathf.Abs(this.listOfObjs[i].transform.position.x - base.gameObject.transform.position.x) > this.killX || Mathf.Abs(this.listOfObjs[i].transform.position.z - base.gameObject.transform.position.z) > this.killZ)
			{
				GameObject gameObject = this.listOfObjs[i];
				this.listOfObjs.Remove(this.listOfObjs[i]);
				Object.Destroy(gameObject);
				i--;
			}
		}
	}

	// Token: 0x04005E37 RID: 24119
	public float minWaitTime = 5f;

	// Token: 0x04005E38 RID: 24120
	public float maxWaitTime = 15f;

	// Token: 0x04005E39 RID: 24121
	public float killX = 10f;

	// Token: 0x04005E3A RID: 24122
	public float killZ = 10f;

	// Token: 0x04005E3B RID: 24123
	public GameObject objPrefab;

	// Token: 0x04005E3C RID: 24124
	private List<GameObject> listOfObjs;
}
