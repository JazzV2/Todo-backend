namespace backend.Core.Dtos.ToDo
{
    public class ToDoEditDto
    {
        public string NewTitle { get; set; } = String.Empty;
        public string NewDescription { get; set; } = String.Empty;
        public bool NewStatus { get; set; }
    }
}
