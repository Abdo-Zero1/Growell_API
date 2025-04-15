using DataAccess.Repository.IRepository;
using Growell_API.DTOs;

namespace Growell_API.Services
{
    public class DoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<DoctorDTO> GetDoctorWithAppointment(int DoctorID)
        {
            var Doctor = await _doctorRepository.GetDoctorWithAppointment(DoctorID);

            if (Doctor == null) return null;

            return new DoctorDTO
            {
                DoctorID = Doctor.DoctorID,
                FirstName = Doctor.FirstName,
                SecondName = Doctor.SecondName,
                LastName = Doctor.LastName,
                Email = Doctor.Email,
                Gender = Doctor.Gender,
                Description = Doctor.Description,
                Bio = Doctor.Bio,
                AboutMe = Doctor.AboutMe,
                AveRating = Doctor.AveRating,
                CreatedAt = Doctor.CreatedAt,
                PhoneNumber = Doctor.PhoneNumber,
                Specialization = Doctor.Specialization,
                YearsOfExperience = Doctor.YearsOfExperience,
                Education = Doctor.Education,
                Age = Doctor.Age,
                ImgUrl = Doctor.ImgUrl,
                appointmentDTOs = Doctor.Appointments.Select(a => new AppointmentDTO
                {
                    
                    Day = a.Day,
                    StartWith = a.StartWith.ToString(@"hh\:mm"),
                    EndWith = a.EndWith.ToString(@"hh\:mm")

                }).ToList()
            };

        }
    }
}
