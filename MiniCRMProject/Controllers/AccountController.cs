using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniCRMProject.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

namespace MiniCRMProject.Controllers
{
    public class AccountController : Controller
    {
        string connectionString;

        public AccountController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Users (FirstName, LastName, Email, Password, Role) 
                               VALUES (@firstName, @lastName, @email, @password, 'Customer')";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@firstName", user.FirstName);
                command.Parameters.AddWithValue("@lastName", user.LastName);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Users WHERE Email = @email AND Password = @password";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    HttpContext.Session.SetInt32("UserId", Convert.ToInt32(reader["UserId"]));
                    HttpContext.Session.SetString("UserFullName", reader["FirstName"].ToString() + " " + reader["LastName"].ToString());
                    HttpContext.Session.SetString("UserRole", reader["Role"].ToString());

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "E-posta veya şifre hatalı. Lütfen tekrar deneyin.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
