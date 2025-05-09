﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_CrmSG.Models
{
    public class SolicitudInversion
    {
        [Key]
        public int IdSolicitudInversion { get; set; }

        [StringLength(32)]
        public string? Identificacion { get; set; }

        public int? IdTipoSolicitud { get; set; }

        public int? IdTipoCliente { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int? IdUsuarioPropietario { get; set; }

        public string? JSONDocument { get; set; }
    }
}
