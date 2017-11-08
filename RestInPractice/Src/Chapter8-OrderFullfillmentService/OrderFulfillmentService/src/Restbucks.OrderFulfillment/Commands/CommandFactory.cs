using Restbucks.OrderFulfillment.Model;

namespace Restbucks.OrderFulfillment.Commands
{
    public class CommandFactory
    {
        private readonly IRepository repository;
        private readonly IIdGenerator idGenerator;
        private readonly IDateTimeProvider dateTimeProvider;

        public CommandFactory(IRepository repository, IIdGenerator idGenerator, IDateTimeProvider dateTimeProvider)
        {
            this.repository = repository;
            this.idGenerator = idGenerator;
            this.dateTimeProvider = dateTimeProvider;
        }

        public GetFulfillment GetFulfillment()
        {
            return new GetFulfillment(repository);
        }

        public GetFulfillmentCollection GetFulfillmentCollection()
        {
            return new GetFulfillmentCollection(repository);
        }

        public CreateFulfillment CreateFulfillment()
        {
            return new CreateFulfillment(repository, idGenerator, dateTimeProvider);
        }

        public UpdateFulfillment UpdateFulfillment()
        {
            return new UpdateFulfillment(repository, dateTimeProvider);
        }

        public DeleteFulfillment DeleteFulfillment()
        {
            return new DeleteFulfillment(repository);
        }
    }
}