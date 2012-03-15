using System;
using System.Configuration;
using Migrator.Framework;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    public class ProviderFactoryTest
    {

        [Test]
        public void CanGetDialectsForProvider()
        {
            string[] providers = new string[] { "SqlServer", "Mysql", "SQLite", "PostgreSQL", "SqlServer2005", "SqlServerCe", "Oracle" };
            Array.ForEach(providers,
                          delegate(string provider) { Assert.IsNotNull(ProviderFactory.DialectForProvider(provider)); });
            Assert.IsNull(ProviderFactory.DialectForProvider(null));
            Assert.IsNull(ProviderFactory.DialectForProvider(""));
            Assert.IsNull(ProviderFactory.DialectForProvider("foofoofoo"));
        }

        [Test, Category("SqlServer")]
        public void CanLoad_SqlServerProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SqlServer",
                                                                      ConfigurationManager.AppSettings[
                                                                          "SqlServerConnectionString"], null);
            Assert.IsNotNull(provider);
        }


        [Test, Category("SqlServerCe")]
        public void CanLoad_SqlServerCeProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SqlServerCe",
                                                                      ConfigurationManager.AppSettings[
                                                                          "SqlServerCeConnectionString"], null);
            Assert.IsNotNull(provider);
        }


        [Test, Category("SqlServer2005")]
        public void CanLoad_SqlServer2005Provider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SqlServer2005",
                                                                      ConfigurationManager.AppSettings[
                                                                          "SqlServer2005ConnectionString"], null);
            Assert.IsNotNull(provider);
        }

        [Test, Category("MySql")]
        public void CanLoad_MySqlProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("MySql",
                                                                      ConfigurationManager.AppSettings[
                                                                          "MySqlConnectionString"], null);
            Assert.IsNotNull(provider);
        }

        [Test, Category("SQLite")]
        public void CanLoad_SQLiteProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("SQLite",
                                                                      ConfigurationManager.AppSettings[
                                                                          "SQLiteConnectionString"], null);
            Assert.IsNotNull(provider);
        }

        [Test, Category("Postgre")]
        public void CanLoad_PostgreSQLProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("PostgreSQL",
                                                                      ConfigurationManager.AppSettings[
                                                                          "NpgsqlConnectionString"], null);
            Assert.IsNotNull(provider);
        }

        [Test, Category("Oracle")]
        public void CanLoad_OracleProvider()
        {
            ITransformationProvider provider = ProviderFactory.Create("Oracle",
                                                                      ConfigurationManager.AppSettings[
                                                                          "OracleConnectionString"], null);
            Assert.IsNotNull(provider);
        }
    }
}
