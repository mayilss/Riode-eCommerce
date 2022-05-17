using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Riode.WebUI.Models.Entities;
using System;
using System.Linq;

namespace Riode.WebUI.Models.DataContexts
{
    public static class RiodeDbSeed
    {
        static internal IApplicationBuilder InitDb(this IApplicationBuilder app)
        {
            using(var scope = app.ApplicationServices.CreateScope()){
                var db = scope.ServiceProvider.GetRequiredService<RiodeDbContext>();
                db.Database.Migrate();
                InitPostTags(db);
            }
            return app;
        }

        private static void InitPostTags(RiodeDbContext db)
        {
            if (!db.PostTags.Any())
            {
                db.PostTags.AddRange(new[]
                {
                    new PostTag{Name="Bag"},
                    new PostTag{Name="Classic"},
                    new PostTag{Name="Converse"},
                    new PostTag{Name="Leather"},
                    new PostTag{Name="Fit"},
                    new PostTag{Name="Grren"},
                    new PostTag{Name="Man"},
                    new PostTag{Name="Woman"},
                    new PostTag{Name="Jeans"}
                });
                db.SaveChanges();
            }
        }
    }
}
