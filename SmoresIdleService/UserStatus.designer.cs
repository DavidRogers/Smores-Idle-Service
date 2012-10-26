﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmoresIdleService
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="DataSource")]
	public partial class UserStatusDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertUserStatus(UserStatus instance);
    partial void UpdateUserStatus(UserStatus instance);
    partial void DeleteUserStatus(UserStatus instance);
    #endregion
		
		public UserStatusDataContext() : 
				base("Data Source=9b40ce93-2d72-42d9-a4c1-a0f6015a9d04.sqlserver.sequelizer.com;User ID" +
						"=ophsjzldiglbranl;Password=q6ZrNmhv2YUCscZJMSHMq2ceE7nTS63MdfYRaw4wA6ZFj5Wrw2x5y" +
						"2kx3VUP7wL5", mappingSource)
		{
			OnCreated();
		}
		
		public UserStatusDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserStatusDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserStatusDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public UserStatusDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<UserStatus> UserStatus
		{
			get
			{
				return this.GetTable<UserStatus>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.UserStatus")]
	public partial class UserStatus : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _UserHash;
		
		private int _Status;
		
		private System.DateTime _LastUpdated;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUserHashChanging(string value);
    partial void OnUserHashChanged();
    partial void OnStatusChanging(int value);
    partial void OnStatusChanged();
    partial void OnLastUpdatedChanging(System.DateTime value);
    partial void OnLastUpdatedChanged();
    #endregion
		
		public UserStatus()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserHash", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string UserHash
		{
			get
			{
				return this._UserHash;
			}
			set
			{
				if ((this._UserHash != value))
				{
					this.OnUserHashChanging(value);
					this.SendPropertyChanging();
					this._UserHash = value;
					this.SendPropertyChanged("UserHash");
					this.OnUserHashChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Status", DbType="Int NOT NULL")]
		public int Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this.OnStatusChanging(value);
					this.SendPropertyChanging();
					this._Status = value;
					this.SendPropertyChanged("Status");
					this.OnStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastUpdated", DbType="DateTime NOT NULL")]
		public System.DateTime LastUpdated
		{
			get
			{
				return this._LastUpdated;
			}
			set
			{
				if ((this._LastUpdated != value))
				{
					this.OnLastUpdatedChanging(value);
					this.SendPropertyChanging();
					this._LastUpdated = value;
					this.SendPropertyChanged("LastUpdated");
					this.OnLastUpdatedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
