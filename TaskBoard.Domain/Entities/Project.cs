using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public ICollection<Column> Columns { get; set; } = new List<Column>();
    }
}
