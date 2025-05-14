using DataAccess.DTOS;
using DataAccess.Paginations;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IDoctorRepository:IRepository<Doctor>
    {
        Task<PaginationResponse<SimpleDoctorsDTO>> GetPagedDoctorsAsync(int pageNumber, int pageSize);
        Task<Doctor> GetDoctorWithAppointment(int doctorID);
    }
}
