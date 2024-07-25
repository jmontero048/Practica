using System;
using System.Net;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

using scc_frontend.Models.Asadas;
using scc_frontend.Models.Asadas.Reportes;
using scc_frontend.Helpers;

namespace scc_frontend.Controllers.Pajas
{
    public class PajasController : Controller
    {
        private readonly HttpClient cliente;

        public PajasController()
        {
            cliente = ClienteApi.Initialize();
        }

        // GET: OperacionController
        public async Task<ActionResult> ListadoAsadasPajas()
        {
            List<scc_frontend.Models.Asadas.Paja> list = new List<scc_frontend.Models.Asadas.Paja>();
            var credencialesEncriptadas = HttpContext.Session.GetString("CredencialesEncriptadas");
            var codigoOrganizacion = 22;// HttpContext.Session.GetInt32("CodigoOrganizacion");
            var fechaUltimoCierre = HttpContext.Session.GetString("FechaUltimoCierre");

            HttpResponseMessage response = await cliente.GetAsync($"api/asadas/pajas/ListadoDePajas/{codigoOrganizacion}/1/1/0/{fechaUltimoCierre.Replace("/","-")}/{credencialesEncriptadas}");
            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;
                var pajasServicio = JsonConvert.DeserializeObject<List<scc_frontend.Models.Asadas.Paja>>(resultados);
                foreach (var pajaServicio in pajasServicio)
                {
                    var paja = new scc_frontend.Models.Asadas.Paja();
                    paja.CodigoOrganizacion = pajaServicio.CodigoOrganizacion;
                    paja.CodigoCompania = pajaServicio.CodigoCompania;
                    paja.CodigoOficina = pajaServicio.CodigoOficina;
                    paja.CodigoPaja = pajaServicio.CodigoPaja;
                    paja.NumeroMedidor = pajaServicio.NumeroMedidor;
                    paja.NumeroAuxiliar = pajaServicio.NumeroAuxiliar;
                    paja.NumeroCedula = pajaServicio.NumeroCedula;
                    paja.NombreAbonado = pajaServicio.NombreAbonado;
                    paja.CodigoTarifa = pajaServicio.CodigoTarifa;
                    paja.NombreTarifa = pajaServicio.NombreTarifa;
                    paja.FechaEmision = pajaServicio.FechaEmision;
                    paja.DetallePaja = pajaServicio.DetallePaja;
                    paja.NumeroRuta = pajaServicio.NumeroRuta;
                    paja.IndicadorFacturar = pajaServicio.IndicadorFacturar;
                    paja.DescripcionFacturar = pajaServicio.DescripcionFacturar;
                    paja.CodigoEstado = pajaServicio.CodigoEstado;
                    paja.Estado = pajaServicio.Estado;
                    list.Add(paja);
                }
            }

            return View(list);
        }


        [HttpPost]
        public async Task<string> EnviarRecordatorioSMSPaja(int codigoOrganizacion, int codigoCompania, int codigoOficina, int codigoPaja, string numeroCedula, string nombrePlataforma)
        {
            var credencialesEncriptadas = HttpContext.Session.GetString("CredencialesEncriptadas");
            var datos = new EnvioRecordatorioPagoAsadaRequest();
            datos.CodigoOrganizacion = codigoOrganizacion;
            datos.CodigoCompania = codigoCompania;
            datos.CodigoOficina = codigoOficina;
            datos.CodigoPaja = codigoPaja;
            datos.NombrePlataforma = nombrePlataforma;
            datos.CredencialesEncriptadas = credencialesEncriptadas;

            try{
                var jsonData = JsonConvert.SerializeObject(datos);
                var stringContent = new StringContent(jsonData, UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage response = await cliente.PostAsync($"api/asadas/pajas/EnviarRecordatorioSMSPaja", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultados = response.Content.ReadAsStringAsync().Result;
                }
                return "ok";
            }
            catch
            {
                return "error";
            }

        }
    }
}
