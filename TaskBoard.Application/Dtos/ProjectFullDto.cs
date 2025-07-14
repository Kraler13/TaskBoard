using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Application.Dtos
{
    public class ProjectFullDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ColumnDto> Columns { get; set; } = new();
    }
}
