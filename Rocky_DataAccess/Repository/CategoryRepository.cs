using Rocky.Data;
using Rocky_Models;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            var objFromDb = FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null) 
            {
                objFromDb.Name = obj.Name;
                objFromDb.DisplayOrder = obj.DisplayOrder;
            }
        }
    }
}
