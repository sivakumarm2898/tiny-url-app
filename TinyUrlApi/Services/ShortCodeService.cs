using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;

namespace TinyUrlApi.Services
{
    public class ShortCodeService
    {
        public string GenerateShortCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        public async Task<string> GenerateUniqueCode(AppDbContext db)
        {
            string code;
            do
            {
                code = GenerateShortCode();
            }
            while (await db.Urls.AnyAsync(x => x.ShortCode == code));

            return code;
        }
    }
}
