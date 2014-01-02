﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeOnlyStoredProcedure;
using System.Data;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeOnlyTests
{
    [TestClass]
    public class StoredProcedureExtensionsTests
    {
        #region WithParameter Tests
        [TestMethod]
        public void TestWithParameterAddsParameterToNewStoredProcedure()
        {
            var orig = new StoredProcedure("Test");

            var toTest = orig.WithParameter("Foo", "Bar");

            Assert.IsFalse(ReferenceEquals(orig, toTest));
            Assert.AreEqual(0, orig.Parameters.Count());
            Assert.AreEqual(1, toTest.Parameters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual("Bar", p.Value);
            Assert.AreEqual(ParameterDirection.Input, p.Direction);
        }

        [TestMethod]
        public void TestWithParamaterAndSqlTypeAddsParameterToNewStoredProcedure()
        {
            var orig = new StoredProcedure("Test");

            var toTest = orig.WithParameter("Foo", "Bar", SqlDbType.NChar);

            Assert.IsFalse(ReferenceEquals(orig, toTest));
            Assert.AreEqual(0, orig.Parameters.Count());
            Assert.AreEqual(1, toTest.Parameters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual("Bar", p.Value);
            Assert.AreEqual(ParameterDirection.Input, p.Direction);
            Assert.AreEqual(SqlDbType.NChar, p.SqlDbType);
        }

        [TestMethod]
        public void TestWithParameterClonesStoredProcedureWithResultType()
        {
            var orig = new StoredProcedure<int>("Test");

            var toTest = orig.WithParameter("Foo", "Bar");

            Assert.AreEqual(typeof(StoredProcedure<int>), toTest.GetType());
            Assert.IsFalse(ReferenceEquals(orig, toTest));
            Assert.AreEqual(0, orig.Parameters.Count());
            Assert.AreEqual(1, toTest.Parameters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Input, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual("Bar", p.Value);
        }
        #endregion

        #region WithOutputParameter Tests
        [TestMethod]
        public void TestWithOutputParameterAddsParameterAndSetter()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithOutputParameter<StoredProcedure, string>("Foo", s => set = s);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }

        [TestMethod]
        public void TestWithOutputParameterSetsSqlDbType()
        {
            var sp = new StoredProcedure("Test");

            int set = 0;
            var toTest = sp.WithOutputParameter<StoredProcedure, int>("Foo", s => set = s, SqlDbType.Int);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(42);
            Assert.AreEqual(42, set);
        }

        [TestMethod]
        public void TestWithOutputParameterSetsSize()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithOutputParameter<StoredProcedure, string>("Foo", s => set = s, size: 10);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(10, p.Size);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }

        [TestMethod]
        public void TestWithOutputParameterAndSqlDbTypeSetsSize()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithOutputParameter<StoredProcedure, string>("Foo", s => set = s, SqlDbType.NVarChar, size: 10);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(SqlDbType.NVarChar, p.SqlDbType);
            Assert.AreEqual(10, p.Size);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }

        [TestMethod]
        public void TestWithOutputParameterSetsScale()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithOutputParameter<StoredProcedure, decimal>("Foo", d => set = d, scale: 4);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(4, p.Scale);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(142.13M);
            Assert.AreEqual(142.13M, set);
        }

        [TestMethod]
        public void TestWithOutputParameterAndSqlDbTypeSetsScale()
        {
            var sp = new StoredProcedure("Test");
            
            decimal set = 0;
            var toTest = sp.WithOutputParameter<StoredProcedure, decimal>("Foo", d => set = d, SqlDbType.Decimal, scale: 4);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(4, p.Scale);
            Assert.AreEqual(SqlDbType.Decimal, p.SqlDbType);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(12.37M);
            Assert.AreEqual(12.37M, set);
        }

        [TestMethod]
        public void TestWithOutputParameterSetsPrecision()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithOutputParameter<StoredProcedure, decimal>("Foo", d => set = d, precision: 11);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.Output, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(11, p.Precision);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(142.13M);
            Assert.AreEqual(142.13M, set);
        }

        [TestMethod]
        public void TestWithOutputParameterAndSqlDbTypeSetsPrecision()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithOutputParameter<StoredProcedure, decimal>("Foo", d => set = d, SqlDbType.Decimal, precision: 11);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(11, p.Precision);
            Assert.AreEqual(SqlDbType.Decimal, p.SqlDbType);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(12.37M);
            Assert.AreEqual(12.37M, set);
        }
        #endregion

        #region WithInputOutputParameter Tests
        [TestMethod]
        public void TestWithInputOutputParameterHasInputAndSetsOutput()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithInputOutputParameter("Foo", "Bar", s => set = s);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Bar", p.Value);
            Assert.AreEqual("Foo", p.ParameterName);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }

        [TestMethod]
        public void TestWithInputOutputParameterAndSqlDbTypeHasInputAndSetsOutput()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithInputOutputParameter("Foo", "Bar", s => set = s, SqlDbType.NVarChar);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Bar", p.Value);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(SqlDbType.NVarChar, p.SqlDbType);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }


        [TestMethod]
        public void TestWithInputOutputParameterSetsSize()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithInputOutputParameter("Foo", "Baz", s => set = s, size: 10);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(10, p.Size);
            Assert.AreEqual("Baz", p.Value);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }

        [TestMethod]
        public void TestWithInputOutputParameterAndSqlDbTypeSetsSize()
        {
            var sp = new StoredProcedure("Test");

            string set = null;
            var toTest = sp.WithInputOutputParameter("Foo", "Fab", s => set = s, SqlDbType.NVarChar, size: 10);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(SqlDbType.NVarChar, p.SqlDbType);
            Assert.AreEqual(10, p.Size);
            Assert.AreEqual("Fab", p.Value);

            var output = toTest.OutputParameterSetters.Single();
            output.Value("Bar");
            Assert.AreEqual("Bar", set);
        }

        [TestMethod]
        public void TestWithInputOutputParameterSetsScale()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithInputOutputParameter("Foo", 99M, d => set = d, scale: 4);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(4, p.Scale);
            Assert.AreEqual(99M, p.Value);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(142.13M);
            Assert.AreEqual(142.13M, set);
        }

        [TestMethod]
        public void TestWithInputOutputParameterAndSqlDbTypeSetsScale()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithInputOutputParameter("Foo", 100M, d => set = d, SqlDbType.Decimal, scale: 4);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(4, p.Scale);
            Assert.AreEqual(100M, p.Value);
            Assert.AreEqual(SqlDbType.Decimal, p.SqlDbType);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(12.37M);
            Assert.AreEqual(12.37M, set);
        }

        [TestMethod]
        public void TestWithInputOutputParameterSetsPrecision()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithInputOutputParameter("Foo", 10M, d => set = d, precision: 11);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(11, p.Precision);
            Assert.AreEqual(10M, p.Value);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(142.13M);
            Assert.AreEqual(142.13M, set);
        }

        [TestMethod]
        public void TestWithInputOutputParameterAndSqlDbTypeSetsPrecision()
        {
            var sp = new StoredProcedure("Test");

            decimal set = 0;
            var toTest = sp.WithInputOutputParameter("Foo", 13M, d => set = d, SqlDbType.Decimal, precision: 11);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual("Foo", p.ParameterName);
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);
            Assert.AreEqual(11, p.Precision);
            Assert.AreEqual(13M, p.Value);
            Assert.AreEqual(SqlDbType.Decimal, p.SqlDbType);

            var output = toTest.OutputParameterSetters.Single();
            output.Value(12.37M);
            Assert.AreEqual(12.37M, set);
        }
        #endregion

        #region WithReturnValue Tests
        [TestMethod]
        public void TestWithReturnValueAddsParameterAndOutputSetter()
        {
            var orig = new StoredProcedure("Test");

            int rv = 0;
            var toTest = orig.WithReturnValue(i => rv = i);

            Assert.IsFalse(ReferenceEquals(orig, toTest));
            Assert.AreEqual(0, orig.Parameters.Count());
            Assert.AreEqual(0, orig.OutputParameterSetters.Count());

            var p = toTest.Parameters.Single();
            Assert.AreEqual(ParameterDirection.ReturnValue, p.Direction);

            var act = toTest.OutputParameterSetters.Single();
            act.Value(100);

            Assert.AreEqual(100, rv);
        }
        #endregion

        #region WithInput Tests
        [TestMethod]
        public void TestWithInputParsesAnonymousType()
        {
            var sp = new StoredProcedure("Test");

            var toTest = sp.WithInput(new
            {
                Id = 1,
                Name = "Foo"
            });

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);
            Assert.AreEqual(2, toTest.Parameters.Count());
            Assert.AreEqual(0, toTest.OutputParameterSetters.Count);

            var p = toTest.Parameters.First();
            Assert.AreEqual("Id", p.ParameterName);
            Assert.AreEqual(1, p.Value);
            Assert.AreEqual(ParameterDirection.Input, p.Direction);

            p = toTest.Parameters.Last();
            Assert.AreEqual("Name", p.ParameterName);
            Assert.AreEqual("Foo", p.Value);
            Assert.AreEqual(ParameterDirection.Input, p.Direction);
        }

        [TestMethod]
        public void TestWithInputUsesParameterName()
        {
            var sp = new StoredProcedure("Test");

            var input = new WithNamedParameter { Foo = "Bar" };
            var toTest = sp.WithInput(input);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);
            Assert.AreEqual(1, toTest.Parameters.Count());
            Assert.AreEqual(0, toTest.OutputParameterSetters.Count);

            var p = toTest.Parameters.First();
            Assert.AreEqual("InputName", p.ParameterName);
            Assert.AreEqual("Bar", p.Value);
            Assert.AreEqual(ParameterDirection.Input, p.Direction);
        }

        [TestMethod]
        public void TestWithInputAddsOutputTypes()
        {
            var sp = new StoredProcedure("Test");

            var output = new WithOutput();
            var toTest = sp.WithInput(output);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);

            var p = toTest.Parameters.Single();
            Assert.AreEqual("Value", p.ParameterName);
            Assert.AreEqual(ParameterDirection.Output, p.Direction);

            var setter = toTest.OutputParameterSetters.Single();
            setter.Value("Foo");
            Assert.AreEqual("Foo", output.Value);
        }

        [TestMethod]
        public void TestWithInputAddsInputOutputTypes()
        {
            var sp = new StoredProcedure("Test");

            var inputOutput = new WithInputOutput { Value = 123M };
            var toTest = sp.WithInput(inputOutput);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);

            var p = toTest.Parameters.Single();
            Assert.AreEqual("Value", p.ParameterName);
            Assert.AreEqual(123M, p.Value);
            Assert.AreEqual(ParameterDirection.InputOutput, p.Direction);

            var setter = toTest.OutputParameterSetters.Single();
            setter.Value(99M);
            Assert.AreEqual(99M, inputOutput.Value);
        }

        [TestMethod]
        public void TestWithInputAddsReturnValue()
        {
            var sp = new StoredProcedure("Test");

            var retVal = new WithReturnValue();
            var toTest = sp.WithInput(retVal);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);

            var p = toTest.Parameters.Single();
            Assert.AreEqual("ReturnValue", p.ParameterName);
            Assert.AreEqual(ParameterDirection.ReturnValue, p.Direction);

            var setter = toTest.OutputParameterSetters.Single();
            setter.Value(10);
            Assert.AreEqual(10, retVal.ReturnValue);
        }

        [TestMethod]
        public void TestWithInputSendsTableValuedParameter()
        {
            var sp = new StoredProcedure("Test");

            var input = new WithTableValuedParameter
            {
                Table = new List<TVPHelper>
                {
                    new TVPHelper { Name = "Hello", Foo = 0, Bar = 100M },
                    new TVPHelper { Name = "World", Foo = 3, Bar = 331M }
                }
            };

            var toTest = sp.WithInput(input);

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);

            Assert.AreEqual(0, toTest.OutputParameterSetters.Count);
            var p = toTest.Parameters.Single();
            Assert.AreEqual(SqlDbType.Structured, p.SqlDbType);
            Assert.AreEqual("Table", p.ParameterName);
            Assert.AreEqual("[TEST].[TVP_TEST]", p.TypeName);

            int i = 0;
            foreach (var record in (IEnumerable<SqlDataRecord>)p.Value)
            {
                var item = input.Table.ElementAt(i);
                Assert.AreEqual("Name", record.GetName(0));
                Assert.AreEqual(item.Name, record.GetString(0));

                Assert.AreEqual("Foo", record.GetName(1));
                Assert.AreEqual(item.Foo, record.GetInt32(1));

                Assert.AreEqual("Bar", record.GetName(2));
                Assert.AreEqual(item.Bar, record.GetDecimal(2));

                ++i;
            }
        }
        #endregion

        #region WithTableValuedParameter Tests
        [TestMethod]
        public void TestWithTableValuedParameterAddsParameter()
        {
            var sp = new StoredProcedure("Test");

            var tvp = new[]
            {
                new TVPHelper { Name = "Hello", Foo = 0, Bar = 100M },
                new TVPHelper { Name = "World", Foo = 3, Bar = 331M }
            };

            var toTest = sp.WithTableValuedParameter("Bar", tvp, "TVP");

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);

            Assert.AreEqual(0, toTest.OutputParameterSetters.Count);
            var p = toTest.Parameters.Single();
            Assert.AreEqual(SqlDbType.Structured, p.SqlDbType);
            Assert.AreEqual("Bar", p.ParameterName);
            Assert.AreEqual("[dbo].[TVP]", p.TypeName);

            int i = 0;
            foreach (var record in (IEnumerable<SqlDataRecord>)p.Value)
            {
                Assert.AreEqual("Name", record.GetName(0));
                Assert.AreEqual(tvp[i].Name, record.GetString(0));

                Assert.AreEqual("Foo", record.GetName(1));
                Assert.AreEqual(tvp[i].Foo, record.GetInt32(1));

                Assert.AreEqual("Bar", record.GetName(2));
                Assert.AreEqual(tvp[i].Bar, record.GetDecimal(2));

                ++i;
            }
        }

        [TestMethod]
        public void TestWithTableValuedParameterWithSchemaAddsParameter()
        {
            var sp = new StoredProcedure("Test");

            var tvp = new[]
            {
                new TVPHelper { Name = "Hello", Foo = 0, Bar = 100M },
                new TVPHelper { Name = "World", Foo = 3, Bar = 331M }
            };

            var toTest = sp.WithTableValuedParameter("Bar", tvp, "TVP", "Table Type");

            Assert.IsFalse(ReferenceEquals(sp, toTest));
            Assert.AreEqual(0, sp.Parameters.Count());
            Assert.AreEqual(0, sp.OutputParameterSetters.Count);

            Assert.AreEqual(0, toTest.OutputParameterSetters.Count);
            var p = toTest.Parameters.Single();
            Assert.AreEqual(SqlDbType.Structured, p.SqlDbType);
            Assert.AreEqual("Bar", p.ParameterName);
            Assert.AreEqual("[TVP].[Table Type]", p.TypeName);

            int i = 0;
            foreach (var record in (IEnumerable<SqlDataRecord>)p.Value)
            {
                Assert.AreEqual("Name", record.GetName(0));
                Assert.AreEqual(tvp[i].Name, record.GetString(0));

                Assert.AreEqual("Foo", record.GetName(1));
                Assert.AreEqual(tvp[i].Foo, record.GetInt32(1));

                Assert.AreEqual("Bar", record.GetName(2));
                Assert.AreEqual(tvp[i].Bar, record.GetDecimal(2));

                ++i;
            }
        }
        #endregion

        #region CreateDataReader Tests
        [TestMethod]
        public void TestCreateDataReaderReturnsReader()
        {
            var reader  = new Mock<IDataReader>().Object;
            var command = new Mock<IDbCommand>();

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader);

            var toTest = command.Object.CreateDataReader(CancellationToken.None);

            Assert.AreEqual(toTest, reader);
        }

        [TestMethod]
        public void TestCreateataReaderCancelsWhenCanceledBeforeExecuting()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var command = new Mock<IDbCommand>();
            command.Setup(d => d.ExecuteReader())
                   .Throws(new Exception("ExecuteReader called after token was canceled"));

            bool exceptionThrown = false;
            try
            {
                command.Object.CreateDataReader(cts.Token);
            }
            catch(OperationCanceledException)
            {
                exceptionThrown = true;
            }
            
            command.Verify(d => d.ExecuteReader(), Times.Never);
            Assert.IsTrue(exceptionThrown, "No TaskCanceledException thrown when token is cancelled");
        }

        [TestMethod]
        public void TestCreateDataReaderCancelsCommandWhenTokenCanceled()
        {
            var sema = new SemaphoreSlim(0, 1);
            var command = new Mock<IDbCommand>();
            command.Setup     (d => d.ExecuteReader())
                   .Callback  (() =>
                               {
                                   sema.Release();
                                   Thread.Sleep(100);
                               })
                   .Returns   (() => null);
            command.Setup     (d => d.Cancel())
                   .Verifiable();

            var cts = new CancellationTokenSource();

            var toTest = Task.Factory.StartNew(() => command.Object.CreateDataReader(cts.Token));
            bool exceptionThrown = false;

            var continuation = 
                toTest.ContinueWith(t => exceptionThrown = t.Exception.InnerException is OperationCanceledException,
                                    TaskContinuationOptions.OnlyOnFaulted);

            sema.Wait();
            cts.Cancel();

            continuation.Wait();
            command.Verify(d => d.Cancel(), Times.Once);
            Assert.IsTrue(exceptionThrown, "No TaskCanceledException thrown when token is cancelled");
        }

        [TestMethod]
        public void TestCreateDataReaderThrowsWhenExecuteReaderThrows()
        {
            var command = new Mock<IDbCommand>();
            command.Setup (d => d.ExecuteReader())
                   .Throws(new Exception("Test Exception"));

            Exception ex = null;
            try
            {
                var toTest = command.Object.CreateDataReader(CancellationToken.None);
            }
            catch(Exception e)
            {
                ex = e;
            }

            Assert.IsNotNull(ex);
            Assert.AreEqual("Test Exception", ex.Message);
        }
        #endregion

        #region Execute Tests
        [TestMethod]
        public void TestExecuteCancelsWhenTokenCanceledBeforeExecuting()
        {
            var reader  = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            command.Setup(c => c.ExecuteReader())
                   .Returns(reader.Object);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            bool exceptionThrown = false;
            try
            {
                command.Object.Execute(cts.Token, Enumerable.Empty<Type>());
            }
            catch (OperationCanceledException)
            {
                exceptionThrown = true;
            }

            reader.Verify(d => d.Read(), Times.Never);
            Assert.IsTrue(exceptionThrown, "No TaskCanceledException thrown when token is cancelled");
        }

        [TestMethod]
        public void TestExecuteCancelsWhenTokenCanceled()
        {
            var sema    = new SemaphoreSlim(0, 1);
            var reader  = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            reader.Setup(d => d.Read())
                  .Callback(() =>
                  {
                      sema.Release();
                      Thread.Sleep(100);
                  })
                  .Returns(true);

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader.Object);

            var cts = new CancellationTokenSource();

            var toTest = Task.Factory.StartNew(() => command.Object.Execute(cts.Token, new[] { typeof(SingleResultSet) }));
            bool exceptionThrown = false;

            var continuation =
                toTest.ContinueWith(t => exceptionThrown = t.Exception.InnerException is OperationCanceledException,
                                    TaskContinuationOptions.OnlyOnFaulted);

            sema.Wait();
            cts.Cancel();

            continuation.Wait();
            Assert.IsTrue(exceptionThrown, "No TaskCanceledException thrown when token is cancelled");
        }

        [TestMethod]
        public void TestExecuteReturnsSingleResultSetOneRow()
        {
            var values = new Dictionary<string, object>
            {
                { "String",  "Hello, World!"           },
                { "Double",  42.0                      },
                { "Decimal", 100M                      },
                { "Int",     99                        },
                { "Long",    1028130L                  },
                { "Date",    new DateTime(1982, 1, 31) }
            };

            var keys = values.Keys.OrderBy(s => s).ToArray();
            var vals = values.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToArray();

            var reader  = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader.Object);

            reader.SetupGet(r => r.FieldCount)
                  .Returns(6);

            var first = true;
            reader.Setup(r => r.Read())
                  .Returns(() =>
                  {
                      if(first)
                      {
                          first = false;
                          return true;
                      }

                      return false;
                  });

            reader.Setup(r => r.GetName(It.IsAny<int>()))
                  .Returns((int i) => keys[i]);
            reader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                  .Callback((object[] arr) => vals.CopyTo(arr, 0))
                  .Returns(6);

            var results = command.Object.Execute(CancellationToken.None, new[] { typeof(SingleResultSet) });

            var toTest = (IList<SingleResultSet>)results[typeof(SingleResultSet)];

            Assert.AreEqual(1, toTest.Count);
            var item = toTest[0];

            Assert.AreEqual("Hello, World!",           item.String);
            Assert.AreEqual(42.0,                      item.Double);
            Assert.AreEqual(100M,                      item.Decimal);
            Assert.AreEqual(99,                        item.Int);
            Assert.AreEqual(1028130L,                  item.Long);
            Assert.AreEqual(new DateTime(1982, 1, 31), item.Date);
        }

        [TestMethod]
        public void TestExecuteHandlesRenamedColumns()
        {
            var reader = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader.Object);

            reader.SetupGet(r => r.FieldCount)
                  .Returns(1);

            var first = true;
            reader.Setup(r => r.Read())
                  .Returns(() =>
                  {
                      if (first)
                      {
                          first = false;
                          return true;
                      }

                      return false;
                  });

            reader.Setup(r => r.GetName(0))
                  .Returns("MyRenamedColumn");
            reader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                  .Callback((object[] arr) => arr[0] = "Hello, World!")
                  .Returns(1);

            var results = command.Object.Execute(CancellationToken.None, new[] { typeof(RenamedColumn) });

            var toTest = (IList<RenamedColumn>)results[typeof(RenamedColumn)];

            Assert.AreEqual(1, toTest.Count);
            var item = toTest[0];

            Assert.AreEqual("Hello, World!", item.Column);
        }

        [TestMethod]
        public void TestExecuteReturnsMultipleRowsInOneResultSet()
        {
            var reader = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader.Object);

            reader.SetupGet(r => r.FieldCount)
                  .Returns(1);

            var results = new[] { "Hello", ", ", "World!" };

            int index = -1;
            reader.Setup(r => r.Read())
                  .Callback(() => ++index)
                  .Returns(() => index < results.Length);

            reader.Setup(r => r.GetName(0))
                  .Returns("Column");
            reader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                  .Callback((object[] arr) => arr[0] = results[index])
                  .Returns(1);

            var res = command.Object.Execute(CancellationToken.None, new[] { typeof(SingleColumn) });

            var toTest = (IList<SingleColumn>)res[typeof(SingleColumn)];

            Assert.AreEqual(3, toTest.Count);

            for (int i = 0; i < results.Length; i++)
            {
                var item = toTest[i];

                Assert.AreEqual(results[i], item.Column);
            }
        }

        [TestMethod]
        public void TestExecuteConvertsDbNullToNullValues()
        {
            var keys = new[] { "Name", "NullableInt", "NullableDouble" };
            var reader = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader.Object);

            reader.SetupGet(r => r.FieldCount)
                  .Returns(3);

            var first = true;
            reader.Setup(r => r.Read())
                  .Returns(() =>
                  {
                      if (first)
                      {
                          first = false;
                          return true;
                      }

                      return false;
                  });

            reader.Setup(r => r.GetName(It.IsAny<int>()))
                  .Returns((int i) => keys[i]);
            reader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                  .Callback((object[] arr) => arr[0] = arr[1] = arr[2] = DBNull.Value)
                  .Returns(3);

            var results = command.Object.Execute(CancellationToken.None, new[] { typeof(NullableColumns) });

            var toTest = (IList<NullableColumns>)results[typeof(NullableColumns)];

            Assert.AreEqual(1, toTest.Count);
            var item = toTest[0];

            Assert.IsNull(item.Name);
            Assert.IsNull(item.NullableDouble);
            Assert.IsNull(item.NullableInt);
        }

        [TestMethod]
        [ExpectedException(typeof(StoredProcedureResultsException))]
        public void TestExecuteThrowsIfMappedColumnDoesNotExistInResultSet()
        {
            var reader = new Mock<IDataReader>();
            var command = new Mock<IDbCommand>();

            command.Setup(d => d.ExecuteReader())
                   .Returns(reader.Object);

            reader.SetupGet(r => r.FieldCount)
                  .Returns(1);

            var first = true;
            reader.Setup(r => r.Read())
                  .Returns(() =>
                  {
                      if (first)
                      {
                          first = false;
                          return true;
                      }

                      return false;
                  });

            reader.Setup(r => r.GetName(It.IsAny<int>()))
                  .Returns("OtherColumnName");
            reader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                  .Callback((object[] arr) => arr[0] = DBNull.Value)
                  .Returns(1);

            command.Object.Execute(CancellationToken.None, new[] { typeof(SingleColumn) });
        }
        #endregion

        #region Dummy Data Classes
        private class WithNamedParameter
        {
            [StoredProcedureParameter(Name = "InputName")]
            public string Foo { get; set; }
        }

        private class WithOutput
        {
            [StoredProcedureParameter(Direction = ParameterDirection.Output)]
            public string Value { get; set; }
        }

        private class WithInputOutput
        {
            [StoredProcedureParameter(Direction = ParameterDirection.InputOutput)]
            public decimal Value { get; set; }
        }

        private class WithReturnValue
        {
            [StoredProcedureParameter(Direction = ParameterDirection.ReturnValue)]
            public int ReturnValue { get; set; }
        }

        private class WithTableValuedParameter
        {
            [TableValuedParameter(Schema = "TEST", TableName = "TVP_TEST")]
            public IEnumerable<TVPHelper> Table { get; set; }
        }

        private class TVPHelper
        {
            public string  Name { get; set; }
            public int     Foo  { get; set; }
            public decimal Bar  { get; set; }
        }

        private class SingleResultSet
        {
            public String   String  { get; set; }
            public Double   Double  { get; set; }
            public Decimal  Decimal { get; set; }
            public Int32    Int     { get; set; }
            public Int64    Long    { get; set; }
            public DateTime Date    { get; set; }
        }

        private class SingleColumn
        {
            public string Column { get; set; }
        }

        private class RenamedColumn
        {
            [Column("MyRenamedColumn")]
            public string Column { get; set; }
        }

        private class NullableColumns
        {
            public string  Name           { get; set; }
            public int?    NullableInt    { get; set; }
            public double? NullableDouble { get; set; }
        }
        #endregion
    }
}