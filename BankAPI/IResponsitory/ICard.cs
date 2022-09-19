using System;
using BankModel;

namespace BankAPI.IResponsitory
{
	public interface ICard
	{
		Task<Card> PostCard(Card card);
		Task<List<Card>> listCard();
		Task<List<Card>> listCard(int accountId);
    }
}

