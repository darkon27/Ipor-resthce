using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoyalSISWS.Models.Entidades
{
    public class SS_GE_PacienteOncologicoHC
    {
        public Nullable<int> IdPaciente { get; set; }
        public string HistoriaClinica { get; set; }
        public Nullable<int> Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
    }
}