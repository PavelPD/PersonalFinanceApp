using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.BusinessLogic
{
    public class AccountProcessor
    {
        private readonly IAccountRepository _accountRepository;

        public AccountProcessor(IAccountRepository accountRepository) 
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<Account>> GetAllAccounts()
        {
            return await _accountRepository.GetAllAccounts();
        }

        public async Task<string> AddAccount(Account account)
        {
            //проверка на пустое имя
            if (string.IsNullOrWhiteSpace(account.Name)) 
            {
                return "Ошибка: имя счета не может быть пустым.";
            }

            if(account.Balance < 0)
            {
                return "Ошибка: баланс счета не может быть отрицательным.";
            }

            await _accountRepository.AddAccount(account);
            return "Счет добавлен.";
        }

        public async Task<string> UpdateAccount(Account account)
        {
            var existingAccount = await _accountRepository.GetAccountById(account.Id);
            if (existingAccount == null)
            {
                return "Ошибка: счет не найден.";
            }

            await _accountRepository.UpdateAccount(account);
            return "Счет обновлен.";
        }

        public async Task<string> DeleteAccount(int id)
        {
            var account = await _accountRepository.GetAccountById(id);
            if (account == null)
            {
                return "Ошибка: счет не найден.";
            }

            await _accountRepository.DeleteAccount(id);
            return "Счет удален.";
        }
    }
}
