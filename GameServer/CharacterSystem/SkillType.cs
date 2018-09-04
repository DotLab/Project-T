using GameLib.Utilities;
using System;
using System.Collections.Generic;

namespace GameLib.CharacterSystem {
	public struct SkillSituation {
		public bool canAttack;
		public bool damageMental;
		public bool canDefend;
	}

	public sealed class SkillType : IEquatable<SkillType> {
		public static readonly SkillType Athletics = new SkillType("Athletics", "运动");
		public static readonly SkillType Burglary = new SkillType("Burglary", "盗窃");
		public static readonly SkillType Contacts = new SkillType("Contacts", "人脉");
		public static readonly SkillType Crafts = new SkillType("Crafts", "工艺");
		public static readonly SkillType Deceive = new SkillType("Deceive", "欺诈");
		public static readonly SkillType Drive = new SkillType("Drive", "驾驶");
		public static readonly SkillType Empathy = new SkillType("Empathy", "共情");
		public static readonly SkillType Fight = new SkillType("Fight", "战斗");
		public static readonly SkillType Investigate = new SkillType("Investigate", "调查");
		public static readonly SkillType Lore = new SkillType("Lore", "学识");
		public static readonly SkillType Notice = new SkillType("Notice", "洞察");
		public static readonly SkillType Physique = new SkillType("Physique", "体格");
		public static readonly SkillType Provoke = new SkillType("Provoke", "威胁");
		public static readonly SkillType Rapport = new SkillType("Rapport", "交际");
		public static readonly SkillType Resources = new SkillType("Resources", "资源");
		public static readonly SkillType Shoot = new SkillType("Shoot", "射击");
		public static readonly SkillType Stealth = new SkillType("Stealth", "潜行");
		public static readonly SkillType Will = new SkillType("Will", "意志");

		private static readonly Dictionary<string, SkillType> skillTypes = new Dictionary<string, SkillType>();
		public static Dictionary<string, SkillType> SkillTypes => skillTypes;

		static SkillType() {
			Athletics._situation.canDefend = true;
			Fight._situation.canAttack = true;
			Fight._situation.canDefend = true;
			Fight._property.useRange = new Range() { low = 0, lowOpen = false, high = 1, highOpen = false };
			Physique._situation.canDefend = true;
			Provoke._situation.canAttack = true;
			Provoke._property.useRange = new Range() { low = 0, lowOpen = false, high = 4, highOpen = false };
			Will._situation.canDefend = true;
			skillTypes.Add(Athletics.ID, Athletics);
			skillTypes.Add(Burglary.ID, Burglary);
			skillTypes.Add(Contacts.ID, Contacts);
			skillTypes.Add(Crafts.ID, Crafts);
			skillTypes.Add(Deceive.ID, Deceive);
			skillTypes.Add(Drive.ID, Drive);
			skillTypes.Add(Empathy.ID, Empathy);
			skillTypes.Add(Fight.ID, Fight);
			skillTypes.Add(Investigate.ID, Investigate);
			skillTypes.Add(Lore.ID, Lore);
			skillTypes.Add(Notice.ID, Notice);
			skillTypes.Add(Physique.ID, Physique);
			skillTypes.Add(Provoke.ID, Provoke);
			skillTypes.Add(Rapport.ID, Rapport);
			skillTypes.Add(Resources.ID, Resources);
			skillTypes.Add(Shoot.ID, Shoot);
			skillTypes.Add(Stealth.ID, Stealth);
			skillTypes.Add(Will.ID, Will);
		}

		private readonly string _id;
		private readonly string _name;
		private SkillProperty _property;
		private SkillSituation _situation;

		public string ID => _id;
		public string Name => _name;
		public SkillProperty Property => _property;
		public SkillSituation Situation => _situation;

		private SkillType(string id, string name) {
			_id = id ?? throw new ArgumentNullException(nameof(id));
			_name = name ?? throw new ArgumentNullException(nameof(name));
			_property = SkillProperty.INIT;
		}

		public bool Equals(SkillType other) {
			return !(other is null) && _id == other._id;
		}

		public override bool Equals(object obj) {
			return this.Equals(obj as SkillType);
		}

		public override int GetHashCode() {
			return _id.GetHashCode();
		}

		public static bool operator ==(SkillType a, SkillType b) {
			return a.Equals(b);
		}

		public static bool operator !=(SkillType a, SkillType b) {
			return !(a == b);
		}
	}
}
