using System;
using BankModel;

namespace BankAPI.IResponsitory
{
	public interface INotification
	{
		Task<List<Notifications>> GetNotifications(int customerId);
        Task<List<Notifications>> List();
        Task<Notifications> PostNotification(Notifications noti);
    }
}

