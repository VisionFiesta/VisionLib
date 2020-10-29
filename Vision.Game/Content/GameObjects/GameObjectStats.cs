using Vision.Game.Characters;

namespace Vision.Game.Content.GameObjects
{
	public class GameObjectStats
	{
		private readonly GameObject _object;

		public byte FreeSTR { get; set; }
		public byte FreeEND { get; set; }
		public byte FreeDEX { get; set; }
		public byte FreeINT { get; set; }
		public byte FreeSPR { get; set; }

		public uint CurrentHP { get; set; }
		public uint CurrentSP { get; set; }
		public uint CurrentLP { get; set; }

		public uint CurrentHPStones { get; set; }
		public uint CurrentSPStones { get; set; }

		public uint CurrentMaxHP => BaseMaxHP + BonusMaxHP;
		public uint CurrentMaxSP => BaseMaxSP + BonusMaxSP;
		public uint CurrentMaxLP => BaseMaxLP + BonusMaxLP;

		public uint CurrentMinHP { get; set; }
		public uint CurrentMinSP { get; set; }

		public uint CurrentMaxHPStones => BaseMaxHPStones + BonusMaxHPStones;
		public uint CurrentMaxSPStones => BaseMaxSPStones + BonusMaxSPStones;

		public uint CurrentHPStoneHealth => BaseHPStoneHealth + BonusHPStoneHealth;
		public uint CurrentSPStoneHealth => BaseSPStoneSpirit + BonusSPStoneSpirit;

		public uint CurrentSTR => BaseSTR + BonusSTR;
		public uint CurrentEND => BaseEND + BonusEND;
		public uint CurrentDEX => BaseDEX + BonusDEX;
		public uint CurrentINT => BaseINT + BonusINT;
		public uint CurrentSPR => BaseSPR + BonusSPR;

		public uint CurrentMinDmg => BaseMinDmg + BonusMinDmg;
		public uint CurrentMaxDmg => BaseMaxDmg + BonusMaxDmg;
		public uint CurrentMinMDmg => BaseMinMDmg + BonusMinMDmg;
		public uint CurrentMaxMDmg => BaseMaxMDmg + BonusMaxMDmg;

		public uint CurrentDef => BaseDef + BonusDef;
		public uint CurrentMDef => BaseMDef + BonusMDef;
		public uint CurrentAim => BaseAim + BonusAim;
		public uint CurrentEvasion => BaseEvasion + BonusEvasion;

		public uint CurrentIllnessResistance => BaseIllnessResistance + BonusIllnessResistance;
		public uint CurrentDiseaseResistance => BaseDiseaseResistance + BonusDiseaseResistance;
		public uint CurrentCurseResistance => BaseCurseResistance + BonusCurseResistance;
		public uint CurrentStunResistance => BaseStunResistance + BonusStunResistance;

		public uint CurrentWalkSpeed => BaseWalkSpeed + BonusWalkSpeed;
		public uint CurrentRunSpeed => BaseRunSpeed + BonusRunSpeed;

		public uint CurrentAttackSpeed => BaseAttackSpeed + BonusAttackSpeed;

		// Base stats
		public uint BaseMaxHP { get; set; }
		public uint BaseMaxSP { get; set; }
		public uint BaseMaxLP { get; set; }

		public uint BaseMaxHPStones { get; set; }
		public uint BaseMaxSPStones { get; set; }

		public uint BaseHPStoneHealth { get; set; }
		public uint BaseSPStoneSpirit { get; set; }

		public uint BaseSTR { get; set; }
		public uint BaseEND { get; set; }
		public uint BaseDEX { get; set; }
		public uint BaseINT { get; set; }
		public uint BaseSPR { get; set; }

		public uint BaseMinDmg { get; set; }
		public uint BaseMaxDmg { get; set; }
		public uint BaseMinMDmg { get; set; }
		public uint BaseMaxMDmg { get; set; }
		public uint BaseDef { get; set; }
		public uint BaseMDef { get; set; }
		public uint BaseAim { get; set; }
		public uint BaseEvasion { get; set; }

