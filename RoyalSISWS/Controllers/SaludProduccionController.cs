using RoyalSISWS.Entidad;
using RoyalSISWS.Models;
using RoyalSISWS.Models.SpringSalud_produccion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace RoyalSISWS.Controllers
{
    public class SaludProduccionController : Controller
    {
        //
        // GET: /SaludProduccion/

        public ActionResult ListaAtencionesHCE(Nullable<int> valor, string msg)
        { 
            try
            {
                if (valor == 1)
                {
                    List<SS_HC_ATENCIONES_SOA_Result> lst = new List<SS_HC_ATENCIONES_SOA_Result>();

                    SS_HC_ATENCIONES_SOA_Result objSC = (SS_HC_ATENCIONES_SOA_Result)Newtonsoft.Json.JsonConvert.DeserializeObject(msg, typeof(SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result));

                    using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
                    {
                        context.Database.Connection.Open();
                        lst = context.SS_HC_ATENCIONES_SOA(
                         objSC.tipoListado , objSC.CitaTipo , objSC.CitaFecha , objSC.Origen , objSC.NombreEspecialidad
                        , objSC.TipoPacienteNombre , objSC.CodigoOA , objSC.Cama , objSC.TipoOrdenAtencionNombre
                        , objSC.CodigoHC , objSC.PacienteNombre , objSC.MedicoNombre , objSC.IdOrdenAtencion
                        , objSC.LineaOrdenAtencion
                        , objSC.IdHospitalizacion
                        , objSC.IdCita
                        , objSC.IdPaciente
                        , objSC.TipoPaciente
                        , objSC.TipoAtencion
                        , objSC.IdEspecialidad
                        , objSC.IdMedico
                        , objSC.TipoOrdenAtencion
                        , objSC.Componente
                        , objSC.Compania
                        , objSC.Sucursal
                        , objSC.EstadoPersona
                        , objSC.EstadoEpiClinico
                        , objSC.UnidadReplicacion
                        , objSC.UnidadReplicacionEC
                        , objSC.IdEpisodioAtencion
                        , objSC.EpisodioClinico
                        , objSC.IdEstablecimientoSalud
                        , objSC.IdUnidadServicio
                        , objSC.IdPersonalSalud
                        , objSC.EpisodioAtencion
                        , objSC.FechaRegistro
                        , objSC.FechaAtencion
                        , objSC.EstadoEpiAtencion
                        , objSC.FechaInicio
                        , objSC.FechaFin
                        , objSC.UsuarioCreacion
                        , objSC.FechaCreacion
                        , objSC.UsuarioModificacion
                        , objSC.FechaModificacion
                        , objSC.Version
                        , objSC.CodigoHCAnterior
                        , objSC.IndicadorCirugia
                        , objSC.IndicadorExamenPrincipal
                        , objSC.IndicadorSeguro
                        , objSC.Modalidad
                        , objSC.sexo
                        , objSC.EstadoCivil
                        , objSC.NivelInstruccion
                        , objSC.EsPaciente
                        , objSC.EsEmpresa
                        , objSC.NumeroFila
                        , objSC.CONTADOR
                        , objSC.Accion
                    ).ToList();
                        context.Database.Connection.Close();
                        context.Dispose();
                    }       
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
                    jsonString = jsonString.Replace("\n", "");
                    jsonString = Regex.Replace(jsonString, @"[^\u0000-\u007F]+", string.Empty);
                    return Content(jsonString, "application/json");
                }
                else
                {
                    return Json("Error: Valores de Parametro ", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                string dd = exception.Source;
                BaseDatos.WriteLog(System.DateTime.Now + " | " + "Error Asignacion: EXEC SS_HC_ATENCIONES_SOA  " + msg);
                return Json("Error : " + " | " + exception.StackTrace, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListaConsultaExternaEmergencia(Nullable<int> valor, string msg)
        {
            ViewResponse obje = new ViewResponse();
            if (valor == 1 || valor == 2)
            {
                using (var context = new SpringSalud_produccionEntities())
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            SS_HCE_ConsultaExterna objSC = (SS_HCE_ConsultaExterna)Newtonsoft.Json.JsonConvert.DeserializeObject(msg, typeof(SS_IT_SaludOFTALMOLOGICOIngreso));
                            Nullable<int> iReturnValue;
                            iReturnValue = context.SP_HCE_ITListarValidacionEmergencia(
                            objSC.UnidadReplicacion
                           , objSC.IdEpisodioAtencion
                           , objSC.IdPaciente
                           , objSC.EpisodioClinico
                           , objSC.IdConsultaExterna
                           , objSC.IdOrdenAtencion
                           , objSC.Linea
                           , objSC.LineaOrdenAtencion
                           , objSC.TipoOrdenMedica
                           , objSC.Componente
                           , objSC.TipoInterConsulta
                           , objSC.Medico
                           , objSC.Especialidad
                           , objSC.EstadoDocumento
                           , objSC.IndicadorEPS
                           , objSC.Estado
                           , objSC.MedicoResponsable
                           , objSC.UsuarioCreacion
                           , objSC.UsuarioModificacion
                           , objSC.SecuenciaHCE
                           , objSC.FechaCreacion
                           , objSC.FechaModificacion
                           , objSC.Accion
                           , objSC.Version).SingleOrDefault();
                            //valorRetorno = Convert.ToInt32(iReturnValue.ToString().Trim());
                            if (valor == 1)
                            {                            
                                context.Entry(objSC).State = EntityState.Added;
                                obje.valor = context.SaveChanges();
                                obje.ok = true;
                                obje.msg = "Se registro Correcto";
                            }
                            if (valor == 2)
                            {                                
                                context.Entry(objSC).State = EntityState.Modified;
                                obje.valor = context.SaveChanges();
                                obje.ok = true;
                                obje.msg = "Se actualizo Correctamente";
                            }
                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                            obje.ok = true;
                            obje.valor = 0;
                        }
                    }
                }    

                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
