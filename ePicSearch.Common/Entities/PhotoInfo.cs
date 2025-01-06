using System.Xml.Linq;

namespace ePicSearch.Infrastructure.Entities
{
    public class PhotoInfo
    {
        public string FilePath { get; set; } = "";
        public string Name { get; set; } = "";
        public string Code { get; set; } = "";
        public int SerialNumber { get; set; }
        public string AdventureName { get; set; } = "";
        public bool IsLocked { get; set; }
        public double Rotation { get; set; }
        public bool ShowArrow { get; set; }
        public bool IsTreasurePhoto { get; set; }

        public override string ToString()
        {
            return $"PhotoInfo [FilePath: {FilePath}, Name: {Name}, " +
                $"Code: {Code}, SerialNumber: {SerialNumber}, AdventureName: {AdventureName}, " +
                $"IsLocked: {IsLocked}, Rotation: {Rotation}]";
        }
    }
}
