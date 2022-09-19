using System;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.DataContext
{
	public class ApplicationDbContext :DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<ConfigurationTransfer> ConfigurationTransfers { get; set; }
        public DbSet<Cheque> Cheques { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}

