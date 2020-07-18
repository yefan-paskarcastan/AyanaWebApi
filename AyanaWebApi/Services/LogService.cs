using System;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class LogService : ILogService
    {
        public LogService(AyDbContext ayDbContext)
        {
            _context = ayDbContext;
        }

        /// <summary>
        /// Записать в лог
        /// </summary>
        /// <param name="serviceResult"></param>
        public void Write(IServiceResult serviceResult)
        {
            _context.Logs.Add(new Log
            {
                Created = DateTime.Now,
                ServiceName = serviceResult.ServiceName,
                MethodName = serviceResult.MethodName,
                Location = serviceResult.Location,
                ExceptionMessage = serviceResult.ExceptionMessage,
                ErrorContent = serviceResult.ErrorContent,
                Message = serviceResult.Comment
            });
            _context.SaveChanges();
        }

        AyDbContext _context;
    }
}
