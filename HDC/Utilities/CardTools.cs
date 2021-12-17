using System;

namespace HDC.Utilities
{
	
	internal static class CardTools
	{
		
		public static CardInfoStat FormatStat(bool pos, string name, int amount, CardInfoStat.SimpleAmount simple = CardInfoStat.SimpleAmount.notAssigned)
		{
			string text = amount.ToString();
			bool flag = amount > 0;
			if (flag)
			{
				text = "+" + text;
			}
			return new CardInfoStat
			{
				positive = pos,
				stat = name,
				amount = text,
				simepleAmount = simple
			};
		}

		
		public static CardInfoStat FormatStat(bool pos, string name, float amount, CardInfoStat.SimpleAmount simple)
		{
			string text = (amount * 100f).ToString() + "%";
			bool flag = amount > 0f;
			if (flag)
			{
				text = "+" + text;
			}
			return new CardInfoStat
			{
				positive = pos,
				stat = name,
				amount = text,
				simepleAmount = simple
			};
		}

		
		public static CardInfoStat FormatStat(bool pos, string name, float amount)
		{
			CardInfoStat.SimpleAmount simple;
			if (amount <= 0f || amount >= 0.3f)
			{
				if (amount < 0.3f || amount >= 0.7f)
				{
					if (amount < 0.7f || amount > 1f)
					{
						if (amount <= 1f)
						{
							if (amount >= 0f || amount <= -0.3f)
							{
								if (amount > -0.3f || amount <= -0.7f)
								{
									if (amount > -0.7f)
									{
										simple = CardInfoStat.SimpleAmount.notAssigned;
									}
									else
									{
										simple = CardInfoStat.SimpleAmount.aLotLower;
									}
								}
								else
								{
									simple = CardInfoStat.SimpleAmount.lower;
								}
							}
							else
							{
								simple = CardInfoStat.SimpleAmount.slightlyLower;
							}
						}
						else
						{
							simple = CardInfoStat.SimpleAmount.aHugeAmountOf;
						}
					}
					else
					{
						simple = CardInfoStat.SimpleAmount.aLotOf;
					}
				}
				else
				{
					simple = CardInfoStat.SimpleAmount.Some;
				}
			}
			else
			{
				simple = CardInfoStat.SimpleAmount.aLittleBitOf;
			}
			return CardTools.FormatStat(pos, name, amount, simple);
		}

		
		public static CardInfoStat FormatStat(bool pos, string name, string value, CardInfoStat.SimpleAmount simple = CardInfoStat.SimpleAmount.notAssigned)
		{
			return new CardInfoStat
			{
				positive = pos,
				stat = name,
				amount = value,
				simepleAmount = simple
			};
		}

		
		public static string RandomDescription(string[] descriptions)
		{
			Random random = new Random();
			int num = random.Next(descriptions.Length);
			return descriptions[num];
		}
	}
}