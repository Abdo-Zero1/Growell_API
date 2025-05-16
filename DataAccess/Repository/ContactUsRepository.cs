using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class ContactUsRepository : Repository<ContactUs>, IContactUsRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ContactUsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
