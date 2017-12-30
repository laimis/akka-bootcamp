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

			var consoleWriterProps = Props.Create<ConsoleWriterActor>();
			var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps, "writer");

			var validationActorProps = Props.Create<ValidationActor>(consoleWriterActor);
			var validationActor = MyActorSystem.ActorOf(validationActorProps, "validation");

			var consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
			var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps, "reader");

			consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.WhenTerminated.Wait();
        }
    }
    #endregion
}
