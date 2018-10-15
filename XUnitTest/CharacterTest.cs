using GameServer.CharacterComponents;
using GameServer.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTest {
	public class CharacterTest {
		private readonly ITestOutputHelper _output;

		public CharacterTest(ITestOutputHelper output) {
			_output = output;
		}
		/*
		[Fact]
		public void DerivedAPI_Test() {
			var testChara = new KeyCharacter("character1", new GameUtil.CharacterView() { battle = "1", story = "1" });
			var testChara2 = new KeyCharacter("character2", new GameUtil.CharacterView() { battle = "2", story = "2" });
			testChara.Consequences.Add(new Consequence());
			testChara.Consequences.Add(new Consequence() { Name = "eeee" });
			string code = @"var consequences = testChara.getConsequenceList();
			consequences.forEach(function(item) {
				log(item.getName());
				item.setName('test');
			});";

			JSEngineManager.EngineRaw.SetVar("log", new Action<string>(_output.WriteLine));
			JSEngineManager.Engine.SynchronizeContext(nameof(testChara), testChara);
			JSEngineManager.Engine.SynchronizeContext(nameof(testChara2), testChara2);
			JSEngineManager.Engine.SynchronizeContext("characterManager", CharacterManager.Instance);
			JSEngineManager.Engine.Execute(code);
			Assert.True(testChara.Consequences[0].Name == "test");
		}
		*/
		[Fact]
		public void AddAspect_Test() {
			var testChara = new KeyCharacter("character1", new GameUtil.CharacterView() { battle = "1", story = "1" });
			var testChara2 = new KeyCharacter("character2", new GameUtil.CharacterView() { battle = "2", story = "2" });
			string code = @"var enemy = testChara;
			var boost = characterManager.createAspect();
			boost.setPersistenceType(2);
			boost.setName('眩晕');
			boost.setBenefiter(testChara2);
			boost.setBenefitTimes(1);
			enemy.getAspectList().add(boost);";

			JSEngineManager.EngineRaw.SetVar("log", new Action<string>(_output.WriteLine));
			JSEngineManager.Engine.SynchronizeContext(nameof(testChara), testChara);
			JSEngineManager.Engine.SynchronizeContext(nameof(testChara2), testChara2);
			JSEngineManager.Engine.SynchronizeContext("characterManager", CharacterManager.Instance);
			JSEngineManager.Engine.Execute(code);
			Assert.True(testChara.Aspects.Count == 1);
			Assert.True(testChara.Aspects[0].Name == "眩晕");
		}
	}
}
