﻿using DataAccess.Paginations;
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
        Doctor? GetDoctorByEmail(string email);
    }
}
