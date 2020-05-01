using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calculator {
    class Calculator : IDisposable{
        private bool IsDisposed = false;
        private static string _Operators = "+-*/()";
        private static string _Delimeters = " =";

        #region Получение результата
        /// <summary>
        /// Метод получения результата математического выражения
        /// </summary>
        /// <param name="inputString">Входное выражение</param>
        /// <returns>Возвращает строковый результат математического выражения</returns>
        public string GetResult(string inputString) {
            //проверка входного выражения на недопустимые символы
            if (Regex.IsMatch(inputString, $@"[0-9.,\-{_Delimeters}{Regex.Escape(_Operators.Replace("-",""))}]") & !Regex.IsMatch(inputString,"[a-z]")) {
                Calculate(Prepare(inputString), out double result);
                return result.ToString();
            } else {
                return "Допущенны ошибки в выражении"; 
            }
        }
        #endregion
        #region Парсинг входного выражения
        /// <summary>
        /// Метод приведения входного выражения к виду обратной польской записи
        /// </summary>
        /// <param name="inputExpression">Входное выражение</param>
        /// <returns>Возвращает приведеную к ОПЗ строку</returns>
        private string Prepare(string inputExpression) {
            string output = string.Empty;
            Stack<char> operators = new Stack<char>();
            for (int i=0;i<inputExpression.Length;i++) {
                // если этот символ - число, то просто помещаем его в выходную строку
                if (Regex.IsMatch($"{inputExpression[i]}","[0-9,.]")) {
                    if (inputExpression[i] == '.') {
                        output += inputExpression[i].ToString().Replace(".",",");
                        continue;
                    }
                    output += inputExpression[i];
                } else {
                    // проверяем знак, если если разделитель, то пропускаем
                    if (IsDelimeter(inputExpression[i])) continue;
                    // проверка последнего символа в выходной строке, является ли оно числом
                    if (char.IsDigit(output.Last())) output += " ";
                    //
                    if (operators.Count == 0 && IsOperator(inputExpression[i]) && !IsDelimeter(inputExpression[i])) {
                        operators.Push(inputExpression[i]);
                        continue;
                    }
                    // проверяем знак операции (+, -, *, / ), если true, то проверяем приоритет данной операции
                    if (inputExpression[i] != ')' && IsOperator(inputExpression[i]) && !IsDelimeter(inputExpression[i])) {
                        // если текущий символ - открывающая скобка, то помещаем ее в стек иначе проверяем приоритет знака
                        if (GetPriority(inputExpression[i]) >= GetPriority(operators.Peek()) | inputExpression[i]=='(') {
                            operators.Push(inputExpression[i]);
                        } else {
                            while (GetPriority(inputExpression[i]) <= GetPriority(operators.Peek())) {
                                output += operators.Pop()+" ";
                            }
                            operators.Push(inputExpression[i]);
                        }
                    }
                    // иначе извлекаем символы из стека в выходную строку до тех пор, пока не встретим в стеке открывающую скобку
                    else {
                        while (operators.Peek() != '(') {
                            output += operators.Pop() + " ";
                        }
                        operators.Pop();
                    }
                }
            }
            // если в стеке еще остаются знаки операций, извлекаем их из стека в выходную строку
            if (operators.Count > 0) {
                if (char.IsDigit(output.Last()))
                    output += " ";
                while (operators.Count > 0) {
                    output += operators.Pop() + " ";
                }
            }
            return output;
        }
        #endregion
        #region Расчет нормализованного выражения
        /// <summary>
        /// Метод расчета нормализованного выражения
        /// </summary>
        /// <param name="preparedExpression">Нормализованное выражение</param>
        /// <param name="result">Результат операции</param>
        private void Calculate(string preparedExpression, out double result) {
            Stack<double> temp = new Stack<double>();
            foreach(string item in preparedExpression.Split(' ')) {
                try {
                    // проверка на число, если число, то добавляем в стек, иначе делаем расчет
                    if (double.TryParse(item, out double num)) {
                        temp.Push(num);
                    }
                    else {
                        if (!Regex.IsMatch(item, "[0-9]") & !string.IsNullOrWhiteSpace(item)) {
                            temp.Push(GetTempResult(temp.Pop(), temp.Pop(), item.ToCharArray().First()));
                        }
                    }
                } catch (Exception) { Console.WriteLine("Допущены ошибки в выражении"); }
                
            }
            result = temp.Count == 1 ? temp.Peek() : 0;
        }
        #endregion
        #region Приоритет
        /// <summary>
        /// Получение приоритета символа
        /// </summary>
        /// <param name="c">Входной символ</param>
        /// <returns>Приоритет (byte)</returns>
        private byte GetPriority(char c) {
            switch (c) {
                case '(': return 1;
                case '+': return 2;
                case '-': return 2;
                case '*': return 3;
                case '/': return 3;
                default: return 0;
            }
        }
        #endregion
        #region Операции
        /// <summary>
        /// Метод операции двух чисел
        /// </summary>
        /// <param name="num1">Первое число</param>
        /// <param name="num2">Второе число</param>
        /// <param name="op">Символ операции</param>
        /// <returns>Возвращает результат операции</returns>
        private double GetTempResult(double num1, double num2, char op) {
            switch (op) {
                case '+': return num2 + num1;
                case '-': return num2 - num1;
                case '*': return num2 * num1;
                case '/': return num2 / num1;
                default: return 0;
            }
        }
        #endregion
        #region Проверки
        /// <summary>
        /// Проверка, является ли входной символ разделителем
        /// </summary>
        /// <param name="c">Входной символ</param>
        /// <returns>true, если вимвол является разделителем</returns>
        private bool IsDelimeter(char c) {
            return _Delimeters.IndexOf(c) != -1 ? true : false;
        }
        /// <summary>
        /// Проверка, является ли входной символ оператором
        /// </summary>
        /// <param name="c">Входной символ</param>
        /// <returns>true, если символ является оператором</returns>
        private bool IsOperator(char c) {
            return _Operators.IndexOf(c) != -1 ? true : false;
        }
        #endregion
        #region Очистка ресурсов
        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) { 
            if (!IsDisposed) {
                if (disposing) 
                    GC.Collect();
                IsDisposed = true;
            }
        }
        #endregion
    }
}
