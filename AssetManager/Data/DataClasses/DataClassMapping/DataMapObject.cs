using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace AssetManager.Data.DataClasses
{
    /// <summary>
    /// Data mapper for classes tagged with <see cref="DataColumnNameAttribute"/>
    /// </summary>
    public abstract class DataMapObject : IDisposable
    {
        #region Fields

        private DataTable populatingTable;

        #endregion Fields

        #region Properties

        public abstract string GUID { get; set; }

        /// <summary>
        /// DataTable that was used to populate this object.
        /// </summary>
        public DataTable PopulatingTable
        {
            get
            {
                return populatingTable;
            }
            set
            {
                populatingTable = value;
                populatingTable.TableName = TableName;
            }
        }

        /// <summary>
        /// Database Tablename for implementing object.
        /// </summary>
        public abstract string TableName { get; set; }

        #endregion Properties

        #region Constructors

        public DataMapObject()
        {
        }

        public DataMapObject(DataTable data)
        {
            var row = ((DataTable)data).Rows[0];
            populatingTable = data;
            populatingTable.TableName = TableName;
            MapProperty(this, row);
        }

        public DataMapObject(DataRow data)
        {
            var row = data;
            populatingTable = row.Table;
            populatingTable.TableName = TableName;
            MapProperty(this, row);
        }

        #endregion Constructors

        #region Methods

        public void MapClassProperties(DataTable data)
        {
            var row = ((DataTable)data).Rows[0];
            populatingTable = row.Table;
            populatingTable.TableName = TableName;
            MapProperty(this, row);
        }

        /// <summary>
        /// Uses reflection to recursively populate/map class properties that are marked with a <see cref="DataColumnNameAttribute"/>.
        /// </summary>
        /// <param name="obj">Object to be populated.</param>
        /// <param name="row">DataRow with columns matching the <see cref="DataColumnNameAttribute"/> in the objects properties.</param>
        private void MapProperty(object obj, DataRow row)
        {
            //Collect list of all properties in the object class.
            List<System.Reflection.PropertyInfo> Props = (obj.GetType().GetProperties().ToList());

            //Iterate through the properties.

            foreach (System.Reflection.PropertyInfo prop in Props)
            {
                //Check if the property contains a target attribute.

                if (prop.GetCustomAttributes(typeof(DataColumnNameAttribute), true).Length > 0)
                {
                    //Get the column name attached to the property.
                    var propColumn = ((DataColumnNameAttribute)prop.GetCustomAttributes(false)[0]).ColumnName;

                    //Make sure the DataTable contains a matching column name.

                    if (row.Table.Columns.Contains(propColumn))
                    {
                        //Check the type of the propery and set its value accordingly.

                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(obj, row[propColumn].ToString(), null);
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            DateTime pDate = default(DateTime);
                            if (DateTime.TryParse(DataConsistency.NoNull(row[propColumn].ToString()), out pDate))
                            {
                                prop.SetValue(obj, pDate);
                            }
                            else
                            {
                                prop.SetValue(obj, null);
                            }
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            prop.SetValue(obj, Convert.ToBoolean(row[propColumn]));
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            prop.SetValue(obj, Convert.ToInt32(row[propColumn]));
                        }
                        else
                        {
                            //Throw an error if type is unexpected.
                            Debug.Print(prop.PropertyType.ToString());
                            throw new Exception("Unexpected property type.");
                        }
                    }
                    //If the property does not contain a target attribute, check to see if it is a nested class inheriting the DataMapping class.
                }
                else
                {
                    if (typeof(DataMapObject).IsAssignableFrom(prop.PropertyType))
                    {
                        //Recurse with nested DataMapping properties.
                        var nestObject = prop.GetValue(obj, null);
                        MapProperty(nestObject, row);
                        // MapProperty(prop.GetValue(obj, null), row);
                    }
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    PopulatingTable.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DataMappingObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        #endregion IDisposable Support

        #endregion Methods
    }
}