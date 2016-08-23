using OSharp.Data.Entity;
using OSharp.Data.Entity.Migrations;


namespace CDKX.Services.Implement.DbSeed
{
    public class CreateDatabaseIfNotExistsWithSeed : CreateDatabaseIfNotExistsWithSeedBase<DefaultDbContext>
    {
        public CreateDatabaseIfNotExistsWithSeed()
        {
            SeedActions.Add(new CreateSysRoleSeedAction());
            SeedActions.Add(new CreateCitySeedAction());
        }
    }
}