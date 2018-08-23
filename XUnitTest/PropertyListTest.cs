using System;
using System.Collections.Generic;
using Xunit;
using System.Text;
using GameLogic.Core.ScriptSystem;
using GameLogic.CharacterSystem;
using Xunit.Abstractions;
using GameLogic.Core.ScriptSystem.EngineWrapper;

namespace XUnitTest {
	public class TestElement : ICharacterProperty {
		public static IJSAPI<TestElement> createTestElement() {
			return (IJSAPI<TestElement>)new TestElement().GetContext();
		}

		private class API : IJSAPI<TestElement> {
			private TestElement _outer;

			public API(TestElement outer) {
				_outer = outer;
			}

			public string name { get => _outer.Name; set => _outer.Name = value; }
			public string description { get => _outer.Description; set => _outer.Description = value; }

			public TestElement Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}

		private API _apiObj;
		private string _description;
		private string _name;

		public string ID => "";
		public Character Belong { get => null; set { } }
		public string Description { get => _description; set => _description = value; }
		public string Name { get => _name; set => _name = value; }

		public TestElement() {
			_apiObj = new API(this);
		}

		public TestElement(string description) :
			this() {
			_description = description;
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) {

		}
	}

	public class PropertyListTest {
		private readonly ITestOutputHelper _output;

		public PropertyListTest(ITestOutputHelper output) {
			_output = output;
		}

		[Fact]
		public void Add_Get_Remove_Test() {
			IJSEngineRaw engineRaw = new JintEngine();
			JSEngine engine = new JSEngine(engineRaw);

			CharacterPropertyList<TestElement> testList = new CharacterPropertyList<TestElement>(null);
			engineRaw.SetVar(nameof(TestElement.createTestElement), new Func<IJSAPI<TestElement>>(TestElement.createTestElement));
			engine.SynchronizeContext(nameof(testList), testList);
			engine.Execute("var ae = createTestElement(); ae.description = 'AddTest'; testList.add(ae);");
			Assert.True(testList.Count == 1);
			Assert.True(testList[0].Description == "AddTest");

			TestElement testElement = new TestElement("RemoveTest");
			testList.Add(testElement);
			engine.Execute("var re = testList.get(0); testList.remove(re);");
			Assert.True(testList.Count == 1);
			Assert.True(testList[0].Description == "RemoveTest");
		}

		[Fact]
		public void Foreach_Test() {
			IJSEngineRaw engineRaw = new JintEngine();
			JSEngine engine = new JSEngine(engineRaw);

			CharacterPropertyList<TestElement> testList = new CharacterPropertyList<TestElement>(null);
			engineRaw.SetVar("log", new Action<string>(_output.WriteLine));
			engineRaw.SetVar("count", 0);
			engine.SynchronizeContext(nameof(testList), testList);
			testList.Add(new TestElement("A"));
			testList.Add(new TestElement("B"));
			testList.Add(new TestElement("C"));
			testList.Add(new TestElement("D"));
			engine.Execute("testList.forEach(function(e){ e.description += ' Engine'; log(e.description); count++; });");
			engine.Execute("log(count);");
			foreach (TestElement e in testList) {
				Assert.Contains("Engine", e.Description);
			}
		}
	}
}
