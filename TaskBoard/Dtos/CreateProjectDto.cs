namespace TaskBoard.Dtos
{
    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
    }
}
