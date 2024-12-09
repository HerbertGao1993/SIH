using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWSInterface
{
    class History
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Volume { get; set; }
        public string Weight { get; set; }

        public string TimeStamp { get; set; }
        public string FileFullName { get; set; }
        public bool ReadStatus { get; set; }
        public string Destination { get; set; }
        public History(string Id, string Code, string Length, string Width, string Height, string Volume, string Weight, string TimeStamp, string FileFullName, bool ReadStatus, string Destination)
        {
            this.Id = Id;
            this.Code = Code;
            this.Length = Length;
            this.Width = Width;
            this.Height = Height;
            this.Volume = Volume;
            this.Weight = Weight;
            this.TimeStamp = TimeStamp;
            this.FileFullName = FileFullName;
            this.ReadStatus = ReadStatus;
            this.Destination = Destination;
        }

    }
}
