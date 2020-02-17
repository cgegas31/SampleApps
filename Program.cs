using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplemindApp
{
    class Program
    {

        static void Main(string[] args)
        {
            int[] randonNbr;
            StringBuilder sb;
            GenerateNumberSeq(out randonNbr, out sb);

#if (DEBUG)
            Console.WriteLine($"Generated Number: {sb.ToString()}");
#endif

            DisplayGenericMessage();

            try
            {
                string userInput;
                int counter = 1;
                bool done = false;

                while (!done)
                {
                    userInput = Console.ReadLine().Trim().Substring(0, AppSettings.NumericValueLength);
                    if (Validator.IsValid(userInput))
                    {
                        var result = Processor.ProcessUserInput(randonNbr, userInput);
                        done = Processor.ShowUserInput(result);
                        if (done)
                        {
                            Console.Write(" =====SUCCESS====");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input! Number value must be a 4 digit number each from 1-6");
                    }

                    Console.WriteLine($"Your attempt#: {counter++}");

                    done =  counter > AppSettings.MaxNumberOfAttempts;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Apologies! There has been an error {ex.Message}");
                Console.ReadLine();
            }
            finally 
            {
                Console.WriteLine("Thanks for playing. Goodbye!");
                Console.ReadLine();
            }

        }

        private static void GenerateNumberSeq(out int[] arr, out StringBuilder sb)
        {
            arr = new int[AppSettings.NumericValueLength]; //
            sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                var randNum = new Random().Next(AppSettings.MinDigitValue, AppSettings.MaxDigitValue);
                arr[i] = randNum;
                sb.Append(randNum);
            }
        }

        private static void DisplayGenericMessage()
        {
            Console.WriteLine("Guess the Number by making a 4-digit entry each digit must be from 1-6");
            Console.WriteLine("Minus (-) sign would be printed for every digit that is correct but in the wrong position,");
            Console.WriteLine("and a plus (+) sign would be printed for every digit that is both correct and in the correct position.");
            Console.WriteLine("Nothing would be printed for incorrect digits.");

            Console.WriteLine(Environment.NewLine);

        }
    }

    /// <summary>
    /// Application settings...Hardcoded for this example
    /// </summary>
    public class AppSettings
    {
        public static int NumericValueLength => 4;
        public static int MinDigitValue => 1;
        public static int MaxDigitValue => 6;
        public static int MaxNumberOfAttempts => 10;

    }

    /// <summary>
    /// Validation of user input constraints
    /// </summary>
    public static class Validator
    {
        public static bool IsValid(string value)
        {
            if (value.Length < AppSettings.NumericValueLength)
            {
                return false;
            }
            if (value.Length > AppSettings.NumericValueLength)
            {
                return false;
            }
            //Ignore all chars beyond after a 4th chars...
            value = value.Substring(0, AppSettings.NumericValueLength);

            //True if all input chars are numeric
            var result = value.Select(i => int.TryParse(i.ToString(), out int intNbr)).All(i => i);
            return result;
        }
    }

    /// <summary>
    /// Core logic that compares user's input against the computer generared numerical value
    /// Minus (-) sign should be printed for every digit that is correct but in the wrong position, 
    /// and a plus (+) sign should be printed for every digit that is both correct and in the correct position.  
    /// Nothing should be printed for incorrect digits
    /// Uses arays to aid in figuring out the placement of Nothing/Minus/Plus 
    /// </summary>
    public class Processor
    {
        public static int[] ProcessUserInput(int[] generatedValue, string userValue)
        {
            var generatedNumericSeq = generatedValue;
            var userNumericSeq = userValue.Select(i => i - '0').ToArray();

            var currentArr = new int[AppSettings.NumericValueLength];

            //var aggregateResult = new List<int[]>();

            //Map blank,minus,plus to the following int values...
            int ablank = 0; int aminus = 1; int aplus = 2;

            for (int i = 0; i < generatedNumericSeq.Length; i++)
            {
                for (int j = 0; j < userNumericSeq.Length; j++)
                {
                    if (generatedNumericSeq[i] == userNumericSeq[j])
                    {
                        if (i == j)
                            currentArr[j] = Math.Max(currentArr[j], aplus);
                        else
                            currentArr[j] = Math.Max(currentArr[j], aminus);
                    }
                    else
                        currentArr[j] = Math.Max(currentArr[j], ablank);
                }

                //var saveInterimResult = new int[4];
                //currentArr.CopyTo(saveInterimResult, 0);
                //aggregateResult.Add(saveInterimResult);

            }

            return currentArr;
        }

        public static bool ShowUserInput(int[] valueArr)
        {
            Array.ForEach(valueArr, i =>
            {
                if (i == 2)
                    Console.Write("+");
                else if (i == 1)
                    Console.Write("-");
                else
                    Console.Write(" ");
            });
            Console.WriteLine();


            int accumValue = 0;
            for (int i = 0; i < valueArr.Length; i++)
            {
                accumValue += valueArr[i];
            }
            return (accumValue == valueArr.Length * 2);
        }
    }
}
