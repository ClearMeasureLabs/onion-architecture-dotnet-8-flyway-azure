using System;
using Core.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings
{
    public class WorkOrderStatusConverter : ValueConverter<WorkOrderStatus, string>
    {
        public WorkOrderStatusConverter() 
            : base(
                v => v.Code,
                v => WorkOrderStatus.FromCode(v),
                null)
        {
        }
    }
}