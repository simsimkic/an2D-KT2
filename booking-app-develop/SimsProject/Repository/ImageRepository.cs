using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class ImageRepository
    {
        private readonly string _filePath = "../../../Resources/Data/";

        private readonly Serializer<Image> _serializer;

        private List<Image> _images;

        public ImageRepository(string fileName)
        {
            _filePath += fileName;
            _serializer = new Serializer<Image>();
            _images = _serializer.FromCsv(_filePath);
        }

        public List<Image> GetAll()
        {
            return _serializer.FromCsv(_filePath);
        }

        public Image Save(Image image)
        {
            image.Id = NextId();
            _images = _serializer.FromCsv(_filePath);
            _images.Add(image);
            _serializer.ToCsv(_filePath, _images);
            return image;
        }

        public void SaveAllByParentId(int parentId, List<Image> images)
        {
            foreach (var image in images)
            {
                image.ParentId = parentId;
                Save(image);
            }
        }

        public int NextId()
        {
            _images = _serializer.FromCsv(_filePath);
            if (_images.Count < 1)
            {
                return 1;
            }
            return _images.Max(i => i.Id) + 1;
        }

        public void Delete(Image image)
        {
            _images = _serializer.FromCsv(_filePath);
            Image found = _images.Find(a => a.Id == image.Id);
            _images.Remove(found);
            _serializer.ToCsv(_filePath, _images);
        }

        public void DeleteAllByParentId(int parentId)
        {
            _images = _serializer.FromCsv(_filePath);
            var found = _images.FindAll(i => i.ParentId == parentId);
            foreach (var image in found)
            {
                _images.Remove(image);
            }
            _serializer.ToCsv(_filePath, _images);
        }

        public List<Image> GetByParentId(int parentId)
        {
            _images = _serializer.FromCsv(_filePath);
            return _images.FindAll(i => i.ParentId == parentId);
        }
    }
}