using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
		public const string StartCommand = "start";
        private IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
			if (StartCommand.Equals(message))
			{
				DoPrintInstructions();
			}
			
			var read = Console.ReadLine();
			
			if (String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
			{
				// shut down the system (acquire handle to system via
				// this actors context)
				Context.System.Terminate();
				return;
			}
			
			_validationActor.Tell(read);
        }

		private static void DoPrintInstructions()
		{
			Console.WriteLine("Write whatever you want into the console!");
			Console.WriteLine("Some entries will pass validation, and some won't.");
			Console.WriteLine("Type 'exit' to quit this application at any time.\n");
		}
    }
}