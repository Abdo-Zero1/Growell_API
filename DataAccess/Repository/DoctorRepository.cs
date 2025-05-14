using DataAccess.DTOS;
using DataAccess.Paginations;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DoctorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


       public async Task<PaginationResponse<SimpleDoctorsDTO>> GetPagedDoctorsAsync(int pageNumber, int pageSize)
        {
            
            var TotalRecords = await dbContext.Doctors.CountAsync();
            var totalPage =( int) Math.Ceiling((double)TotalRecords/pageSize);

            if(pageNumber>totalPage&&totalPage>0)
            {
                pageNumber = totalPage;
            }
         

            var PageData = await dbContext.Doctors.OrderBy(d=>d.DoctorID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
               .Select(doc => new SimpleDoctorsDTO
               {
                   DoctorID = doc.DoctorID,
                   Name = doc.FirstName + " " + doc.LastName,
                   Description = doc.Description,
                  Bio=doc.Bio,
                  Specialization = doc.Specialization,
                   AveRating = doc.AveRating,
                   ImgUrl = doc.ImgUrl

               }).ToListAsync();

            return new PaginationResponse<SimpleDoctorsDTO>(PageData, pageNumber, pageSize, TotalRecords);
        }


    
        public async Task<Doctor>GetDoctorWithAppointment(int doctorID)
        {
            return await dbContext.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.DoctorID == doctorID);
        }

    }
}
