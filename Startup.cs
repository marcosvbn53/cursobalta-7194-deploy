using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Controllers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;

namespace Shop{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration {get;}

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();            
            services.AddSwaggerGen();

            services.AddResponseCompression(options => 
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {"application/json"});

            });


            //habilitando o método abaixo você vai colocar todos os métodos da aplicação com Cache
            //services.AddResponseCaching();

            services.AddControllers();    

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(px => {
                px.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                px.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(pw =>{

                pw.RequireHttpsMetadata = false;
                pw.SaveToken = true;
                pw.TokenValidationParameters = new TokenValidationParameters{
                    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));

            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));

            //services.AddScoped<DataContext>();    
            
                
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo{Title = "Shop Api", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //  if(env.IsDevelopment())
            //  {
                app.UseDeveloperExceptionPage();
            //      app.UseSwagger();
            //      app.UseSwaggerUI();
            //  }
            
            //Vai forçar que nossa api só aceite HTTPS, é muito importante pra evitar requisições inseguras
            app.UseHttpsRedirection();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
        
            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
                endpoint.Map("/Status",()=> "Só testando");
            });            
        }
    }
}