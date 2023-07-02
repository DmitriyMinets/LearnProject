using Rocky_Models;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IInquiryDetailsRepository : IRepository<InquiryDetails>
    {
        void Update(InquiryDetails obj);
    }
}
