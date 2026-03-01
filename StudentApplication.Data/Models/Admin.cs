using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApplication.Data.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public List<Professor> ApprovedProfessors { get; set; } = new List<Professor>();
    }
}
