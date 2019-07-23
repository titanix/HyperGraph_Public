using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data.SQLite;

using Leger;
using Leger.IO;
using Leger.Extra.SqlBinding;

namespace HyperGraph.Extras.VsTests
{
    [TestClass]
    public class SqliteBindingTests
    {
        private static string connectionStringTemplate = "Data Source={0};Version=3;";
        private static string absoluteIdsFilePath = @"C:\Users\Louis\Documents\Visual Studio 2017\Projects\HyperGraph\HyperGraphData\Data_Xml_v3\ids.xml";
        private static string absoluteN1FilePath = @"C:\Users\Louis\Documents\Visual Studio 2017\Projects\HyperGraph\HyperGraphData\Data_Xml_v3\n1.xml";

        [TestMethod]
        public void CreateSqliteFile()
        {
            string dbPath = Path.GetRandomFileName();

            SQLiteConnection.CreateFile(dbPath);

            SQLiteConnection connection = new SQLiteConnection(String.Format(connectionStringTemplate, dbPath));
            connection.Open();
        }

        private SQLiteConnection GetSqliteConnection()
        {
            string dbPath = Path.GetRandomFileName();
            SQLiteConnection.CreateFile(dbPath);
            SQLiteConnection connection = new SQLiteConnection(String.Format(connectionStringTemplate, dbPath));
            return connection;
        }

        //[DeploymentItem(@"..\..\..\HyperGraphData\Data_Xml_v3\ids.xml")]
        //[DeploymentItem(@"x86\SQLite.Interop.dll", "x86")]
        //[DeploymentItem(@"x64\SQLite.Interop.dll", "x64")]
        // using a deployment item make the dll loading fails
        // https://stackoverflow.com/a/24411049
        [TestMethod]
        public void CreateSqliteCreateOrientedBinaryGraphDatabase()
        {
            Graph idsGraph = GraphXmlDeserializer.GetGraphInstance(absoluteIdsFilePath, new FileLoader(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(idsGraph);

            SQLiteConnection connection = GetSqliteConnection();
            Assert.IsNotNull(connection);

            OrientedBinaryGraphToDatabaseConverter<SQLiteCommand> converter = new OrientedBinaryGraphToDatabaseConverter<SQLiteCommand>(connection);
            Assert.IsNotNull(converter);

            converter.CreateDatabase();
            converter.PopulateDatabase(idsGraph);
        }

        [TestMethod]
        public void QueryGraph()
        {
            Graph idsGraph = GraphXmlDeserializer.GetGraphInstance(absoluteIdsFilePath, new FileLoader(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(idsGraph);

            SQLiteConnection connection = GetSqliteConnection();
            Assert.IsNotNull(connection);

            OrientedBinaryGraphToDatabaseConverter<SQLiteCommand> converter = new OrientedBinaryGraphToDatabaseConverter<SQLiteCommand>(connection);
            Assert.IsNotNull(converter);
            converter.CreateDatabase();
            converter.PopulateDatabase(idsGraph);

            connection.Open();
            IGraph sqlGraph = new SqlGraph<SQLiteCommand>(new DbProvider<SQLiteCommand>(connection, 
                new SqliteVertexStoreRawSqlProvider(), new SqliteEdgeStoreRawSqlProvider()));
            Assert.IsNotNull(sqlGraph);

            IEnumerable<IVertex> result = sqlGraph.SearchNode("", "乃");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            IVertex v = result.First();
            Assert.AreEqual(103, v.GetLinkedObjects().Count);
        }

        [TestMethod]
        public void CreateSqliteCreateHyperGraphDatabase()
        {
            Graph n1Graph = GraphXmlDeserializer.GetGraphInstance(absoluteN1FilePath, new FileLoader(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(n1Graph);

            SQLiteConnection connection = GetSqliteConnection();
            Assert.IsNotNull(connection);

            GeneralHyperGraphDatabaseConverter<SQLiteCommand> converter = new GeneralHyperGraphDatabaseConverter<SQLiteCommand>(connection);
            Assert.IsNotNull(converter);

            converter.CreateDatabase();
        }

        [TestMethod]
        public void CreateSqlitePopulateHyperGraphDatabase()
        {
            Graph n1Graph = GraphXmlDeserializer.GetGraphInstance(absoluteN1FilePath, new FileLoader(), ExternalRessourcesLoadingPolicy.None);
            Assert.IsNotNull(n1Graph);

            SQLiteConnection connection = GetSqliteConnection();
            Assert.IsNotNull(connection);

            GeneralHyperGraphDatabaseConverter<SQLiteCommand> converter = new GeneralHyperGraphDatabaseConverter<SQLiteCommand>(connection);
            Assert.IsNotNull(converter);

            converter.CreateDatabase();
            converter.PopulateDatabase(n1Graph);
        }
    }
}