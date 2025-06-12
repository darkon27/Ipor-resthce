using Newtonsoft.Json;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.DirectoryServices;  //Hay que añadirlo en References
using System.DirectoryServices.AccountManagement;
using System.Text;

using RoyalSISWS.Models;
using RoyalSISWS.Models.Entidades;
using RoyalSISWS.Models.SpringSalud_produccion;
using RoyalSISWS.Entidad;
using RoyalSISWS.BasicAuthentication;
using RoyalSISWS.Models.WEB_ERPSALUD;
using System.Text.RegularExpressions;

namespace RoyalSISWS.Controllers
{
    public class ConsultaController : Controller
    {
        //
        // GET: /Consulta/

        Metodos m = new Metodos();

        public ActionResult Index()
        {
            return View();
        }

        #region ActiveDirectory

        public JsonResult ListarPassword(String Usuario, String Password)
        {
            List<ResponseData> Resultado = new List<ResponseData>();
            ResponseData ObjREs = new ResponseData();
            try
            {
                List<SP_ParametrosMastListar_Result> listParamWService = new List<SP_ParametrosMastListar_Result>();
                SP_ParametrosMastListar_Result objParam = new SP_ParametrosMastListar_Result();
                objParam.Accion = "LISTAR";
                objParam.CompaniaCodigo = "999999";
                objParam.AplicacionCodigo = "WA";//obs  
                objParam.ParametroClave = "RUTA_WSALD";
                listParamWService = m.listarParametro(objParam, 0, 0);
                if (listParamWService.Count > 0)
                {
                    String SERVER = (listParamWService[0].Texto.Trim() + "").Trim();
                    String URL_SERVER = (listParamWService[0].DescripcionParametro.Trim() + "").Trim();
                    if (SERVER == "S")
                    {
                        string contrasenaEncriptada = Password; //LibEncrypt.Class1.Encrypt(Password);
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(URL_SERVER);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = client.GetAsync("ws_dev_getEmpleado1/" + Usuario.Trim() + "/" + contrasenaEncriptada).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            Resultado = (List<ResponseData>)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(List<ResponseData>));
                            return Json(Resultado, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ObjREs.TipoPago = "Error";
                            Resultado.Add(ObjREs);
                            return Json(Resultado, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ObjREs.TipoPago = "";
                        Resultado.Add(ObjREs);
                        return Json(Resultado, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ObjREs.TipoPago = "";
                    Resultado.Add(ObjREs);
                    return Json(Resultado, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ObjREs.TipoPago = "Error";
                ObjREs.Nombre = ex.Message;
                Resultado.Add(ObjREs);
                return Json(Resultado, JsonRequestBehavior.AllowGet);
            }
        }

        public DirectoryEntry GetUser(string path, string admUser, string admPass, string UserName, ref string cErr)
        {
            cErr = "";
            DirectoryEntry de = null;

            try
            {
                de = GetDirectoryObject(path, admUser, admPass, ref cErr);
                DirectorySearcher deSearch = new DirectorySearcher();
                deSearch.SearchRoot = de;

                deSearch.Filter = "(&(objectClass=user)(SAMAccountName=" + UserName + "))";
                deSearch.SearchScope = SearchScope.Subtree;
                SearchResult results = deSearch.FindOne();

                if (results != null)
                    de = new DirectoryEntry(results.Path, admUser, admPass, AuthenticationTypes.Secure);
                else
                    de = null;
            }
            catch (Exception ex)
            {
                cErr = ex.Message;
                de = null;
            }
            return de;
        }

        public DirectoryEntry GetDirectoryObject(string path, string user, string pass, ref string cErr)
        {
            cErr = "";
            DirectoryEntry de;

            try
            {
                de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
            }
            catch (Exception ex)
            {
                cErr = ex.Message;
                de = null;
            }
            return de;
        }


        #endregion


        #region CitaHistoria

        public ActionResult ListarVisorHistoria(int? accion, string tipoDocumento, string documento, string cod_sucursal)
        {
            try
            {
                if (accion != 1)
                {
                    return Json(new { error = "Error: Valores de Parámetro no válidos" }, JsonRequestBehavior.AllowGet);
                }

                var visorHistoria = new CW_DisponibilidadMedica
                {
                    UnidadReplicacion = tipoDocumento,
                    CMP = documento,
                    IdHorario = 1,
                    IdEspecialidad_Nombre = cod_sucursal
                };

                List<VW_SS_HCE_VisorHistoria> listaVisor = m.HCE_VisorHistoria(visorHistoria);
                if (listaVisor == null || listaVisor.Count == 0)
                {
                    return Json(new { error = "Error: " +  "No se encontraron historias" }, JsonRequestBehavior.AllowGet);
                }

                // Obtener IdPaciente de las historias encontradas
                var idPaciente = listaVisor.First().IdPaciente;

                var entidadOncologica = new CW_DisponibilidadMedica
                {
                    IdCita = idPaciente
                };

                List<SS_GE_PacienteOncologicoHC> historiasOncologicas = m.listarHistoriaOncologica(entidadOncologica);

                if (historiasOncologicas != null && historiasOncologicas.Count > 0)
                {
                    var historiaClinica = historiasOncologicas.First().HistoriaClinica;
                    // Asignar historia clínica a todos los elementos (si aplica)
                    foreach (var item in listaVisor)
                    {
                        item.NivelInstruccion = historiaClinica;
                    }
                }
                string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(listaVisor);
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                string logMessage = string.Format(
                    "{0} | Error en ListarVisorHistoria: tipoDocumento='{1}', Documento='{2}', Sucursal='{3}' | {4} | {5}",
                    DateTime.Now, tipoDocumento, documento, cod_sucursal, ex.StackTrace, ex.Source);

                BaseDatos.WriteLog(logMessage);

                return Json(new { error = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ListarVisorHistoriaFecha(Nullable<int> Accion, string tipoDocumento, string Documento, DateTime FechaInicio, DateTime FechaFin, string cod_sucursal)
        {
            try
            {
                List<CW_VisorHistoria> lstSalida = new List<CW_VisorHistoria>();
                if (Accion == 1)
                {
                    CW_DisponibilidadMedica VisorHistoria = new CW_DisponibilidadMedica();
                    VisorHistoria.UnidadReplicacion = tipoDocumento;
                    VisorHistoria.CMP = Documento;
                    VisorHistoria.IdHorario = 1;
                    VisorHistoria.IdEspecialidad_Nombre = cod_sucursal;
                    var lst = new List<A_SP_SS_HCE_VisorHistoria_Result>();
                    List<VW_SS_HCE_VisorAnamnesis> lstAnamnesis = new List<VW_SS_HCE_VisorAnamnesis>();
                    List<VW_SS_HCE_VisorDiagnostico> lstDiagnostico = new List<VW_SS_HCE_VisorDiagnostico>();
                    List<VW_SS_HCE_VisorExamen> lstExamen = new List<VW_SS_HCE_VisorExamen>();
                    List<VW_SS_HCE_VisorReceta> lstReceta = new List<VW_SS_HCE_VisorReceta>();

                    lst = m.getOas(Documento, FechaInicio, FechaFin, cod_sucursal);
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
                BaseDatos.WriteLog(System.DateTime.Now + " | " + "Error Asignacion: EXEC A_SP_SS_HCE_VisorHistoria  '" + FechaInicio + "' ,'" + FechaFin + "','" + Documento + "','" + cod_sucursal + "' | " + exception.StackTrace + " | " + dd);
                return Json("Error : " + " | " + exception.StackTrace, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarVisorHistoriaId(Nullable<int> Accion, string tipoDocumento, string Documento, string IdOrdenAtencion, string cod_sucursal)
        {
            Nullable<int> IdOrden = null;
            Nullable<int> Linea = null;

            if (!string.IsNullOrEmpty(IdOrdenAtencion))
            {
                IdOrden = Convert.ToInt32(IdOrdenAtencion);
            }
            if (!string.IsNullOrEmpty(tipoDocumento))
            {
                Linea = Convert.ToInt32(tipoDocumento);
            }

            try
            {
                List<CW_VisorHistoria> lstSalida = new List<CW_VisorHistoria>();
                if (Accion == 3)
                {
                    CW_DisponibilidadMedica VisorHistoria = new CW_DisponibilidadMedica
                    {
                        CMP = Documento,
                        IdHorario = 3,
                        IdCita = IdOrden,
                        IdTurno = Linea,
                        IdEspecialidad_Nombre = cod_sucursal
                    };

                    List<VW_SS_HCE_VisorHistoria> lst = m.HCE_VisorHistoria(VisorHistoria);
                    List<VW_SS_HCE_VisorAnamnesis> lstAnamnesis = new List<VW_SS_HCE_VisorAnamnesis>();
                    List<VW_SS_HCE_VisorDiagnostico> lstDiagnostico = new List<VW_SS_HCE_VisorDiagnostico>();
                    List<VW_SS_HCE_VisorExamen> lstExamen = new List<VW_SS_HCE_VisorExamen>();
                    List<VW_SS_HCE_VisorReceta> lstReceta = new List<VW_SS_HCE_VisorReceta>();
                    List<VW_SS_HCE_VisorDescansoMedico> lstDescansoMedico = new List<VW_SS_HCE_VisorDescansoMedico>();
                    List<VW_SS_HCE_VisorProcedimiento> lstProcedimiento = new List<VW_SS_HCE_VisorProcedimiento>();

                    if (lst.Count > 0)
                    {
                        lstAnamnesis = m.HCE_VisorHistoria_Anamnesis(VisorHistoria);
                        lstDiagnostico = m.HCE_VisorHistoria_Diagnostico(VisorHistoria);
                        lstExamen = m.HCE_VisorHistoria_Examen(VisorHistoria);
                        lstReceta = m.HCE_VisorHistoria_Receta(VisorHistoria);
                        lstDescansoMedico = m.HCE_VisorHistoria_DescansoMedico(VisorHistoria);
                        lstProcedimiento = m.HCE_VisorHistoria_Procedimiento(VisorHistoria);
                    }

                    foreach (VW_SS_HCE_VisorHistoria intobj2 in lst)
                    {
                        CW_VisorHistoria pObjVisor = new CW_VisorHistoria
                        {
                            CitaTipo = intobj2.CitaTipo,
                            CitaFecha = intobj2.CitaFecha,
                            CitaHora = intobj2.CitaHora,
                            Origen = intobj2.Origen,
                            NombreEspecialidad = intobj2.NombreEspecialidad,
                            TipoPacienteNombre = intobj2.TipoPacienteNombre,
                            CodigoOA = intobj2.CodigoOA,
                            Cama = intobj2.Cama,
                            FechaInicio = intobj2.FechaInicio,
                            TipoOrdenAtencionNombre = intobj2.TipoOrdenAtencionNombre,
                            CodigoHC = intobj2.CodigoHC,
                            CodigoHCAnterior = intobj2.CodigoHCAnterior,
                            PacienteNombre = intobj2.PacienteNombre,
                            MedicoNombre = intobj2.MedicoNombre,
                            IdOrdenAtencion = intobj2.IdOrdenAtencion,
                            LineaOrdenAtencion = intobj2.LineaOrdenAtencion,
                            Aseguradora = intobj2.Aseguradora,
                            IdHospitalizacion = intobj2.IdHospitalizacion,
                            LineaHospitalizacion = intobj2.LineaHospitalizacion,
                            IdConsultaExterna = intobj2.IdConsultaExterna,
                            IdProcedimiento = intobj2.IdProcedimiento,
                            Modalidad = intobj2.Modalidad,
                            IndicadorSeguro = intobj2.IndicadorSeguro,
                            IdCita = intobj2.IdCita,
                            FechaFin = intobj2.FechaFin,
                            TipoPaciente = intobj2.TipoPaciente,
                            TipoAtencion = intobj2.TipoAtencion,
                            IdEspecialidad = intobj2.IdEspecialidad,
                            TipoOrdenAtencion = intobj2.TipoOrdenAtencion,
                            Componente = intobj2.Componente,
                            ComponenteNombre = intobj2.ComponenteNombre,
                            Compania = intobj2.Compania,
                            Sucursal = intobj2.Sucursal,
                            UnidadReplicacionHCE = intobj2.UnidadReplicacionHCE,
                            EstadoEpiAtencion = intobj2.EstadoEpiAtencion,
                            SecuenciaHCE = intobj2.SecuenciaHCE,
                            sexo = intobj2.sexo,
                            FechaNacimiento = intobj2.FechaNacimiento,
                            EstadoCivil = intobj2.EstadoCivil,
                            NivelInstruccion = intobj2.NivelInstruccion,
                            Direccion = intobj2.Direccion,
                            TipoDocumento = intobj2.TipoDocumento,
                            Documento = intobj2.Documento,
                            ApellidoPaterno = intobj2.ApellidoPaterno,
                            ApellidoMaterno = intobj2.ApellidoMaterno,
                            Nombres = intobj2.Nombres,
                            LugarNacimiento = intobj2.LugarNacimiento,
                            CodigoPostal = intobj2.CodigoPostal,
                            Provincia = intobj2.Provincia,
                            Departamento = intobj2.Departamento,
                            Telefono = intobj2.Telefono,
                            CorreoElectronico = intobj2.CorreoElectronico,
                            EsPaciente = intobj2.EsPaciente,
                            EsEmpresa = intobj2.EsEmpresa,
                            Pais = intobj2.Pais,
                            EstadoPersona = intobj2.EstadoPersona,
                            UnidadReplicacionEC = intobj2.UnidadReplicacionEC,
                            TipoAtencionDescX = intobj2.TipoAtencionDescX,
                            list_VW_SS_HCE_VisorAnamnesis = lstAnamnesis.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.LineaOrdenAtencion == intobj2.LineaOrdenAtencion).ToList(),
                            list_VW_SS_HCE_VisorDiagnostico = lstDiagnostico.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.LineaOrdenAtencion == intobj2.LineaOrdenAtencion).ToList()
                        };

                        if (!string.IsNullOrEmpty(intobj2.IdConsultaExterna.ToString()))
                        {
                            pObjVisor.list_VW_SS_HCE_VisorReceta = lstReceta.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.IdConsultaExterna == intobj2.IdConsultaExterna).ToList();
                            pObjVisor.list_VW_SS_HCE_VisorExamen = lstExamen.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.IdConsultaExterna == intobj2.IdConsultaExterna).ToList();
                            pObjVisor.list_VW_SS_HCE_VisorDescansoMedico = lstDescansoMedico.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.IdConsultaExterna == intobj2.IdConsultaExterna).ToList();
                        }
                        else
                        {
                            pObjVisor.list_VW_SS_HCE_VisorExamen = lstExamen.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.Linea == intobj2.LineaOrdenAtencion).ToList();
                            pObjVisor.list_VW_SS_HCE_VisorReceta = lstReceta.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.LineaOrdenAtencion == intobj2.LineaOrdenAtencion).ToList();
                            if (!string.IsNullOrEmpty(intobj2.IdProcedimiento.ToString()))
                            {
                                pObjVisor.list_VW_SS_HCE_VisorProcedimiento = lstProcedimiento.Where(o => o.IdOrdenAtencion == intobj2.IdOrdenAtencion && o.LineaOrdenAtencion == intobj2.LineaOrdenAtencion).ToList();
                            }
                        }

                        lstSalida.Add(pObjVisor);
                    }

                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lstSalida);
                    return Content(jsonString, "application/json");
                }
                else
                {
                    return Json(new { error = "Error: Valores de Parametro" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                string logMessage = string.Format("{0} | Error Asignacion: SELECT * FROM CW_VisorHistoria WHERE tipoDocumento= '{1}' AND Documento='{2}' AND Sucursal='{3}' | {4} | {5}",
                    System.DateTime.Now, tipoDocumento, Documento, cod_sucursal, exception.StackTrace, exception.Source);
                BaseDatos.WriteLog(logMessage);
                return Json(new { error = "Error: " + exception.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarVisorProcedimientoInforme(Nullable<int> valor, string msg)
        {
            Metodos m = new Metodos();
            try
            {
                if (valor == 1)
                {
                    List<SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result> lst = new List<SP_SS_HC_ProcedimientoInformeSPRING_LISTAR_Result>();
                    lst = m.HCE_VisorProcedimientoInformeSPRING(valor, msg);
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
                BaseDatos.WriteLog(System.DateTime.Now + " | " + "Error Asignacion: EXEC SP_SS_HC_ProcedimientoInformeSPRING_LISTAR  " + msg);
                return Json("Error : " + " | " + exception.StackTrace, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region intervecion quirurgica

        public JsonResult obtenerCheckBox(string sucursal, int tipo)
        {
            try
            {
                ViewResponseContenido obje = new ViewResponseContenido();
                //Consultamos a la BD
                obje = m.hce_getChcjBox(sucursal, tipo);
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult obtenerCheckBoxAll()
        {
            try
            {
                ViewResponseContenido obje = new ViewResponseContenido();
                //Consultamos a la BD
                obje = m.hce_getChcjBoxAll();
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult obtenerCheckBoxAllBuscar(string Descripcion, string CodigoMedi, int Tipo, string Sucursal, int PageNumber, int PageSize)
        {
            try
            {
                ViewResponseContenidoPaginado obje = new ViewResponseContenidoPaginado();
                //Consultamos a la BD
                obje = m.hce_getChcjBoxAllBuscar(Sucursal, Tipo, CodigoMedi, Descripcion, PageNumber, PageSize);
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult saveCheckBoxAll(string Sucursal, int Tipo, string CodigoMedi, string Descripcion)
        {
            try
            {
                ViewResponseContenido obje = new ViewResponseContenido();
                //Consultamos a la BD
                obje = m.hce_saveComponenteQuirurgico(Sucursal, Tipo, CodigoMedi, Descripcion);
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult updateCheckBoxAll(int Id, string Sucursal, int Tipo, string CodigoMedi, string Descripcion)
        {
            try
            {
                ViewResponseContenido obje = new ViewResponseContenido();
                //Consultamos a la BD
                obje = m.hce_updateChcjBoxAll(Id, Sucursal, Tipo, CodigoMedi, Descripcion);
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult deleteCheckBoxAll(int Id)
        {
            try
            {
                ViewResponseContenido obje = new ViewResponseContenido();
                //Consultamos a la BD
                obje = m.hce_deleteChcjBoxAll(Id);
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult obtenerCheckBoxId(int Id)
        {
            try
            {
                ViewResponseContenido obje = new ViewResponseContenido();
                //Consultamos a la BD
                obje = m.hce_getChcjBoxId(Id);
                return Json(obje, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }


        #endregion

    }
}
