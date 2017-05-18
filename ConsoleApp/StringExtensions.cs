namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class StringExtensions
    {
        public static string Ask(this string question)
        {
            question.Say();
            return Console.ReadLine();
        }

        public static void Choose(this string prompt, Dictionary<string, Action> choices)
        {
            prompt.Say();
            var numberToChoice = choices.Select((kvp, i) => new { Choice = kvp.Key, Number = i }).ToDictionary(x => x.Number, x => x.Choice);
            int number;
            {
                var i = 0;
                foreach (var choice in choices.Keys)
                {
                    ++i;
                    $"\t{i} - {choice}".Say();
                }
                var response = $"1-{i}?".Ask();
                int.TryParse(response, out number);
                --number;
            }
            {
                var choice = numberToChoice[number];
                var action = choices[choice];
                action();
            }
        }

        public static void Say(this string message) => Console.WriteLine(message);

        public static void Wait(this string message)
        {
            message.Say();
            Console.ReadKey(true);
        }
    }
}
