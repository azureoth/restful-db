# Azureoth RESTful Database Module

This is a middleware that can be added to any ASP.NET core project to create a RESTful API for the underlying SQL Database

## Usage
```C#
        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureRestfulDb(
                connectionString: @"<Connection_String>",	// Database connection string
                apiPrefix: "/api/apps");					// API path that the db will be exposed under

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRestfulDb();

            app.UseMvc();
        }
```