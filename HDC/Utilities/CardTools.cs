using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using HDC.MonoBehaviours;
using HDC.Utilities;
using HDC.Extentions;
using ModdingUtils.MonoBehaviours;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ClassesManagerReborn.Util;
using System.Collections.ObjectModel;
using UnboundLib.Utils;
using System.Reflection;
using HDC.Cards;
using System.Collections;
using ModdingUtils.GameModes;
using ModdingUtils.Extensions;
using HDC.Class;


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
			System.Random random = new System.Random();
			int num = random.Next(descriptions.Length);
			return descriptions[num];
		}

		public static void RemoveFirstCardByName(Player player, string name)
        {
			var targetCard = player.data.currentCards.FirstOrDefault(c => c.cardName == name);
			ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, targetCard, ModdingUtils.Utils.Cards.SelectionType.Oldest, true);
		}
	}
}