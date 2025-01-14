using System;

namespace HistoryAttribute
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    class History : System.Attribute
    {
        private string programmer;
        public double version;
        public string changes;

        public History(string programmer)
        {
            this.programmer = programmer;
            this.version = 1.0;
            changes = "First release";
        }

        public string GetProgrammer()
        {
            return programmer;
        }
    }

    [History("Sean",
        version =0.1, changes = "2017-11-01 Created class stub")]
    [History("Bob",
        version = 0.2, changes = "2020-12-03 Added Func() Method")]
    class MyClass
    {
        public void Func()
        {
            Console.WriteLine("Func()");
        }
    }
    class MainApp
    {
        static void Main(string[] args)
        {
            Type type = typeof(MyClass);
            Attribute[] attributes = Attribute.GetCustomAttributes(type);

            Console.WriteLine("MyClass change history...");

            foreach (Attribute attribute in attributes)
            {
                History h = attribute as History;
                if(h != null)
                {
                    Console.WriteLine("Ver:{0}, Programmer:{1}, Changes:{2}",
                        h.version, h.GetProgrammer(), h.changes);
                }
            }

        }
    }
}
