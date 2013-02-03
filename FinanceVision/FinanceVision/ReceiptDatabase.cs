using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceVision
{
    public class ReceiptDatabase : DataContext
    {
        public ReceiptDatabase(String connectionString) : base(connectionString)
        {
        }

        public Table<ReceiptEntry> entries;
        public Table<ActivityCategory> categories;

    }

    [Table]
    public class ReceiptEntry : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private int _entryId;
        
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int EntryId
        {
            get { return _entryId;  }
            set
            {
                if (_entryId != value)
                {
                    NotifyPropertyChanging("EntryId");
                    _entryId = value;
                    NotifyPropertyChanged("EntryId");
                }
            }
        }

        private string _entryDate;

        [Column]
        public string EntryDate
        {
            get { return _entryDate;  }
            set { _entryDate = value;  }
        }

        private string _entryName;

        [Column]
        public string EntryName
        {
            get { return _entryName; }
            set
            {
                if (_entryName != value)
                {
                    NotifyPropertyChanging("EntryName");
                    _entryName = value;
                    //_entryDate = DateTime.Today;
                    NotifyPropertyChanged("EntryName");

                }
            }
        }

        private float _entryPrice;

        [Column]
        public float EntryPrice
        {
            get { return _entryPrice;  }
            set
            {
                if (_entryPrice != value)
                {
                    NotifyPropertyChanging("EntryPrice");
                    _entryPrice = value;
                    NotifyPropertyChanged("EntryPrice");
                }
            }
        }

        [Column] internal int _categoryId;

        private EntityRef<ActivityCategory> _entryCategory;

        [Association(Storage = "_entryCategory", ThisKey = "_categoryId", OtherKey = "Id", IsForeignKey = true)]
        public ActivityCategory EntryCategory
        {
            get { return _entryCategory.Entity;  }
            set
            {
                NotifyPropertyChanging("EntryCategory");
                _entryCategory.Entity = value;
                if (value != null)
                {
                    _categoryId = value.Id;
                }
                NotifyPropertyChanged("EntryCategory");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }

    [Table]
    public class ActivityCategory : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // Define category name: private field, public property, and database column.
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                _imagePath = "/Images/" + value + "_Icon_202_white.png";
                NotifyPropertyChanged("Name");
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath;  }
        }

        // Define the entity set for the collection side of the relationship.
        private EntitySet<ReceiptEntry> _entries;

        [Association(Storage = "_entries", OtherKey = "_categoryId", ThisKey = "Id")]
        public EntitySet<ReceiptEntry> Entries
        {
            get { return this._entries; }
            set { this._entries.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public ActivityCategory()
        {
            _entries = new EntitySet<ReceiptEntry>(
                new Action<ReceiptEntry>(this.attach_Entry),
                new Action<ReceiptEntry>(this.detach_Entry)
                );
        }

        // Called during an add operation
        private void attach_Entry(ReceiptEntry entry)
        {
            NotifyPropertyChanging("ReceiptEntry");
            entry.EntryCategory = this;
        }

        // Called during a remove operation
        private void detach_Entry(ReceiptEntry entry)
        {
            NotifyPropertyChanging("ReceiptEntry");
            entry.EntryCategory = null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

}
