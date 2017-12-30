using Akka.Actor;

namespace WinTail
{
	public class ValidationActor : UntypedActor
	{
		private readonly IActorRef _consoleWriterActor;

		public ValidationActor(IActorRef consoleWriterActor)
		{
			_consoleWriterActor = consoleWriterActor;
		}
		
		protected override void OnReceive(object message)
		{
			var msg = message as string;

			var result = ValidationResult(msg);

			_consoleWriterActor.Tell(result);

			Sender.Tell(new Messages.ContinueProcessing());
		}

		private static object ValidationResult(string msg)
		{
			object result;
			if (string.IsNullOrEmpty(msg))
			{
				result = new Messages.InputError("No input received");
			}
			else
			{
				var valid = IsValid(msg);
				if (valid)
				{
					result = new Messages.InputSuccess("Thank you! Message was valid.");
				}
				else
				{
					result = new Messages.ValidationError(
						"Invalid: input had odd number of characters."
					);
				}
			}

			return result;
		}

		private static bool IsValid(string message)
		{
			var valid = message.Length % 2 == 0;
			return valid;
		}
	}
}