using System;
using UnityEngine;

// Token: 0x02000F21 RID: 3873
public class PlayOnce : MonoBehaviour
{
	// Token: 0x06007375 RID: 29557 RVA: 0x0022012C File Offset: 0x0021E32C
	private void Start()
	{
		if (this.tester != null)
		{
			this.tester.SetActive(false);
		}
		if (this.tester2 != null)
		{
			this.tester2.SetActive(false);
		}
		if (this.tester3 != null)
		{
			this.tester3.SetActive(false);
		}
	}

	// Token: 0x06007376 RID: 29558 RVA: 0x00220190 File Offset: 0x0021E390
	private void OnGUI()
	{
		Event current = Event.current;
		if (current.isKey)
		{
			if (this.tester != null)
			{
				this.tester.SetActive(true);
				this.tester.GetComponent<Animation>().Stop(this.testerAnim);
				this.tester.GetComponent<Animation>().Play(this.testerAnim);
			}
			else
			{
				Debug.Log("NO 'tester' object.");
			}
			if (this.tester2 != null)
			{
				this.tester2.SetActive(true);
				this.tester2.GetComponent<Animation>().Stop(this.tester2Anim);
				this.tester2.GetComponent<Animation>().Play(this.tester2Anim);
			}
			else
			{
				Debug.Log("NO 'tester2' object.");
			}
			if (this.tester3 != null)
			{
				this.tester3.SetActive(true);
				this.tester3.GetComponent<Animation>().Stop(this.tester3Anim);
				this.tester3.GetComponent<Animation>().Play(this.tester3Anim);
			}
			else
			{
				Debug.Log("NO 'tester3' object.");
			}
		}
	}

	// Token: 0x06007377 RID: 29559 RVA: 0x002202B9 File Offset: 0x0021E4B9
	private void Update()
	{
	}

	// Token: 0x04005DFE RID: 24062
	public string notes;

	// Token: 0x04005DFF RID: 24063
	public string notes2;

	// Token: 0x04005E00 RID: 24064
	public GameObject tester;

	// Token: 0x04005E01 RID: 24065
	public string testerAnim;

	// Token: 0x04005E02 RID: 24066
	public GameObject tester2;

	// Token: 0x04005E03 RID: 24067
	public string tester2Anim;

	// Token: 0x04005E04 RID: 24068
	public GameObject tester3;

	// Token: 0x04005E05 RID: 24069
	public string tester3Anim;
}
