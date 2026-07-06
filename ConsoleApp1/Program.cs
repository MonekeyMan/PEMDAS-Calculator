namespace PENDASCalculator
{
    class Program
    {

        //For priority handling: higher the in value, the higher priority it has
        static Dictionary<char, int> priorityList = new()
        {
            ['^'] = 2,
            ['*'] = 1,
            ['/'] = 1,
            ['+'] = 0,
            ['-'] = 0,
            ['\0'] = -1

        };

        static public double Solve(string exp)
        {
            if (!isValidEquation(exp))
            {
                return 0;
            }
            string[] tokens = tokenize(exp);
            double total = 0.0;
            char currentOperator, storedOperator = '\0';
            Stack<double> nums = new Stack<double>();
            Stack<char> opps = new Stack<char>();

            int i = 1;

            while (tokens.Length > i)
            {
                opps.TryPeek(out storedOperator);
                currentOperator = char.Parse(tokens[i]);
                if (priorityList[storedOperator] >= priorityList[currentOperator])
                {
                    tokens[i - 1] = task(nums.Pop(), opps.Pop(), double.Parse(tokens[i - 1])).ToString();
                }
                else
                {
                    opps.Push(currentOperator);
                    nums.Push(double.Parse(tokens[i - 1]));
                    i += 2;
                }
            }

            total = double.Parse(tokens[tokens.Length - 1]);
            double storedNum;

            while (opps.TryPop(out storedOperator))
            {
                storedNum = nums.Pop();
                total = task(storedNum, storedOperator, total);
            }

            return total;
        }

        static private string[] tokenize(string exp)
        {

            List<string> tokens = new List<string>();
            int beginingIndex = 0;

            for(int i = 0; i < exp.Length; i++)
            {

                if (!(char.IsDigit(exp[i])))
                {

                    tokens.Add(exp.Substring(beginingIndex, i - beginingIndex));
                    tokens.Add(exp[i].ToString());
                    beginingIndex = i + 1;

                }
            }

            tokens.Add(exp.Substring(beginingIndex, exp.Length - beginingIndex));

            return tokens.Select(s => s.ToUpper()).ToArray();
        }

        static public Boolean isValidEquation(string exp)
        {
            if (exp == null) return false;

            for(int i = 0; i < exp.Length; i++)
            {
                if (!(isValidOperator(exp[i])) && !(char.IsDigit(exp[i]))) return false;

            }

            return !HasConsecutiveOperators(exp) && !StartOrEndWithOperators(exp);
        }

        static public Boolean HasConsecutiveOperators(string exp)
        {
            for(int i = 1; i < exp.Length; i++)
            {
                if (isValidOperator(exp[i]) && isValidOperator(exp[i-1])) return true;
            }
            return false;
        }
        static public Boolean StartOrEndWithOperators(string exp)
        {
            return isValidOperator(exp[0]) || isValidOperator(exp[exp.Length - 1]);
        }

        static public Boolean isValidOperator(char c)
        {
            return (c == '+' ||
                    c == '-' || 
                    c == '*' ||
                    c == '/' ||
                    c == '^');
        }

        static public double task(double num1, char op, double num2)
        {
            switch (op)
            {
                case '+': return num1 + num2;
                case '-': return num1 - num2;
                case '/': return num1 / num2;
                case '*': return num1 * num2;
                case '^': return Math.Pow(num1, num2);
                default: return 0.0;
            }
        }

        static void Main(string[] args)
        {
            string[] problems = File.ReadAllLines(@"..\..\..\files\questions.txt");
            string[] answers = File.ReadAllLines(@"..\..\..\files\answers.txt");

            int count = 0;
            int correctCount = 0;
            string answer;
            while (count < problems.Length)
            {
                answer = Solve(problems[count]).ToString();
                if (answer.Equals(answers[count]))
                {
                    correctCount++;
                }
                else
                {
                    Console.WriteLine("Incorrect " + problems[count] + ": expected " + answers[count] + " | got: " + answer);
                }
                count++;
                Console.WriteLine("Question " + count + ": " + problems[count - 1] + " | Answer: " + answer);
            }

            Console.WriteLine(correctCount + " out of " + count );
            Console.WriteLine(((double)correctCount/(double)count)*100 + "%");
        }
    }
}