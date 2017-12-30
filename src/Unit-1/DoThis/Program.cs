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

			var validationActor = CreateFileValidator(consoleWriterActor, tailCoordinatorActor);

			var consoleReaderActor = CreateReader(validationActor);

			consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

			// blocks the main thread from exiting until the actor system is shut down
			MyActorSystem.WhenTerminated.Wait();
		}

		private static IActorRef CreateReader(IActorRef validationActor)
		{
			var consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
			var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "reader");
			return consoleReaderActor;
		}

		private static IActorRef CreateFileValidator(IActorRef consoleWriterActor, IActorRef tailCoordinatorActor)
		{
			var validationActorProps = Props.Create<FileValidatorActor>(consoleWriterActor, tailCoordinatorActor);
			var validationActor = MyActorSystem.ActorOf(validationActorProps, "validation");
			return validationActor;
		}

		private static IActorRef CreateTailCoordinator()
		{
			var tailCoordinatorProps = Props.Create<TailCoordinatorActor>();
			var tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps, "tailcoordinator");
			return tailCoordinatorActor;
		}

		private static IActorRef CreateConsoleWriter()
		{
			var consoleWriterProps = Props.Create<ConsoleWriterActor>();
			var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "writer");
			return consoleWriterActor;
		}
	}
    #endregion
}
