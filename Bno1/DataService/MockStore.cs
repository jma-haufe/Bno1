using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using transmate.DataService;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace transmate.DataService
{
    public class MockStore
    {
        private const bool ADD_FAKE_DATA = true;
        private const string ACCOUNT_LIST_FILE_NAME = "account.txt";
        private const string ADVICE_LIST_FILE_NAME = "advices.txt";
        private const string OFFICES_LIST_FILE_NAME = "offices.txt";
        // This cannot be a const because the LocalFolder is accessed at runtime
        private IPersistenceWriter _persistenceWriter;
        private List<Advice> _mockDatabaseAdviceCatList;
        private List<Office> _mockDatabaseOfficeList;
        private List<Account> _mockDatabaseAccountList;
        private object _syncRoot = new object();

        public MockStore(IPersistenceWriter persistenceImplementation)
        {
            try
            {
                _persistenceWriter = persistenceImplementation;
                _mockDatabaseAdviceCatList = new List<Advice>();
                _mockDatabaseOfficeList = new List<Office>();
                _mockDatabaseAccountList = new List<Account>();
                LoadAsync().Wait(1000);
                
            }
            catch (Exception ex)
            {
                //TODO
            }
        }

        internal async Task GenerateFakeData()
        {
            if (ADD_FAKE_DATA)
            {
                Random r = new Random();
                foreach (var office in _mockDatabaseOfficeList)
                {
                    if (!office.IsOpen() && office.GetNextOpeningToday() == DateTime.MaxValue)
                    {
                        //Not Open Today
                        office.OpeningHours.Add(new OpeningHours()
                        {
                            DayOfWeek = (int) DateTime.Now.DayOfWeek,
                            StartOpen = DateTime.Now.Hour - 1,
                            EndOpen = DateTime.Now.Hour + 2,
                            IsMock = true
                        });
                    }
                }
               
                    var query = from ac in _mockDatabaseAdviceCatList
                        from asc in ac.Advices
                        from a in asc.Advices
                        select a;
                    foreach (var advice in query.ToList())
                    {
                        if (advice.Office == null) continue;
                        if (advice.Office.GetEstimatedWaitTimeInMinutes() > 125)
                        {
                            //Avoid to much waiters
                            continue;
                        }
                        int maxFakeValue = 50;
                        if (advice.Office.GetCurrentWaiting() > 30)
                        {
                            maxFakeValue = 20;
                        }
                        else if (advice.Office.GetCurrentWaiting() > 50)
                        {
                            maxFakeValue = 5;
                        }

                        for (int i = r.Next(maxFakeValue - 2); i < maxFakeValue; i++)
                        {
                            Ticket ticket = new Ticket(null, advice);
                            if (advice.Office.GetEstimatedWaitTimeInMinutes() > 125)
                            {
                                //Avoid to much waiters
                                break;
                            }
                        }
                    
                }
            }
        }


        internal async Task GenerateFakeTickets()
        {
            if (ADD_FAKE_DATA)
            {
               Random r = new Random();

                var query = from ac in _mockDatabaseAdviceCatList
                            from asc in ac.Advices
                            from a in asc.Advices
                            select a;
                foreach (var advice in query.ToList())
                {
                    if (advice.Office == null) continue;
                    if (advice.Office.GetEstimatedWaitTimeInMinutes() > 90)
                    {
                        //Avoid to much waiters
                        continue;
                    }
                    int maxFakeValue = 25;
                    if (advice.Office.GetCurrentWaiting() > 15)
                    {
                        maxFakeValue = 5;
                    }
                    else if (advice.Office.GetCurrentWaiting() > 30)
                    {
                        maxFakeValue = 2;
                    }

                    for (int i = r.Next(maxFakeValue - 1); i < maxFakeValue; i++)
                    {
                        Ticket ticket = new Ticket(null, advice);
                        if (advice.Office.GetEstimatedWaitTimeInMinutes() > 70)
                        {
                            //Avoid to much waiters
                            break;
                        }
                    }

                }
            }
        }

        private void InitializeSampleAccounts()
        {
            Account a = new Account()
            {
                Name = "user",
                Password = "password",
                TelephoneNumber = "01707654321",
                AccountId = Guid.NewGuid()
            };

            _mockDatabaseAccountList.Add(a);
        }

        internal Account GetAccountByName(string username)
        {
            return
                _mockDatabaseAccountList.FirstOrDefault(a => a.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        internal Account GetAccountByPhone(string phone)
        {
            return
                _mockDatabaseAccountList.FirstOrDefault(a => a.TelephoneNumber.Equals(phone, StringComparison.OrdinalIgnoreCase));
        }

        internal Account GetAccountByMail(string mail)
        {
            return
                _mockDatabaseAccountList.FirstOrDefault(a => a.Mail != null && a.Mail.Equals(mail, StringComparison.OrdinalIgnoreCase));
        }

        internal Account AddUser(string name, string number, string email, string password)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("name must be set");
            if (String.IsNullOrEmpty(number)) throw new ArgumentException("number must be set");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("no email");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("password must be set");

            if (
                _mockDatabaseAccountList.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) != null)
            {
                throw new ArgumentException("Name already in use");
            }

            if (
                _mockDatabaseAccountList.FirstOrDefault(a => a.TelephoneNumber.Equals(number, StringComparison.OrdinalIgnoreCase)) != null)
            {
                throw new ArgumentException("Telephone Number already in use");
            }

            Account na = new Account()
            {
                AccountId = Guid.NewGuid(),
                Name = name,
                TelephoneNumber = number,
                Password = password,
                Mail = email
            };
            _mockDatabaseAccountList.Add(na);
            return na;
        }

        private void InitializeSampleAdvices()
        {
            // Create a sample Traditional User Account that only has a Username and Password
            // This will be used initially to demonstrate how to migrate to use Microsoft Passport

            Advice cat = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "Living & Housing"
            };

            Advice subCat = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "Housing"
            };


            Advice sampleAdvice = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "Registering / changing residence",
                Text = "sample text....",
                Office = GetOfficeByName("Office for Community Services and Information Processing"),
                Fees = "initial issue of the residence permit: EUR 100 to 250 extension of the residence permit: EUR 80 "+
                    "For underage children the administration fees are reduced respectively by half.",
            };

            sampleAdvice.CheckListItems.Add(new CheckListItem() {Name="passport"});
            sampleAdvice.CheckListItems.Add(new CheckListItem() { Name = "biometric photograph" });
            sampleAdvice.CheckListItems.Add(new CheckListItem() { Name = "proof of adequate health insurance" });
            sampleAdvice.CheckListItems.Add(new CheckListItem() { Name = "completed and signed application form", Link = "Download" });

            // Add the sampleUserAccount to the _mockDatabase
            subCat.Advices.Add(sampleAdvice);

            sampleAdvice = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "Deregistering residenc",
                Text = "sample text....",
                Office = GetOfficeByName("Office for Community Services and Information Processing")
            };

            // Add the sampleUserAccount to the _mockDatabase
            subCat.Advices.Add(sampleAdvice);

            cat.Advices.Add(subCat);
            _mockDatabaseAdviceCatList.Add(cat);

            //2. cat
            cat = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "Legal Advice"
            };

            subCat = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "XXX"
            };

            sampleAdvice = new Advice()
            {
                AdviceId = Guid.NewGuid(),
                Caption = "XXX Legal Advice",
                Text = "sample text....",
                Office = GetOfficeByName("Office for Community Services and Information Processing"),
                Fees = "Super expensive",
            };

            sampleAdvice.CheckListItems.Add(new CheckListItem() { Name = "passport" }); 
            sampleAdvice.CheckListItems.Add(new CheckListItem() { Name = "lot of papers", Link = "Download" });

            // Add the sampleUserAccount to the _mockDatabase
            subCat.Advices.Add(sampleAdvice);

            cat.Advices.Add(subCat);
            _mockDatabaseAdviceCatList.Add(cat);

            //SaveAsync();
        }

        private void InitializeSampleOffices()
        {
            // Create a sample Traditional User Account that only has a Username and Password
            // This will be used initially to demonstrate how to migrate to use Microsoft Passport

            Office off = new Office()
            {
                OfficeId = Guid.NewGuid(),
                Name = "Office for Community Services and Information Processing",
                Street = "Basler Straße 2",
                PLZ = "79100",
                City = "Freiburg",
                Phone = "0761 / 201 - 0",
                Mail = "buergeramt@stadt.freiburg.de"
            };

            off.OpeningHours.Add(new OpeningHours() {DayOfWeek = 1, StartOpen = 9, EndOpen = 12});
            off.OpeningHours.Add(new OpeningHours() { DayOfWeek = 2, StartOpen = 9, EndOpen = 12 });
            off.OpeningHours.Add(new OpeningHours() { DayOfWeek = 3, StartOpen = 9, EndOpen = 12 });
            off.OpeningHours.Add(new OpeningHours() { DayOfWeek = 3, StartOpen = 15, EndOpen = 18 });
            off.OpeningHours.Add(new OpeningHours() { DayOfWeek = 4, StartOpen = 9, EndOpen = 12 });
            off.OpeningHours.Add(new OpeningHours() { DayOfWeek = 5, StartOpen = 9, EndOpen = 12 });

            _mockDatabaseOfficeList.Add(off);

            //SaveAsync();
        }

        public List<Advice> GetAllAdviceCategories()
        {
            return _mockDatabaseAdviceCatList;
        }

        public Office GetOfficeByName(String name)
        {
            if (_mockDatabaseOfficeList.Any())
            {
                Office account = _mockDatabaseOfficeList.FirstOrDefault(f => f.Name.Equals(name));
                if (account != null)
                {
                    return account;
                }
            }
            return null;
        }
        public Office GetOfficeById(Guid id)
        {
            if (_mockDatabaseOfficeList.Any())
            {
                Office account = _mockDatabaseOfficeList.FirstOrDefault(f => ((Office)f).OfficeId.Equals(id));
                if (account != null)
                {
                    return account;
                }
            }
            return null;
        }


        #region Save and Load Helpers
        /// <summary>
        /// Create and save a useraccount list file. (Replacing the old one)
        /// </summary>
        private async void SaveAsync()
        {
            string accountsXml = SerializeAccountListToXml();
            string adviceListToXml = SerializeAdviceListToXml();
            string officeXml = SerializeOfficeListToXml();

            if (_persistenceWriter.HasFile(ADVICE_LIST_FILE_NAME))
            {
                _persistenceWriter.WriteFile(ADVICE_LIST_FILE_NAME, adviceListToXml);                
            }
            else
            {
                _persistenceWriter.CreateAndWriteFile(ADVICE_LIST_FILE_NAME, adviceListToXml);
            }

            if (_persistenceWriter.HasFile(OFFICES_LIST_FILE_NAME))
            {
                _persistenceWriter.WriteFile(OFFICES_LIST_FILE_NAME, officeXml);
            }
            else
            {
                _persistenceWriter.CreateAndWriteFile(OFFICES_LIST_FILE_NAME, officeXml);
            }
            
            if (_persistenceWriter.HasFile(ACCOUNT_LIST_FILE_NAME))
            {
                _persistenceWriter.WriteFile(ACCOUNT_LIST_FILE_NAME, accountsXml);
            }
            else
            {
                _persistenceWriter.CreateAndWriteFile(ACCOUNT_LIST_FILE_NAME, accountsXml);
            }
        }

        /// <summary>
        /// Gets the useraccount list file and deserializes it from XML to a list of useraccount objects.
        /// </summary>
        /// <returns>List of useraccount objects</returns>
        private async 
        /// <summary>
        /// Gets the useraccount list file and deserializes it from XML to a list of useraccount objects.
        /// </summary>
        /// <returns>List of useraccount objects</returns>
        Task
