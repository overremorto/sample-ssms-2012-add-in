using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlParser;

namespace SSMSAddin.Test
{
    [TestClass]
    public class SimpleSqlParserTest
    {
        [TestMethod]
        public void GetTableFromSql()
        {
            var sql = @"SELECT TOP 1000 [RowID]
      ,[SystemName]
      ,[SourceServerName]
      ,[SourceDBName]
      ,[SourceObjectName]
      ,[SourceSchemaName]
      ,[TargetServerName]
      ,[TargetDBName]
      ,[TargetObjectName]
      ,[TargetSchemaName]
      ,[DependencyFlg]
      ,[Sequence]
      ,[InactiveFlg]
      ,[SelectFieldNames]
      ,[InsertFieldNames]
      ,[OpenQueryFlg]
      ,[PrimaryKeyColumns]
      ,[PrimaryKeyDataTypes]
      ,[SchemaPrefixFlg]
      ,[IndexColumns1]
      ,[IndexColumns2]
      ,[PrimaryKeyFlg]
      ,[InsertIntoFlg]
      ,[TableHistoryFlg]
      ,[IdentityInsertFlg]
      ,[CreatedDate]
      ,[ModifiedDate]
      ,[ModifiedBy]
      ,[SourceKeyAttribute]
      ,[TargetKeyAttribute]
      ,[WhereClause]
      ,[DeleteFromDestUsingWhereFlg]
      ,[UseSSISBuilderFlg]
      ,[SSISOptionalODBCConnectionString]
      ,[RESTURL]
      ,[RESTURLAdditional]
      ,[RESTFileType]
      ,[RESTUser]
      ,[RESTPassword]
      ,[CreateClusteredColumnStoreIndexFlg]
      ,[ETLType]
      ,[FileDirectory]
      ,[FileExtension]
      ,[FormatFileDirectory]
      ,[FileLikeStatement]
      ,[FileFirstRow]
      ,[RebuildTableIndexes]
      ,[MergeSkipDeletesFlag]
      ,[FinalSchema]
      ,[SourceCredentialID]
      ,[SourceIsAzure]
      ,[TargetCredentialID]
      ,[TargetIsAzure]
  FROM [ETLGen].[dbo].[tuETLObjectList]";


            var ssp = new SimpleSqlParser();
            var table = ssp.GetTableFromSql(sql, null);
            Assert.AreEqual("[ETLGen].[dbo].[tuETLObjectList]".ToLower(), table);
        }
    }
}
