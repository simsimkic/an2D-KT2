using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class CheckPointRepository
    {
        private const string FilePath = "../../../Resources/Data/checkPoints.csv";

        private readonly Serializer<CheckPoint> _serializer;
        private List<CheckPoint> _checkPoints;

        public CheckPointRepository()
        {
            _serializer = new Serializer<CheckPoint>();
            _checkPoints = _serializer.FromCsv(FilePath);
        }

        public List<CheckPoint> GetAll()
        {
            List<CheckPoint> checkPoints = _serializer.FromCsv(FilePath);
            return checkPoints;
        }

        public int NextId()
        {
            _checkPoints = GetAll();
            if (_checkPoints.Count < 1)
            {
                return 1;
            }
            return _checkPoints.Max(c => c.Id) + 1;
        }

        public void Save(bool isActive, Tour tour, int serialNumber, string name)
        {
            var id = NextId();
            CheckPoint checkPoint = new CheckPoint(id, isActive, tour, serialNumber, name);
            _checkPoints = GetAll();
            _checkPoints.Add(checkPoint);
            _serializer.ToCsv(FilePath, _checkPoints);
        }

        public List<CheckPoint> GetByParentId(int parentId)
        {
            _checkPoints = GetAll();
            return _checkPoints.FindAll(i => i.Tour.Id == parentId);
        }

        public void Update(CheckPoint checkPoint, int tourId)
        {
            _checkPoints = GetAll();
            int index;
            CheckPoint oldCheckPoint = _checkPoints.Find(c => c.IsActive && c.Tour.Id == tourId);
            if (oldCheckPoint != null)
            {
                oldCheckPoint.IsActive = false;
                index = _checkPoints.IndexOf(oldCheckPoint);
                _checkPoints.Remove(oldCheckPoint);
                _checkPoints.Insert(index, oldCheckPoint);
            }

            CheckPoint currentCheckPoint = _checkPoints.Find(c => c.Id == checkPoint.Id && c.Tour.Id == tourId);
            currentCheckPoint.IsActive = true;
            index = _checkPoints.IndexOf(currentCheckPoint);
            _checkPoints.Remove(currentCheckPoint);
            _checkPoints.Insert(index, currentCheckPoint);
            _serializer.ToCsv(FilePath, _checkPoints);
        }
        public void SetDefault()
        {
            foreach (var checkPoint in _checkPoints)
            {
                checkPoint.IsActive = false;
                _serializer.ToCsv(FilePath, _checkPoints);
            }
        }

        internal CheckPoint Get(int id)
        {
            _checkPoints = GetAll();
            return _checkPoints.FirstOrDefault(i => i.Id == id);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _checkPoints = _serializer.FromCsv(FilePath);
            var found = _checkPoints.FindAll(i => i.Tour.Id == parentId);
            foreach (var checkPoint in found)
            {
                _checkPoints.Remove(checkPoint);
            }
            _serializer.ToCsv(FilePath, _checkPoints);
        }
    }
}
