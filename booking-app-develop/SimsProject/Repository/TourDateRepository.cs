using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class TourDateRepository
    {

        private const string FilePath = "../../../Resources/Data/tourDates.csv";

        private readonly Serializer<TourDate> _serializer;
        private List<TourDate> _tourDates;

        public TourDateRepository()
        {
            _serializer = new Serializer<TourDate>();
            _tourDates = _serializer.FromCsv(FilePath);
        }

        public List<TourDate> GetAll()
        {
            List<TourDate> tourDates = _serializer.FromCsv(FilePath);
            return tourDates;
        }

        public int NextId()
        {
            _tourDates = GetAll();
            if (_tourDates.Count < 1)
            {
                return 1;
            }
            return _tourDates.Max(t => t.Id) + 1;
        }

        public void Save(Tour tour, DateTime? dateTime)
        {
            var id = NextId();
            TourDate tourDate = new TourDate(id, tour, dateTime, false);
            _tourDates = GetAll();
            _tourDates.Add(tourDate);
            _serializer.ToCsv(FilePath, _tourDates);
        }

        public void Update(TourDate tourDate)
        {
            _tourDates = GetAll();
            TourDate currentDate = _tourDates.Find(d => d.Id == tourDate.Id);

            currentDate.HasEnded = tourDate.HasEnded;
            var index = _tourDates.IndexOf(currentDate);

            _tourDates.Remove(currentDate);
            _tourDates.Insert(index, currentDate);
            _serializer.ToCsv(FilePath, _tourDates);
        }

        public List<TourDate> GetByParentId(int parentId)
        {
            _tourDates = GetAll();
            return _tourDates.FindAll(t => t.Tour.Id == parentId);
        }

        public void DeleteByParentId(TourDate tourDate)
        {
            _tourDates = _serializer.FromCsv(FilePath);
            var found = _tourDates.Find(d => d.Id == tourDate.Id);
            _tourDates.Remove(found);
            _serializer.ToCsv(FilePath, _tourDates);
        }
    }
}
