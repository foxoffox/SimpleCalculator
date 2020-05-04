using Calculator.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calculator {
    class Calculator : IDisposable {
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
            if (Regex.IsMatch(inputString, $@"[0-9.,\-{_Delimeters}{Regex.Escape(_Operators.Replace("-", ""))}]") & !Regex.IsMatch(inputString, "[a-z]")) {
                Calculate(Prepare(inputString), out double result);
                return result.ToString();
            }
            else {
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
            //try {
                for (int i = 0; i < inputExpression.Length; i++) {
                    // если текущий символ - число, то просто помещаем его в выходную строку
                    if (char.IsDigit(inputExpression[i]) | inputExpression[i] == '.' | inputExpression[i] == ',') {
                        if (i == inputExpression.Length) {
                            output += inputExpression[i] + " ";
                        }
                        else {
                            output += inputExpression[i];
                        }
                    }
                    // если символ - не число
                    else {
                    if (output.Last() != ' ') {
                        output += " ";
                    }   //если стек операторов пуст, то добавляем в стек
                        if (operators.Count == 0) {
                            operators.Push(inputExpression[i]);
                        }
                        else {
                        //если символ - открывающая скобка, то она имеет наивысший приоритет и добавляем ее в стек
                            if (inputExpression[i] == '(') {
                                operators.Push(inputExpression[i]);
                                continue;
                            //если символ - закрывающая скобка, то все операторы до входной скобки выгружаем из стека во входную строку
                            }else if (inputExpression[i] == ')') {
                                while(operators.Peek() != '(') {
                                    output += operators.Pop() + " ";
                                }operators.Pop();
                                continue;
                            // если символ не скобка, то проверяем приоритет знака, если приоритет текущего знака меньше приоритета знака вверху стека, выгужаем знак(и)
                            // до тех пор пока приоритет знака в стеке не будет меньше приоритета входного знака
                            }if(operators.Count > 0 && GetPriority(inputExpression[i]) <= GetPriority(operators.Peek())) {
                                while(operators.Count > 0 && GetPriority(inputExpression[i]) <= GetPriority(operators.Peek())) {
                                    output += operators.Pop() + " ";
                                }
                                operators.Push(inputExpression[i]);
                            }
                            else {
                                operators.Push(inputExpression[i]);
                            }
                        }   
                    }   
                }
            //} catch (Exception) {
            //    Console.WriteLine("Допущены ошибки в ввыражении");
            //}
            // если в стеке еще остаются знаки операций, извлекаем их из стека в выходную строку
            if (operators.Count > 0) {
                output += " ";
                while (operators.Count > 0) {
                    output += operators.Pop()+ " ";
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
            MathOperation operation = new MathOperation();
            Stack<double> temp = new Stack<double>();
            foreach (string item in preparedExpression.Split(' ')) {
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
                case ')': return 2;
                case '+': return 3;
                case '-': return 3;
                case '*': return 4;
                case '/': return 4;
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
