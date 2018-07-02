using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Common.Common
{
    public class MultiResultSetReader : IDisposable
    {
        private readonly DbContext _context;
        private readonly DbCommand _command;
        private bool _connectionNeedsToBeClosed;
        private DbDataReader _reader;

        public MultiResultSetReader(DbContext context, string query, SqlParameter[] parameters)
        {
            _context = context;
            _command = _context.Database.Connection.CreateCommand();
            _command.CommandText = query;

            if (parameters != null && parameters.Any()) _command.Parameters.AddRange(parameters);
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            if (_connectionNeedsToBeClosed)
            {
                _context.Database.Connection.Close();
                _connectionNeedsToBeClosed = false;
            }
        }

        public ObjectResult<T> ResultSetFor<T>()
        {
            if (_reader == null)
                _reader = GetReader();
            else
                _reader.NextResult();

            var objContext = ((IObjectContextAdapter)_context).ObjectContext;

            return objContext.Translate<T>(_reader);
        }

        public T SingleResultSetFor<T>()
        {
            if (_reader == null)
                _reader = GetReader();
            else
                _reader.NextResult();

            var objContext = ((IObjectContextAdapter)_context).ObjectContext;

            return objContext.Translate<T>(_reader).FirstOrDefault();
        }

        public async Task<T> SingleResultSetAsync<T>()
        {
            if (_reader == null)
                _reader = await GetReaderAsync();
            else
                await _reader.NextResultAsync();

            var objContext = ((IObjectContextAdapter)_context).ObjectContext;

            return objContext.Translate<T>(_reader).FirstOrDefault();
        }

        public DbDataReader GetReader()
        {
            if (_context.Database.Connection.State != ConnectionState.Open)
            {
                _context.Database.Connection.Open();
                _connectionNeedsToBeClosed = true;
            }

            return _command.ExecuteReader();
        }

        public async Task<List<T>> GetResultWithJsonAsync<T>(string jsonEntity)
        {
            if (_reader == null)
                _reader = await GetReaderAsync();
            else
                await _reader.NextResultAsync();

            return GetJsonResponse<T>(_reader, jsonEntity);
        }

        public List<T> GetResultWithJson<T>(string jsonEntity)
        {
            if (_reader == null)
                _reader = GetReader();
            else
                _reader.NextResult();

            return GetJsonResponse<T>(_reader, jsonEntity);
        }

        public T GetSingleResultWithJson<T>(string jsonEntity)
        {
            if (_reader == null)
                _reader = GetReader();
            else
                _reader.NextResult();

            var result = GetJsonResponse<T>(_reader, jsonEntity);

            if (result != null && result.Any())
                return result.FirstOrDefault();

            return Enumerable.Empty<T>().FirstOrDefault();
        }

        public async Task<string> GetJsonStringResult()
        {
            if (_reader == null)
                _reader = await GetReaderAsync();
            else
                await _reader.NextResultAsync();

            var jsonResult = new StringBuilder();

            //If reader has rows, then get the value of each row and add it in to the json builder object
            while (_reader.Read())
            {
                //Append value row in string builder object
                jsonResult.Append(_reader.GetValue(0).ToString());
            }
            //var jsonData = jsonResult.ToString().Replace(@"\", "");
            return jsonResult.ToString();
        }

        public ObjectResult<T> ResultSetFor<T>(DbDataReader reader)
        {
            var objContext = ((IObjectContextAdapter)_context).ObjectContext;
            return objContext.Translate<T>(reader);
        }

        public async Task<ObjectResult<T>> ResultSetForAsync<T>()
        {
            if (_reader == null)
                _reader = await GetReaderAsync();
            else
            {
                await _reader.NextResultAsync();
            }

            var objContext = ((IObjectContextAdapter)_context).ObjectContext;
            return objContext.Translate<T>(_reader);
        }

        public async Task<DbDataReader> GetReaderAsync()
        {
            if (_context.Database.Connection.State != ConnectionState.Open)
            {
                _context.Database.Connection.Open();
                _connectionNeedsToBeClosed = true;
            }

            return await _command.ExecuteReaderAsync();
        }

        private static List<T> GetJsonResponse<T>(DbDataReader reader, string parserString)
        {
            //Check reader has some rowss
            try
            {
                if (reader.HasRows)
                {
                    var jsonResult = new StringBuilder();

                    //If reader has rows, then get the value of each row and add it in to the json builder object
                    while (reader.Read())
                        jsonResult.Append(reader.GetValue(0).ToString());


                    //Create object of JObject class and parse the json result
                    JObject jsonResponse = JObject.Parse(jsonResult.ToString());

                    var objResponse = jsonResponse[parserString];
                    if (objResponse != null)
                        return JsonConvert.DeserializeObject<List<T>>(Convert.ToString(objResponse));

                }
                return Enumerable.Empty<T>().ToList();
            }
            catch (Exception ex)
            {
                var ss = ex.Message;
                return Enumerable.Empty<T>().ToList();
            }
        }
    }
}
