using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimsProject.Domain.Model;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class TourVoucherRepository 
    {
        private const string FilePath = "../../../Resources/Data/tourVouchers.csv";

        private readonly Serializer<TourVoucher> _serializer;
        private List<TourVoucher> _tourVouchers;

        public TourVoucherRepository()
        {
            _serializer = new Serializer<TourVoucher>();
            _tourVouchers = _serializer.FromCsv(FilePath);
        }

        public List<TourVoucher> GetAll()
        {
            List<TourVoucher> tourVouchers = _serializer.FromCsv(FilePath);
            return tourVouchers;
        }

        public int NextId()
        {
            _tourVouchers = GetAll();
            if (_tourVouchers.Count < 1)
            {
                return 1;
            }
            return _tourVouchers.Max(v => v.Id) + 1;
        }

        public void Save(User guest, DateTime validUntil)
        {
            var id = NextId();
            TourVoucher tourVoucher = new (id, guest, validUntil);
            _tourVouchers = GetAll();
            _tourVouchers.Add(tourVoucher);
            _serializer.ToCsv(FilePath, _tourVouchers);
        }
    }
}
