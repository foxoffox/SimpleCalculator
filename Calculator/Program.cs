using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator {
    class Program {
        static void Main(string[] args) {
            while (true) {
                Console.WriteLine("Введите выражение:");
                using (Calculator c = new Calculator()) {
                    Console.WriteLine($"{Decorator.Decorate($"Результат: {c.GetResult(Console.ReadLine())}")}");
                }
            }
        }
    }
}
