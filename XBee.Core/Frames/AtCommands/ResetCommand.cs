namespace XBee.Frames.AtCommands
{
    internal class ResetCommand : AtCommand
    {
        public const string Name = "FR";
  
        public ResetCommand() : base(Name)
        {
        }
    }
}
