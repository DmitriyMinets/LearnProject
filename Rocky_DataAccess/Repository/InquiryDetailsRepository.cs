using Rocky.Data;
using Rocky_Models;
using Rocky_DataAccess.Repository.IRepository;

namespace Rocky_DataAccess.Repository
{
    public class InquiryDetailsRepository : Repository<InquiryDetails>, IInquiryDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InquiryDetails obj)
        {
            _db.InquiryDetails.Update(obj);
        }
    }
}
