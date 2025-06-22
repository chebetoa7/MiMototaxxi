using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Usuario
{
    public class UsuarioResponse
    {
        public string Message { get; set; }
        public string IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
    public class ErrorResponse
    {
        public string Error { get; set; }
    }
}
