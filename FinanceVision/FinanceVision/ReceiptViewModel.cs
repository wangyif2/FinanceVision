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

        private ObservableCollection<ReceiptEntry> _thisWeekEntries;
        public ObservableCollection<ReceiptEntry> ThisWeekEntries
        {
            get { return _thisWeekEntries; }
            set
            {
                _thisWeekEntries = value;
                NotifyPropertyChanged("ThisWeekEntries");
            }
        }

        private ObservableCollection<ReceiptEntry> _thisMonthEntries;
        public ObservableCollection<ReceiptEntry> ThisMonthEntries
        {
            get { return _thisMonthEntries; }
            set
            {
                _thisMonthEntries = value;
                NotifyPropertyChanged("ThisMonthEntries");
            }
        }

        private ObservableCollection<ReceiptEntry> _todayEntries;
        public ObservableCollection<ReceiptEntry> TodayEntries
        {
            get { return _todayEntries; }
            set
            {
                _todayEntries = value;
                NotifyPropertyChanged("TodayEntries");
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

        //This is the default load function that loads all entries
        public void LoadEntriesFromDatabase()
        {

            //AllReceiptEntries = DB_LoadAll();
            ThisMonthEntries = DB_LoadByMonth(DateTime.Today.Month);
            ThisWeekEntries = DB_LoadThisWeek();
            TodayEntries = DB_LoadByDate(DateTime.Today);
            //var categoriesInDB = from ActivityCategory category in receiptDatabase.categories
            //                  select category;
            //AllCategories = new ObservableCollection<ActivityCategory>(categoriesInDB);
            
        }

        //Load all entries
        public ObservableCollection<ReceiptEntry> DB_LoadAll()
        {
            var entriesInDB = from ReceiptEntry entry in receiptDatabase.entries
                              orderby entry.EntryDate
                              select entry;
            return new ObservableCollection<ReceiptEntry>(entriesInDB);
        }
        
        //Select a very specific date
        public ObservableCollection<ReceiptEntry> DB_LoadByDate(DateTime date)
        {
            var entries = from ReceiptEntry entry in receiptDatabase.entries
                          where DateTime.Compare(entry.EntryDate,date) == 0
                          orderby entry.EntryDate
                          select entry;
            return new ObservableCollection<ReceiptEntry>(entries);
        }

        //Specify a month
        public ObservableCollection<ReceiptEntry> DB_LoadByMonth(int Month)
        {
            var entries = from ReceiptEntry entry in receiptDatabase.entries
                          where entry.EntryDate.Month == Month
                          orderby entry.EntryDate
                          select entry;
            return new ObservableCollection<ReceiptEntry>(entries);
        }

        //Specify a week 
        public ObservableCollection<ReceiptEntry> DB_LoadThisWeek()
        {


            var entries = from ReceiptEntry entry in receiptDatabase.entries
                          where DateTime.Compare(entry.EntryDate, Helper.StartOfWeek) > 0 &&
                                DateTime.Compare(entry.EntryDate, Helper.EndOfWeek) < 0
                          orderby entry.EntryDate
                          select entry;
            return new ObservableCollection<ReceiptEntry>(entries);
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

            if (newEntry.EntryDate.Month == DateTime.Today.Month)
                ThisMonthEntries.Add(newEntry);

            if (Helper.IsThisWeek(newEntry.EntryDate))
                ThisWeekEntries.Add(newEntry);

            if (DateTime.Compare(newEntry.EntryDate, DateTime.Today) == 0)
                TodayEntries.Add(newEntry);
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