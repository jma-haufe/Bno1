using System;
using System.Collections.Generic;

namespace transmate.DataService
{
    public interface IDataService
    {
        List<IAdviceCategory> GetAllAdviceCategories();
        IOffice GetOfficeById(Guid id);

        /// <summary>
        /// Add a new user, throws exception in case user or phone number exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <param name="password"></param>
        void AddUser(String name, String number, String password);

        /// <summary>
        /// Check a user login is correct
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true in case login is correct, false if user not exists or password wrong.</returns>
        bool TryLogin(String username, String password);

        /// <summary>
        /// Pull a ticked in a office for a advice
        /// </summary>
        /// <param name="account"></param>
        /// <param name="advice"></param>
        /// <returns></returns>
        ITicket PullTicketForAdvice(IAccount account, IAdvice advice);
    }
}