using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using BankModel.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
    public class ContactService : IContact
    {
        public ApplicationDbContext db;
        public ContactService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<List<TotalMonth>> GetAll()
        {
            var Cus = from con in db.Contacts
                      group con by new { month = con.DateContact.Month } into d
                      select new TotalMonth { Month = d.Key.month, Count = d.Count() };
            return await Cus.ToListAsync();
        }

        public async Task<Contact> GetOne(int contactId)
        {
            return await db.Contacts.Include(x => x.Customers).FirstOrDefaultAsync(x=>x.ContactId.Equals(contactId));
        }

        public async Task<List<Contact>> GetWaitingContact()
        {
            return await db.Contacts.ToListAsync();
        }

        public async Task<List<Contact>> ListContact()
        {
            return await db.Contacts.Include(x=>x.Customers).ToListAsync();
        }

        public async Task<Contact> PostContact(Contact contact)
        {
            await db.Contacts.AddAsync(contact);
            await db.SaveChangesAsync();
            return contact;
        }
        public async Task<Contact> PutContact(Contact contact)
        {
            var model = await db.Contacts.FindAsync(contact.ContactId);
            model.Status = contact.Status;
            await db.SaveChangesAsync();
            return model;
        }
    }
}
