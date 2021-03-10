using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using HarmonyLib;
using RimWorld;
using Verse;

namespace VisiblePawnCompatibility
{
	[StaticConstructorOnStartup]
	public class VisiblePawnCompatibility
	{
		static VisiblePawnCompatibility()
		{
			var harmony = new Harmony("hyperum.VisiblePawnCompatibility");

			var typeofCachedSocialTabEntry = typeof(SocialCardUtility).GetNestedType("CachedSocialTabEntry", BindingFlags.Static | BindingFlags.NonPublic);
			var original = typeof(SocialCardUtility).GetMethod("GetPawnRowTooltip", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeofCachedSocialTabEntry, typeof(Pawn) }, null);
			var postfix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("GetPawnRowTooltip_Postfix"));

			harmony.Patch(original, postfix: postfix);

			Log.Message("[VisiblePawnCompatibility] initialized.");
		}
		
	}

    public class HarmonyPatches
    {
		public class CachedSocialTabEntry
		{
			public Pawn otherPawn = new Pawn();

			public int opinionOfOtherPawn = 0;

			public int opinionOfMe = 0;

			public List<PawnRelationDef> relations = new List<PawnRelationDef>();
		}

		public static void GetPawnRowTooltip_Postfix(ref string __result, CachedSocialTabEntry entry, Pawn selPawnForSocialInfo)
		{
			if (!Prefs.DevMode)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(__result);
				stringBuilder.AppendLine("Compatibility: " + selPawnForSocialInfo.relations.CompatibilityWith(entry.otherPawn).ToString("F2"));
				stringBuilder.Append("Romance Chance Factor: " + selPawnForSocialInfo.relations.SecondaryRomanceChanceFactor(entry.otherPawn).ToString("F2"));

				__result = stringBuilder.ToString();
			}
		}
	}
}