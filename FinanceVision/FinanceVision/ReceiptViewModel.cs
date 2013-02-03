using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceVision
{
    public class ReceiptViewModel : INotifyPropertyChanged
    {
    
        // LINQ to SQL data context for the local database.
        private ReceiptDatabase receiptDatabase;

        // Class constructor, create the data context object.
        public ReceiptViewModel(string receiptDatabaseConnectionString)
        {
            receiptDatabase = new ReceiptDatabase(receiptDatabaseConnectionString);
        }

        // All receipt entries
        private ObservableCollection<ReceiptEntry> _allReceiptEntries;
        public ObservableCollection<ReceiptEntry> AllReceiptEntries
        {
            get { return _allReceiptEntries; }
            set
            {
                _allReceiptEntries = value;
                NotifyPropertyChanged("AllReceiptEntries");
            }
        }

        // All categories entries
        private ObservableCollection<ActivityCategory> _allCategories;
        public ObservableCollection<ActivityCategory> AllCategories
        {
            get { return _allCategories; }
            set
            {
                _allCategories = value;
                NotifyPropertyChanged("AllCategories");
            }
        }


        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            receiptDatabase.SubmitChanges();
        }

        public void LoadEntriesFromDatabase()
        {
            var entriesInDB = from ReceiptEntry entry in receiptDatabase.entries
                              select entry;

            AllReceiptEntries = new ObservableCollection<ReceiptEntry>(entriesInDB);

            var categoriesInDB = from ActivityCategory category in receiptDatabase.categories
                              select category;
            AllCategories = new ObservableCollection<ActivityCategory>(categoriesInDB);
            
        }

        // Add a new entry to the database and collections.
        public void AddEntry(ReceiptEntry newEntry)
        {
            // Add a new entry to the data context.
            receiptDatabase.entries.InsertOnSubmit(newEntry);

            // Save changes to the database.
            receiptDatabase.SubmitChanges();

            // Add a new entry to the "all" observable collection.
            AllReceiptEntries.Add(newEntry);
            

        }
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}