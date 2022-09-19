using BankModel;
using BankModel.ViewModel;

namespace BankAPI.IResponsitory
{
    public interface IContact
    {
        public Task<Contact> PostContact(Contact contact);
        public Task<List<Contact>> ListContact();
        public Task<Contact> GetOne(int contactId);
        public Task<Contact> PutContact(Contact contact);
        public Task<List<Contact>> GetWaitingContact();

        public Task<List<TotalMonth>> GetAll();
    }
}
