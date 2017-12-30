using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {
        public class StartTail
		{
			public StartTail(string filepath, IActorRef reporterActor)
			{
				this.FilePath = filepath;
				this.ReporterActor = reporterActor;
			}

			public string FilePath { get; private set; }
			public IActorRef ReporterActor { get; private set; }
		}

		public class StopTail
		{
			public StopTail(string filepath)
			{
				this.FilePath = filepath;
			}

			public string FilePath { get; private set; }
		}

		protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;
                
				var props = Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath));

				// here we are creating our first parent/child relationship!
				// the TailActor instance created here is a child
				// of this instance of TailCoordinatorActor
				Context.ActorOf(props, "tailactor");
			}
        }

		protected override SupervisorStrategy SupervisorStrategy()
		{
			return new OneForOneStrategy (
				10, // maxNumberOfRetries
				TimeSpan.FromSeconds(30), // withinTimeRange
				x => // localOnlyDecider
				{
					//Maybe we consider ArithmeticException to not be application critical
					//so we just ignore the error and keep going.
					if (x is ArithmeticException) return Directive.Resume;

					//Error that we cannot recover from, stop the failing actor
					else if (x is NotSupportedException) return Directive.Stop;

					//In all other cases, just restart the failing actor
					else return Directive.Restart;
				});
		}
    }
}