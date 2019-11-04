using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNetCore.Hosting; // core 2.2
using Microsoft.Extensions.Hosting; // core 3.0
using System;
using WebSocketManager;

namespace ChatApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketService();
        }

        //public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider) // core 2.2
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider) // core 3.0

        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebSockets();
            app.MapWebSocketManager("/channel", serviceProvider.GetService<ChatMessageHandler>());
            app.UseStaticFiles();
        }
    }
}
