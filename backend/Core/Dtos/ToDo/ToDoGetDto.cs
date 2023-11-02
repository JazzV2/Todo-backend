namespace backend.Core.Dtos.ToDo
{
    public class ToDoGetDto
    {
        public DateTime UpdatedAt { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public bool IsImportant { get; set; }
    }
}
