﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarComparison.Areas.Admin.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CompareCarEntities : DbContext
    {
        public CompareCarEntities()
            : base("name=CompareCarEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Automaker> Automaker { get; set; }
        public virtual DbSet<Car> Car { get; set; }
        public virtual DbSet<CategoryArticle> CategoryArticle { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<InfoAccount> InfoAccount { get; set; }
        public virtual DbSet<Model> Model { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<TypeUser> TypeUser { get; set; }
        public virtual DbSet<User_Permission> User_Permission { get; set; }
        public virtual DbSet<Version> Version { get; set; }
    
        public virtual ObjectResult<XemTenXe_Result> XemTenXe()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<XemTenXe_Result>("XemTenXe");
        }
    
        [DbFunction("CompareCarEntities", "fc_loadModel")]
        public virtual IQueryable<fc_loadModel_Result> fc_loadModel(string id)
        {
            var idParameter = id != null ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fc_loadModel_Result>("[CompareCarEntities].[fc_loadModel](@id)", idParameter);
        }
    
        [DbFunction("CompareCarEntities", "fc_LoadModel1")]
        public virtual IQueryable<fc_LoadModel1_Result> fc_LoadModel1(string id)
        {
            var idParameter = id != null ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<fc_LoadModel1_Result>("[CompareCarEntities].[fc_LoadModel1](@id)", idParameter);
        }
    }
}
