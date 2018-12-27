using System;

namespace Library1
{
    public class Person
    {
        public Person()
        {
            Name = "Dart Waider";
            Age = 50;
        }
        
        public string Name { get; set; }

        public int Age { get; set; }

        public void WhoAreYou()
        {
            Console.WriteLine($"My name is {Name}. I am {Age} yers old");
        }


        public override string ToString()
        {
            return $"{Name} {Age}";
        }
    }
}