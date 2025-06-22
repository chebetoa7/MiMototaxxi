using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Usuario
{
    public class SolicitudViaje
    {
        public string idPasajero { get; set; }
        public string nomPasajero { get; set; }
        public string ubicacion { get; set; }
        public double lat { get; set; }

        public double lon { get; set; }
        public string fcmToken { get; set; }
    }
}
