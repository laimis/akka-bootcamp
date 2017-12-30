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

        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
			if (StartCommand.Equals(message))
			{
				DoPrintInstructions();
			}
			else if(message is Messages.InputError)
			{
				_consoleWriterActor.Tell(message as Messages.InputError);
			}

			GetAndValidateInput();
        }

		private void GetAndValidateInput()
		{
			var read = Console.ReadLine();
			if (string.IsNullOrEmpty(read))
			{
				Self.Tell(new Messages.InputError("No input received"));
			}
			else if (String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
			{
				// shut down the system (acquire handle to system via
				// this actors context)
				Context.System.Terminate();
			}
			else
			{
				var valid = IsValid(read);
				if (valid)
				{
					_consoleWriterActor.Tell(
						new Messages.InputSuccess("Thank you! Message was valid.")
					);

					// continue reading messages from console
					Self.Tell(new Messages.ContinueProcessing());
				}
				else
				{
					var err = new Messages.ValidationError(
						"Invalid: input had odd number of characters."
					);

					Self.Tell(err);
				}
			}
		}

		private static bool IsValid(string message)
		{
			var valid = message.Length % 2 == 0;
			return valid;
		}

		private static void DoPrintInstructions()
		{
			Console.WriteLine("Write whatever you want into the console!");
			Console.WriteLine("Some entries will pass validation, and some won't.");
			Console.WriteLine("Type 'exit' to quit this application at any time.\n");
		}
    }
}