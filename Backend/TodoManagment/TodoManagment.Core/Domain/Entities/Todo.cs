using TodoManagment.Core.Domain.Entities.Common;
using TodoManagment.Core.Helper;

namespace TodoManagment.Core.Domain.Entities
{
    public class Todo : Entity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TodoStatus Status { get; set; }
        public TodoPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        private readonly List<DomainEvent> _domainEvents = new();
    }
}