LoadAsync()
        {
            bool saveRequired = false;
            //1. Accounts
            if (_persistenceWriter.HasFile(ACCOUNT_LIST_FILE_NAME))
            {


                string accountsXml = _persistenceWriter.ReadFile(ACCOUNT_LIST_FILE_NAME);
                DeserializeXmlToAccountList(accountsXml);
            }

            // If the UserAccountList does not contain the sampleUser Initialize the sample users
            // This is only needed as it in a Hand on Lab to demonstrate a user migrating
            // In the real world user accounts would just be in a database
            if (!_mockDatabaseAccountList.Any(f => f.Name.Equals("user")))
            {
                //If the list is empty InitializeSampleAccounts and return the list
                InitializeSampleAccounts();
                saveRequired = true;
            }


            //2. Offices
            //ms-appx:///Resources/logo.big.png
            var uri = new System.Uri("ms-appx:///Resources/offices.txt");
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            if (file.IsAvailable)
            {
                string accountsXml = await FileIO.ReadTextAsync(file);
                DeserializeXmlToOfficeList(accountsXml);
            } else if (_persistenceWriter.HasFile(OFFICES_LIST_FILE_NAME))
            {


                string accountsXml = _persistenceWriter.ReadFile(OFFICES_LIST_FILE_NAME);
                DeserializeXmlToOfficeList(accountsXml);
            }

            // If the UserAccountList does not contain the sampleUser Initialize the sample users
            // This is only needed as it in a Hand on Lab to demonstrate a user migrating
            // In the real world user accounts would just be in a database
            if (!_mockDatabaseOfficeList.Any(f => f.Name.Equals("Office for Community Services and Information Processing")))
            {
                //If the list is empty InitializeSampleAccounts and return the list
                InitializeSampleOffices();
                saveRequired = true;
            }
             
            await LoadAdvices(saveRequired);
            GenerateFakeTickets();
        }

        private async Task LoadAdvices(bool saveRequired)
        {
            //2. Avices
            var uri = new System.Uri("ms-appx:///Resources/advices.txt");
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            if (file.IsAvailable)
            {
                string accountsXml = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
                DeserializeXmlToAdviceList(accountsXml);
            }
            else if (_persistenceWriter.HasFile(ADVICE_LIST_FILE_NAME))
            {
                string accountsXml = _persistenceWriter.ReadFile(ADVICE_LIST_FILE_NAME);
                DeserializeXmlToAdviceList(accountsXml);
            }

            // If the UserAccountList does not contain the sampleUser Initialize the sample users
            // This is only needed as it in a Hand on Lab to demonstrate a user migrating
            // In the real world user accounts would just be in a database
            if (!_mockDatabaseAdviceCatList.Any(f => f.Caption.Equals("Living & Housing") && f.Advices.Count > 0))
            {
                //If the list is empty InitializeSampleAccounts and return the list
                InitializeSampleAdvices();
                saveRequired = true;
            }

            /*if (saveRequired)
            {
                SaveAsync();
            }*/
        }

        /// <summary>
        /// Uses the local list of advices and returns an XML formatted string representing the list
        /// </summary>
        /// <returns>XML formatted list of accounts</returns>
        private string SerializeAdviceListToXml()
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(List<Advice>));
            StringWriter writer = new StringWriter();
            xmlizer.Serialize(writer, _mockDatabaseAdviceCatList);
            return writer.ToString();
        }

        /// <summary>
        /// Uses the local list of accounts and returns an XML formatted string representing the list
        /// </summary>
        /// <returns>XML formatted list of accounts</returns>
        private string SerializeAccountListToXml()
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(List<Account>));
            StringWriter writer = new StringWriter();
            xmlizer.Serialize(writer, _mockDatabaseAccountList);
            return writer.ToString();
        }

        

        /// <summary>
        /// Takes an XML formatted string representing a list of accounts and returns a list object of accounts
        /// </summary>
        /// <param name="listAsXml">XML formatted list of accounts</param>
        /// <returns>List object of accounts</returns>
        private List<Advice> DeserializeXmlToAdviceList(string listAsXml)
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(List<Advice>));
            TextReader textreader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(listAsXml)));
            return _mockDatabaseAdviceCatList = (xmlizer.Deserialize(textreader)) as List<Advice>;
        }

        /// <summary>
        /// Takes an XML formatted string representing a list of accounts and returns a list object of accounts
        /// </summary>
        /// <param name="listAsXml">XML formatted list of accounts</param>
        /// <returns>List object of accounts</returns>
        private List<Account> DeserializeXmlToAccountList(string listAsXml)
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(List<Account>));
            TextReader textreader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(listAsXml)));
            return _mockDatabaseAccountList = (xmlizer.Deserialize(textreader)) as List<Account>;
        }

        /// <summary>
        /// Uses the local list of accounts and returns an XML formatted string representing the list
        /// </summary>
        /// <returns>XML formatted list of accounts</returns>
        private string SerializeOfficeListToXml()
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(List<Office>));
            StringWriter writer = new StringWriter();
            //Remove Fake hours
            List<Office> _trueOffices = _mockDatabaseOfficeList.Select(item => (Office)item.Clone()).ToList();


            xmlizer.Serialize(writer, _trueOffices);
            return writer.ToString();
        }

        /// <summary>
        /// Takes an XML formatted string representing a list of accounts and returns a list object of accounts
        /// </summary>
        /// <param name="listAsXml">XML formatted list of accounts</param>
        /// <returns>List object of accounts</returns>
        private List<Office> DeserializeXmlToOfficeList(string listAsXml)
        {
            XmlSerializer xmlizer = new XmlSerializer(typeof(List<Office>));
            TextReader textreader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(listAsXml)));
            return _mockDatabaseOfficeList = (xmlizer.Deserialize(textreader)) as List<Office>;
        }
        #endregion
    }
}
