using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Model.Products;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	public sealed class ProductMapper : EntityMapper<Product>
	{
		private static readonly string LOAD_SQL = "SELECT Products.Id, Products.Name, Products.Description, Products.Stock, Products.ModifiedBy, Products.Modified, Products.VersionId, v.Value as vValue, v.ModifiedBy as vModifiedBy, v.Modified as vModified FROM Products INNER JOIN Version AS v ON Products.VersionId = v.Id WHERE Products.Id='{0}'";
		private static readonly string LOAD_ALL_SQL = "SELECT Products.Id, Products.Name, Products.Description, Products.Stock, Products.ModifiedBy, Products.Modified, Products.VersionId, v.Value as vValue, v.ModifiedBy as vModifiedBy, v.Modified as vModified FROM Products INNER JOIN Version AS v ON Products.VersionId = v.Id";
		private static readonly string INSERT_SQL = "INSERT INTO Products VALUES ('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}')";
		private static readonly string UPDATE_SQL = "UPDATE Products SET Name = '{0}', Description = '{1}', Stock = {2}, ModifiedBy = '{3}', Modified = '{4}' WHERE Id = '{5}' AND VersionId = '{6}'";
		private static readonly string DELETE_SQL = "DELETE FROM Products WHERE Id = '{0}' AND VersionId='{1}'";

		protected override string Table { get { return "Products"; } }

		protected override string GetLoadSQL(Guid id)
		{
			return string.Format(LOAD_SQL, id.ToString());
		}

		protected override string GetLoadAllSQL()
		{
			return LOAD_ALL_SQL;
		}

		protected override string GetInsertSQL(Product entity)
		{
			return string.Format(INSERT_SQL, entity.Id.ToString(), entity.Name, entity.Description, entity.Stock, entity.ModifiedBy, entity.Modified, entity.Version.Id.ToString());
		}

		protected override string GetUpdateSQL(Product entity)
		{
			return string.Format(UPDATE_SQL, entity.Name, entity.Description, entity.Stock, entity.ModifiedBy, entity.Modified.ToString(), entity.Id.ToString(), entity.Version.Id.ToString());
		}

		protected override string GetDeleteSQL(Product entity)
		{
			return string.Format(DELETE_SQL, entity.Id.ToString(), entity.Version.Id.ToString());
		}

		protected override Product LoadEntity(Guid id, IDataReader reader)
		{
			var name = reader["Name"].ToString();
			var description = reader["Description"].ToString();
			int stock = int.Parse(reader["Stock"].ToString());
			return Product.Activate(id, name, description, stock);
		}
	}
}