		public uint BaseIllnessResistance { get; set; }
		public uint BaseDiseaseResistance { get; set; }
		public uint BaseCurseResistance { get; set; }
		public uint BaseStunResistance { get; set; }

		public uint BaseWalkSpeed { get; set; }
		public uint BaseRunSpeed { get; set; }

		public uint BaseAttackSpeed { get; set; }

		// Bonus Stats
		public uint BonusMaxHP { get; set; }
		public uint BonusMaxSP { get; set; }
		public uint BonusMaxLP { get; set; }

		public uint BonusMaxHPStones { get; set; }
		public uint BonusMaxSPStones { get; set; }

		public uint BonusHPStoneHealth { get; set; }
		public uint BonusSPStoneSpirit { get; set; }

		public uint BonusSTR { get; set; }
		public uint BonusEND { get; set; }
		public uint BonusDEX { get; set; }
		public uint BonusINT { get; set; }
		public uint BonusSPR { get; set; }

		public uint BonusMinDmg { get; set; }
		public uint BonusMaxDmg { get; set; }
		public uint BonusMinMDmg { get; set; }
		public uint BonusMaxMDmg { get; set; }
		public uint BonusDef { get; set; }
		public uint BonusMDef { get; set; }
		public uint BonusAim { get; set; }
		public uint BonusEvasion { get; set; }

		public double BonusDefRate { get; set; }
		public double BonusMDefRate { get; set; }
		public double BonusDmgRate { get; set; }
		public double BonusMDmgRate { get; set; }

		public uint BonusIllnessResistance { get; set; }
		public uint BonusDiseaseResistance { get; set; }
		public uint BonusCurseResistance { get; set; }
		public uint BonusStunResistance { get; set; }

		public uint BonusWalkSpeed { get; set; }
		public uint BonusRunSpeed { get; set; }

		public uint BonusAttackSpeed { get; set; }

		public uint BonusLPRegenRate { get; set; }

		// Critical stats
		public uint CriticalMinDmg { get; set; }
		public uint CriticalMaxDmg { get; set; }
		public uint CriticalMinMDmg { get; set; }
		public uint CriticalMaxMDmg { get; set; }

		public decimal CriticalRate { get; set; }
		public decimal BlockRate { get; set; }

		public GameObjectStats(GameObject obj)
		{
			_object = obj;
		}

		public void Update()
        {
            if (!(_object is Character character)) return;

            var stats = character.Parameters;

            BaseSTR = stats.Strength.Base;
            BaseEND = stats.Constitution.Base;
            BaseDEX = stats.Dexterity.Base;
            BaseINT = stats.Intelligence.Base;
            BaseSPR = stats.Wisdom.Base;

            BonusSTR = stats.Strength.Bonus;
            BonusEND = stats.Constitution.Bonus;
            BonusDEX = stats.Dexterity.Bonus;
            BonusINT = stats.Intelligence.Bonus;
            BonusSPR = stats.Wisdom.Bonus;

            BaseHPStoneHealth = stats.HPStoneHealth;
            BaseSPStoneSpirit = stats.SPStoneSpirit;
			
            BaseMaxHPStones = stats.MaxHPStones;
            BaseMaxSPStones = stats.MaxSPStones;

            BaseIllnessResistance = stats.IllnessResistance.Base;
            BaseDiseaseResistance = stats.DiseaseResistance.Base;
            BaseCurseResistance = stats.CurseResistance.Base;
            BaseStunResistance = stats.StunResistance.Base;

            BonusIllnessResistance = stats.IllnessResistance.Bonus;
            BonusDiseaseResistance = stats.DiseaseResistance.Bonus;
            BonusCurseResistance = stats.CurseResistance.Bonus;
            BonusStunResistance = stats.StunResistance.Bonus;

            BaseMaxHP = stats.MaxHP;
            BaseMaxSP = stats.MaxSP;
            BaseMaxLP = stats.MaxLP;
        }
	}
}
