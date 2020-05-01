
namespace Calculator {
    static class Decorator {

        /// <summary>
        /// Декорация текста
        /// </summary>
        /// <param name="input">Входной текст</param>
        /// <returns>Возвращает текст в рамке</returns>
        public static string Decorate(string input) {
            string result = "";
            bool even = false;
            for(int i = 0; i < 3; i++) {
                if (i % 2 != 0) {
                    even = true;
                    result += " ";
                } else {
                    result += "+";
                    even = false;
                }
                for (int j = 0; j < input.Length; j++) {
                    if (even) {
                        result += input[j];
                    } else {
                        result += "-";
                    }
                }
                if (even) {
                    result += "\n";
                } else {
                    result += "+\n";
                }
            }
            return result;
        }
    }
}
