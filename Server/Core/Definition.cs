namespace Server.Core
{
    public static class Define
    {
        public interface IConsumer<in T>
            where T : ICommand
        {
            void Consume(ICommandContext context, T command);
        }

        public interface IConsumer<in TCommand, out TResponse>
            where TCommand : ICommand
            where TResponse : IResponse
        {
            TResponse Consume(ICommandContext context, TCommand command);
        }

        public interface IQuery<in TRequest, out TResponse>
            where TRequest : IRequest
            where TResponse : IResponse
        {
            TResponse Consume(IQueryContext consumeContext, TRequest request);
        }

        public interface ICommand
        {

        }

        public interface IRequest
        {

        }

        public interface IResponse
        {

        }
    }
}