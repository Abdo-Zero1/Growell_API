using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class VideoEventRepository : Repository<VideoEvent>, IVideoEventRepository
    {
        private readonly ApplicationDbContext dbContext;

        public VideoEventRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
