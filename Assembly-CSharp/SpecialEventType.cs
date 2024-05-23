using System;
using System.ComponentModel;

// Token: 0x02000021 RID: 33
public enum SpecialEventType
{
	// Token: 0x0400011C RID: 284
	UNKNOWN = -1,
	// Token: 0x0400011D RID: 285
	[Description("none")]
	IGNORE,
	// Token: 0x0400011E RID: 286
	[Description("always")]
	ALWAYS = 0,
	// Token: 0x0400011F RID: 287
	[Description("launch")]
	LAUNCH_DAY,
	// Token: 0x04000120 RID: 288
	[Description("naxx_1")]
	NAXX_1_OPENS,
	// Token: 0x04000121 RID: 289
	[Description("naxx_2")]
	NAXX_2_OPENS,
	// Token: 0x04000122 RID: 290
	[Description("naxx_3")]
	NAXX_3_OPENS,
	// Token: 0x04000123 RID: 291
	[Description("naxx_4")]
	NAXX_4_OPENS,
	// Token: 0x04000124 RID: 292
	[Description("naxx_5")]
	NAXX_5_OPENS,
	// Token: 0x04000125 RID: 293
	[Description("gvg_promote")]
	GVG_PROMOTION,
	// Token: 0x04000126 RID: 294
	[Description("gvg_begin")]
	GVG_LAUNCH_PERIOD,
	// Token: 0x04000127 RID: 295
	[Description("gvg_arena")]
	GVG_ARENA_PLAY,
	// Token: 0x04000128 RID: 296
	[Description("lunar_new_year")]
	LUNAR_NEW_YEAR = 11,
	// Token: 0x04000129 RID: 297
	[Description("brm_1")]
	BRM_1_OPENS,
	// Token: 0x0400012A RID: 298
	[Description("brm_2")]
	BRM_2_OPENS,
	// Token: 0x0400012B RID: 299
	[Description("brm_3")]
	BRM_3_OPENS,
	// Token: 0x0400012C RID: 300
	[Description("brm_4")]
	BRM_4_OPENS,
	// Token: 0x0400012D RID: 301
	[Description("brm_5")]
	BRM_5_OPENS,
	// Token: 0x0400012E RID: 302
	[Description("brm_pre_sale")]
	BRM_PRE_SALE,
	// Token: 0x0400012F RID: 303
	[Description("brm_normal_sale")]
	BRM_NORMAL_SALE,
	// Token: 0x04000130 RID: 304
	[Description("tb_pre_event")]
	SPECIAL_EVENT_PRE_TAVERN_BRAWL,
	// Token: 0x04000131 RID: 305
	[Description("tgt_pre_sale")]
	SPECIAL_EVENT_TGT_PRE_SALE = 29,
	// Token: 0x04000132 RID: 306
	[Description("tgt_normal_sale")]
	SPECIAL_EVENT_TGT_NORMAL_SALE,
	// Token: 0x04000133 RID: 307
	[Description("samsung_galaxy_gifts")]
	SPECIAL_EVENT_SAMSUNG_GALAXY_GIFTS,
	// Token: 0x04000134 RID: 308
	[Description("tgt_arena_draftable")]
	SPECIAL_EVENT_TGT_ARENA_DRAFTABLE,
	// Token: 0x04000135 RID: 309
	[Description("loe_1")]
	SPECIAL_EVENT_LOE_WING_1_OPEN = 56,
	// Token: 0x04000136 RID: 310
	[Description("loe_2")]
	SPECIAL_EVENT_LOE_WING_2_OPEN,
	// Token: 0x04000137 RID: 311
	[Description("loe_3")]
	SPECIAL_EVENT_LOE_WING_3_OPEN,
	// Token: 0x04000138 RID: 312
	[Description("loe_4")]
	SPECIAL_EVENT_LOE_WING_4_OPEN,
	// Token: 0x04000139 RID: 313
	[Description("set_rotation_2016")]
	SPECIAL_EVENT_SET_ROTATION_2016 = 65,
	// Token: 0x0400013A RID: 314
	[Description("set_rotation_2017")]
	SPECIAL_EVENT_SET_ROTATION_2017,
	// Token: 0x0400013B RID: 315
	[Description("feast_of_winter_veil")]
	FEAST_OF_WINTER_VEIL = 85,
	// Token: 0x0400013C RID: 316
	[Description("og_pre_purchase")]
	SPECIAL_EVENT_OLD_GODS_PRE_PURCHASE = 105,
	// Token: 0x0400013D RID: 317
	[Description("og_normal_sale")]
	SPECIAL_EVENT_OLD_GODS_NORMAL_SALE,
	// Token: 0x0400013E RID: 318
	[Description("apple_charity_promo_2016")]
	SPECIAL_EVENT_APPLE_CHARITY_PROMO_2016 = 108,
	// Token: 0x0400013F RID: 319
	[Description("naxx_gvg_real_money_sale")]
	SPECIAL_EVENT_NAXX_GVG_REAL_MONEY_SALE = 160,
	// Token: 0x04000140 RID: 320
	[Description("set_rotation_2016_freepacks")]
	SPECIAL_EVENT_SET_ROTATION_2016_FREEPACKS,
	// Token: 0x04000141 RID: 321
	[Description("set_rotation_2016_questline")]
	SPECIAL_EVENT_SET_ROTATION_2016_QUESTLINE
}
