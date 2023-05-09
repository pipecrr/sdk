using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Siesa.SDK.Frontend.Report.Controllers
{
    internal class SDKReportParameterCollection : DbParameterCollection
    {
        public List<SDKReportParameter> _parameters = new List<SDKReportParameter>();
        public override int Count => _parameters.Count;
        public override object SyncRoot => _parameters;

        public SDKReportParameterCollection()
        {
            
        }

        public override int Add(object value)
        {
            _parameters.Add((SDKReportParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var item in values)
            {
                _parameters.Add((SDKReportParameter)item);
            }
        }

        public override void Clear()
        {
            _parameters.Clear();
        }

        public override bool Contains(object value)
        {
            return _parameters.Contains((SDKReportParameter)value);
        }

        public override bool Contains(string value)
        {
            return _parameters.Any(x => x.ParameterName == value);
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public override int IndexOf(object value)
        {
            return _parameters.IndexOf((SDKReportParameter)value);
        }

        public override int IndexOf(string parameterName)
        {
            return _parameters.FindIndex(x => x.ParameterName == parameterName);
        }

        public override void Insert(int index, object value)
        {
            _parameters.Insert(index, (SDKReportParameter)value);
        }

        public override void Remove(object value)
        {
            _parameters.Remove((SDKReportParameter)value);
        }

        public override void RemoveAt(int index)
        {
            _parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            _parameters.RemoveAt(_parameters.FindIndex(x => x.ParameterName == parameterName));
        }

        protected override DbParameter GetParameter(int index)
        {
            return _parameters[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return  _parameters.FirstOrDefault(x => x.ParameterName == parameterName);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            _parameters[index] = (SDKReportParameter)value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            _parameters[_parameters.FindIndex(x => x.ParameterName == parameterName)] = (SDKReportParameter)value;
        }
    }
}