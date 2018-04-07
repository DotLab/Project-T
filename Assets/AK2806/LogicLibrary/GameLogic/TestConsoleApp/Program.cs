using System;
using System.Collections;
using System.Collections.Generic;
using Jint;
using Jint.Native;

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

    public class Test
    {
        private int x;
        
        public int X { get => x; set => x = value; }

        public int TestMethod(int v)
        {
            this.X = v + 1;
            return v * v;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine();
            Test testObj = new Test();
            testObj.X = 1;
            engine.SetValue("eee", testObj);
            engine.SetValue("log", new Action<object>(Console.WriteLine));
            engine.Execute(@"
                log(eee.X);
            ");
            engine.SetValue("eee", JsValue.Undefined);
            IBase derivedb = new DerivedB();
            engine.SetValue("interface", derivedb);
            engine.Execute(@"
                interface.Test();
                interface.Test2();
            ");
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
            Console.ReadKey();
        }
    }
}
