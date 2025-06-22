using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Moto.Response.Mototaxi
{
    public class MototaxiResponse
    {
        public string id { get; set; }
        public string idMototaxi { get; set; }
        public string idPasajero { get; set; }
        public string nomPasajero { get; set; }
        public string ubicacionPasajero { get; set; }
        public string cp { get; set; }
        public Origen origen { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string distancia { get; set; }
        public DetallesMototaxi detallesMototaxi { get; set; }
        public Notificacion notificacion { get; set; }
        public PinUp PinUp { get; set; }
        public PinDestination PinDestination { get; set; }
        public string estatus { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DetallesMototaxi
    {
        public string marca { get; set; }
        public string modelo { get; set; }
        public string color { get; set; }
        public string operador { get; set; }
    }

    public class Location
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Notificacion
    {
        public bool enviada { get; set; }
        public DateTime fecha { get; set; }
        public object error { get; set; }
    }

    public class Origen
    {
        public double lat { get; set; }
        public double @long { get; set; }
    }

    public class PinDestination
    {
        public DateTime date { get; set; }
        public string driverComment { get; set; }
        public Location location { get; set; }
        public bool confirmed { get; set; }
        public double fareAmount { get; set; }
        public string passengerComment { get; set; }
    }

    public class PinUp
    {
        public string CodigoConfirmacion { get; set; }
        public bool confirmed { get; set; }
        public DateTime date { get; set; }
        public Location location { get; set; }
    }

   


}
