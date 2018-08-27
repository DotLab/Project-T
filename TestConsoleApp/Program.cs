using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Dynamic;
using System.Text;
using GameLib.Core;
using GameLib.Utilities.Network;
using GameLib.CharacterSystem;
using NetworkHelper = Networkf.NetworkHelper;
using NetworkService = Networkf.NetworkService;
using System.Threading;
using System.Linq;
using GameLib.Utilities;
using GameLib.Container;
using GameLib.Container.BattleComponent;
using GameLib.Campaign;

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
			var skillProperty = brught_jackson.GetSkillProperty(SkillType.Lore);
			skillProperty.level = 4;
			brught_jackson.SetSkillProperty(SkillType.Lore, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Investigate);
			skillProperty.level = 3;
			brught_jackson.SetSkillProperty(SkillType.Investigate, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Will);
			skillProperty.level = 3;
			brught_jackson.SetSkillProperty(SkillType.Will, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Empathy);
			skillProperty.level = 2;
			brught_jackson.SetSkillProperty(SkillType.Empathy, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Crafts);
			skillProperty.level = 2;
			brught_jackson.SetSkillProperty(SkillType.Crafts, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Notice);
			skillProperty.level = 2;
			brught_jackson.SetSkillProperty(SkillType.Notice, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Drive);
			skillProperty.level = 1;
			brught_jackson.SetSkillProperty(SkillType.Drive, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Contacts);
			skillProperty.level = 1;
			brught_jackson.SetSkillProperty(SkillType.Contacts, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Rapport);
			skillProperty.level = 1;
			brught_jackson.SetSkillProperty(SkillType.Rapport, skillProperty);
			skillProperty = brught_jackson.GetSkillProperty(SkillType.Stealth);
			skillProperty.level = 1;
			brught_jackson.SetSkillProperty(SkillType.Stealth, skillProperty);

			
			var ranbo = player2characters[0] = new KeyCharacter("PlayerRanbo", new CharacterView() { battle = "蓝波", story = "蓝波" });
			ranbo.Name = "蓝波";
			ranbo.Description = "从小就备受欺负的蓝波，坚信一身强健的肌肉可以保护自己，长大后成为了亚特兰大市民中心内的健身房教练，出生于美国中部州的他对政府极度不信任。";
			ranbo.PhysicsStressMax = 4;
			ranbo.MentalStressMax = 3;
			ranbo.Aspects.Add(new Aspect() { Name = "“所有人都在注视着我的肌肉！”", PersistenceType = PersistenceType.Fixed });
			ranbo.Aspects.Add(new Aspect() { Name = "政府阴谋论支持者", PersistenceType = PersistenceType.Fixed });
			ranbo.Aspects.Add(new Aspect() { Name = "体育中心的健身教练", PersistenceType = PersistenceType.Fixed });
			ranbo.FatePoint = ranbo.RefreshPoint = 3;
			skillProperty = ranbo.GetSkillProperty(SkillType.Physique);
			skillProperty.level = 4;
			ranbo.SetSkillProperty(SkillType.Physique, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Athletics);
			skillProperty.level = 3;
			ranbo.SetSkillProperty(SkillType.Athletics, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Drive);
			skillProperty.level = 3;
			ranbo.SetSkillProperty(SkillType.Drive, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Will);
			skillProperty.level = 2;
			ranbo.SetSkillProperty(SkillType.Will, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Shoot);
			skillProperty.level = 2;
			ranbo.SetSkillProperty(SkillType.Shoot, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Provoke);
			skillProperty.level = 2;
			ranbo.SetSkillProperty(SkillType.Provoke, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Resources);
			skillProperty.level = 1;
			ranbo.SetSkillProperty(SkillType.Resources, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Contacts);
			skillProperty.level = 1;
			ranbo.SetSkillProperty(SkillType.Contacts, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Lore);
			skillProperty.level = 1;
			ranbo.SetSkillProperty(SkillType.Lore, skillProperty);
			skillProperty = ranbo.GetSkillProperty(SkillType.Empathy);
			skillProperty.level = 1;
			ranbo.SetSkillProperty(SkillType.Empathy, skillProperty);


			var lily = player2characters[1] = new KeyCharacter("Lily", new CharacterView() { battle = "李莉", story = "李莉" });
			lily.Name = "李莉";
			lily.Description = "联邦调查局，负责人口失踪与贩卖类别的调查。由于移民身份总是受到同事的排挤。";
			lily.PhysicsStressMax = lily.MentalStressMax = 3;
			lily.Aspects.Add(new Aspect() { Name = "身手不凡的调查员", PersistenceType = PersistenceType.Fixed });
			lily.Aspects.Add(new Aspect() { Name = "种族问题", PersistenceType = PersistenceType.Fixed });
			lily.FatePoint = lily.RefreshPoint = 3;
			skillProperty = lily.GetSkillProperty(SkillType.Fight);
			skillProperty.level = 4;
			lily.SetSkillProperty(SkillType.Fight, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Shoot);
			skillProperty.level = 3;
			lily.SetSkillProperty(SkillType.Shoot, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Investigate);
			skillProperty.level = 3;
			lily.SetSkillProperty(SkillType.Investigate, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Physique);
			skillProperty.level = 2;
			lily.SetSkillProperty(SkillType.Physique, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Lore);
			skillProperty.level = 2;
			lily.SetSkillProperty(SkillType.Lore, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Athletics);
			skillProperty.level = 2;
			lily.SetSkillProperty(SkillType.Athletics, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Rapport);
			skillProperty.level = 1;
			lily.SetSkillProperty(SkillType.Rapport, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Empathy);
			skillProperty.level = 1;
			lily.SetSkillProperty(SkillType.Empathy, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Will);
			skillProperty.level = 1;
			lily.SetSkillProperty(SkillType.Will, skillProperty);
			skillProperty = lily.GetSkillProperty(SkillType.Notice);
			skillProperty.level = 1;
			lily.SetSkillProperty(SkillType.Notice, skillProperty);
			

			Player[] players = new Player[2];
			players[0] = new Player("Player1", "Player1", player1Connection, 1, player1characters);
			players[1] = new Player("Player2", "Player2", player2Connection, 2, player2characters);

			DM dm = new DM("DM", "DM", dmConnection);

			NetworkHelper.StartServer(OnConnectionEstablished);

			Game.Init(dm, players);

			TestInit();

			while (!Game.GameOver) {
				Game.Update();
				Thread.Sleep(100);
			}

			Game.Cleanup();

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		static void OnConnectionEstablished(NetworkService service) {
			var initializer = new NSInitializer(service);
			byte[] code = initializer.ServerRequireClientVerify();
			if (code != null) {
				if (code.SequenceEqual(dmVerificationCode)) {
					if (dmConnection.HasAppliedService()) {
						initializer.ServerApplyConnection(null);
					} else {
						initializer.ServerApplyConnection(dmConnection);
					}
				} else if (code.SequenceEqual(player1VerificationCode)) {
					if (player1Connection.HasAppliedService()) {
						initializer.ServerApplyConnection(null);
					} else {
						initializer.ServerApplyConnection(player1Connection);
					}
				} else if (code.SequenceEqual(player2VerificationCode)) {
					if (player2Connection.HasAppliedService()) {
						initializer.ServerApplyConnection(null);
					} else {
						initializer.ServerApplyConnection(player2Connection);
					}
				} else {
					initializer.ServerApplyConnection(null);
				}
			}
		}

		static void TestInit() {
			CampaignManager.Instance.CurrentContainer = ContainerType.BATTLE;

			var battleScene = BattleSceneContainer.Instance;
			battleScene.Reset(12, 12);

			var groundView = new CharacterView();
			groundView.battle = "地面";
			for (int i = 0; i < 12; ++i) {
				for (int j = 0; j < 12; ++j) {
					var ground = new GridObject(CharacterManager.Instance.CreateTemporaryCharacter(CharacterManager.DataLevel.Temporary, groundView), true);
					ground.Stagnate = 1;
					battleScene.PushGridObject(i, j, false, ground);
				}
			}

			battleScene.PushGridObject(1, 1, false, new ActableGridObject(player1characters[0]));
			battleScene.PushGridObject(2, 2, false, new ActableGridObject(player2characters[0]));
			battleScene.PushGridObject(3, 3, false, new ActableGridObject(player2characters[1]));

			battleScene.NewRound();
		}
	}
}
