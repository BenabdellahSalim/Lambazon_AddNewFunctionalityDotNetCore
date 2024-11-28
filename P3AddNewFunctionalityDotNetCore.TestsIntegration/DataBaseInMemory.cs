using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.TestsIntegration
{
    public class DataBaseInMemory
    {
        public P3Referential GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<P3Referential>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new P3Referential(options);
            context.Database
                .EnsureCreated();
            return context;
        }
    }
    
}
