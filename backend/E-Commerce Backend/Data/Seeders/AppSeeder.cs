namespace E_Commerce_Backend.Data.Seeders
{
    public class AppSeeder : IAppSeeder
    {
        private readonly IdentitySeeder _identitySeeder;
        private readonly DomainSeeder _domainSeeder;

        public AppSeeder(IdentitySeeder identitySeeder, DomainSeeder domainSeeder)
        {
            _identitySeeder = identitySeeder;
            _domainSeeder = domainSeeder;
        }

        public async Task SeedAsync()
        {
            await _identitySeeder.SeedRolesAsync();
            await _identitySeeder.SeedAdminAsync();
            await _domainSeeder.SeedAsync();
        }
    }
}
