using System;
using BankModel;

namespace BankAPI.IResponsitory
{
	public interface IMessage
	{
		Task<List<Message>> list();
		Task<Message> Post(Message mess);
	}
}

