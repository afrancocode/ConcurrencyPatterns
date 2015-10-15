using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConcurrencyPatterns.Infrastructure.Data;
using ConcurrencyPatterns.Infrastructure.Domain;
using ConcurrencyPatterns.Model.Core;

namespace ConcurrencyPatterns.Repository.Sql.Mapping
{
	using Version = ConcurrencyPatterns.Model.Core.Version;

	public abstract class OptimisticLockingBaseMapper : IMapper
	{
		private IMapper mapper;

		public OptimisticLockingBaseMapper(IMapper mapper)
		{
			this.mapper = mapper;
		}

		protected IMapper Mapper { get { return this.mapper; } }

		public EntityBase Find(Guid id)
		{
			return this.mapper.Find(id);
		}

		public IEnumerable<EntityBase> FindAll()
		{
			return this.mapper.FindAll();
		}

		public abstract void Insert(EntityBase entity);
		public abstract void Update(EntityBase entity);
		public abstract void Delete(EntityBase entity);

		protected Version GetVersion(EntityBase entity) { return ((Entity)entity).Version; }
	}

	public sealed class OptimisticLockingMapper : OptimisticLockingBaseMapper
	{
		public OptimisticLockingMapper(IMapper mapper)
			: base(mapper)
		{ }

		public override void Insert(EntityBase entity)
		{
			GetVersion(entity).Insert();
			Mapper.Insert(entity);
		}

		public override void Update(EntityBase entity)
		{
			GetVersion(entity).Increment();
			Mapper.Update(entity);
		}

		public override void Delete(EntityBase entity)
		{
			GetVersion(entity).Delete();
			Mapper.Delete(entity);
		}
	}

	public sealed class OptimisticLockingChildMapper : IChildMapper
	{
		private IChildMapper mapper;

		public OptimisticLockingChildMapper(IChildMapper mapper)
		{
			this.mapper = mapper;
		}

		public void Load(Entity parent)
		{
			mapper.Load(parent);
		}

		public void Insert(Entity entity)
		{
			entity.Version.Increment();
			mapper.Insert(entity);
		}

		public void Update(Entity entity)
		{
			entity.Version.Increment();
			mapper.Update(entity);
		}

		public void Delete(Entity entity)
		{
			entity.Version.Increment();
			mapper.Delete(entity);
		}
	}
}
