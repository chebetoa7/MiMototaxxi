using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Moto.Response.Viaje
{
   public class ResponseSolicitudViaje
   {
        public bool success { get; set; }
        public string viajeId { get; set; }
        public string nomPasajero { get; set; }
        public Mototaxista mototaxista { get; set; }
   }
    public class Mototaxista
    {
        public string id { get; set; }
        public string operador { get; set; }
        public string vehiculo { get; set; }
        public string distancia { get; set; }
    }

}
