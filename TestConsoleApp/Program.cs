using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Dynamic;
using System.Text;
using GameServer.Core;
using GameUtil.Network;
using GameServer.CharacterSystem;
using NetworkHelper = Networkf.NetworkHelper;
using NetworkService = Networkf.NetworkService;
using System.Threading;
using System.Linq;
using GameUtil;
using GameServer.Container;
using GameServer.Container.BattleComponent;
using GameServer.Campaign;

namespace TestConsoleApp {
	class Program {
		static readonly byte[] dmVerificationCode = { 0x00, 0x10, 0x20, 0xAB };
		static readonly byte[] player1VerificationCode = { 0x00, 0x10, 0x20, 0x3B };
		static readonly byte[] player2VerificationCode = { 0x00, 0x10, 0x20, 0xC5 };

		static readonly NetworkfConnection dmConnection = new NetworkfConnection();
		static readonly NetworkfConnection player1Connection = new NetworkfConnection();
		static readonly NetworkfConnection player2Connection = new NetworkfConnection();

		static readonly KeyCharacter[] player1characters = new KeyCharacter[1];
		static readonly KeyCharacter[] player2characters = new KeyCharacter[2];

		static void Main(string[] args) {
			var brught_jackson = player1characters[0] = new KeyCharacter("PlayerBrughtJackson", new CharacterView() { battle = "布鲁特", story = "布鲁特·杰克逊" });
			brught_jackson.Name = "布鲁特·杰克逊";
			brught_jackson.Description = "正在读大学的布鲁特是他所成长的街区内这一辈年轻人里唯一一个拿到私立大学奖学金的人，被考古专业录取的他对于一切古老而神秘的事务充满好奇。";
			brught_jackson.PhysicsStressMax = 2;
			brught_jackson.MentalStressMax = 4;
			brught_jackson.Aspects.Add(new Aspect() { Name = "年少老成", PersistenceType = PersistenceType.Fixed });
			brught_jackson.Aspects.Add(new Aspect() { Name = "痴迷于古代文化", PersistenceType = PersistenceType.Fixed });
			brught_jackson.Aspects.Add(new Aspect() { Name = "相较于发达大脑的贫弱体格", PersistenceType = PersistenceType.Fixed });
			brught_jackson.FatePoint = brught_jackson.RefreshPoint = 3;
			brught_jackson.GetSkill(SkillType.Lore).Level = 4;
			brught_jackson.GetSkill(SkillType.Investigate).Level = 3;
			brught_jackson.GetSkill(SkillType.Will).Level = 3;
			brught_jackson.GetSkill(SkillType.Empathy).Level = 2;
			brught_jackson.GetSkill(SkillType.Crafts).Level = 2;
			brught_jackson.GetSkill(SkillType.Notice).Level = 2;
			brught_jackson.GetSkill(SkillType.Drive).Level = 1;
			brught_jackson.GetSkill(SkillType.Contacts).Level = 1;
			brught_jackson.GetSkill(SkillType.Rapport).Level = 1;
			brught_jackson.GetSkill(SkillType.Stealth).Level = 1;
			
			var stunt1 = new Stunt(new InitiativeEffect(new Command("专家主动效果", Resource.Stunt1JSCode)));
			stunt1.Name = "专家";
			stunt1.Description = "你声明一个专业领域，例如草药学，犯罪学或者动物学。在你针对那个领域相关的内容进行学识掷骰时，你获得 + 2 的掷骰结果。";
			var limit1 = new StuntSituationLimit() {
				canUseOnInteract = true,
				usableSituation = CharacterAction.CREATE_ASPECT | CharacterAction.HINDER,
				resistableSituation = CharacterAction.CREATE_ASPECT | CharacterAction.HINDER
			};
			stunt1.SituationLimit = limit1;
			stunt1.BattleMapProperty = brught_jackson.GetSkill(SkillType.Lore).BattleMapProperty;
			stunt1.CopyResistTable(SkillType.Lore);
			brught_jackson.Stunts.Add(stunt1);
			
			var stunt2 = new Stunt(new InitiativeEffect(new Command("决心的力量主动效果", Resource.Stunt2JSCode)));
			stunt2.Name = "决心的力量";
			stunt2.Description = "任何被动掷骰中都可以使用意志代替体格，这表示超凡的力量。";
			var limit2 = new StuntSituationLimit() {
				canUseOnInteract = false,
				usableSituation = 0,
				resistableSituation = CharacterAction.ATTACK | CharacterAction.CREATE_ASPECT | CharacterAction.HINDER
			};
			stunt2.SituationLimit = limit2;
			stunt2.BattleMapProperty = brught_jackson.GetSkill(SkillType.Will).BattleMapProperty;
			stunt2.CopyResistTable(SkillType.Physique);
			stunt2.PassiveEffects.Add(new PassiveEffect("event.battleScene.onceCheckOver", new Command("决心的力量被动效果", Resource.Stunt2Trigger1JSCode)));
			brught_jackson.Stunts.Add(stunt2);


			var ranbo = player2characters[0] = new KeyCharacter("PlayerRanbo", new CharacterView() { battle = "蓝波", story = "蓝波" });
			ranbo.Name = "蓝波";
			ranbo.Description = "从小就备受欺负的蓝波，坚信一身强健的肌肉可以保护自己，长大后成为了亚特兰大市民中心内的健身房教练，出生于美国中部州的他对政府极度不信任。";
			ranbo.PhysicsStressMax = 4;
			ranbo.MentalStressMax = 3;
			ranbo.Aspects.Add(new Aspect() { Name = "“所有人都在注视着我的肌肉！”", PersistenceType = PersistenceType.Fixed });
			ranbo.Aspects.Add(new Aspect() { Name = "政府阴谋论支持者", PersistenceType = PersistenceType.Fixed });
			ranbo.Aspects.Add(new Aspect() { Name = "体育中心的健身教练", PersistenceType = PersistenceType.Fixed });
			ranbo.FatePoint = ranbo.RefreshPoint = 3;
			ranbo.GetSkill(SkillType.Physique).Level = 4;
			ranbo.GetSkill(SkillType.Athletics).Level = 3;
			ranbo.GetSkill(SkillType.Drive).Level = 3;
			ranbo.GetSkill(SkillType.Will).Level = 2;
			ranbo.GetSkill(SkillType.Shoot).Level = 2;
			ranbo.GetSkill(SkillType.Provoke).Level = 2;
			ranbo.GetSkill(SkillType.Resources).Level = 1;
			ranbo.GetSkill(SkillType.Contacts).Level = 1;
			ranbo.GetSkill(SkillType.Lore).Level = 1;
			ranbo.GetSkill(SkillType.Empathy).Level = 1;
			
			var stunt3 = new Stunt(new InitiativeEffect(new Command("", "")));
			stunt3.Name = "晕眩反击";
			stunt3.Description = "每当你在对抗对手格斗技能的防御行动中获得大成功时，你自动反击对手。你可以免费在你的对手身上激活晕眩特性。";
			stunt3.PassiveEffects.Add(new PassiveEffect("event.battleScene.onceCheckOver", new Command("晕眩反击被动效果", Resource.Stunt3Trigger1JSCode)));
			ranbo.Stunts.Add(stunt3);
			
			var stunt4 = new Stunt(new InitiativeEffect(new Command("坚韧如钉主动效果", Resource.Stunt4JSCode)));
			stunt4.UsingCondition = new Condition(new Command("坚韧如钉使用判定", Resource.Stunt4UsingConditionJSCode));
			stunt4.TargetCondition = new Condition(new Command("坚韧如钉目标判定", Resource.Stunt4TargetConditionJSCode));
			stunt4.Name = "坚韧如钉";
			stunt4.Description = "每局游戏一次，消耗一点命运点数，你可以将一个物理性的中度伤痕减弱为轻微（如果你的轻微不良状态槽正空着），或者完全解除一个轻微伤痕。";
			var limit4 = new StuntSituationLimit() {
				canUseOnInteract = true,
				usableSituation = 0,
				resistableSituation = 0
			};
			stunt4.SituationLimit = limit4;
			ranbo.Stunts.Add(stunt4);


			var lily = player2characters[1] = new KeyCharacter("PlayerLily", new CharacterView() { battle = "李莉", story = "李莉" });
			lily.Name = "李莉";
			lily.Description = "联邦调查局，负责人口失踪与贩卖类别的调查。由于移民身份总是受到同事的排挤。";
			lily.PhysicsStressMax = lily.MentalStressMax = 3;
			lily.Aspects.Add(new Aspect() { Name = "身手不凡的调查员", PersistenceType = PersistenceType.Fixed });
			lily.Aspects.Add(new Aspect() { Name = "种族问题", PersistenceType = PersistenceType.Fixed });
			lily.FatePoint = lily.RefreshPoint = 3;
			lily.GetSkill(SkillType.Fight).Level = 4;
			lily.GetSkill(SkillType.Shoot).Level = 3;
			lily.GetSkill(SkillType.Investigate).Level = 3;
			lily.GetSkill(SkillType.Physique).Level = 2;
			lily.GetSkill(SkillType.Lore).Level = 2;
			lily.GetSkill(SkillType.Athletics).Level = 2;
			lily.GetSkill(SkillType.Rapport).Level = 1;
			lily.GetSkill(SkillType.Empathy).Level = 1;
			lily.GetSkill(SkillType.Will).Level = 1;
			lily.GetSkill(SkillType.Notice).Level = 1;
			
			var stunt5 = new Stunt(new InitiativeEffect(new Command("", "")));
			stunt5.UsingCondition = new Condition(new Command("备用武器使用判定", Resource.Stunt5UsingConditionJSCode));
			stunt5.Name = "备用武器";
			stunt5.Description = "每当其他人在你身上创造“缴械”、“武器落手”或类似的情景特征时，你可以花费1 个命运点来宣告细节，表示你带着备用武器，对手将因此而无法创造情景特征，但他可以获得一个增益，以此表示你在不得不更换武器时所出现的短时间分心。";
			stunt5.PassiveEffects.Add(new PassiveEffect("event.battleScene.onceCheckOverAspectCreated", new Command("备用武器被动效果", Resource.Stunt5Trigger1JSCode)));
			lily.Stunts.Add(stunt5);

			var pistol = new Extra(CharacterManager.Instance.CreateCharacterWithSaving(CharacterManager.DataLevel.Common, "ItemPistol", new CharacterView() { battle = "手枪", story = "手枪" }));
			pistol.Name = "M1911手枪";
			pistol.Description = "一种在1911年起生产的.45 ACP口径半自动手枪。";
			pistol.IsLongRangeWeapon = true;
			lily.Extras.Add(pistol);

			var stunt6 = new Stunt(new InitiativeEffect(new Command("洞若观火主动效果", Resource.Stunt6JSCode)));
			stunt6.Name = "洞若观火";
			stunt6.Description = "你可以使用调查技能代替洞察技能来防御欺诈。其他人凭借本能反应和直觉感知的信息，你可以通过仔细观察别人的细微表情来获悉。";
			var limit6 = new StuntSituationLimit() {
				canUseOnInteract = false,
				usableSituation = 0,
				resistableSituation = CharacterAction.ATTACK | CharacterAction.CREATE_ASPECT | CharacterAction.HINDER
			};
			stunt6.SituationLimit = limit6;
			stunt6.BattleMapProperty = lily.GetSkill(SkillType.Investigate).BattleMapProperty;
			stunt6.CopyResistTable(SkillType.Notice);
			stunt6.PassiveEffects.Add(new PassiveEffect("event.battleScene.onceCheckOver", new Command("洞若观火被动效果", Resource.Stunt6Trigger1JSCode)));
			lily.Stunts.Add(stunt6);

			var franz = CharacterManager.Instance.CreateCharacterWithSaving(CharacterManager.DataLevel.Key, "NPCFranz", new CharacterView { battle = "弗兰兹", story = "弗兰兹" });
			franz.Name = "弗兰兹·欧洛克";
			franz.Description = "弗兰兹·欧洛克本是一名前途似锦的足球新星，但一切都在本赛季的第一场球赛前的一次私人会面后改变了。一个自称特拉维尔爵士的人找到了他，并告诉他，在他的体内拥有着古老而高贵的血族血统，只不过还没有觉醒。在那个“爵士”的女秘书掏出那块怀表之前，欧洛克只当这个人不过是一个江湖骗子而已，但现在他对此深信不疑，毕竟获得永生总是赚到了。";
			franz.PhysicsStressMax = 5;
			franz.MentalStressMax = 4;
			franz.Aspects.Add(new Aspect() { Name = "天降死神", PersistenceType = PersistenceType.Fixed });
			franz.Aspects.Add(new Aspect() { Name = "吸血鬼诅咒", PersistenceType = PersistenceType.Fixed });
			franz.Aspects.Add(new Aspect() { Name = "多疑的秃脑袋", PersistenceType = PersistenceType.Fixed });
			franz.FatePoint = franz.RefreshPoint = 3;
			franz.GetSkill(SkillType.Athletics).Level = 5;
			franz.GetSkill(SkillType.Notice).Level = 4;
			franz.GetSkill(SkillType.Physique).Level = 4;
			franz.GetSkill(SkillType.Stealth).Level = 3;
			franz.GetSkill(SkillType.Fight).Level = 3;
			franz.GetSkill(SkillType.Deceive).Level = 2;
			franz.GetSkill(SkillType.Crafts).Level = 1;
			franz.GetSkill(SkillType.Lore).Level = 1;
			franz.GetSkill(SkillType.Will).Level = 1;

			var stunt7 = new Stunt(new InitiativeEffect(new Command("掠空攻击主动效果", Resource.Stunt7JSCode)));
			stunt7.Name = "掠空攻击";
			stunt7.Description = "从空中滑翔而下，在飞行的最低点攻击目标。如果目标足够轻，则将其携带至高空中摔死。";
			var limit7 = new StuntSituationLimit() {
				canUseOnInteract = false,
				usableSituation = CharacterAction.ATTACK,
				resistableSituation = 0
			};
			stunt7.SituationLimit = limit7;
			var mapProperty7 = franz.GetSkill(SkillType.Athletics).BattleMapProperty;
			mapProperty7.useRange = new Range() { low = 0, lowOpen = false, high = 8, highOpen = false };
			stunt7.BattleMapProperty = mapProperty7;
			stunt7.PassiveEffects.Add(new PassiveEffect("event.battleScene.onceCheckOver", new Command("掠空攻击被动效果", Resource.Stunt7Trigger1JSCode)));
			franz.Stunts.Add(stunt7);

			var stunt8 = new Stunt(new InitiativeEffect(new Command("", "")));
			stunt8.Name = "不死之身";
			stunt8.Description = "作为吸血鬼，除非受到阳光（紫外线）和银制物品的攻击，否则不会受到伤害，就算有外伤也会迅速愈合。生理压力槽 + 2，精神压力槽 + 1。";
			stunt8.PassiveEffects.Add(new PassiveEffect("event.battleScene.onceCheckOverCauseDamage", new Command("不死之身被动效果", Resource.Stunt8Trigger1JSCode)));
			franz.Stunts.Add(stunt8);

			var emma = CharacterManager.Instance.CreateCharacterWithSaving(CharacterManager.DataLevel.Common, "NPCEmma", new CharacterView { battle = "艾玛", story = "艾玛" });
			emma.Name = "艾玛·蒂姆";
			emma.Description = "艾玛·蒂姆从小就是一个富有正义感的迷人女孩儿，为人处世从来都遵从自己的那一套“正义”准则。在成年后选择做一名警察后，这种天性变得更加不可收拾，每每不考虑后果就先鲁莽行事，好在她的运气就和她的正义感一样丰富，一次次帮助她化险为夷。这次的行动也是她自己率性而为，根据手中的证据前来这里探查结果却遇见了一场邪恶仪式的举行，还将自己陷入无解的困境之中。";
			emma.PhysicsStressMax = 3;
			emma.MentalStressMax = 2;
			emma.Aspects.Add(new Aspect() { Name = "我要做正义的伙伴！", PersistenceType = PersistenceType.Fixed });
			emma.Aspects.Add(new Aspect() { Name = "无知者无畏", PersistenceType = PersistenceType.Fixed });
			emma.Aspects.Add(new Aspect() { Name = "警局中的红发安妮", PersistenceType = PersistenceType.Fixed });
			emma.GetSkill(SkillType.Fight).Level = 3;
			emma.GetSkill(SkillType.Shoot).Level = 3;
			emma.GetSkill(SkillType.Investigate).Level = 2;
			emma.GetSkill(SkillType.Stealth).Level = 2;
			emma.GetSkill(SkillType.Physique).Level = 2;
			emma.GetSkill(SkillType.Lore).Level = 1;
			emma.GetSkill(SkillType.Athletics).Level = 1;
			emma.GetSkill(SkillType.Will).Level = 1;
			emma.GetSkill(SkillType.Resources).Level = 1;

			var stunt9 = new Stunt(new InitiativeEffect(new Command("爆发射击主动效果", Resource.Stunt9JSCode)));
			stunt9.UsingCondition = new Condition(new Command("爆发射击使用判定", Resource.Stunt9UsingConditionJSCode));
			stunt9.Name = "爆发射击（雷明顿M870霰弹枪）";
			stunt9.Description = "在攻击检定结果上获得 + 2 的奖励，并且可以攻击同方向至多3 个目标。";
			var limit9 = new StuntSituationLimit() {
				canUseOnInteract = false,
				usableSituation = CharacterAction.ATTACK,
				resistableSituation = 0
			};
			stunt9.SituationLimit = limit9;
			var mapProperty9 = emma.GetSkill(SkillType.Shoot).BattleMapProperty;
			mapProperty9.affectRange = new Range() { low = 0, lowOpen = false, high = 3, highOpen = false };
			mapProperty9.targetMaxCount = 3;
			stunt9.BattleMapProperty = mapProperty9;
			emma.Stunts.Add(stunt9);

			var gun = new Extra(CharacterManager.Instance.CreateCharacterWithSaving(CharacterManager.DataLevel.Common, "ItemGun", new CharacterView() { battle = "霰弹枪", story = "霰弹枪" }));
			gun.Name = "雷明顿M870霰弹枪";
			gun.Description = "一种近距离杀伤性武器，是雷明顿兵工厂于50年代初研制成功的，它一直是美国军、警界的专用装备。";
			gun.IsLongRangeWeapon = true;
			emma.Extras.Add(gun);

			/*
			var lance = CharacterManager.Instance.CreateCharacterWithSaving(CharacterManager.DataLevel.Common, "NPCLance", new CharacterView { battle = "兰斯", story = "兰斯" });
			lance.Name = "兰斯·帕特洛西";
			lance.Description = "兰斯·帕特洛西来自一个富足的商人家庭，生性内敛的他并没有成为一名花花公子，相反成为了一名彬彬有礼的绅士，但是在一次意外的冲突之中，他发现一定的自我保护是必须的，甚至他需要有保护别人的能力。之后他志愿成为一名海军陆战队员，背着家里偷偷参军并在伊拉克待了2年多，最后被家人强行带回了美国，进入了国民警卫队这种“安全”的部队。";
			lance.PhysicsStressMax = 4;
			lance.MentalStressMax = 3;
			lance.Aspects.Add(new Aspect() { Name = "人类的伟大，就是勇气的伟大！", PersistenceType = PersistenceType.Fixed });
			lance.Aspects.Add(new Aspect() { Name = "一三得三，二三得九，三八妇女节~", PersistenceType = PersistenceType.Fixed });
			lance.Aspects.Add(new Aspect() { Name = "循规蹈矩的好孩子", PersistenceType = PersistenceType.Fixed });
			lance.GetSkill(SkillType.Shoot).Level = 4;
			lance.GetSkill(SkillType.Fight).Level = 3;
			lance.GetSkill(SkillType.Athletics).Level = 3;
			lance.GetSkill(SkillType.Investigate).Level = 2;
			lance.GetSkill(SkillType.Will).Level = 2;
			lance.GetSkill(SkillType.Notice).Level = 2;
			lance.GetSkill(SkillType.Stealth).Level = 1;
			lance.GetSkill(SkillType.Resources).Level = 1;
			*/

			ranbo.MakePartyWith(brught_jackson);
			lily.MakePartyWith(brught_jackson);
			emma.MakePartyWith(brught_jackson);

			brught_jackson.Token = ranbo.Token = lily.Token = CharacterToken.PLAYER;
			emma.Token = CharacterToken.FRIENDLY;
			franz.Token = CharacterToken.HOSTILE;

			Player[] players = new Player[2];
			players[0] = new Player("Player1", "Player1", player1Connection, 1, player1characters);
			players[1] = new Player("Player2", "Player2", player2Connection, 2, player2characters);

			DM dm = new DM("DM", "DM", dmConnection);

			Game.InitGame(dm, players);

			NetworkHelper.StartServer(OnConnectionEstablished);

			CampaignManager.Instance.CurrentContainer = ContainerType.BATTLE;
			
			var battleScene = BattleSceneContainer.Instance;
			battleScene.Reset(8, 8);

			var groundView = new CharacterView();
			var stelaeView = new CharacterView();
			groundView.battle = "地面";
			stelaeView.battle = "石柱";
			for (int i = 0; i < 8; ++i) {
				for (int j = 0; j < 8; ++j) {
					var ground = new GridObject(CharacterManager.Instance.CreateTemporaryCharacter(CharacterManager.DataLevel.Temporary, groundView));
					ground.CharacterRef.PhysicsInvincible = true;
					ground.CharacterRef.MentalInvincible = true;
					ground.Stagnate = 1;
					battleScene.PushGridObject(i, j, false, ground);
				}
			}
			var stelae1 = new GridObject(CharacterManager.Instance.CreateTemporaryCharacter(CharacterManager.DataLevel.Temporary, stelaeView));
			var stelae2 = new GridObject(CharacterManager.Instance.CreateTemporaryCharacter(CharacterManager.DataLevel.Temporary, stelaeView));
			var stelae3 = new GridObject(CharacterManager.Instance.CreateTemporaryCharacter(CharacterManager.DataLevel.Temporary, stelaeView));
			var stelae4 = new GridObject(CharacterManager.Instance.CreateTemporaryCharacter(CharacterManager.DataLevel.Temporary, stelaeView));
			stelae1.Obstacle = stelae2.Obstacle = stelae3.Obstacle = stelae4.Obstacle = true;
			stelae1.CharacterRef.MentalInvincible = stelae2.CharacterRef.MentalInvincible = stelae3.CharacterRef.MentalInvincible = stelae4.CharacterRef.MentalInvincible = true;
			stelae1.CharacterRef.PhysicsStressMax = stelae2.CharacterRef.PhysicsStressMax = stelae3.CharacterRef.PhysicsStressMax = stelae4.CharacterRef.PhysicsStressMax = 12;
			battleScene.PushGridObject(1, 1, false, stelae1);
			battleScene.PushGridObject(6, 1, false, stelae2);
			battleScene.PushGridObject(1, 6, false, stelae3);
			battleScene.PushGridObject(5, 5, false, stelae4);

			battleScene.PushGridObject(7, 6, false, new ActableGridObject(player1characters[0]));
			battleScene.PushGridObject(7, 5, false, new ActableGridObject(player2characters[0]));
			battleScene.PushGridObject(7, 4, false, new ActableGridObject(player2characters[1]));

			battleScene.PushGridObject(4, 3, false, new ActableGridObject(franz));
			battleScene.PushGridObject(4, 4, false, new ActableGridObject(emma));

			battleScene.StartBattle();

			Game.RunGameLoop();
		}

		static void OnConnectionEstablished(NetworkService service) {
			var initializer = new NSInitializer(service);
			byte[] code = initializer.ServerRequireClientVerify();
			if (code != null) {
				if (code.SequenceEqual(dmVerificationCode)) {
					if (dmConnection.Available()) {
						initializer.ServerApplyConnection(null);
					} else {
						initializer.ServerApplyConnection(dmConnection);
					}
				} else if (code.SequenceEqual(player1VerificationCode)) {
					if (player1Connection.Available()) {
						initializer.ServerApplyConnection(null);
					} else {
						initializer.ServerApplyConnection(player1Connection);
					}
				} else if (code.SequenceEqual(player2VerificationCode)) {
					if (player2Connection.Available()) {
						initializer.ServerApplyConnection(null);
					} else {
						initializer.ServerApplyConnection(player2Connection);
					}
				} else {
					initializer.ServerApplyConnection(null);
				}
			}
		}
		
	}
}
