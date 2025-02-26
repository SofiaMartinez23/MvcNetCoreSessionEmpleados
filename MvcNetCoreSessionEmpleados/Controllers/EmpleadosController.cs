using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcNetCoreSessionEmpleados.Extensions;
using MvcNetCoreSessionEmpleados.Models;
using MvcNetCoreSessionEmpleados.Repositories;

namespace MvcNetCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if (salario != null)
            {
                int sumaSalario = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    sumaSalario = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                sumaSalario += salario.Value;
                HttpContext.Session.SetObject("SUMASALARIAL", sumaSalario);
                ViewData["MENSAJE"] = "Salario almacenado: " + salario.Value;
            }
            List<Empleado> emp = await this.repo.GetEmpleadosAsync();
            return View(emp);
        }
        public async Task<IActionResult> SessionEmpleados(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                Empleado empleado = await this.repo.FindEmpleadosAsync(idEmpleado.Value);
                List<Empleado> empleadosList;

                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    empleadosList = new List<Empleado>();
                }
                empleadosList.Add(empleado);
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado " + empleado.Apellido + " almacenado correctamente.";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public IActionResult SumaSalarial()
        {
            return View();
        }
        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }
        public async Task<IActionResult> EmpleadosAlmacenadosOK()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        } 

        public async Task<IActionResult> SessionEmpleadosOK(int? idempleado)
        {
            if (idempleado != null)
            {
                List<int> idsEmpleados;

                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    idsEmpleados = new List<int>();
                }
                else
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }

                idsEmpleados.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count;

            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult>SessionEmpleadosV4(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                //ALMACENAREMOS LO MINIMO QUE PODAMOS (int) 
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    //NO EXISTE Y CREAMOS LA COLECCION 
                    idsEmpleados = new List<int>();
                }
                else
                {
                    //EXISTE Y RECUPERAMOS LA COLECCION 
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                idsEmpleados.Add(idEmpleado.Value);
                //REFRESCAMOS LOS DATOS DE SESSION 
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: "
                + idsEmpleados.Count;
            }
            //COMPROBAMOS SI TENEMOS IDS EN SESSION 
            List<int> ids = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (ids == null)
            {
                List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                List<Empleado> empleados =
                await this.repo.GetEmpleadosSessionV4Async(ids);
                return View(empleados);
            }
        }

        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            //DEBEMOS RECUPERAR LOS IDS DE EMPLEADOS QUE TENGAMOS 
            //EN SESSION 
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados "
                + " en Session.";
                return View();
            }
            else
            {
                List<Empleado> empleados =
                await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

        public IActionResult EmpleadosFavoritos(int? ideliminar)
        {
            if (ideliminar != null)
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                Empleado empDelete = favoritos.Find(z => z.IdEmpleado == ideliminar.Value);
                favoritos.Remove(empDelete);

                if (favoritos.Count == 0)
                {
                    this.memoryCache.Remove("FAVORITOS");
                }
                else
                {
                    this.memoryCache.Set("FAVORITOS", favoritos);
                }
            }
           return View();
        }

        public async Task<IActionResult> SessionEmpleadosV5(int? idempleado, int? idfavorito)
        {
            if (idfavorito != null)
            {
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    empleadosFavoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                Empleado emp = await this.repo.FindEmpleadosAsync(idfavorito.Value);
                empleadosFavoritos.Add(emp);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);

            }
            

            if (idempleado != null)
            {
                List<int> idsEmpleados;

                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    idsEmpleados = new List<int>();
                }
                else
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }

                if (!idsEmpleados.Contains(idempleado.Value))
                {
                    idsEmpleados.Add(idempleado.Value);
                    HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                }

                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count;
                ViewData["IDS_EMPLEADOS_ALMACENADOS"] = idsEmpleados;
            }
            else
            {
                var idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") ?? new List<int>();
                ViewData["IDS_EMPLEADOS_ALMACENADOS"] = idsEmpleados;
            }

            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosV5()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session.";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                ViewData["IDS_EMPLEADOS_ALMACENADOS"] = idsEmpleados;
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session.";
                return View(empleados);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarEmpleadoDeSesion(int idempleado)
        {
            // Obtener la lista de empleados almacenados en la sesión
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");

            // Si la lista no es nula y contiene el empleado a eliminar
            if (idsEmpleados != null && idsEmpleados.Contains(idempleado))
            {
                // Eliminar el empleado
                idsEmpleados.Remove(idempleado);
                // Actualizar la lista en la sesión
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
            }

            // Redirigir a la vista "EmpleadosAlmacenadosV5" para actualizar la lista
            return RedirectToAction("EmpleadosAlmacenadosV5");
        }



    }
}
