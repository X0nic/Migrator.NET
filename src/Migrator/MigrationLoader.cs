using System;
using System.Collections.Generic;
using System.Reflection;
using Migrator.Framework;

namespace Migrator
{
    /// <summary>
    /// Handles inspecting code to find all of the Migrations in assemblies and reading
    /// other metadata such as the last revision, etc.
    /// </summary>
    public class MigrationLoader
    {
        private readonly ITransformationProvider _provider;
        private readonly List<Type> _migrationsTypes = new List<Type>();

        public MigrationLoader(ITransformationProvider provider, Assembly migrationAssembly, bool trace, string schemaName)
        {
            _provider = provider;
            AddMigrations(migrationAssembly, schemaName);

            if (trace)
            {
                provider.Logger.Trace("Loaded migrations:");
                foreach (Type t in _migrationsTypes)
                {
                    provider.Logger.Trace("{0} {1}", GetMigrationVersion(t).ToString().PadLeft(5), StringUtils.ToHumanName(t.Name));
                }
            }
        }

        public void AddMigrations(Assembly migrationAssembly, string schemaName)
        {
            if (migrationAssembly != null)
                _migrationsTypes.AddRange(GetMigrationTypes(migrationAssembly, schemaName));
        }

        /// <summary>
        /// Returns registered migration <see cref="System.Type">types</see>.
        /// </summary>
        public List<Type> MigrationsTypes
        {
            get { return _migrationsTypes; }
        }

        /// <summary>
        /// Returns the last version of the migrations.
        /// </summary>
        public long LastVersion
        {
            get
            {
                if (_migrationsTypes.Count == 0)
                    return 0;
                return GetMigrationVersion(_migrationsTypes[_migrationsTypes.Count - 1]);
            }
        }

        /// <summary>
        /// Check for duplicated version in migrations.
        /// </summary>
        /// <exception cref="CheckForDuplicatedVersion">CheckForDuplicatedVersion</exception>
        public void CheckForDuplicatedVersion()
        {
            var versions = new List<long>();
            foreach (Type t in _migrationsTypes)
            {
                long version = GetMigrationVersion(t);

                if (versions.Contains(version))
                    throw new DuplicatedVersionException(version);

                versions.Add(version);
            }
        }

        /// <summary>
        /// Collect migrations in one <c>Assembly</c>.
        /// </summary>
        /// <param name="asm">The <c>Assembly</c> to browse.</param>
        /// <param name="schemaName"> </param>
        /// <returns>The migrations collection</returns>
        public List<Type> GetMigrationTypes(Assembly asm, string schemaName)
        {
            var migrations = new List<Type>();
            foreach (Type t in asm.GetExportedTypes())
            {
                var attrib = (MigrationAttribute)  Attribute.GetCustomAttribute(t, typeof (MigrationAttribute));

                if (attrib != null && typeof(IMigration).IsAssignableFrom(t))
                {
                    if (!attrib.Ignore && Lower(attrib.Schema) == Lower(schemaName))
                        migrations.Add(t);
                    else
                    {
                        if (attrib.Ignore)
                            _provider.Logger.Trace("Ignoring migration: {0} {1}", GetMigrationVersion(t).ToString().PadLeft(5), StringUtils.ToHumanName(t.Name));
                        else if (Lower(attrib.Schema) != Lower(schemaName))
                            _provider.Logger.Trace("Migration schema not {0}: ({1}){2} {3}", GetSchemaName(schemaName), GetSchemaName(attrib.Schema), GetMigrationVersion(t).ToString().PadLeft(5), StringUtils.ToHumanName(t.Name));
                    }
                }
            }

            migrations.Sort(new MigrationTypeComparer(true));
            return migrations;
        }

        private string Lower(string stringToLower)
        {
            if (stringToLower == null) return null;
            return stringToLower.ToLower();
        }

        private static string GetSchemaName(string schemaName)
        {
            if (schemaName != null)
                return schemaName;
            return "-Blank-";
        }

        /// <summary>
        /// Returns the version of the migration
        /// <see cref="MigrationAttribute">MigrationAttribute</see>.
        /// </summary>
        /// <param name="t">Migration type.</param>
        /// <returns>Version number sepcified in the attribute</returns>
        public static long GetMigrationVersion(Type t)
        {
            var attrib = GetMigrationAttribute(t);

            return attrib.Version;
        }

        public static MigrationAttribute GetMigrationAttribute(Type t)
        {
            return (MigrationAttribute) Attribute.GetCustomAttribute(t, typeof(MigrationAttribute));
        }

        public IList<long> GetAvailableMigrations()
        {
        	//List<int> availableMigrations = new List<int>();
            _migrationsTypes.Sort(new MigrationTypeComparer(true));
            return _migrationsTypes.ConvertAll(GetMigrationVersion);
        }
        
        public IMigration GetMigration(long version)
        {
            foreach (Type t in _migrationsTypes)
            {
                if (GetMigrationVersion(t) == version)
                {
                    var migration = (IMigration)Activator.CreateInstance(t);
                    migration.Database = _provider;
                    return migration;
                }
            }

            return null;
        }
    }
}
