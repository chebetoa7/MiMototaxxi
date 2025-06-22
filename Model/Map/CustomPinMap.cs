using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMototaxxi.Model.Map
{
    public class CustomPinMap : Pin
    {
        public string IconName { get; set; }

        // Asegúrate que Location siempre tenga valor
        private Location _location;
        public new Location Location
        {
            get => _location;
            set => _location = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
