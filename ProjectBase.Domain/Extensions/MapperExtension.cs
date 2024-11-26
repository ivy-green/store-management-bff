using ProjectBase.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ProjectBase.Application.Extensions
{
    public static class MapperExtension
    {
        public static TEntity ProjectToEntity<TDto, TEntity>(this TDto dto)
        {
            if (dto == null)
            {
                throw new NullException("Item got null before being converted");
            }

            var entityType = typeof(TEntity);
            var dtoType = typeof(TDto);
            var dtoProps = dtoType.GetProperties();

            var entity = Activator.CreateInstance(entityType);
            foreach (var prop in dtoProps)
            {
                var entityProp = entityType.GetProperty(prop.Name);
                if (entityProp == null ||
                    entityProp.GetCustomAttributes<NotMappedAttribute>().Any() ||
                    prop.GetCustomAttributes<NotMappedAttribute>().Any())
                {
                    continue;
                }

                var value = prop.GetValue(dto, null);
                entityProp.SetValue(entity, value);
            }

            if (entity is null)
            {
                throw new NullException("Entity got null when being converted");
            }

            return (TEntity)entity;
        }
        public static TEntity ProjectToEntity<TDto, TEntity>(this TDto dto, TEntity entity)
        {
            if (dto == null)
            {
                return entity;
            }

            var entityType = typeof(TEntity);
            var dtoType = typeof(TDto);
            var dtoProps = dtoType.GetProperties();

            foreach (var prop in dtoProps)
            {
                var entityProp = entityType.GetProperty(prop.Name);
                if (entityProp == null ||
                    entityProp.GetCustomAttributes<NotMappedAttribute>().Any() ||
                    prop.GetCustomAttributes<NotMappedAttribute>().Any())
                {
                    continue;
                }

                var value = prop.GetValue(dto, null);
                entityProp.SetValue(entity, value);
            }
            return (TEntity)entity;
        }

        public static List<TEntity> ProjectToEntities<TEntity, TDto>(this List<TDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return new List<TEntity>();
            }

            var entityType = typeof(TEntity);
            var dtoType = typeof(TDto);
            var dtoPros = dtoType.GetProperties();
            //var entity = Activator.CreateInstance(entityType);

            var entities = new List<TEntity>();

            foreach (var dto in dtos)
            {
                var entity = Activator.CreateInstance(entityType);
                foreach (var prop in dtoPros)
                {
                    var entityPro = entityType.GetProperty(prop.Name);
                    if (entityPro == null ||
                        entityPro.GetCustomAttribute<NotMappedAttribute>() != null ||
                        prop.GetCustomAttribute<NotMappedAttribute>() != null)
                    {
                        continue;
                    }

                    var valor = prop.GetValue(dto, null);
                    entityType.GetProperty(prop.Name)?.SetValue(entity, valor);
                }

                if (entity is null)
                {
                    throw new NullException("Entity got null when being converted");
                }

                entities.Add((TEntity)entity);
            }
            return entities;
        }
    }
}
