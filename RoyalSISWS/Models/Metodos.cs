using RoyalSISWS.Models.Entidades;
using RoyalSISWS.Models.SpringSalud_produccion;
using RoyalSISWS.Models.WEB_ERPSALUD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Transactions;
using Newtonsoft.Json;

namespace RoyalSISWS.Models
{
    public class Metodos
    {

        #region Historia

        public List<A_SP_SS_HCE_VisorHistoria_Result> getOas(String documento, DateTime FechaInicio, DateTime FechaFin, string cod_sucursal)
        {
            //ProductoPaginacion p = new ProductoPaginacion();
            using (var context = new SpringSalud_produccionEntities())
            {
                return context.A_SP_SS_HCE_VisorHistoria(FechaInicio, FechaFin, documento, cod_sucursal).OrderByDescending(s => s.FechaInicio).ToList();
            }
        }

        public List<A_SP_SS_HCE_ListaServiciosAuxiliares_Result> getDiagnostico(int IdOrdenAtencion, int IdEspecialidad)
        {
            using (var context = new SpringSalud_produccionEntities())
            {
                return context.A_SP_SS_HCE_ListaServiciosAuxiliares(IdOrdenAtencion, IdEspecialidad).ToList();
            }
        }

        public List<VW_SS_HCE_VisorHistoria> HCE_VisorHistoria(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorHistoria> lst = new List<VW_SS_HCE_VisorHistoria>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorHistoria.Where(
                        t => t.TipoDocumento == Disponibilidad.UnidadReplicacion && t.Documento == Disponibilidad.CMP && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).OrderByDescending(s => s.FechaInicio).AsNoTracking().ToList();
                }

