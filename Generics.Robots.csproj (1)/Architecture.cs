using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public interface IRobotAI<out TCommand>
    {
        TCommand GetCommand();
    }

    public class ShooterAI : IRobotAI<ShooterCommand>
    {
        public int counter = 1;

        public ShooterCommand GetCommand()=>ShooterCommand.ForCounter(counter++);
    }

    public class BuilderAI : IRobotAI<BuilderCommand>
    {
        public int counter = 1;

        public BuilderCommand GetCommand() =>BuilderCommand.ForCounter(counter++);
    }

    public interface IDevice<in IMoveCommand>
    {
        string ExecuteCommand(IMoveCommand command);
    }

    public class Mover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class ShooterMover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand tCommand)
        {
            var command = tCommand as ShooterCommand;
            if (command == null)
                throw new ArgumentException();
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }
    
    public static class Robot
    { 
        public static Robot<IMoveCommand> Create<IMoveCommand>(
            IRobotAI<IMoveCommand> ai, 
            IDevice<IMoveCommand> executor) 
            => new Robot<IMoveCommand>(ai, executor);
    }

    public class Robot<IMoveCommand>
    {
        private readonly IRobotAI<IMoveCommand> ai;
        private readonly IDevice<IMoveCommand> device;

        public Robot(IRobotAI<IMoveCommand> rAi, IDevice<IMoveCommand> executor)
        {
            ai = rAi;
            device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                var command = ai.GetCommand();
                if (command == null)
                    break;
                yield return device.ExecuteCommand(command);
            }
        }
    }
}