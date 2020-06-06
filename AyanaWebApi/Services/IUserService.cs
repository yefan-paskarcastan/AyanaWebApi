﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AyanaWebApi.Models;

namespace AyanaWebApi.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Если пользователь существует и пароль верен, выдает ему токен
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Пользователь с токеном</returns>
        Task<User> Authenticate(string username, string password);

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="password">Пароль</param>
        /// <returns>Созданный пользователь</returns>
        Task<User> Create(User user);
    }
}