                if (Disponibilidad.IdHorario == 2)
                {
                    //lst = context.VW_SS_HCE_VisorHistoria.Where( t => t.IdMedico == Disponibilidad.IdMedico &&
                    //    t.FechaInicio >= Disponibilidad.FechaInicio && t.FechaFin <= Disponibilidad.FechaInicio).ToList();

                    lst = context.VW_SS_HCE_VisorHistoria.Where(t => t.IdMedico == Disponibilidad.IdMedico && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).OrderByDescending(s => s.FechaInicio).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorHistoria.Where(
                        t => //t.TipoDocumento == Disponibilidad.UnidadReplicacion && 
                            t.Documento == Disponibilidad.CMP &&
                            t.IdOrdenAtencion == Disponibilidad.IdCita && t.LineaOrdenAtencion == Disponibilidad.IdTurno && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).OrderByDescending(s => s.FechaInicio).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorExamen> HCE_VisorHistoria_Examen(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorExamen> lst = new List<VW_SS_HCE_VisorExamen>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorExamen.Where(
                        t => t.tipodocumento == Disponibilidad.UnidadReplicacion && t.documento == Disponibilidad.CMP && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorExamen.Where(
                        t =>
                            t.documento == Disponibilidad.CMP && t.IdOrdenAtencion == Disponibilidad.IdCita && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorAnamnesis> HCE_VisorHistoria_Anamnesis(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorAnamnesis> lst = new List<VW_SS_HCE_VisorAnamnesis>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorAnamnesis.Where(
                    t => t.TipoInforme == "CE" && t.tipodocumento == Disponibilidad.UnidadReplicacion && t.documento == Disponibilidad.CMP
                        && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 2)
                {
                    lst = context.VW_SS_HCE_VisorAnamnesis.Where(
                        //t => t.Medico == Disponibilidad.IdMedico && t.FechaCreacion == Disponibilidad.FechaInicio).ToList();
                      t => t.FechaCreacion == Disponibilidad.FechaInicio && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorAnamnesis.Where(
                    t => t.TipoInforme == "CE" && //t.tipodocumento == Disponibilidad.UnidadReplicacion && 
                        t.documento == Disponibilidad.CMP &&
                            t.IdOrdenAtencion == Disponibilidad.IdCita && t.LineaOrdenAtencion == Disponibilidad.IdTurno && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorDiagnostico> HCE_VisorHistoria_Diagnostico(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorDiagnostico> lst = new List<VW_SS_HCE_VisorDiagnostico>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorDiagnostico.Where(
                        t => t.tipodocumento == Disponibilidad.UnidadReplicacion && t.documento == Disponibilidad.CMP && t.UsuarioModificacion == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 2)
                {
                    lst = context.VW_SS_HCE_VisorDiagnostico.Where(
                        t => t.FechaCreacion == Disponibilidad.FechaInicio && t.UsuarioModificacion == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorDiagnostico.Where(
                         t => //t.tipodocumento == Disponibilidad.UnidadReplicacion && 
                             t.documento == Disponibilidad.CMP &&
                         t.IdOrdenAtencion == Disponibilidad.IdCita && t.LineaOrdenAtencion == Disponibilidad.IdTurno && t.UsuarioModificacion == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorReceta> HCE_VisorHistoria_Receta(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorReceta> lst = new List<VW_SS_HCE_VisorReceta>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorReceta.Where(
                        t => t.tipodocumento == Disponibilidad.UnidadReplicacion && t.documento == Disponibilidad.CMP && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 2)
                {
                    lst = context.VW_SS_HCE_VisorReceta.Where(
                        t => t.FechaCreacion == Disponibilidad.FechaInicio && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorReceta.Where(
                        t => //t.tipodocumento == Disponibilidad.UnidadReplicacion && 
                            t.documento == Disponibilidad.CMP && t.IdOrdenAtencion == Disponibilidad.IdCita && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorDescansoMedico> HCE_VisorHistoria_DescansoMedico(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorDescansoMedico> lst = new List<VW_SS_HCE_VisorDescansoMedico>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorDescansoMedico.Where(
                        t => t.TipoDocumento == Disponibilidad.UnidadReplicacion && t.Documento == Disponibilidad.CMP && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 2)
                {
                    lst = context.VW_SS_HCE_VisorDescansoMedico.Where(
                        t => t.FechaCreacion == Disponibilidad.FechaInicio && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorDescansoMedico.Where(
                        t => //t.tipodocumento == Disponibilidad.UnidadReplicacion && 
                            t.Documento == Disponibilidad.CMP && t.IdOrdenAtencion == Disponibilidad.IdCita && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorProcedimiento> HCE_VisorHistoria_Procedimiento(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorProcedimiento> lst = new List<VW_SS_HCE_VisorProcedimiento>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorProcedimiento.Where(
                        t => t.tipodocumento == Disponibilidad.UnidadReplicacion && t.documento == Disponibilidad.CMP && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 2)
                {
                    lst = context.VW_SS_HCE_VisorProcedimiento.Where(
                        t => t.FechaCreacion == Disponibilidad.FechaInicio && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorProcedimiento.Where(
                        t => //t.tipodocumento == Disponibilidad.UnidadReplicacion && 
                            t.documento == Disponibilidad.CMP && t.IdOrdenAtencion == Disponibilidad.IdCita && t.Sucursal == Disponibilidad.IdEspecialidad_Nombre).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorProcedimientoInforme> HCE_VisorInformes(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorProcedimientoInforme> lst = new List<VW_SS_HCE_VisorProcedimientoInforme>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdHorario == 1)
                {
                    lst = context.VW_SS_HCE_VisorProcedimientoInforme.Where(
                        t => t.tipodocumento == Disponibilidad.UnidadReplicacion && t.documento == Disponibilidad.CMP).AsNoTracking().ToList();
                }
                if (Disponibilidad.IdHorario == 3)
                {
                    lst = context.VW_SS_HCE_VisorProcedimientoInforme.Where(
                        t => //t.tipodocumento == Disponibilidad.UnidadReplicacion && 
                            t.documento == Disponibilidad.CMP &&
                        t.IdOrdenAtencion == Disponibilidad.IdCita && t.Linea == Disponibilidad.IdTurno).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<VW_SS_HCE_VisorEmergencia> HCE_VisoreEmergencia(CW_DisponibilidadMedica Disponibilidad)
        {
            List<VW_SS_HCE_VisorEmergencia> lst = new List<VW_SS_HCE_VisorEmergencia>();
            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                if (Disponibilidad.IdTurno == 1)
                {
                    lst = context.VW_SS_HCE_VisorEmergencia.Where(
                        t => t.Origen == Disponibilidad.UnidadReplicacion && t.CodigoOA == Disponibilidad.CMP).AsNoTracking().ToList();
                }
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result> HCE_VisorProcedimientoInformeSPRING(Nullable<int> Accion, string Objeto)
        {
            List<SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result> lst = new List<SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result>();
            SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result objSC = (SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result));

            using (SpringSalud_produccionEntities context = new SpringSalud_produccionEntities())
            {
                Nullable<int> inicio = objSC.IdProcedimiento;
                Nullable<int> NumeroFilas = objSC.IdOrdenAtencion;
                Nullable<int> LineaOA;

                context.Database.Connection.Open();
                lst = context.SP_SS_HC_ProcedimientoInformeSPRING_LISTAR(objSC.IdPaciente, objSC.Paciente, objSC.CodigoOA,
                    objSC.CodigoComponente, objSC.DescripcionComponente, inicio, NumeroFilas, objSC.accion).ToList();
                context.Database.Connection.Close();
                context.Dispose();
            }
            return lst;
        }

        public List<SS_GE_PacienteOncologicoHC> listarHistoriaOncologica(CW_DisponibilidadMedica Disponibilidad)
        {
            List<SS_GE_PacienteOncologicoHC> lstLinea = new List<SS_GE_PacienteOncologicoHC>();
            using (var ctx = new SpringSalud_produccionEntities())
            {
                lstLinea = ctx.Database.SqlQuery<SS_GE_PacienteOncologicoHC>(
                  "SELECT * FROM SS_GE_PacienteOncologicoHC WHERE IdPaciente = " + Disponibilidad.IdCita
                 ).ToList();
            }
            return lstLinea;
        }

        #endregion


        #region Metodobdhce
        public List<SP_ParametrosMastListar_Result> listarParametro(SP_ParametrosMastListar_Result objSC, int inicio, int final)
        {
            List<SP_ParametrosMastListar_Result> objLista = new List<SP_ParametrosMastListar_Result>();
            using (var context = new WEB_ERPSALUDEntities())
            {
                objLista = context.SP_ParametrosMastListar(objSC.CompaniaCodigo, objSC.AplicacionCodigo, objSC.ParametroClave,
                  objSC.DescripcionParametro, objSC.Explicacion, objSC.TipodeDatoFlag, objSC.Texto,
                  objSC.Numero, objSC.Fecha, objSC.FinanceComunFlag, objSC.Estado,
                  objSC.UltimoUsuario, objSC.UltimaFechaModif, objSC.ExplicacionAdicional, objSC.Texto1, objSC.Texto2,
                  objSC.UnidadReplicacion, objSC.Accion, inicio, final, objSC.UsuarioCreacion, objSC.FechaCreacion
                 ).ToList();
            }
            return objLista;
        }
        
        public List<SP_SS_HC_SG_Agente_LISTAR_Result> SG_Agente_Listar(SP_SS_HC_SG_Agente_LISTAR_Result objSC)
        {
            List<SP_SS_HC_SG_Agente_LISTAR_Result> objLista = new List<SP_SS_HC_SG_Agente_LISTAR_Result>();
            using (var context = new WEB_ERPSALUDEntities())
            {
                objLista = context.SP_SS_HC_SG_Agente_LISTAR(
                            objSC.IdAgente
                            , objSC.IdGrupo
                            , objSC.IdOrganizacion
                            , objSC.TipoAgente
                            , objSC.CodigoAgente
                            , objSC.IdEmpleado
                            , objSC.IndicadorMultiple
                            , objSC.Clave
                            , objSC.ExpiraClave
                            , objSC.FechaInicio
                            , objSC.FechaFin
                            , objSC.FechaExpiracion
                            , objSC.Nombre
                            , objSC.Descripcion
                            , objSC.Estado
                            , objSC.UsuarioCreacion
                            , objSC.FechaCreacion
                            , objSC.UsuarioModificacion
                            , objSC.FechaModificacion
                            , objSC.IdGrupoTransaccion
                            , objSC.TipoTransaccion
                            , objSC.IdOpcionDefecto
                            , objSC.Plataforma
                            , objSC.tipotrabajador
                            , objSC.IdCodigo
                            , 0, 0
                            , objSC.ACCION
                            ).ToList();
            }
            return objLista;
        }

        public List<SS_HC_InformeConsultaExterna_FE> InformeConsultaExternaListar(SS_HC_InformeConsultaExterna_FE objSC)
        {
            List<SS_HC_InformeConsultaExterna_FE> objLista = new List<SS_HC_InformeConsultaExterna_FE>();
            using (var context = new WEB_ERPSALUDEntities())
            {
                List<SS_HC_InformeConsultaExterna_FE> objConsultas = (from t in context.SS_HC_InformeConsultaExterna_FE
                                                                      where
                                                                      t.UnidadReplicacion == objSC.UnidadReplicacion
                                                                      && t.IdPaciente == objSC.IdPaciente
                                                                      && t.EpisodioClinico == objSC.EpisodioClinico
                                                                      && t.IdEpisodioAtencion == objSC.IdEpisodioAtencion
                                                                      orderby t.IdEpisodioAtencion descending
                                                                      select t).ToList();

                if (objConsultas != null)
                {
                    objLista.AddRange(objConsultas);
                }

            }
            return objLista;
        }

        public ViewResponse HC_InformeConsultaExternaMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            using (var context = new WEB_ERPSALUDEntities())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        SS_IT_SaludOFTALMOLOGICOIngreso objSC = (SS_IT_SaludOFTALMOLOGICOIngreso)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SS_IT_SaludOFTALMOLOGICOIngreso));

                        SS_HC_InformeConsultaExterna_FE objSave = new SS_HC_InformeConsultaExterna_FE();
                        objSave.UnidadReplicacion = objSC.UnidadReplicacion;
                        objSave.IdPaciente = objSC.IdPaciente;
                        objSave.EpisodioClinico = (int)objSC.EpisodioClinico;
                        objSave.IdEpisodioAtencion = objSC.IdEpisodioAtencion;
                        objSave.Observacion = objSC.ENFACTUAL;
                        objSave.Version = "CCEPF330";
                        objSave.Estado = objSC.Estado;
                        objSave.UsuarioModificacion = objSC.UsuarioCreacion;
                        objSave.UsuarioModificacion = objSC.UsuarioModificacion;
                        objSave.FechaCreacion = objSC.FechaCreacion;
                        objSave.FechaModificacion = objSC.FechaModificacion;
                        if (Accion == 1)
                        {
                            objSave.Accion = "NUEVO";
                            context.Entry(objSave).State = EntityState.Added;
                            obje.valor = context.SaveChanges();
                            obje.ok = true;
                            obje.msg = "Se registro Correcto";
                        }
                        if (Accion == 2)
                        {
                            objSave.Accion = "UPDATE";
                            context.Entry(objSave).State = EntityState.Modified;
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
            return obje;
        }

        public List<SP_SS_HC_InterConsultaSalida_Lista_Result> Lista_InterConsultaSalida(SP_SS_HC_InterConsultaSalida_Lista_Result objSC)
        {
            List<SP_SS_HC_InterConsultaSalida_Lista_Result> objLista = new List<SP_SS_HC_InterConsultaSalida_Lista_Result>();
            using (var context = new WEB_ERPSALUDEntities())
            {
                List<SP_SS_HC_InterConsultaSalida_Lista_Result> objConsultas = context.SP_SS_HC_InterConsultaSalida_Lista(objSC.UnidadReplicacion,
                    objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico, objSC.IdOrdenAtencion).ToList();
                if (objConsultas != null)
                {
                    objLista.AddRange(objConsultas);
                }

            }
            return objLista;
        }

        #endregion


        #region LlamadoHCE

        public ViewResponse SALUD_GenerarLlamado(Nullable<int> Accion, string Objeto)
        {
            ViewResponse response = new ViewResponse();

            using (var context = new SpringSalud_produccionEntities())
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        SP_SS_GenerarLlamado_Result objSC = JsonConvert.DeserializeObject<SP_SS_GenerarLlamado_Result>(Objeto);

                        switch (Accion)
                        {
                            case 1:
                                response.msg = JsonConvert.SerializeObject(context.SP_SS_GenerarLlamado(objSC.IdCita, objSC.Usuario, objSC.Observacion));
                                response.ok = true;
                                response.valor = 1;
                                break;
                            case 2:
                                response.msg = JsonConvert.SerializeObject(context.SP_SS_EliminarLlamado(objSC.IdCita, objSC.Usuario, objSC.Observacion, Accion));
                                response.ok = true;
                                response.valor = 1;
                                break;
                            case 3:
                                response.msg = JsonConvert.SerializeObject(context.SP_SS_EliminarLlamado(objSC.IdCita, objSC.Usuario, objSC.Observacion, Accion));
                                response.ok = true;
                                response.valor = 1;
                                break;
                            default:
                                response.msg = "Invalid Accion value";
                                response.ok = false;
                                response.valor = 0;
                                break;
                        }

                        scope.Complete(); // Commit the transaction if successful
                    }
                    catch (Exception ex)
                    {
                        response.msg = JsonConvert.SerializeObject(ex);
                        response.ok = false;
                        response.valor = 0;
                        string logMessage = "{DateTime.Now} | Error: {Accion} | {response.msg}";
                        BaseDatos.WriteLog(logMessage);
                    }
                }
            }

            return response;
        }

        //public ViewResponse SALUD_GenerarLlamado(Nullable<int> Accion, string Objeto)
        //{
        //    ViewResponse obje = new ViewResponse();
        //    using (var context = new SpringSalud_produccionEntities())
        //    {
        //        using (TransactionScope scope = new TransactionScope())
        //        {

        //            string valor = "";
        //            try
        //            {

        //                if (Accion == 1)
        //                {
        //                    valor = "SP_SS_GenerarLlamado ";
        //                    SP_SS_GenerarLlamado_Result objSC = (SP_SS_GenerarLlamado_Result)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SP_SS_GenerarLlamado_Result));
        //                    var VAAA = context.SP_SS_GenerarLlamado(objSC.IdCita, objSC.Usuario, objSC.Observacion);
        //                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(VAAA);
        //                    obje.ok = true;
        //                    obje.valor = 1;
        //                }

        //                if (Accion == 2)
        //                {
        //                    valor = "SP_SS_EliminarLlamado 2 ";
        //                    SP_SS_GenerarLlamado_Result objSC = (SP_SS_GenerarLlamado_Result)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SP_SS_GenerarLlamado_Result));
        //                    var VAAA = context.SP_SS_EliminarLlamado(objSC.IdCita, objSC.Usuario, objSC.Observacion, Accion);
        //                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(VAAA);
        //                    obje.ok = true;
        //                    obje.valor = 1;
        //                }
        //                if (Accion == 3)
        //                {
        //                    valor = "SP_SS_EliminarLlamado 3 ";
        //                    SP_SS_GenerarLlamado_Result objSC = (SP_SS_GenerarLlamado_Result)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SP_SS_GenerarLlamado_Result));
        //                    var VAAA = context.SP_SS_EliminarLlamado(objSC.IdCita, objSC.Usuario, objSC.Observacion, Accion);
                            
        //                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(VAAA);
        //                    obje.ok = true;
        //                    obje.valor = 1;
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
        //                obje.ok = false;
        //                obje.valor = 0;
        //                BaseDatos.WriteLog(DateTime.Now + " | " + "Error : " + valor + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(obje));
        //            }
        //        }
        //    }
        //    return obje;
        //}


        #endregion


        #region MetodoMirth

        public ViewResponse Mirth_DiagnosticoIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            if (!Accion.HasValue)
            {
                obje.msg = "El valor de Accion no puede ser nulo.";
                obje.ok = false;
                obje.valor = 0;

                return obje;
            }
            try
            {
                List<SS_IT_SaludDiagnosticoIngreso> LstEntyt = (List<SS_IT_SaludDiagnosticoIngreso>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new SpringSalud_produccionEntities())
                {
                    foreach (SS_IT_SaludDiagnosticoIngreso objSC in LstEntyt)
                    {
                        try
                        {
                            var VAAA = context.SP_SS_IT_SaludDiagnosticoIngreso(objSC.UnidadReplicacion, objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico,
                                 objSC.IdOrdenAtencion, objSC.LineaOrdenAtencionConsulta, objSC.IdDiagnostico, objSC.Secuencia, objSC.Determinacion, objSC.TIPOORDENATENCION,
                                 objSC.ObservacionDIAGNOSTICO, objSC.TIPOIT, objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);
                            foreach (SP_SS_IT_SaludDiagnosticoIngreso_Result obj in VAAA)
                            {
                                obje.msg = obj.Mensaje;
                                obje.valor = obj.Retorno;
                            }

                        }
                        catch (Exception ex)
                        {
                            // Manejo del error específico para el elemento actual
                            obje.msg = "Error al procesar el elemento: " + Newtonsoft.Json.JsonConvert.SerializeObject(objSC) + ". Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                            obje.ok = false;
                            obje.valor = 0;
                            BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludDiagnosticoIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(obje));
                            return obje; // Termina la función si ocurre un error
                        }
                    }
                    obje.msg = "Correcto";
                    obje.ok = true;
                    //obje.valor = LstEntyt.Count;
                }
            }
            catch (Exception ex)
            {
                obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                obje.ok = false;
                obje.valor = 0;
                BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludDiagnosticoIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(obje));
            }
            return obje;
        }

        public ViewResponse SaludRecetaIndicacionesGENIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            if (!Accion.HasValue)
            {
                obje.msg = "El valor de Accion no puede ser nulo.";
                obje.ok = false;
                obje.valor = 0;
                return obje;
            }
            try
            {
                using (var context = new SpringSalud_produccionEntities())
                {
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    try
                    {
                        SS_IT_SaludRecetaIndicacionesGENIngreso objSC = (SS_IT_SaludRecetaIndicacionesGENIngreso)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SS_IT_SaludRecetaIndicacionesGENIngreso));
                        var VAAA = context.SP_SS_IT_SaludRecetaIndicacionesGENIngreso(objSC.UnidadReplicacion, objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico,
                             objSC.IdOrdenAtencion, objSC.LineaOrdenAtencionConsulta, objSC.TipoIndicacion, objSC.Descripcion, objSC.Secuencia, objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);

                        obje.msg = "Correcto";
                        obje.ok = true;
                        obje.valor = 1;
                    }
                    catch (Exception ex)
                    {
                        obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                        obje.ok = false;
                        obje.valor = 0;
                        BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludRecetaIndicacionesGENIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(obje));
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                obje.ok = false;
                obje.valor = 0;
                BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludRecetaIndicacionesGENIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(obje));

            }
            return obje;
        }

        public ViewResponse SaludRecetaIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            if (!Accion.HasValue)
            {
                obje.msg = "El valor de Accion no puede ser nulo.";
                obje.ok = false;
                obje.valor = 0;
                return obje;
            }

            try
            {
                List<SS_IT_SaludRecetaIngreso> LstEntyt = (List<SS_IT_SaludRecetaIngreso>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludRecetaIngreso>));
                using (var context = new SpringSalud_produccionEntities())
                {
                    foreach (SS_IT_SaludRecetaIngreso objSC in LstEntyt)
                    {
                        try
                        {
                            var VAAA = context.SP_SS_IT_SaludRecetaIngreso(objSC.UnidadReplicacion, objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico,
                                 objSC.IdOrdenAtencion, objSC.LineaOrdenAtencionConsulta, objSC.Secuencia, objSC.Componente, objSC.SubFamilia, objSC.Familia, objSC.Linea,
                                 objSC.UnidadMedida, objSC.Cantidad, objSC.Via, objSC.Dosis, objSC.DiasTratamiento, objSC.Frecuencia, objSC.IndicadorEPS, objSC.TipoReceta,
                                 objSC.INDICACIONESPECIFICA, objSC.TipoOrdenAtencion, objSC.SECUENCIALHCE, objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);
                            foreach (SP_SS_IT_SaludRecetaIngreso_Result obj in VAAA)
                            {
                                obje.msg = obj.Mensaje;
                                obje.valor = obj.Retorno;
                            }
                            //obje.valor = VAAA.FirstOrDefault().Value;
                        }
                        catch (Exception ex)
                        {
                            // Manejo del error específico para el elemento actual
                            obje.msg = "Error al procesar el elemento: " + Newtonsoft.Json.JsonConvert.SerializeObject(objSC) + ". Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message); obje.ok = false;
                            obje.valor = 0;
                            BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludRecetaIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(objSC));
                            return obje; // Termina la función si ocurre un error
                        }
                    }

                    obje.msg = "Correcto";
                    obje.ok = true;
                    //obje.valor = LstEntyt.Count;
                }
            }
            catch (Exception ex)
            {
                obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                obje.ok = false;
                obje.valor = 0;
                BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludRecetaIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(obje));
            }

            return obje;
        }

        public ViewResponse Mirth_ProcedimientoIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            if (!Accion.HasValue)
            {
                obje.msg = "El valor de Accion no puede ser nulo.";
                obje.ok = false;
                obje.valor = 0;
                return obje;
            }
            using (var context = new SpringSalud_produccionEntities())
            {
                try
                {
                    List<SS_IT_SaludProcedimientoIngreso> LstEntyt = (List<SS_IT_SaludProcedimientoIngreso>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludProcedimientoIngreso>));
                    foreach (SS_IT_SaludProcedimientoIngreso objSC in LstEntyt)
                    {
                        try
                        {
                            var VAAA = context.SP_SS_IT_SaludProcedimientoIngreso(objSC.UnidadReplicacion, objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico,
                              objSC.IdOrdenAtencion, objSC.LineaOrdenAtencionConsulta, objSC.Componente, objSC.Secuencia, objSC.idtipoordenatencion, objSC.Cantidad,
                              objSC.IndicadorEPS, objSC.IdMedico, objSC.Especialidad, objSC.IdCita, objSC.Observacion, objSC.SecuencialHCE,
                              objSC.EstadoDocumento, objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);

                            foreach (SP_SS_IT_SaludProcedimientoIngreso_Result obj in VAAA)
                            {
                                obje.msg = obj.Mensaje;
                                obje.valor = obj.Retorno;
                            }

                        }
                        catch (Exception ex)
                        {
                            // Manejo del error específico para el elemento actual
                            obje.msg = "Error al procesar el elemento: " + Newtonsoft.Json.JsonConvert.SerializeObject(objSC) + ". Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message); obje.ok = false;
                            obje.valor = 0;
                            BaseDatos.WriteLog(DateTime.Now + " | " + "Error : SP_SS_IT_SaludProcedimientoIngreso" + " | " + Newtonsoft.Json.JsonConvert.SerializeObject(objSC));
                            return obje; // Termina la función si ocurre un error
                        }

                    }

                    obje.msg = "Correcto";
                    obje.ok = true;
                    //obje.valor = 1;
                    //scope.Complete();
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = true;
                    obje.valor = 0;
                }
                //}
            }
            return obje;
        }

        public ViewResponse SaludInformeRutaIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            using (var context = new SpringSalud_produccionEntities())
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                try
                {
                    List<SS_IT_SaludInformeRutaIngreso> LstEntyt = (List<SS_IT_SaludInformeRutaIngreso>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludInformeRutaIngreso>));
                    foreach (SS_IT_SaludInformeRutaIngreso objSC in LstEntyt)
                    {
                        var VAAA = context.SP_SS_IT_SaludInformeRutaIngreso(objSC.UnidadReplicacion, objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico,
                         objSC.IdOrdenAtencion, objSC.LineaOrdenAtencion, objSC.RutaInforme, objSC.Observacion,
                          objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);
                        foreach (SP_SS_IT_SaludInformeRutaIngreso_Result obj in VAAA)
                        {
                            obje.msg = obj.Mensaje;
                            obje.valor = obj.Retorno;
                            obje.ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = false;
                    obje.valor = 0;
                }
                //}
            }
            return obje;
        }

        public ViewResponse SaludInformePROCIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            using (var context = new SpringSalud_produccionEntities())
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                try
                {
                    SS_IT_SaludInformePROCIngreso objSC = (SS_IT_SaludInformePROCIngreso)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SS_IT_SaludInformePROCIngreso));

                    var VAAA = context.SP_SS_IT_SaludInformePROCIngreso(objSC.UnidadReplicacion, objSC.IdEpisodioAtencion, objSC.IdPaciente, objSC.EpisodioClinico,
                     objSC.IdOrdenAtencion, objSC.LineaOrdenAtencion, objSC.Informe, objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);
                    foreach (SP_SS_IT_SaludInformePROCIngreso_Result obj in VAAA)
                    {
                        obje.msg = obj.Mensaje;
                        obje.valor = obj.Retorno;
                        obje.ok = true;
                    }
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = false;
                    obje.valor = 0;
                }
                //}
            }
            return obje;
        }

        public ViewResponse Mirth_OftalmologicoIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            using (var context = new SpringSalud_produccionEntities())
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                try
                {
                    SS_IT_SaludOFTALMOLOGICOIngreso objSC = (SS_IT_SaludOFTALMOLOGICOIngreso)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SS_IT_SaludOFTALMOLOGICOIngreso));

                    var VAAA = context.SP_SS_IT_SaludOFTALMOLOGICOIngresoIntermedia(objSC.IdOrdenAtencion, objSC.LineaOrdenAtencion, objSC.UnidadReplicacion, objSC.IdEpisodioAtencion,
                        objSC.IdPaciente, objSC.EpisodioClinico, objSC.Secuencia, objSC.ENFACTUAL, objSC.ANTIMPORTANCIA, objSC.AVscOD,
                        objSC.AvCCOD, objSC.AEAVODPIN, objSC.CERCAVAD, objSC.AVSCOI, objSC.AVCCOI, objSC.AEAVOIDPIN, objSC.CERCAVAOI, objSC.SPHodREFRA,
                        objSC.CILodREFA, objSC.EJEodREFRA, objSC.AVodREFRA, objSC.ADDodREFRA, objSC.DIPodREFRA, objSC.SPHoiSCICLO, objSC.CILoiSCICLO,
                        objSC.EJEoiSCICLO, objSC.AVoiSCICLO, objSC.ADDoiSCICLO, objSC.DIPoiSCICLO, objSC.PAPARPADOSANEXOS, objSC.CORNEACRISTESCLERA,
                        objSC.IRISPUPILA, objSC.TonoOD, objSC.TonoOI, objSC.MMHHTonShiotz, objSC.MMHHTonAplanacion, objSC.MMHHTonOtra, objSC.FONDOJOyG,
                        objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion);
                    foreach (SP_SS_IT_SaludOFTALMOLOGICOIngresoIntermedia_Result obj in VAAA)
                    {
                        obje.msg = obj.Mensaje;
                        obje.valor = obj.Retorno;
                        obje.ok = true;
                    }
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = true;
                    obje.valor = 0;
                }
                //}
            }
            return obje;
        }

        public ViewResponse Mirth_SaludAnamnesisIngresoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            System.Nullable<int> iReturnValue;
            ViewResponse obje = new ViewResponse();
            using (var context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                //using (TransactionScope scope = new TransactionScope())
                //{
                try
                {
                    if (Accion == 1)
                    {
                        SS_IT_SaludAnamnesisIngreso ObjTrace = (SS_IT_SaludAnamnesisIngreso)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SS_IT_SaludAnamnesisIngreso));

                        var VAAA = context.SP_SS_IT_SALUDAnamnesisIngresoMirth(ObjTrace.UnidadReplicacion, ObjTrace.IdEpisodioAtencion, ObjTrace.IdPaciente,
                          ObjTrace.EpisodioClinico, ObjTrace.IdOrdenAtencion, ObjTrace.LineaOrdenAtencion, "1", ObjTrace.TiempoEnfermedad, ObjTrace.TiempoEnfermedadUnidad,
                          ObjTrace.RelatoCronologico, ObjTrace.PresionArterialMSD1, ObjTrace.PresionArterialMSD2, ObjTrace.PresionArterialMSI1
                          , ObjTrace.PresionArterialMSI2, ObjTrace.FrecuenciaCardiaca, ObjTrace.FrecuenciaRespiratoria, ObjTrace.Temperatura, ObjTrace.SaturacionOxigeno
                          , ObjTrace.Peso, ObjTrace.Talla, ObjTrace.EXAMENCLINICOOBS, ObjTrace.Estado, ObjTrace.UsuarioCreacion, DateTime.Now);
                        foreach (SP_SS_IT_SALUDAnamnesisIngresoMirth_Result obj in VAAA)
                        {
                            obje.msg = obj.Mensaje;
                            obje.valor = obj.Retorno;
                            obje.ok = true;
                        }
                    }
                    //scope.Complete();
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = true;
                    obje.valor = 0;
                }
                context.Database.Connection.Close();
                context.Dispose();
                //}
            }
            return obje;
        }

        public ViewResponse Mirth_SaludDescansoMedicoMantenimiento(Nullable<int> Accion, string Objeto)
        {
            ViewResponse obje = new ViewResponse();
            using (var context = new SpringSalud_produccionEntities())
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                try
                {
                    VW_SS_HCE_VisorDescansoMedico objSC = new VW_SS_HCE_VisorDescansoMedico();
                    List<VW_SS_HCE_VisorDescansoMedico> LstEntyt = (List<VW_SS_HCE_VisorDescansoMedico>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<VW_SS_HCE_VisorDescansoMedico>));
                    foreach (VW_SS_HCE_VisorDescansoMedico obj in LstEntyt)
                    {
                        objSC = obj;
                    }
                    var VAAA = context.SP_SS_IT_SALUDDescansoMedico(objSC.IdOrdenAtencion, objSC.IdPaciente, objSC.LineaOrdenAtencion,
                        objSC.FechaInicio, objSC.FechaFinal, objSC.Observaciones, objSC.Estado, objSC.UsuarioCreacion, objSC.FechaCreacion,
                        objSC.UsuarioCreacion, objSC.FechaCreacion);
                    foreach (SP_SS_IT_SALUDDescansoMedico_Result obj in VAAA)
                    {
                        obje.msg = obj.Mensaje;
                        obje.valor = obj.Retorno;
                        obje.ok = true;
                    }
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = false;
                    obje.valor = 0;
                }
                //}
            }
            return obje;
        }

        public ViewResponse Mirth_AnamnesisInFormeMedicoMantenimiento(Nullable<int> Accion, SS_IT_SaludOFTALMOLOGICOIngreso ObjTrace)
        {
            ViewResponse obje = new ViewResponse();
            using (var context = new SpringSalud_produccionEntities())
            {
                context.Database.Connection.Open();
                //using (TransactionScope scope = new TransactionScope())
                //{
                try
                {
                    if (Accion == 1)
                    {
                        var VAAA = context.SP_SS_IT_SALUDAnamnesisInFormeMedico(ObjTrace.IdOrdenAtencion, ObjTrace.IdPaciente,
                              ObjTrace.LineaOrdenAtencion, 1, ObjTrace.ENFACTUAL, ObjTrace.Estado, ObjTrace.UsuarioCreacion,
                           ObjTrace.FechaCreacion, "", ObjTrace.UsuarioModificacion, DateTime.Now);
                        foreach (SP_SS_IT_SALUDAnamnesisInFormeMedico_Result obj in VAAA)
                        {
                            obje.msg = obj.Mensaje;
                            obje.valor = obj.Retorno;
                            obje.ok = true;
                        }
                    }
                    //scope.Complete();
                }
                catch (Exception ex)
                {
                    obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    obje.ok = true;
                    obje.valor = 0;
                }
                context.Database.Connection.Close();
                context.Dispose();
                //}
            }
            return obje;
        }

        #endregion

        #region Formulario Examen Quirur.

        //public ViewResponse HC_InformeConsultaExternaMantenimiento(Nullable<int> Accion, string Objeto)
        //{
        //    ViewResponse obje = new ViewResponse();
        //    using (var context = new WEB_ERPSALUDEntities())
        //    {
        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            try 
        //            {
        //                SS_HC_OrdenIntervencionQuirurgicaCab_FE objSC = (SS_HC_OrdenIntervencionQuirurgicaCab_FE)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(SS_HC_OrdenIntervencionQuirurgicaCab_FE));

        //                SS_HC_OrdenIntervencionQuirurgicaCab_FE objSave = new SS_HC_OrdenIntervencionQuirurgicaCab_FE();
        //                objSave.UnidadReplicacion = objSC.UnidadReplicacion;
        //                objSave.IdPaciente = objSC.IdPaciente;
        //                objSave.EpisodioClinico = (int)objSC.EpisodioClinico;
        //                objSave.IdEpisodioAtencion = objSC.IdEpisodioAtencion;
        //                objSave.Observacion = objSC.ENFACTUAL;
        //                objSave.Version = "CCEPF330";
        //                objSave.Estado = objSC.Estado;
        //                objSave.UsuarioModificacion = objSC.UsuarioCreacion;
        //                objSave.UsuarioModificacion = objSC.UsuarioModificacion;
        //                objSave.FechaCreacion = objSC.FechaCreacion;
        //                objSave.FechaModificacion = objSC.FechaModificacion;
        //                if (Accion == 1)
        //                {
        //                    objSave.Accion = "NUEVO";
        //                    context.Entry(objSave).State = EntityState.Added;
        //                    obje.valor = context.SaveChanges();
        //                    obje.ok = true;
        //                    obje.msg = "Se registro Correcto";
        //                }
        //                if (Accion == 2)
        //                {
        //                    objSave.Accion = "UPDATE";
        //                    context.Entry(objSave).State = EntityState.Modified;
        //                    obje.valor = context.SaveChanges();
        //                    obje.ok = true;
        //                    obje.msg = "Se actualizo Correctamente";
        //                }
        //                scope.Complete();
        //            }
        //            catch (Exception ex)
        //            {
        //                obje.msg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
        //                obje.ok = true;
        //                obje.valor = 0;
        //            }
        //        }
        //    }
        //    return obje;
        //}


        public ViewResponseContenido hce_getChcjBox(string sucursal, int tipo)
        {
            ViewResponseContenido obje = new ViewResponseContenido();
            try
            {
                List<SS_HC_OrdenInterQuiruComponentes> lst = new List<SS_HC_OrdenInterQuiruComponentes>();
                //List<SS_HC_OrdenInterQuiruComponentes> LstEntyt = (List<SS_HC_OrdenInterQuiruComponentes>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new WEB_ERPSALUDEntities())
                {

                    try
                    {
                        lst = context.SS_HC_OrdenInterQuiruComponentes.Where(x => x.Sucursal == sucursal.Trim() && x.Tipo == tipo).ToList();

                        if (lst != null)
                        {
                            obje.contenido = lst;
                            obje.valor = 1;
                            obje.msg = "Correcto";
                            obje.ok = true;
                        }
                        else
                        {
                            obje.contenido = null;
                            obje.valor = 1;
                            obje.msg = "Correcto";
                            obje.ok = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                        obje.ok = false;
                        obje.valor = 0;

                    }

                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;

            }
            return obje;
        }

        public ViewResponseContenido hce_getChcjBoxId(int id)
        {
            ViewResponseContenido obje = new ViewResponseContenido();
            try
            {
                List<SS_HC_OrdenInterQuiruComponentes> lst = new List<SS_HC_OrdenInterQuiruComponentes>();
                //List<SS_HC_OrdenInterQuiruComponentes> LstEntyt = (List<SS_HC_OrdenInterQuiruComponentes>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new WEB_ERPSALUDEntities())
                {

                    try
                    {
                        lst = context.SS_HC_OrdenInterQuiruComponentes.Where(x => x.Id == id).ToList();

                        if (lst != null)
                        {
                            obje.contenido = lst;
                            obje.valor = 1;
                            obje.msg = "Correcto";
                            obje.ok = true;
                        }
                        else
                        {
                            obje.contenido = null;
                            obje.valor = 1;
                            obje.msg = "Correcto";
                            obje.ok = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                        obje.ok = false;
                        obje.valor = 0;

                    }

                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;

            }
            return obje;
        }

        public ViewResponseContenido hce_getChcjBoxAll()
        {
            ViewResponseContenido obje = new ViewResponseContenido();
            try
            {
                List<SS_HC_OrdenInterQuiruComponentes> lst = new List<SS_HC_OrdenInterQuiruComponentes>();
                //List<SS_HC_OrdenInterQuiruComponentes> LstEntyt = (List<SS_HC_OrdenInterQuiruComponentes>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new WEB_ERPSALUDEntities())
                {

                    try
                    {
                        lst = context.SS_HC_OrdenInterQuiruComponentes.ToList();

                        if (lst != null)
                        {
                            obje.contenido = lst;
                            obje.valor = 1;
                            obje.msg = "Correcto";
                            obje.ok = true;
                        }
                        else
                        {
                            obje.contenido = null;
                            obje.valor = 1;
                            obje.msg = "Correcto";
                            obje.ok = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                        obje.ok = false;
                        obje.valor = 0;

                    }

                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;

            }
            return obje;
        }

        public ViewResponseContenido hce_deleteChcjBoxAll(int tipo)
        {
            ViewResponseContenido obje = new ViewResponseContenido();
            try
            {
                List<SS_HC_OrdenInterQuiruComponentes> lst = new List<SS_HC_OrdenInterQuiruComponentes>();
                //List<SS_HC_OrdenInterQuiruComponentes> LstEntyt = (List<SS_HC_OrdenInterQuiruComponentes>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new WEB_ERPSALUDEntities())
                {

                    try
                    {

                        // Busca el objeto en la base de datos
                        var obj = context.SS_HC_OrdenInterQuiruComponentes.FirstOrDefault(x => x.Id == tipo);

                        if (obj != null)
                        {
                            // Elimina el objeto
                            context.SS_HC_OrdenInterQuiruComponentes.Remove(obj);

                            // Guarda los cambios
                            context.SaveChanges();
                            obje.contenido = obj;
                            obje.valor = 1;
                            obje.msg = "Se eliminó el registro correctamente";
                            obje.ok = true;
                        }
                        else
                        {
                            obje.contenido = null;
                            obje.valor = 0;
                            obje.msg = "No se pudo eliminar el registro.";
                            obje.ok = true;
                        }



                    }
                    catch (Exception ex)
                    {
                        obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                        obje.ok = false;
                        obje.valor = 0;

                    }

                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;

            }
            return obje;
        }

        public ViewResponseContenido hce_updateChcjBoxAll(int Id, string sucursal, int tipo, string codMedicamento, string descripcion)
        {
            ViewResponseContenido obje = new ViewResponseContenido();
            try
            {
                List<SS_HC_OrdenInterQuiruComponentes> lst = new List<SS_HC_OrdenInterQuiruComponentes>();
                //List<SS_HC_OrdenInterQuiruComponentes> LstEntyt = (List<SS_HC_OrdenInterQuiruComponentes>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new WEB_ERPSALUDEntities())
                {

                    try
                    {
                        //probando
                        // Busca el objeto en la base de datos
                        var obj = context.SS_HC_OrdenInterQuiruComponentes.FirstOrDefault(x => x.Id == Id);

                        if (obj != null)
                        {
                            obj.Sucursal = sucursal;
                            obj.Tipo = tipo;
                            obj.CodigoMedi = codMedicamento;
                            obj.Descripcion = descripcion;
                            // Guarda los cambios
                            context.SaveChanges();

                            obje.contenido = obj;
                            obje.valor = 1;
                            obje.msg = "Actualizado correctamente.";
                            obje.ok = true;
                        }
                        else
                        {
                            obje.contenido = null;
                            obje.valor = 0;
                            obje.msg = "No se pudo actualizar el registro.";
                            obje.ok = true;
                        }



                    }
                    catch (Exception ex)
                    {
                        obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                        obje.ok = false;
                        obje.valor = 0;

                    }

                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;

            }
            return obje;
        }


        public ViewResponseContenidoPaginado hce_getChcjBoxAllBuscar(string sucursal, int tipo, string codMedicamento, string descripcion, int pageNumber, int pageSize)
        {
            ViewResponseContenidoPaginado obje = new ViewResponseContenidoPaginado();
            try
            {
                using (var context = new WEB_ERPSALUDEntities())
                {
                    // Inicializa la consulta base
                    var query = context.SS_HC_OrdenInterQuiruComponentes.AsQueryable();

                    int contador = query.Count();

                    // Filtro por "tipo", si es mayor a 0 (si se quiere filtrar por este campo)
                    if (tipo > 0)
                    {
                        query = query.Where(x => x.Tipo == tipo);
                    }

                    // Filtro por "sucursal", si no es nulo ni vacío
                    if (!string.IsNullOrEmpty(sucursal))
                    {
                        query = query.Where(x => x.Sucursal.Contains(sucursal));
                    }

                    // Filtro por "codMedicamento", si no es nulo ni vacío
                    if (!string.IsNullOrEmpty(codMedicamento))
                    {
                        query = query.Where(x => x.CodigoMedi.Contains(codMedicamento));
                    }

                    // Filtro por "descripcion", si no es nulo ni vacío
                    if (!string.IsNullOrEmpty(descripcion))
                    {
                        query = query.Where(x => x.Descripcion.Contains(descripcion));
                    }

                    // Paginación: omitir los registros previos y tomar solo el tamaño de página
                    var result = query
                                        .OrderBy(x => x.Id) // O cualquier campo que desees ordenar
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    // Si se encontraron resultados
                    if (result.Any())
                    {
                        obje.contenido = result;
                        obje.valor = 1;
                        obje.msg = "Datos obtenidos correctamente.";
                        obje.ok = true;
                        obje.total = contador;
                    }
                    else
                    {
                        obje.contenido = null;
                        obje.valor = 0;
                        obje.msg = "No se encontraron registros.";
                        obje.ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;
            }

            return obje;
        }


        public ViewResponseContenido hce_saveComponenteQuirurgico(string sucursal, int tipo, string codMedicamento, string descripcion)
        {
            ViewResponseContenido obje = new ViewResponseContenido();
            try
            {
                List<SS_HC_OrdenInterQuiruComponentes> lst = new List<SS_HC_OrdenInterQuiruComponentes>();
                //List<SS_HC_OrdenInterQuiruComponentes> LstEntyt = (List<SS_HC_OrdenInterQuiruComponentes>)Newtonsoft.Json.JsonConvert.DeserializeObject(Objeto, typeof(List<SS_IT_SaludDiagnosticoIngreso>));
                using (var context = new WEB_ERPSALUDEntities())
                {

                    try
                    {
                        //primero buscamos si existe el elemento registrado
                        lst = context.SS_HC_OrdenInterQuiruComponentes.Where(x => x.Sucursal == sucursal && x.CodigoMedi == codMedicamento && x.Tipo == tipo).ToList();

                        if (lst.Count() == 0)
                        {

                            SS_HC_OrdenInterQuiruComponentes obj = new SS_HC_OrdenInterQuiruComponentes();
                            obj.Sucursal = sucursal;
                            obj.Tipo = tipo;
                            obj.Descripcion = descripcion;
                            obj.CodigoMedi = codMedicamento;
                            context.SS_HC_OrdenInterQuiruComponentes.Add(obj);
                            context.SaveChanges();
                            obje.contenido = obj;
                            obje.valor = 1;
                            obje.msg = "Se agregó el registro correctamente.";
                            obje.ok = true;
                        }
                        else
                        {
                            obje.contenido = null;
                            obje.valor = 0;
                            obje.msg = "El elemento por registrar ya existe.";
                            obje.ok = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                        obje.ok = false;
                        obje.valor = 0;

                    }

                }
            }
            catch (Exception ex)
            {
                obje.msg = "Detalles del error: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex.Message);
                obje.ok = false;
                obje.valor = 0;

            }
            return obje;
        }
        #endregion

    }
}