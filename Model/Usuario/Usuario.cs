using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Usuario
{
    public class Usuario
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public int? Age { get; set; } = null;
        public required string Briday { get; set; }
        public string? Sex { get; set; } = null;
        public  string TypeUser { get; set; }
        public required string StatusTable { get; set; }
        public required string DateRegistre { get; set; }
        public required string CP { get; set; }
        public required double Estrellas { get; set; }
        public string? Metadatos { get; set; } = null;
        public string? pass { get; set; } = null;
        public string? fcmToken { get; set; } = null;
    }
}
