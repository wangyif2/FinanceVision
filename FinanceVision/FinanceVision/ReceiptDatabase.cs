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

        private void NotifyPropertyChanged(string entryprice)
        {
            throw new NotImplementedException();
        }

        private void NotifyPropertyChanging(string entryprice)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
    }

}
