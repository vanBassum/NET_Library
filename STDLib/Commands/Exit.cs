namespace STDLib.Commands
{
    public class Exit : BaseCommand
    {
        public override string Description => "Exits the program";
        public override void Execute(string[] args)
        {
            Work = false;
        }
    }
}
