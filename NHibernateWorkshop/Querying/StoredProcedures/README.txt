These tests won't run by default. Here's what you need to do to run these tests:

- select the HBMSQLSERVER build configuration and run the test-suite once
	(this makes sure you have a database with the correct schema)
- In SqlServerHbmSessionFactoryBuilder.cs, comment out the following 2 lines:
            var schemaExport = new SchemaExport(configuration);
            schemaExport.Create(true, true);
- create the stored procedures listed in this folder manually in your NHibernateWorkshop database on your sql server instance
- explicitly run the tests in the CallingStoredProcedures fixture
