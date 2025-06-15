namespace JishitongBackend
{
    using JishitongBackend.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using JishitongBackend.Services;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            // ���� Web Ӧ�ó���
            var builder = WebApplication.CreateBuilder(args);

            // ��ȡ�����ַ���
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 1. ע�� MySqlConnection ���񣨱���ԭ�й��ܣ�
            builder.Services.AddSingleton<MySqlConnection>(new MySqlConnection(connectionString));

            // 2. ע�� Service ����
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<GroupOrderService>();
            builder.Services.AddScoped<MessageService>();

            // 3. ���� JWT ��֤
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // ���ӿ�������Swagger �ȷ���
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // 注册 HttpClient 服务
            builder.Services.AddHttpClient();

            // ����Ӧ�ó���
            var app = builder.Build();

            // ���ÿ��������� Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // �����м��
            app.UseHttpsRedirection();
            app.UseAuthentication();  // ������֤�м��
            app.UseAuthorization();   // ������Ȩ�м��
            app.MapControllers();
            app.UseCors("AllowAll");

            // ����Ӧ��
            app.Run();
        }
    }
}
