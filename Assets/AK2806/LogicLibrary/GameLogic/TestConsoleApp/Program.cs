using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Dynamic;
using System.Text;

namespace TestConsoleApp
{
    public interface IBase
    {
        void Test();
    }

    public class DerivedA : IBase
    {
        public virtual void Test()
        {
            Console.WriteLine("Derived A");
        }

        public void Test2()
        {
            Console.WriteLine("Test 2  A");
        }

        public virtual void Test3()
        {
            Console.WriteLine("Test 3 A");
        }
    }

    public class DerivedB : DerivedA
    {
        private int x = 1;
        
        public override void Test()
        {
            base.Test();
            Console.WriteLine("Derived B");
        }

        public new void Test2()
        {
            base.Test2();
            Console.WriteLine("Test 2  B");
        }

        public new void Test3()
        {
            base.Test3();
            Console.WriteLine("Test 3 B");
        }
    }

    public struct Stest
    {
        public int a;
        public int b;
        public string str;
        
        public Stest(int a, int b, string str)
        {
            this.a = a;
            this.b = b;
            this.str = str;
        }
    }
    /*
    public class VarTest : IJSContextProvider
    {
        public struct Var
        {
            public int x;
            public int y;
        }

        private Var _var;

        public object GetContext()
        {
            return _var;
        }

        public void SetContext(object context)
        {
            _var = (Var)context;
        }

        public int VarX()
        {
            return _var.x;
        }

        public int VarY()
        {
            return _var.y;
        }

    }

    public class VarTest2 : IJSContextProvider
    {
        private struct Var
        {
            public string s;
        }

        private Var _var;

        public object GetContext()
        {
            return _var;
        }

        public void SetContext(object context)
        {
            _var = (Var)context;
        }

        public string VarS()
        {
            return _var.s;
        }
        
    }

    public class ApiTest : IJSContextProvider
    {
        private int x;
        private Dictionary<string, List<Stest>> dict;
        private Inner _inner;
        public string testStr;

        private class Inner
        {
            private ApiTest _outer;
            private string _eee="222";
            



            public Inner(ApiTest outer)
            {
                _outer = outer;
            }

            public ApiTest Outer { get => _outer; set => _outer = value; }

            public void Foo()
            {
                _outer.x++;
            }


        }

        public Dictionary<string, List<Stest>> Dict => dict;
        public int X { get => x; }

        public ApiTest()
        {
            this.dict = new Dictionary<string, List<Stest>>();
            _inner = new Inner(this);
            //testStr = "";
        }

        public int TestMethod(int v)
        {
            return v * v;
        }
        
        public object GetContext()
        {
            return _inner;
        }

        public void SetContext(object context)
        {
            
        }

        public override string ToString()
        {
            return "1234";
        }
    }

    public enum TestEnum
    {
        A,B,C,D
    }
    public class TestElement : ICharacterProperty
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

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context)
        {

        }
    }
    */

    class Program
    {
        static void Foo(int x)
        {
            Console.WriteLine(x);
        }

