using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using scc_frontend.Helpers;
using scc_frontend.Models.Asadas;
//Saludos
namespace scc_frontend.Controllers.Pajas
{
    public class AsadaCobroController : SscController
    {
        private readonly HttpClient cliente;

        public AsadaCobroController()
        {
            cliente = ClienteApi.Initialize();
        }

        //recuperar los cobros de una asada para un periodo especifico
        [HttpGet]
        public async Task<ActionResult> ListadoAsadasCobros(int codigoOrganizacion = 0, int codigoCompania = 0, int codigoOficina = 0, int numeroPaja = 0, int codigoEstado = 0,  int codigoEstadoFE = 0, string fechaInicio = null, string fechaFinal = null)
        {
            List<AsadaCobroResumen> lista = new List<AsadaCobroResumen>();  
            var codigoOrganizacionLocal = HttpContext.Session.GetInt32("CodigoOrganizacion");
            //crear una fecha del dia 
            DateTime today = DateTime.Today;
            //crear una fecha del ultimo dia del mes actual 
            DateTime lastDayOfMonth = new DateTime(today.Year, today.Month+1, 1).AddDays(-1);

            //validar que la organizacion no sea nula
            if (codigoOrganizacion is 0){
                codigoOrganizacion = Convert.ToInt32(codigoOrganizacionLocal);
            }

            //cargar las listas para los filtros
            await ObtenerListadoDeCompanias(codigoOrganizacion);
            await ObtenerListadoDeOficinas(codigoOrganizacion);
            await ObtenerListadoDeCatalogo(codigoOrganizacion,"TBL_ESTADOS_FACTURAELECTRONICA");
            await ObtenerListadoDeCatalogo(codigoOrganizacion,"TBL_SINO");

            //validar que la fecha de inicio no sea nula
            if (fechaInicio == null){
                fechaInicio =  new DateTime(today.Year, today.Month, 1).ToString("dd-MM-yyyy");
            }else{
                DateTime fechaInicioRangoFecha = Convert.ToDateTime(fechaInicio);
                fechaInicio = fechaInicioRangoFecha.ToString("dd-MM-yyyy");
            }

            //validar que la fecha final no sea nula
            if (fechaFinal == null){
                fechaFinal = lastDayOfMonth.ToString("dd-MM-yyyy");
            }else{
                DateTime fechaFinalRangoFecha = Convert.ToDateTime(fechaFinal);
                fechaFinal = fechaFinalRangoFecha.ToString("dd-MM-yyyy");
            }

            HttpResponseMessage respuestaApi = await CallApiSinJWT($"api/asadas/cobros/ListadoAsadasCobros/{codigoOrganizacion}/{codigoCompania}/{codigoOficina}/{numeroPaja}/{codigoEstado}/{codigoEstadoFE}/{fechaInicio}/{fechaFinal}");
            if (respuestaApi.IsSuccessStatusCode)
            {
                var resultadoApi = respuestaApi.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<AsadaCobroResumen>>(resultadoApi);
                foreach (var item in items)
                {
                    var nuevoitem = new AsadaCobroResumen();
                    nuevoitem.CodigoOrganizacion = item.CodigoOrganizacion;
                    nuevoitem.CodigoCompania = item.CodigoCompania;
                    nuevoitem.CodigoOficina = item.CodigoOficina;
                    nuevoitem.CodigoPaja = item.CodigoPaja;
                    nuevoitem.NumeroLocalizacion = item.NumeroLocalizacion;
                    nuevoitem.NumeroAuxiliar = item.NumeroAuxiliar;
                    nuevoitem.NumeroCobro = item.NumeroCobro;
                    nuevoitem.NumeroCedula = item.NumeroCedula;
                    nuevoitem.NombrePersona = item.NombrePersona;
                    nuevoitem.FechaCobro = item.FechaCobro;
                    nuevoitem.FechaVencimiento = item.FechaVencimiento;
                    nuevoitem.CodigoEstadoFE = item.CodigoEstadoFE;
                    nuevoitem.NumeroFactura = item.NumeroFactura;
                    nuevoitem.ClaveFactura = item.ClaveFactura;
                    nuevoitem.MontoCobro = item.MontoCobro;
                    nuevoitem.MontoPagado = item.MontoPagado;
                    nuevoitem.MontoImpuesto = item.MontoImpuesto;
                    nuevoitem.TextoCodigoEstadoFE = item.TextoCodigoEstadoFE;
                    lista.Add(nuevoitem);
                }
            }
            return View(lista);
        }

        //recuperar el detalle de un cobro
        [HttpGet]
        public async Task<ActionResult>  DetalleDeCobroAsada(int codigoOrganizacion, int codigoCompania, int codigoOficina, int codigoPaja, double numeroCobro, string nombrePlataforma)
        {
            List<AsadaCobro> lista = new List<AsadaCobro>();  
            var codigoOrganizacionLocal = HttpContext.Session.GetInt32("CodigoOrganizacion");

            //validar que la organizacion no sea nula
            if (codigoOrganizacion is 0){
                codigoOrganizacion = Convert.ToInt32(codigoOrganizacionLocal);
            }
            
            HttpResponseMessage respuestaApi = await CallApiSinJWT($"api/asadas/cobros/ListadoDeCobroAsada/{codigoOrganizacion}/{codigoCompania}/{codigoOficina}/{codigoPaja}/{numeroCobro}");
            if (respuestaApi.IsSuccessStatusCode)
            {
                var resultadoApi = respuestaApi.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<AsadaCobro>>(resultadoApi);
                foreach (var item in items)
                {
                    var nuevoitem = new AsadaCobro();
                    nuevoitem.CodigoOrganizacion = item.CodigoOrganizacion;
                    nuevoitem.CodigoCompania = item.CodigoCompania;
                    nuevoitem.CodigoOficina = item.CodigoOficina;
                    nuevoitem.CodigoPaja = item.CodigoPaja;
                    nuevoitem.NumeroCobro = item.NumeroCobro;
                    nuevoitem.NumeroLineaCobro = item.NumeroLineaCobro;
                    nuevoitem.FechaCobro = item.FechaCobro;
                    nuevoitem.FechaVencimiento = item.FechaVencimiento;
                    nuevoitem.TipoCobro = item.TipoCobro;
                    nuevoitem.MontoCobro = item.MontoCobro;
                    nuevoitem.MontoImpuesto = item.MontoImpuesto;
                    nuevoitem.MontoTotalCobro = item.MontoTotalCobro;
                    nuevoitem.MontoPagado = item.MontoPagado;
                    nuevoitem.CodigoEstado = item.CodigoEstado;
                    nuevoitem.TextoTipoMovimiento = item.TextoTipoMovimiento;
                    nuevoitem.TextoCodigoEstado = item.TextoCodigoEstado;

                    lista.Add(nuevoitem);
                }
            }
            return View(lista);
        }

        //mostrar la visa de bitacora de estado de factura electronica desde la pantalla de asadas
        public async Task<ActionResult>  ListadoFacturaElectronicaBitacora(int codigoOrganizacion, int codigoCompania, int codigoOficina, int codigoPaja, string campoLlave)
        {
            return RedirectToAction("ListadoFacturaElectronicaBitacora", "FacturacionElectronica", new { codigoOrganizacion = codigoOrganizacion, codigoCompania = codigoCompania, codigoOficina = codigoOficina, codigoPaja = codigoPaja, campoLlave = campoLlave, nombrePlataforma = "Asadas" });
        }

    }
}
