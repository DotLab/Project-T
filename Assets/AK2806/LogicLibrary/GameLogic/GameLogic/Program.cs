using System;
using Newtonsoft.Json;
using Jint;
namespace GameLogic
{
    class MainClass
    {
        public static void Main(string[] args)
        {
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
        }
    }
}
