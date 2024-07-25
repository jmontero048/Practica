using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using scc_frontend.Helpers;
using scc_frontend.Models.Asadas;

namespace scc_frontend.Controllers.Pajas
{
    public class AsadaTarifaController : SscController
    {
        private readonly HttpClient cliente;

        public AsadaTarifaController()
        {
            cliente = ClienteApi.Initialize();
        }

        #region AsadaTarifa
        //recuperar los datos del listado general 
        [HttpGet]
        public async Task<ActionResult> ListadoAsadasTarifas(int codigoOrganizacion = 0, int codigoCompania = 0, int codigoTarifa = 0, int codigoEstado = 0)
        {
            //variables o constantes 
            List<AsadaCobroResumen> listaItems = new List<AsadaCobroResumen>();

            //cargar los catalogos o listas para los filtros y combos
            await ObtenerListadoDeCompanias(codigoOrganizacion);
            await ObtenerListadoDeOficinas(codigoOrganizacion);
            await ObtenerListadoDeCatalogo(codigoOrganizacion,"TBL_ACTIVOINACTIVO");

            //llamado del api para recuperar los datos
            try{
                HttpResponseMessage respuestaApi = await CallApiSinJWT($"api/asadas/cobros/ListadoAsadasCobros/{codigoOrganizacion}/{codigoCompania}/{codigoEstado}");
                if (respuestaApi.IsSuccessStatusCode)
                {
                    var resultadoApi = respuestaApi.Content.ReadAsStringAsync().Result;
                    listaItems = JsonConvert.DeserializeObject<List<AsadaCobroResumen>>(resultadoApi);
                }
            }catch(Exception ex){
                Console.WriteLine("ListadoAsadasTarifas error presentado recuperando los datos " + ex.Message);
            }

            //retornar la vista con los datos recuperados
            return View(listaItems);
        }

        //mostrar la vista para agregar 

        //metodo que recibe el modelo desde la vista para agregar el registro

        //mostrar la vista para editar

        //metodo que recibe el modelo desde la vista para editar el registro

        //mostrar la vista para borrar

        //metodo que recibe el modelo desde la vista para borrar el registro

        #endregion AsadaTarifa
    }
}
