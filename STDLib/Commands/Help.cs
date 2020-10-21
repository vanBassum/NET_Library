using System;

namespace STDLib.Commands
{
    public class Help : BaseCommand
    {
        public override string Description => "Lists all available commands";
        public override void Execute()
        {
            Console.WriteLine($"Available commands:");

            foreach (BaseCommand cmd in Commands)
            {
                Console.WriteLine($" - {cmd.CMD},{new string(' ', LongestCmd - cmd.CMD.Length)} {Description}");
            }
        }
    }
}
