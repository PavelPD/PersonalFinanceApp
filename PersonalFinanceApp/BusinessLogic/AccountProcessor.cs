using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;

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
                return "Имя счета не может быть пустым";
            }

            await _accountRepository.AddAccount(account);
            return "OK";
        }

        public async Task<string> UpdateAccount(Account account)
        {
            if (string.IsNullOrWhiteSpace(account.Name))
            {
                return "Имя счета не может быть пустым";
            }

            var existingAccount = await _accountRepository.GetAccountById(account.Id);
            if (existingAccount == null)
            {
                return "Ошибка: счет не найден.";
            }

            await _accountRepository.UpdateAccount(account);
            return "OK";
        }

        public async Task<string> DeleteAccount(int id)
        {
            var account = await _accountRepository.GetAccountById(id);
            if (account == null)
            {
                return "Ошибка: счет не найден.";
            }            

            await _accountRepository.DeleteAccount(id);
            return "OK";
        }
    }
}
