namespace TaskBoard.Application.Dtos
{
    public class CreateColumnDto
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
    }
}
