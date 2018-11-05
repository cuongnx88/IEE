using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Configuration.Types;

namespace IEE.Infrastructure
{
    // Reference https://social.msdn.microsoft.com/Forums/en-US/420a0a36-984f-4922-bc72-3e9739422049/custom-convention-for-oracle-schema-naming?forum=adonetefx
    
    public class DefaultSchemaConvention 
        //:IConfigurationConvention<Type, EntityTypeConfiguration>
    {
        //string defaultSchema;
        //public DefaultSchemaConvention(string defaultSchema)
        //{
        //    if (String.IsNullOrWhiteSpace(defaultSchema))
        //        throw new ArgumentException("defaultSchema");
        //    this.defaultSchema = defaultSchema;
        //}

        //void IConfigurationConvention<Type, EntityTypeConfiguration>.Apply(
        //     Type memberInfo, Func<EntityTypeConfiguration> configuration)
        //{
        //    EntityTypeConfiguration cfg = configuration();
        //    string tableName = cfg.EntitySetName;
        //    if (String.IsNullOrEmpty(tableName))
        //        tableName = memberInfo.Name;
        //    cfg.ToTable(tableName, this.defaultSchema);
        //}
    }  
}
