using System;
using BankModel;

namespace BankAPI.IResponsitory
{
	public interface IConfigurationTransfer
	{
		Task<ConfigurationTransfer> GetConfiguration();
        Task<ConfigurationTransfer> PutConfiguration(ConfigurationTransfer confi);
    }
}

