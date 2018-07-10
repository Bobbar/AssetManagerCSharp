using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Database.Data
{
    public class DBParameter
    {
        public string FieldName { get; set; }
        public object Value { get; set; }
        public WildCardType WildCardType { get; set; }

        public DBParameter(string fieldName, object fieldValue)
        {
            this.FieldName = fieldName;
            this.Value = fieldValue;
        }
    }

    public class DBQueryParameter : DBParameter
    {
        public bool IsExact { get; set; }
        public string OperatorString { get; set; }

        public DBQueryParameter(string fieldName, object fieldValue, string operatorString) : base(fieldName, fieldValue)
        {
            this.IsExact = IsExact;
            this.WildCardType = WildCardType.Both;
            this.OperatorString = operatorString;
        }

        public DBQueryParameter(string fieldName, object fieldValue, bool isExact) : base(fieldName, fieldValue)
        {
            this.IsExact = isExact;
            this.WildCardType = WildCardType.Both;
            this.OperatorString = "AND";
        }

        public DBQueryParameter(string fieldName, object fieldValue, bool isExact, string operatorString) : base(fieldName, fieldValue)
        {
            this.IsExact = isExact;
            this.WildCardType = WildCardType.Both;
            this.OperatorString = operatorString;
        }

        public DBQueryParameter(string fieldName, object fieldValue, WildCardType wildCardType, string operatorString) : base(fieldName, fieldValue)
        {
            this.IsExact = false;
            this.WildCardType = wildCardType;
            this.OperatorString = operatorString;
        }

        /// <summary>
        /// Parses and adds the specified <see cref="DBQueryParameter"/> collection to the specified <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="cmd"><see cref="DbCommand>"/> the parameters will be added to.</param>
        /// <param name="query">The partial query string that will be concatenated with the field names from the <see cref="DBQueryParameter"/> collection.</param>
        /// <param name="parameters">Collection of <see cref="DBQueryParameter"/> to be added.</param>
        public static void AddParamsToCommand(DbCommand cmd, string query, List<DBQueryParameter> parameters)
        {
            string paramQuery = "";
            foreach (var param in parameters)
            {
                if (param.Value is bool)
                {
                    paramQuery += " " + param.FieldName + "=@" + param.FieldName;
                    cmd.AddParameterWithValue("@" + param.FieldName, Convert.ToInt32(param.Value));
                }
                else
                {
                    if (param.IsExact)
                    {
                        paramQuery += " " + param.FieldName + "=@" + param.FieldName;
                        cmd.AddParameterWithValue("@" + param.FieldName, param.Value);
                    }
                    else
                    {
                        switch (param.WildCardType)
                        {
                            case WildCardType.StartsWith:
                                paramQuery += " " + param.FieldName + " LIKE @" + param.FieldName;
                                cmd.AddParameterWithValue("@" + param.FieldName, param.Value.ToString() + "%");
                                break;

                            case WildCardType.EndsWith:
                                paramQuery += " " + param.FieldName + " LIKE @" + param.FieldName;
                                cmd.AddParameterWithValue("@" + param.FieldName, "%" + param.Value.ToString());
                                break;

                            case WildCardType.Both:
                                paramQuery += " " + param.FieldName + " LIKE @" + param.FieldName;
                                cmd.AddParameterWithValue("@" + param.FieldName, "%" + param.Value.ToString() + "%");
                                break;

                            case WildCardType.None:
                                paramQuery += " " + param.FieldName + " LIKE @" + param.FieldName;
                                cmd.AddParameterWithValue("@" + param.FieldName, param.Value.ToString());
                                break;
                        }
                    }
                }

                // Add operator if we are not on the last entry.
                if (parameters.IndexOf(param) < parameters.Count - 1)
                {
                    paramQuery += " " + param.OperatorString;
                }
            }
            cmd.CommandText = query + paramQuery;
        }
    }

    public static class DbCommandExtensions
    {
        /// <summary>
        /// Adds a parameter to the command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameterName">
        /// Name of the parameter.
        /// </param>
        /// <param name="parameterValue">
        /// The parameter value.
        /// </param>
        /// <remarks>
        /// </remarks>
        public static void AddParameterWithValue(this DbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }
    }

    public enum WildCardType
    {
        StartsWith,
        EndsWith,
        None,
        Both // Default
    }
}