using System;
using Newtonsoft.Json;
using Jint;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GameLogic.Utilities;

namespace GameLogic
{
    class MainClass
    {
        public static void Main(string[] args)
        {
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

            var engine = new Engine()
                .SetValue("log", new Action<object>(Console.WriteLine))
                ;

            engine.Execute(@"
              (function(){ 
                log('Hello World');
              })();
              
            ");


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
            DicePoint[] dicePoints = new DicePoint[2];
            dicePoints[0]= new DicePoint(0, 0.8);
            dicePoints[1]= new DicePoint(1, 0.8);
            Dice dice = Dice.Create(dicePoints);
            */
            /*
            int[] points = { 1, 2, 3, 6, 7, 8 };
            Dice dice = Dice.Create(DiceType.Create(points));
            StreamWriter stream2 = new StreamWriter(@"D:\test.txt");
            int i = 0;
            while (i++ < 1000)
            {
                stream2.WriteLine(dice.Roll(1));
            }
            stream2.Close();
            */
            System.Console.Write("请按任意键继续...");
            System.Console.ReadKey();

            
        }
    }
}
