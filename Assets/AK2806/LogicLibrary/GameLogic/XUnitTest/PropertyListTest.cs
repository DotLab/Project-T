using System;
using System.Collections.Generic;
using Xunit;
using System.Text;
using GameLogic.Core.ScriptSystem;
using GameLogic.CharacterSystem;
using Xunit.Abstractions;

namespace XUnitTest
{
    public class TestElement : IProperty
    {
        public static IJSAPI createTestElement()
        {
            return (IJSAPI)new TestElement().GetContext();
        }

        private class API : IJSAPI
        {
            private TestElement _outer;

            public API(TestElement outer)
            {
                _outer = outer;
            }

            public string description { get => _outer.Description; set => _outer.Description = value; }

            public IJSContextProvider Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private API _apiObj;
        private string _description;

        public Character Belong { get => null; set { } }
        public string Description { get => _description; set => _description = value; }

        public TestElement()
        {
            _apiObj = new API(this);
        }

        public TestElement(string description) :
            this()
        {
            _description = description;
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context)
        {

        }
    }

    public class PropertyListTest
    {
        private readonly ITestOutputHelper _output;

        public PropertyListTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Add_Get_Remove_Test()
        {
            PropertyList<TestElement> testList = new PropertyList<TestElement>(null);
            JSEngineManager.EngineRaw.SetVar(nameof(TestElement.createTestElement), new Func<IJSAPI>(TestElement.createTestElement));
            JSEngineManager.Engine.SynchronizeContext(nameof(testList), testList);
            JSEngineManager.Engine.Execute("var ae = createTestElement(); ae.description = 'AddTest'; testList.add(ae);");
            Assert.True(testList.Count == 1);
            Assert.True(testList[0].Description == "AddTest");

            TestElement testElement = new TestElement("RemoveTest");
            testList.Add(testElement);
            JSEngineManager.Engine.Execute("var re = testList.get(0); testList.remove(re);");
            Assert.True(testList.Count == 1);
            Assert.True(testList[0].Description == "RemoveTest");

        }

        [Fact]
        public void Foreach_Test()
        {
            PropertyList<TestElement> testList = new PropertyList<TestElement>(null);
            JSEngineManager.EngineRaw.SetVar("log", new Action<string>(_output.WriteLine));
            JSEngineManager.Engine.SynchronizeContext(nameof(testList), testList);
            testList.Add(new TestElement("A"));
            testList.Add(new TestElement("B"));
            testList.Add(new TestElement("C"));
            testList.Add(new TestElement("D"));
            JSEngineManager.Engine.Execute("testList.forEach(function(e){ e.description += ' Engine'; log(e.description); });");
            foreach (TestElement e in testList)
            {
                Assert.Contains("Engine", e.Description);
            }
        }
    }
}
