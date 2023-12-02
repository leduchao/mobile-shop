using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileWeb.Models.Entities
{
    public class Specifications // thong so ky thuat
    {
        [Key]
        public int Id { get; set; }

        public int RAM { get; set; }

        public int ROM { get; set; }

        public int NumberOfFrontCamera { get; set; } // so luong camera truoc

        public int NumberOfBehindCamera { get; set; } // so luong camera sau

        public string? OperatingSystem { get; set; }

        public string? Brand { get; set; } // nhan hieu = ten category tuong ung

        public string? Model { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
