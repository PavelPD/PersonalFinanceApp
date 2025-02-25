using PersonalFinanceApp.Data.Repositories;

namespace PersonalFinanceApp.BusinessLogic
{
    public class DataBaseProcessor
    {
        private readonly IDataBaseRepository _dataBaseRepository;

        public DataBaseProcessor(IDataBaseRepository dataBaseRepository)
        {
            _dataBaseRepository = dataBaseRepository;
        }

        public async Task ResetDatabase()
        {
            await _dataBaseRepository.RestDataBase();
        }
    }
}
