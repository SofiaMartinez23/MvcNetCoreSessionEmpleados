using Microsoft.EntityFrameworkCore;
using MvcNetCoreSessionEmpleados.Models;

namespace MvcNetCoreSessionEmpleados.Data
{
    public class HospitalContext: DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) 
            :base(options) { }

        public DbSet<Empleado> Empleados { get; set; }
    }
}
