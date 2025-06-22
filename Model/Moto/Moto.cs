using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Moto
{
    public class Moto
    {
        public required string Id { get; set; }
        public required string IdUsuario { get; set; }
        public required string Marca { get; set; }
        public required string Modelo { get; set; }
        public int? Age { get; set; }
        public required string Color { get; set; }
        public required string Caracteristicas { get; set; }
        public required string Estatus { get; set; }
        public required string Operador { get; set; }
        public required string StatusTable { get; set; }
        public required string DateRegistre { get; set; }
        public required string CP { get; set; }
        public string? Lat { get; set; } = null;
        public string? Lon { get; set; } = null;
        public string? Dir { get; set; } = null;
        public string? Metadatos { get; set; } = null;
        public string? Distancias { get; set; } = null;
        public string? Estrellas { get; set; } = null;
        public string? fcmToken { get; set; } = null;
    }
}
