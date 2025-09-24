using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Services.Moto
{
    public interface IApiServiceMoto
    {
        Task<bool> UpdateMotoAsync(Object data, string motoId);
    }
}
