using System;
using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
		{
			MyActorSystem = ActorSystem.Create("actorsystem");

			var consoleWriterActor = CreateConsoleWriter();

			var tailCoordinatorActor = CreateTailCoordinator();

			var validationActor = CreateFileValidator(consoleWriterActor);

			var consoleReaderActor = CreateReader();

			consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

			// blocks the main thread from exiting until the actor system is shut down
			MyActorSystem.WhenTerminated.Wait();
		}

		private static IActorRef CreateReader()
		{
			var consoleReaderProps = Props.Create<ConsoleReaderActor>();
			return MyActorSystem.ActorOf(consoleReaderProps, "reader");
		}

		private static IActorRef CreateFileValidator(IActorRef consoleWriterActor)
		{
			var validationActorProps = Props.Create<FileValidatorActor>(consoleWriterActor);
			return MyActorSystem.ActorOf(validationActorProps, "validation");
		}

		private static IActorRef CreateTailCoordinator()
		{
			var tailCoordinatorProps = Props.Create<TailCoordinatorActor>();
			return MyActorSystem.ActorOf(tailCoordinatorProps, "tailcoordinator");
		}

		private static IActorRef CreateConsoleWriter()
		{
			var consoleWriterProps = Props.Create<ConsoleWriterActor>();
			return MyActorSystem.ActorOf(consoleWriterProps, "writer");
		}
	}
    #endregion
}
