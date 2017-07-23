﻿using System;
using System.Data;
using System.Threading.Tasks;
using MiningForce.Persistence;

namespace MiningForce.Extensions
{
    public static class ConnectionFactoryExtensions
    {
	    public static void WithConnection(this IConnectionFactory factory, Func<IDbConnection, Task> action)
	    {
		    using (var con = factory.OpenConnection())
		    {
			    action(con);
		    }
	    }

	    public static T WithConnection<T>(this IConnectionFactory factory, Func<IDbConnection, T> action)
	    {
			using (var con = factory.OpenConnection())
			{
			    return action(con);
		    }
	    }

	    public static void WithTransaction(this IConnectionFactory factory, Action<IDbConnection, IDbTransaction> action, bool autoCommit = true, IsolationLevel isolation = IsolationLevel.ReadCommitted)
	    {
			using (var con = factory.OpenConnection())
			{
			    using (var tx = con.BeginTransaction(isolation))
			    {
				    try
				    {
					    action(con, tx);

					    if (autoCommit)
						    tx.Commit();
				    }

				    catch
				    {
					    tx.Rollback();
					    throw;
				    }
			    }
		    }
	    }

	    public static T WithTransaction<T>(this IConnectionFactory factory, Func<IDbConnection, IDbTransaction, T> action, bool autoCommit = true, IsolationLevel isolation = IsolationLevel.ReadCommitted)
	    {
			using (var con = factory.OpenConnection())
			{
			    using (var tx = con.BeginTransaction(isolation))
			    {
				    try
				    {
					    var result = action(con, tx);

					    if (autoCommit)
						    tx.Commit();

					    return result;
				    }

				    catch
				    {
					    tx.Rollback();
					    throw;
				    }
			    }
		    }
	    }
    }
}