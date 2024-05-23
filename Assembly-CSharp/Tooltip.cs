using System;
using UnityEngine;

// Token: 0x02000EC8 RID: 3784
public class Tooltip : MonoBehaviour
{
	// Token: 0x0600719D RID: 29085 RVA: 0x00216890 File Offset: 0x00214A90
	public void UpdateText(string headline, string description)
	{
		this.headlineText.text = headline;
		this.descriptionText.text = description;
	}

	// Token: 0x04005BD3 RID: 23507
	public TextMesh headlineText;

	// Token: 0x04005BD4 RID: 23508
	public TextMesh descriptionText;
}
