using System.ComponentModel.DataAnnotations;
using Lemoo_pos.Common.Enums;

namespace Lemoo_pos.Models.Entities
{
    public class Store : BaseEntity
    {
        public required string Name { get; set; }
        // public bool AllowManageShift { get; set; } = false;
        public SaveStatus SaveStatus { get; set; } = SaveStatus.PENDING;

    }
}