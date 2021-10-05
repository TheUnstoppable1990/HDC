using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using Photon.Pun;
using HarmonyLib;
using HDC.Cards;
using UnboundLib.GameModes;
using Jotunn.Utils;
using HDC.MonoBehaviours;

namespace HDC.Utilities
{
    static class CardTools
    {
        static public CardInfoStat FormatStat(bool pos, string name, int amount, CardInfoStat.SimpleAmount simple = CardInfoStat.SimpleAmount.notAssigned)
        {
            string formatAmount;
            formatAmount = amount.ToString();
            if (amount > 0)
            {
                formatAmount = "+" + formatAmount;
            }
            return new CardInfoStat()
            {
                positive = pos,
                stat = name,
                amount = formatAmount,
                simepleAmount = simple
            };
        }
        static public CardInfoStat FormatStat(bool pos, string name, float amount, CardInfoStat.SimpleAmount simple)
        {
            string formatAmount;
            formatAmount = (amount * 100).ToString() + "%";
            if (amount > 0)
            {
                formatAmount = "+" + formatAmount;
            }            
            return new CardInfoStat()
            {
                positive = pos,
                stat = name,
                amount = formatAmount,
                simepleAmount = simple
            };
        }
        static public CardInfoStat FormatStat(bool pos, string name, float amount)
        {
            CardInfoStat.SimpleAmount simple;         
            switch (amount)
            {
                case float n when (n > 0 && n < 0.3f):
                    simple = CardInfoStat.SimpleAmount.aLittleBitOf;
                    break;
                case float n when (n >= 0.3f && n < 0.7f):
                    simple = CardInfoStat.SimpleAmount.Some;
                    break;
                case float n when (n >= 0.7f && n <= 1f):
                    simple = CardInfoStat.SimpleAmount.aLotOf;
                    break;
                case float n when (n > 1f):
                    simple = CardInfoStat.SimpleAmount.aHugeAmountOf;
                    break;
                case float n when (n < 0 && n > -0.3f):
                    simple = CardInfoStat.SimpleAmount.slightlyLower;
                    break;
                case float n when (n <= -0.3f && n > -0.7f):
                    simple = CardInfoStat.SimpleAmount.lower;
                    break;
                case float n when (n <= -0.7f):
                    simple = CardInfoStat.SimpleAmount.aLotLower;
                    break;
                default:
                    simple = CardInfoStat.SimpleAmount.notAssigned;
                    break;
            }
            return FormatStat(pos, name, amount, simple);
        }
        static public CardInfoStat FormatStat(bool pos, string name, string value, CardInfoStat.SimpleAmount simple = CardInfoStat.SimpleAmount.notAssigned)
        {
            return new CardInfoStat()
            {
                positive = pos,
                stat = name,
                amount = value,
                simepleAmount = simple

            };
        }

        static public string RandomDescription(string[] descriptions)
        {
            System.Random random = new System.Random();
            int choice = random.Next(descriptions.Length);
            return descriptions[choice];
        }
    }
}
