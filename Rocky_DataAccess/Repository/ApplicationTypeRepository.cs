using Rocky.Data;
using Rocky_Models;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky_DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationType obj)
        {
            var objFromDb = FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null) 
            {
                objFromDb.Name = obj.Name;
            }
        }
    }
}
