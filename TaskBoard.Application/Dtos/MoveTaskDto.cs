﻿namespace TaskBoard.Application.Dtos
{
    public class MoveTaskDto
    {
        public Guid ColumnId { get; set; }
        public int Order { get; set; }
    }
}
