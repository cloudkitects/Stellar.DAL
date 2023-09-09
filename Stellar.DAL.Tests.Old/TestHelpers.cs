using System;
using System.Dynamic;
using System.IO;
using System.Linq;

using AutoMapper;

using Stellar.DAL.Tests.Data;

namespace Stellar.DAL.Tests
{
    public class TestHelpers
    {
        #region Automapper setup
        public static readonly MapperConfiguration MapperConfig;
        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                CreateMap<CustomerModel, Customer>();
                CreateMap<Customer, CustomerModel>();
            }
        }

        public static readonly Mapper Mapper;
        #endregion

        #region constructors
        static TestHelpers()
        {
            MapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            Mapper = new Mapper(MapperConfig);
        }
        #endregion

        #region helpers
        private static readonly Random Random = new();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string RandomDigits(int length)
        {
            const string chars = "0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string ParseSqlFile(string name, string database)
        {
            using var reader = new StreamReader(new FileStream(name, FileMode.Open));
            
            var template = reader.ReadToEnd();

            return string.Format(template, database);
        }

        /// <summary>
        /// Map an object with auto mapper.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        public static T Map<T>(object o)
        {
            return Mapper.Map<T>(o);
        }
        #endregion
    }
}