        static void Main(string[] args)
        {
            /*
            PropertyList < TestElement > testList = new PropertyList<TestElement>(null);
            JSEngineManager.EngineRaw.SetVar("log", new Action<string>(Console.WriteLine));
            JSEngineManager.Engine.SynchronizeContext(nameof(testList), testList);
            testList.Add(new TestElement("A"));
            testList.Add(new TestElement("B"));
            testList.Add(new TestElement("C"));
            testList.Add(new TestElement("D"));
            JSEngineManager.Engine.Execute("function foo(e){ e.description += ' Engine'; log(e.description); }  testList.forEach(foo);");
            */
            /*
            JintEngine engine = new JintEngine();
            int[] intarray = { 1, 5, 3, 4, 5 };
            dynamic expandoObject = new ExpandoObject();
            engine.SetVar("log", new Action<object>(Console.WriteLine));
            engine.SetVar("$", expandoObject);
            expandoObject.intarray = intarray;
            engine.Execute(@"
                (function () {
                    var i = 0;
                    for (; i < $.intarray.length; i++)
                        log($.intarray[i]);
                })();
            ");
            engine.SetVar("$", intarray);
            engine.Execute(@"
                (function () {
                    var i = 0;
                    for (; i < $.length; i++)
                        log($[i]);
                })();
            ");
            */
            IEnumerable<object> objs = new object[] { "33", "44" };
            IEnumerable<string> strs = new string[] { "33", "44" };
            Console.WriteLine(strs is IEnumerable<object>);
            
            /*
            string json = @"
                {
                    'name':'John',
                    'age':32,
                    'phoneNum':[
                        1234567,
                        9876543
                    ]
                }
            ";
            
            object test = JsonConvert.DeserializeObject(json);
            Console.WriteLine(test.GetHashCode());
            */
            /*
            JSEngineManager.EngineRaw.BindType("Stest", typeof(Stest));
            JSEngineManager.EngineRaw.SetVar("log", new Action<object>(Console.WriteLine));
            JSEngineManager.EngineRaw.SetVar("foo", new Action<int>(Foo));
            try
            {
                JSEngineManager.EngineRaw.Execute("var s = new Stest(); s.w = 'aaa'; log(s.w);");
            } catch (JSException e)
            {
                Console.WriteLine(e.Message);
            }
            */

            /*
            ApiTest test = new ApiTest();
            
            JSEngineManager.EngineRaw.SetVar("testRaw", test);
            JSEngineManager.Engine.SynchronizeContext("test", test);
            JSEngineManager.Engine.Execute("log(test._eee == undefined? '1':'0');");
            JSEngineManager.Engine.Execute("test.Foo();");
            Console.WriteLine(test.X);
            JSEngineManager.Engine.Execute("test.Foo();");
            Console.WriteLine(test.X);
            JSEngineManager.Engine.Execute("log(test.Outer.Dict.Count);");

            object i = 1;
            string s = (string)i;
            Console.WriteLine(s);
            */
            /*
            VarTest varTest = new VarTest();
            JSEngineManager.Engine.SynchronizeContext("test", varTest);
            Console.WriteLine(varTest.VarX() + ", " + varTest.VarY());
            TestEnum testEnum = TestEnum.B;
            JSEngineManager.EngineRaw.SetVar("testEnum", testEnum);
            JSEngineManager.Engine.Execute("log(testEnum);");
            Console.WriteLine(varTest.VarX() + ", " + varTest.VarY());
            JSEngineManager.Engine.SynchronizeContext("test", varTest);
            Console.WriteLine(varTest.VarX() + ", " + varTest.VarY());
            JSEngineManager.Engine.Execute("var e = {}; e.a = 3; log(e.a);");
            */
            /*
            Engine engine = new Engine();
            try
            {
                engine.Execute("function test(){ test(); } test();");
            }
            catch (ParserException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (JavaScriptException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (RecursionDepthOverflowException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (StatementsCountOverflowException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (StackOverflowException e)
            {
                Console.WriteLine(e.Message);
            }
            */

            /*
            Test test = new Test();
            List<Stest> list = new List<Stest>();
            list.Add(new Stest(1, 1, "ss"));
            list.Add(new Stest(2, 3, "aaaaddd"));
            test.Dict.Add("aaa", list);
            string json = JsonConvert.SerializeObject(test);
            Console.WriteLine(json);
            */

            /*
            string x = "";
            var engine = new Engine()
                .SetValue("x", x)
                ;

            engine.Execute(@"
              x = 'sss';
              
            ");
            
            Console.WriteLine(x);
            */
            /*
            IFormatter formatter = new BinaryFormatter();

            Class3 testSerialize = new Class3();
            testSerialize.interface1s.Add(new Class1());
            testSerialize.interface1s.Add(new Class2());
            Stream stream = new FileStream(@"D:\MyFile.bin", FileMode.Create,
            FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, testSerialize);
            stream.Close();
            
            Stream stream2 = new FileStream(@"D:\MyFile.bin", FileMode.Open,
            FileAccess.Read, FileShare.Read);
            Class3 obj = (Class3)formatter.Deserialize(stream2);
            stream2.Close();

            foreach (Interface1 i in obj.interface1s)
            {
                System.Console.WriteLine(i.Desc());
            }
            */

            /*
            string[] test = "event.character.getf".Split(".");

            foreach (string s in test)
            {
                Console.WriteLine(s);
            }
            */
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
