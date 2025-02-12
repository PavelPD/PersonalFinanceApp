using PersonalFinanceApp.Data.Repositories;
using PersonalFinanceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.BusinessLogic
{
    public class TransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionProcessor(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Transaction>> GetAllTransaction()
        {
            try
            {
                return await _transactionRepository.GetAllTransactions();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении транзакций", ex);
            }
        }

        public async Task<string> AddTransaction(Transaction transaction)
        {
            if(transaction.Amount <= 0) /*добавить проверки*/
            {
                return "Ошибка: сумма транзакции должна быть больше 0.";
            }

            await _transactionRepository.AddTransaction(transaction);
            return "Транзакция добавлена.";
        }

        public async Task<string> UpdateTransaction(Transaction transaction)
        {
            var existingTransaction = await _transactionRepository.GetTransactionById(transaction.Id);
            if (existingTransaction == null)
            {
                return "Ошибка: транзакция не найдена.";
            }

            //оставляем тип оригинала
            transaction.Type = existingTransaction.Type;

            await _transactionRepository.UpdateTransaction(transaction);
            return "Транзакция обновлена";
        }

        public async Task<string> DeleteTransaction(int id)
        {
            var transaction = await _transactionRepository.GetTransactionById(id);
            if (transaction == null)
            {
                return "Ошибка: транзакция не найдена.";
            }

            await _transactionRepository.DeleteTransaction(id);
            return "Транзакция удалена.";
        }
    }
}
