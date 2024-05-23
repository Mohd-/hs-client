using System;
using System.ComponentModel;

// Token: 0x02000460 RID: 1120
public class CardBackData
{
	// Token: 0x0600371D RID: 14109 RVA: 0x0010E620 File Offset: 0x0010C820
	public CardBackData(int id, CardBackData.CardBackSource source, long sourceData, string name, bool enabled, string prefabName)
	{
		this.ID = id;
		this.Source = source;
		this.SourceData = sourceData;
		this.Name = name;
		this.Enabled = enabled;
		this.PrefabName = prefabName;
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x0600371E RID: 14110 RVA: 0x0010E660 File Offset: 0x0010C860
	// (set) Token: 0x0600371F RID: 14111 RVA: 0x0010E668 File Offset: 0x0010C868
	public int ID { get; private set; }

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x06003720 RID: 14112 RVA: 0x0010E671 File Offset: 0x0010C871
	// (set) Token: 0x06003721 RID: 14113 RVA: 0x0010E679 File Offset: 0x0010C879
	public CardBackData.CardBackSource Source { get; private set; }

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x06003722 RID: 14114 RVA: 0x0010E682 File Offset: 0x0010C882
	// (set) Token: 0x06003723 RID: 14115 RVA: 0x0010E68A File Offset: 0x0010C88A
	public long SourceData { get; private set; }

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x06003724 RID: 14116 RVA: 0x0010E693 File Offset: 0x0010C893
	// (set) Token: 0x06003725 RID: 14117 RVA: 0x0010E69B File Offset: 0x0010C89B
	public string Name { get; private set; }

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x06003726 RID: 14118 RVA: 0x0010E6A4 File Offset: 0x0010C8A4
	// (set) Token: 0x06003727 RID: 14119 RVA: 0x0010E6AC File Offset: 0x0010C8AC
	public bool Enabled { get; private set; }

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x06003728 RID: 14120 RVA: 0x0010E6B5 File Offset: 0x0010C8B5
	// (set) Token: 0x06003729 RID: 14121 RVA: 0x0010E6BD File Offset: 0x0010C8BD
	public string PrefabName { get; private set; }

	// Token: 0x0600372A RID: 14122 RVA: 0x0010E6C8 File Offset: 0x0010C8C8
	public override string ToString()
	{
		return string.Format("[CardBackData: ID={0}, Source={1}, SourceData={2}, Name={3}, Enabled={4}, PrefabName={5}]", new object[]
		{
			this.ID,
			this.Name,
			this.Source,
			this.SourceData,
			this.Enabled,
			this.PrefabName
		});
	}

	// Token: 0x02000465 RID: 1125
	public enum CardBackSource
	{
		// Token: 0x04002291 RID: 8849
		[Description("startup")]
		STARTUP,
		// Token: 0x04002292 RID: 8850
		[Description("season")]
		SEASON,
		// Token: 0x04002293 RID: 8851
		[Description("achieve")]
		ACHIEVE,
		// Token: 0x04002294 RID: 8852
		[Description("fixed_reward")]
		FIXED_REWARD,
		// Token: 0x04002295 RID: 8853
		[Description("tavern_brawl")]
		TAVERN_BRAWL = 5
	}
}
