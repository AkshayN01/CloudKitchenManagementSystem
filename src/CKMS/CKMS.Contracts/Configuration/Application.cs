﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Contracts.Configuration
{
    public class Application
    {
        public String VerficationUrl { get; set; }
        public Connection connection { get; set; }
        public JWTAuthentication JWTAuthentication { get; set; }
    }
    public class JWTAuthentication
    {
        public String secretKey { get; set; }
        public String issuer { get; set; }
        public String audience { get; set; }
    }
    public class Connection
    {
        public SendGridOptions sendGridOptions { get; set; }
        public RabbitMqConfiguration rabbitMqConfiguration { get; set; }
    }
    public class SendGridOptions
    {
        public String APIKey { get; set; }
        public String FromEmail { get; set; }
        public String FromName { get; set; }
    }
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
