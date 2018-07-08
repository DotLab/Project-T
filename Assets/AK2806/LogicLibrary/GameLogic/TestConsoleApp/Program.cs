using System;
using System.Collections;
using System.Collections.Generic;
using Jint;
using Jint.Native;
using Newtonsoft.Json;
using System.Numerics;
using GameLogic.Character;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.Core.ScriptSystem.EngineWrapper;
using Jint.Parser;
using Jint.Runtime;

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
    }

    public class DerivedB : DerivedA
    {
        private int x = 1;
        
        public override void Test()
        {
            base.Test();
            Console.WriteLine("Derived B");
        }

        public void Test2()
        {
            base.Test();
            Console.WriteLine("Test 2");
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

    public class VarTest : JSContext
    {
        private struct Var
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

    public class ApiTest : JSContext
    {
        private int x;
        private Dictionary<string, List<Stest>> dict;
        private Inner _inner;

        private class Inner
        {
            private ApiTest _outer;
            private string _eee="222";
            

            public Inner(ApiTest outer)
            {
                _outer = outer;
            }
            
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

    class Program
    {
        static void Main(string[] args)
        {
            /*
            Engine engine = new Engine();
            Test testObj = new Test();
            testObj.X = 1;
            engine.SetValue("eee", testObj);
            engine.SetValue("log", new Action<object>(Console.WriteLine));
            engine.SetValue("eee", JsValue.Undefined);
            IBase derivedb = new DerivedB();
            engine.SetValue("interface", derivedb);
            engine.Execute(@"
                xTest = 3;
                //interface.Test();
                //interface.Test2();
            ");
            engine.Execute(@"log(xTest);");
            */
            /*
            JintEngine engine = new JintEngine();
            int[] intarray = { 1, 5, 3, 4, 5 };
            engine.Bind("eee", intarray);
            engine.Bind("log", new Action<object>(Console.WriteLine));
            engine.Execute(@"
                function aa() {
                    var i = 0;
                    for (; i < 5; i++)
                        log(eee[i]);
                };
                aa();
            ");
            */
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
            
            ApiTest test = new ApiTest();
            JSEngineManager.EngineRaw.SetVar("log", new Action<object>(Console.WriteLine));
            JSEngineManager.EngineRaw.SetVar("testRaw", test);
            JSEngineManager.Engine.SetVar("test", test);
            JSEngineManager.Engine.Execute("log(test._eee == undefined? '1':'0');");
            JSEngineManager.Engine.Execute("test.Foo();");
            Console.WriteLine(test.X);
            JSEngineManager.Engine.Execute("test.Foo();");
            Console.WriteLine(test.X);
            try
            {
                JSEngineManager.Engine.Execute("function foo() { foo(); } foo();");
            }
            catch (JSException e)
            {
                Console.WriteLine(e.Message);
            }

            /*VarTest varTest = new VarTest();
            JSEngineManager.Engine.SetVar("test", varTest);
            Console.WriteLine(varTest.VarX() + ", " + varTest.VarY());
            JSEngineManager.Engine.Execute("test.x = 3; test.y = 2;");
            Console.WriteLine(varTest.VarX() + ", " + varTest.VarY());
            JSEngineManager.Engine.GetVar(varTest, "test");
            Console.WriteLine(varTest.VarX() + ", " + varTest.VarY());
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
