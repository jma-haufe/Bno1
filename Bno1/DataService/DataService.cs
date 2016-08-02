using transmate.DataService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace transmate.DataService
{
    public class DataService
    {
        private Task _loadTask;

        // Singleton instance of the AuthService
        // The AuthService is a mock of what a real world server and service implementation would be
        private static DataService _instance;
        public static DataService Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new DataService();
                    _instance._mockStore.GenerateFakeData();
                }
                return _instance;
            }
        }

        private DataService()
        {
            _persistenceImplementation = new PersistenceHelper();
            _mockStore = new MockStore(_persistenceImplementation);
        }

        private MockStore _mockStore;
        private Account _currentAccount;
        private PersistenceHelper _persistenceImplementation;

        public Account CurrentAccount
        {
            get
            {
                return _currentAccount;
            }
        }

        public async Task<StorageFile> GetFilePath(String fileName)
        {
            string fp = _persistenceImplementation.GetFullPath(fileName);
            if (!_persistenceImplementation.HasFile(fileName))
            {
                _persistenceImplementation.CreateAndWriteFile(fileName, "DOCUMENT MUST BE STORED UNDER: " + fp);

            }
            return await _persistenceImplementation.GetFolder().GetFileAsync(fileName);
        }

        public List<Advice> GetAllAdviceCategories()
        {
            
            return _mockStore.GetAllAdviceCategories();
        }

        public Office GetOfficeById(Guid id)
        {
            
            return _mockStore.GetOfficeById(id);
        }

        /// <summary>
        /// Add a new user, throws exception in case user or phone number exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <param name="password"></param>
        public void AddUser(String name, String number, String email, String password)
        {
            
            this._currentAccount = _mockStore.AddUser(name, number, email, password);
        }

        public bool HasUserWithName(String name)
        {
            return _mockStore.GetAccountByName(name) != null;
        }

        public bool HasUserWithPhoneNumber(String number)
        {
            return _mockStore.GetAccountByPhone(number) != null;
        }

        public bool HasUserWithMail(String mail)
        {
            return _mockStore.GetAccountByMail(mail) != null;
        }

        /// <summary>
        /// Check a user login is correct
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true in case login is correct, false if user not exists or password wrong.</returns>
        public bool TryLogin(String username, String password)
        {
            
            Account a = _mockStore.GetAccountByName(username);
            if (a == null) return false;
            if (a.Password.Equals(password, StringComparison.Ordinal))
            {
                this._currentAccount = a;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pull a ticked in a office for a advice
        /// </summary>
        /// <param name="account"></param>
        /// <param name="advice"></param>
        /// <returns></returns>
        public Ticket PullTicketForAdvice(Account account, Advice advice)
        {
            if (account == null)
            {
                if (this.CurrentAccount!=null)
                {
                    if (this.CurrentAccount.MyTickets.Count >= 2)
                    {
                        return null;
                    }
                }
                
                Ticket ticket = advice.Office.GetTicketForAdviceIfExists(this.CurrentAccount);
                if (ticket==null) ticket = new Ticket(this.CurrentAccount, advice);
                return ticket;
            }
            else
            {
                
                if (account.MyTickets.Count >= 2)
                {
                    return null;
                }
                
                Ticket ticket = advice.Office.GetTicketForAdviceIfExists(account);
                if (ticket == null) ticket = new Ticket(account, advice);
                return ticket;
            }
            
        }

        
    }
}
